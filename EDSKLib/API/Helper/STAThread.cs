namespace EOSDigital.API
{
    /// <summary>
    /// This static class executes things on an STA thread and provides a method to create an STA thread
    /// </summary>
    public class STAThread
    {
        #region Variables

        /// <summary>
        /// Object that is used for the lock keyword to ensure only one SDK command is executed at a time
        /// </summary>
        public static readonly object ExecLock = new();

        /// <summary>
        /// States if the calling thread is in a Single Threaded Apartment or not
        /// </summary>
        public static bool IsSTAThread
        {
            get { return Thread.CurrentThread.GetApartmentState() == ApartmentState.STA; }
        }

        /// <summary>
        /// States if this thread is currently running
        /// </summary>
        public bool IsRunning
        {
            get { return isRunning; }
        }

        /// <summary>
        /// ID of the associated thread
        /// </summary>
        public int ThreadID
        {
            get { return mainThread.ManagedThreadId; }
        }

        /// <summary>
        /// The main thread where everything will be executed on
        /// </summary>
        private Thread mainThread = default!;

        /// <summary>
        /// States if the execution thread is currently running
        /// </summary>
        private bool isRunning = false;

        /// <summary>
        /// Lock object to make sure only one command at a time is executed
        /// </summary>
        private readonly Lock runLock = new();

        /// <summary>
        /// Lock object to ensure that an action executed on the thread does not invoke on itself
        /// </summary>
        private readonly Lock cmdLock = new();

        /// <summary>
        /// Lock object to synchronize between execution and calling thread
        /// </summary>
        protected readonly object threadLock1 = new();

        /// <summary>
        /// Lock object to synchronize between execution and calling thread
        /// </summary>
        private readonly object threadLock2 = new();

        /// <summary>
        /// States if the first lock is currently blocking or not
        /// </summary>
        protected bool block1 = true;

        /// <summary>
        /// States if the second lock is currently blocking or not
        /// </summary>
        private bool block2 = true;

        /// <summary>
        /// The action to be executed
        /// </summary>
        private Action runAction = default!;

        /// <summary>
        /// Storage for an exception that might have happened on the execution thread
        /// </summary>
        private Exception runException = default!;

        #endregion

        /// <summary>
        /// Creates a new instance of the <see cref="STAThread"/> class
        /// </summary>
        internal STAThread() { }

        #region Public Methods

        /// <summary>
        /// Starts the execution loop
        /// </summary>
        public void Start()
        {
            lock (runLock)
            {
                if (!isRunning)
                {
                    mainThread = CreateThread(ExecutionLoop);
                    isRunning = true;
                    mainThread.Start();
                    WaitForThread();
                }
            }
        }

        /// <summary>
        /// Shuts down the execution loop and waits for it to finish
        /// </summary>
        public void Shutdown()
        {
            lock (runLock)
            {
                if (isRunning)
                {
                    isRunning = false;
                    NotifyThread();
                    mainThread.Join();
                }
            }
        }

        /// <summary>
        /// Executes an action on this STA thread
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <exception cref="ArgumentNullException">will be thrown if action is null</exception>
        /// <exception cref="ExecutionException">if an exception is thrown on the thread,
        /// it will be rethrown as inner exception of this exception</exception>
        public void Invoke(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            //If the method is called from the execution thread, directly execute it.
            //This prevents possible deadlocks when trying to acquire the runLock while waiting within for the thread to finish.
            if (cmdLock.TryEnter())
            {
                try
                {
                    action();
                }
                finally
                {
                    cmdLock.Exit();
                }
            }
            else
            {
                lock (runLock)
                {
                    if (!isRunning)
                    {
                        throw new InvalidOperationException("Thread is not running");
                    }

                    runAction = action;
                    NotifyThread();
                    WaitForThread();

                    if (runException != null)
                    {
                        throw new ExecutionException(runException.Message, runException);
                    }
                }
            }
        }

        /// <summary>
        /// Executes a function on this STA thread
        /// </summary>
        /// <param name="func">The function to execute</param>
        /// <returns>the return value of the function</returns>
        /// <exception cref="ArgumentNullException">will be thrown if func is null</exception>
        /// <exception cref="ExecutionException">if an exception is thrown on the thread,
        /// it will be rethrown as inner exception of this exception</exception>
        public T Invoke<T>(Func<T> func)
        {
            ArgumentNullException.ThrowIfNull(func);

            T result = default!;
            Invoke(
                delegate
                {
                    result = func();
                }
            );
            return result;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Creates a thread in a Single Threaded Apartment
        /// </summary>
        /// <param name="action">The command to run on this thread</param>
        /// <returns>An STA thread</returns>
        public static Thread CreateThread(Action action)
        {
            var thread = new Thread(new ThreadStart(action));
            thread.SetApartmentState(ApartmentState.STA);
            return thread;
        }

        /// <summary>
        /// Executes an action on a newly created STA Thread
        /// and optionally waits for it to finish executing
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="wait">If true, the action is executed synchronously or if false, asynchronously.</param>
        public static void ExecuteThread(Action action, bool wait)
        {
            Exception runException = default!;
            Thread thread = CreateThread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    if (wait)
                        runException = ex;
                    else
                        throw;
                }
            });

            thread.Start();

            if (wait)
            {
                thread.Join();
                if (runException is not null)
                {
                    throw new ExecutionException(runException.Message, runException);
                }
            }
        }

        /// <summary>
        /// Executes a function on a newly created STA Thread
        /// </summary>
        /// <param name="func">The function to execute</param>
        /// <returns>The return value of the given function</returns>
        public static T ExecuteThread<T>(Func<T> func)
        {
            Exception runException = default!;
            T result = default!;
            Thread thread = CreateThread(
                delegate
                {
                    try
                    {
                        result = func();
                    }
                    catch (Exception ex)
                    {
                        runException = ex;
                    }
                }
            );
            thread.Start();
            thread.Join();
            if (runException != null)
                throw new ExecutionException(runException.Message, runException);
            return result;
        }

        #endregion

        #region Subroutines

        /// <summary>
        /// Notifies the execution loop to execute something
        /// </summary>
        private void NotifyThread()
        {
            lock (threadLock1)
            {
                block1 = false;
                Monitor.Pulse(threadLock1);
            }
        }

        /// <summary>
        /// Blocks until the execution loop is done with the work
        /// </summary>
        private void WaitForThread()
        {
            lock (threadLock2)
            {
                while (block2)
                    Monitor.Wait(threadLock2);
                block2 = true;
            }
        }

        /// <summary>
        /// Releases the waiting <see cref="WaitForThread"/> method to continue
        /// </summary>
        private void ReleaseWait()
        {
            lock (threadLock2)
            {
                block2 = false;
                Monitor.Pulse(threadLock2);
            }
        }

        /// <summary>
        /// The waiting routine on the execution loop
        /// </summary>
        protected virtual void WaitForNotification()
        {
            lock (threadLock1)
            {
                while (block1 && isRunning)
                {
                    Monitor.Wait(threadLock1);
                }
                block1 = true;
            }
        }

        /// <summary>
        /// The loop that is executed on the thread and where the work is done
        /// </summary>
        private void ExecutionLoop()
        {
            ReleaseWait();

            try
            {
                lock (cmdLock)
                {
                    while (isRunning)
                    {
                        WaitForNotification();
                        if (!isRunning)
                            return;

                        runException = default!;
                        try
                        {
                            lock (ExecLock)
                            {
                                runAction();
                            }
                        }
                        catch (Exception ex)
                        {
                            runException = ex;
                        }

                        ReleaseWait();
                    }
                }
            }
            finally
            {
                isRunning = false;
                ReleaseWait();
            }
        }

        #endregion
    }
}
