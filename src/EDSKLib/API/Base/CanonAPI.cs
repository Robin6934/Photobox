﻿using System.Drawing;
using System.Drawing.Imaging;
using EOSDigital.SDK;
using Size = EOSDigital.SDK.Size;

namespace EOSDigital.API
{
    /// <summary>
    /// Handles camera connections
    /// </summary>
    public class CanonAPI : IDisposable
    {
        #region Events

        /// <summary>
        /// Fires if a new camera is added
        /// </summary>
        public event CameraAddedHandler CameraAdded;

        /// <summary>
        /// The SDK camera added delegate
        /// </summary>
        private static SDKCameraAddedHandler? CameraAddedEvent;

        private ErrorCode CanonAPI_CameraAddedEvent(IntPtr inContext)
        {
            ThreadPool.QueueUserWorkItem((state) => CameraAdded?.Invoke(this));
            return ErrorCode.OK;
        }

        #endregion

        #region Variables

        /// <summary>
        /// States if the SDK is initialized or not
        /// </summary>
        public static bool IsSDKInitialized
        {
            get { return _IsSDKInitialized; }
        }

        /// <summary>
        /// The main SDK thread where the event loop runs
        /// </summary>
        private static STAThread? MainThread;

        /// <summary>
        /// Field for the public <see cref="IsSDKInitialized"/> property
        /// </summary>
        private static bool _IsSDKInitialized;

        /// <summary>
        /// States if the instance is disposed or not
        /// </summary>
        private bool IsDisposed;

        /// <summary>
        /// Number of instances of this class
        /// </summary>
        private static int RefCount = 0;

        /// <summary>
        /// Object to lock on to safely de- and increment the <see cref="RefCount"/> value
        /// </summary>
        private static readonly Lock InitLock = new();

        /// <summary>
        /// List of currently connected cameras (since the last time GetCameraList got called)
        /// </summary>
        private readonly static List<Camera> CurrentCameras = [];

        /// <summary>
        /// Object to lock on to safely add/remove cameras from the <see cref="CurrentCameras"/> list
        /// </summary>
        private static readonly Lock CameraLock = new();

        #endregion

        #region Init/Dispose

        /// <summary>
        /// Initializes the SDK
        /// </summary>
        public CanonAPI()
            : this(false) { }

        /// <summary>
        /// Initializes the SDK
        /// </summary>
        /// <param name="useCallingThread">If true, the calling thread will be used as SDK main thread;
        /// if false, a separate thread will be created</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public CanonAPI(bool useCallingThread)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            try
            {
                //Ensure that only one caller at a time can increase the counter
                lock (InitLock)
                {
                    //If no instance exists yet, initialize everything
                    if (RefCount == 0)
                    {
                        if (useCallingThread)
                        {
                            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                                throw new ThreadStateException("Calling thread must be in STA");
                            ErrorHandler.CheckError(this, CanonSDK.EdsInitializeSDK());
                        }
                        else
                        {
                            //Trying to trigger DllNotFoundException so it's not thrown
                            //in the event loop on a different thread:
#pragma warning disable CA1806
                            CanonSDK.EdsRelease(IntPtr.Zero);
#pragma warning restore CA1806
                            //Start the main thread where SDK will run on
                            MainThread = new ApiThread();
                            MainThread.Start();
                            //Initialize the SDK on the main thread
                            MainThread.Invoke(() =>
                                ErrorHandler.CheckError(this, CanonSDK.EdsInitializeSDK())
                            );
                        }

                        CanonSDK.InitializeVersion();
                        //Subscribe to the CameraAdded event
                        CameraAddedEvent = new SDKCameraAddedHandler(CanonAPI_CameraAddedEvent);
                        ErrorHandler.CheckError(
                            this,
                            CanonSDK.EdsSetCameraAddedHandler(CameraAddedEvent, IntPtr.Zero)
                        );
                        _IsSDKInitialized = true;
                    }
                    RefCount++;
                }
            }
            catch
            {
                IsDisposed = true;
                if (MainThread?.IsRunning == true)
                    MainThread.Shutdown();
                throw;
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~CanonAPI()
        {
            Dispose(false);
        }

        /// <summary>
        /// Terminates the SDK and disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Terminates the SDK and disposes resources
        /// </summary>
        /// <param name="managed">True if called from Dispose, false if called from the finalizer/destructor</param>
        protected virtual void Dispose(bool managed)
        {
            //Ensure that only one caller at a time can decrease the counter
            lock (InitLock)
            {
                if (!IsDisposed)
                {
                    //If it's the last instance, release everything
                    if (RefCount == 1)
                    {
                        _IsSDKInitialized = false; //Set beforehand because if an error happens, the SDK will be in an unstable state anyway

                        //Remove event handler for the CameraAdded event
                        ErrorCode err = CanonSDK.EdsSetCameraAddedHandler(null!, IntPtr.Zero);
                        if (managed)
                        {
                            ErrorHandler.CheckError(this, err);
                            //Dispose all the connected cameras
                            CurrentCameras.ForEach(t => t.Dispose());
                        }
                        //Terminate the SDK
                        if (MainThread?.IsRunning == true)
                            err = MainThread.Invoke(() =>
                            {
                                return CanonSDK.EdsTerminateSDK();
                            });
                        //Close the main thread
                        if (MainThread?.IsRunning == true)
                            MainThread.Shutdown();
                        if (managed)
                            ErrorHandler.CheckError(this, err);
                    }
                    RefCount--;
                    IsDisposed = true;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a list of all cameras connected to the host.
        /// <para>If a camera has been connected previously, the same instance of the class is returned.</para>
        /// </summary>
        /// <returns>A list of connected cameras</returns>
        /// <exception cref="ObjectDisposedException">This instance has been disposed already</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public List<Camera> GetCameraList()
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            //Ensure that only one caller at a time can access the camera list
            lock (CameraLock)
            {
                //Get a list of camera pointers
                IEnumerable<IntPtr> ptrList = GetCameraPointerList();
                List<Camera> camList = [];

                //Find cameras that were connected before and add new ones
                foreach (var ptr in ptrList)
                {
                    var oldCam = CurrentCameras.FirstOrDefault(t => t.Reference == ptr);
                    if (oldCam != null && !oldCam.IsDisposed)
                        camList.Add(oldCam); //Pointer exists already so we reuse it
                    else
                        camList.Add(new Camera(ptr)); //Pointer does not exists yet, so we add it
                }

                //Ensure that cameras not connected anymore are disposed properly
                var oldCameras = CurrentCameras.Where(t => !ptrList.Any(u => u == t.Reference));
                foreach (var cam in oldCameras)
                {
                    if (!cam.IsDisposed)
                        cam.Dispose();
                }

                CurrentCameras.Clear();
                CurrentCameras.AddRange(camList);
                return camList;
            }
        }

        /// <summary>
        /// Get a list of all pointer of the cameras connected to the host
        /// </summary>
        /// <returns>A list of connected cameras as pointer</returns>
        /// <exception cref="ObjectDisposedException">This instance has been disposed already</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        protected IEnumerable<IntPtr> GetCameraPointerList()
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            //Get camera list
            ErrorHandler.CheckError(this, CanonSDK.EdsGetCameraList(out nint camList));

            //Get number of connected cameras
            ErrorHandler.CheckError(this, CanonSDK.EdsGetChildCount(camList, out int camCount));
            List<IntPtr> ptrList = [];
            for (int i = 0; i < camCount; i++)
            {
                //Get camera pointer
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.EdsGetChildAtIndex(camList, i, out nint cptr)
                );
                ptrList.Add(cptr);
            }
            //Release the list
            ErrorHandler.CheckError(CanonSDK.EdsRelease(camList));
            return ptrList;
        }

        /// <summary>
        /// Gets a thumbnail from a Raw or Jpg image
        /// </summary>
        /// <param name="filePath">Path to the image file</param>
        /// <returns>A <see cref="Bitmap"/> thumbnail from the provided image file</returns>
        public Bitmap GetFileThumb(string filePath)
        {
            //create a file stream to given file
            using var stream = new SDKStream(
                filePath,
                FileCreateDisposition.OpenExisting,
                EOSDigital.SDK.FileAccess.Read
            );
            //Create a thumbnail Bitmap from the stream
            return GetImage(stream.Reference, ImageSource.Thumbnail);
        }

        /// <summary>
        /// Gets a <see cref="Bitmap"/> from an EDSDK pointer to an image (Jpg or Raw)
        /// </summary>
        /// <param name="imgStream">Stream pointer to the image</param>
        /// <param name="imageSource">The result image type</param>
        /// <returns>A <see cref="Bitmap"/> image from the given stream pointer</returns>
        protected Bitmap GetImage(IntPtr imgStream, ImageSource imageSource)
        {
            IntPtr imgRef = IntPtr.Zero;
            IntPtr streamPointer = IntPtr.Zero;

            try
            {
                //create reference and get image info
                ErrorHandler.CheckError(this, CanonSDK.EdsCreateImageRef(imgStream, out imgRef));
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.EdsGetImageInfo(imgRef, imageSource, out ImageInfo imageInfo)
                );

                Size outputSize = new()
                {
                    Width = imageInfo.EffectiveRect.Width,
                    Height = imageInfo.EffectiveRect.Height,
                };
                //calculate amount of data
                int dataLength = outputSize.Height * outputSize.Width * 3;
                //create buffer that stores the image
                byte[] buffer = new byte[dataLength];
                //create a stream to the buffer
                using var stream = new SDKStream(buffer);
                //load image into the buffer
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.EdsGetImage(
                        imgRef,
                        imageSource,
                        TargetImageType.RGB,
                        imageInfo.EffectiveRect,
                        outputSize,
                        stream.Reference
                    )
                );

                //make BGR from RGB (System.Drawing (i.e. GDI+) uses BGR)
                unsafe
                {
                    byte tmp;
                    fixed (byte* pix = buffer)
                    {
                        for (long i = 0; i < dataLength; i += 3)
                        {
                            tmp = pix[i]; //Save B value
                            pix[i] = pix[i + 2]; //Set B value with R value
                            pix[i + 2] = tmp; //Set R value with B value
                        }
                    }
                }

                //Get pointer to stream data
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.EdsGetPointer(stream.Reference, out streamPointer)
                );
                //Create bitmap with the data in the buffer
                return new Bitmap(
                    outputSize.Width,
                    outputSize.Height,
                    dataLength,
                    PixelFormat.Format24bppRgb,
                    streamPointer
                );
            }
            finally
            {
                //Release all data
                if (imgStream != IntPtr.Zero)
                    ErrorHandler.CheckError(CanonSDK.EdsRelease(imgStream));
                if (imgRef != IntPtr.Zero)
                    ErrorHandler.CheckError(CanonSDK.EdsRelease(imgRef));
            }
        }

        #endregion
    }
}
