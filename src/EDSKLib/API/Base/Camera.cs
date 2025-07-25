﻿using EOSDigital.SDK;
using FileAccess = EOSDigital.SDK.FileAccess;

namespace EOSDigital.API
{
    /// <summary>
    /// Represents a physical camera and provides methods to control it
    /// </summary>
    public class Camera : IDisposable
    {
        #region Events

        /// <summary>
        /// The SDK object event
        /// </summary>
        protected event SDKObjectEventHandler SDKObjectEvent = default!;

        /// <summary>
        /// The SDK progress event
        /// </summary>
        protected event SDKProgressCallback SDKProgressCallbackEvent = default!;

        /// <summary>
        /// The SDK property event
        /// </summary>
        protected event SDKPropertyEventHandler SDKPropertyEvent = default!;

        /// <summary>
        /// The SDK state event
        /// </summary>
        protected event SDKStateEventHandler SDKStateEvent = default!;

        /// <summary>
        /// Fires if any process reports progress
        /// </summary>
        public event ProgressHandler ProgressChanged = default!;

        /// <summary>
        /// Fires if the live view image is updated
        /// </summary>
        public event LiveViewUpdate LiveViewUpdated = default!;

        /// <summary>
        /// Fires if an image is ready for download.
        /// Call the <see cref="DownloadFile(DownloadInfo,string)"/> or <see cref="DownloadFile(DownloadInfo)"/> method to get the image or <see cref="CancelDownload"/> to cancel.
        /// </summary>
        public event DownloadHandler DownloadReady = default!;

        /// <summary>
        /// Fires if a property has changed
        /// </summary>
        public event PropertyChangeHandler PropertyChanged = default!;

        /// <summary>
        /// Fires if a state has changed
        /// </summary>
        public event StateChangeHandler StateChanged = default!;

        /// <summary>
        /// Fires if an object has changed
        /// </summary>
        public event ObjectChangeHandler ObjectChanged = default!;

        /// <summary>
        /// This event fires if the camera is disconnected or has shut down
        /// </summary>
        public event CameraUpdateHandler CameraHasShutdown = default!;

        /// <summary>
        /// This event fires when the live view loop has ended
        /// </summary>
        public event CameraUpdateHandler LiveViewStopped = default!;

        #endregion

        #region Variables

        /// <summary>
        /// Info about this camera (can be retrieved without an open session)
        /// </summary>
        protected DeviceInfo Info = default!;

        /// <summary>
        /// Thread for executing SDK commands
        /// </summary>
        protected STAThread MainThread = default!;

        /// <summary>
        /// Pointer to the SDKs camera object
        /// </summary>
        public IntPtr Reference
        {
            get { return CamRef; }
        }

        /// <summary>
        /// An ID for the camera object in this session. It's essentially the pointer of the Canon SDK camera object
        /// </summary>
        public long ID
        {
            get { return CamRef.ToInt64(); }
        }

        /// <summary>
        /// States if a session with this camera is open
        /// </summary>
        public bool SessionOpen
        {
            get { return _SessionOpen; }
            protected set { _SessionOpen = value; }
        }

        /// <summary>
        /// States if the camera is disposed. If true, it can't be used anymore
        /// </summary>
        public bool IsDisposed
        {
            get { return _IsDisposed; }
        }

        /// <summary>
        /// The name of the camera (can be retrieved without an open session)
        /// </summary>
        public string DeviceName
        {
            get { return Info.DeviceDescription; }
        }

        /// <summary>
        /// The name of the port the camera is connected to (can be retrieved without an open session)
        /// </summary>
        public string PortName
        {
            get { return Info.PortName; }
        }

        /// <summary>
        /// States if the live view is running on the computer or not
        /// </summary>
        public bool IsLiveViewOn
        {
            get { return _IsLiveViewOn; }
            protected set { _IsLiveViewOn = value; }
        }

        /// <summary>
        /// States if Record property is available for this camera
        /// </summary>
        public bool IsRecordAvailable
        {
            get { return _IsRecordAvailable; }
        }

        /// <summary>
        /// Pointer to the camera object
        /// </summary>
        private readonly IntPtr CamRef = default!;

        /// <summary>
        /// Variable to let the live view download loop continue or stop
        /// </summary>
        private bool KeepLVAlive = false;

        /// <summary>
        /// Thread for the live view download routine
        /// </summary>
        private Thread LVThread = default!;

        /// <summary>
        /// Field for the public <see cref="IsLiveViewOn"/> property
        /// </summary>
        private bool _IsLiveViewOn;

        /// <summary>
        /// Field for the public <see cref="IsDisposed"/> property
        /// </summary>
        private bool _IsDisposed;

        /// <summary>
        /// Field for the public <see cref="SessionOpen"/> property
        /// </summary>
        private bool _SessionOpen;

        /// <summary>
        /// Field for the public <see cref="IsRecordAvailable"/> property
        /// </summary>
        private bool _IsRecordAvailable;

        /// <summary>
        /// Object to set a lock around starting/stopping the live view thread
        /// </summary>
        private readonly Lock lvThreadLockObj = new();

        /// <summary>
        /// States if a film should be downloaded after filming or not
        /// </summary>
        private bool saveFilm;

        /// <summary>
        /// States if the live view should be shown on the PC while filming
        /// </summary>
        private bool useFilmingPcLv = false;

        #endregion

        #region Init/Open/Close/Dispose

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="camRef">Reference to the camera object</param>
        /// <exception cref="ArgumentNullException">Pointer to camera is zero</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        internal protected Camera(IntPtr camRef)
        {
            if (camRef == IntPtr.Zero)
                throw new ArgumentNullException(nameof(camRef));
            CamRef = camRef;
            MainThread = new STAThread();
            MainThread.Start();
            MainThread.Invoke(() =>
                ErrorHandler.CheckError(this, CanonSDK.EdsGetDeviceInfo(CamRef, out Info))
            );
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Camera()
        {
            Dispose(false);
        }

        /// <summary>
        /// Open a new session with camera
        /// </summary>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void OpenSession()
        {
            CheckState(false);

            if (!SessionOpen)
            {
                MainThread.Invoke(() =>
                {
                    ErrorHandler.CheckError(this, CanonSDK.EdsOpenSession(CamRef));

                    //Check if Record is available
                    _IsRecordAvailable =
                        CanonSDK.GetPropertyData(CamRef, PropertyID.Record, 0, out int property)
                        == ErrorCode.OK;

                    //Subscribe to events
                    SDKStateEvent += new SDKStateEventHandler(Camera_SDKStateEvent);
                    SDKPropertyEvent += new SDKPropertyEventHandler(Camera_SDKPropertyEvent);
                    SDKProgressCallbackEvent += new SDKProgressCallback(
                        Camera_SDKProgressCallbackEvent
                    );
                    SDKObjectEvent += new SDKObjectEventHandler(Camera_SDKObjectEvent);
                    ErrorHandler.CheckError(
                        this,
                        CanonSDK.EdsSetCameraStateEventHandler(
                            CamRef,
                            StateEventID.All,
                            SDKStateEvent,
                            CamRef
                        )
                    );
                    ErrorHandler.CheckError(
                        this,
                        CanonSDK.EdsSetObjectEventHandler(
                            CamRef,
                            ObjectEventID.All,
                            SDKObjectEvent,
                            CamRef
                        )
                    );
                    ErrorHandler.CheckError(
                        this,
                        CanonSDK.EdsSetPropertyEventHandler(
                            CamRef,
                            PropertyEventID.All,
                            SDKPropertyEvent,
                            CamRef
                        )
                    );

                    SessionOpen = true;
                });
            }
        }

        /// <summary>
        /// Close session with camera
        /// </summary>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void CloseSession()
        {
            CheckState(false);

            if (SessionOpen)
            {
                //Unsubscribe from all events
                UnsubscribeEvents();

                //If the live view is on, stop it
                if (IsLiveViewOn)
                {
                    KeepLVAlive = false;
                    LVThread.Join(5000);
                }

                MainThread.Invoke(() =>
                {
                    //Close the session with the camera
                    ErrorHandler.CheckError(this, CanonSDK.EdsCloseSession(CamRef));
                    SessionOpen = false;
                });
            }
        }

        /// <summary>
        /// Releases all data and closes session
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all data and closes session
        /// </summary>
        /// <param name="managed">True if called from Dispose, false if called from the finalizer/destructor</param>
        protected virtual void Dispose(bool managed)
        {
            if (!IsDisposed)
            {
                //Unsubscribe from all events
                UnsubscribeEvents();

                //If the live view is on, stop it
                if (IsLiveViewOn)
                {
                    KeepLVAlive = false;
                    LVThread.Join(1000);
                }
                IsLiveViewOn = false;

                MainThread.Invoke(() =>
                {
                    if (CanonAPI.IsSDKInitialized)
                    {
                        //If it's open, close the session
                        if (SessionOpen)
                            CanonSDK.EdsCloseSession(CamRef);
                        //Release the camera
#pragma warning disable CA1806
                        CanonSDK.EdsRelease(CamRef);
#pragma warning restore CA1806
                    }
                    _IsDisposed = true;
                });
                //Shutdown the main camera thread
                MainThread.Shutdown();
            }
        }

        /// <summary>
        /// Unsubscribes all camera events from this class and the SDK
        /// </summary>
        private void UnsubscribeEvents()
        {
            SDKStateEvent -= Camera_SDKStateEvent;
            SDKPropertyEvent -= Camera_SDKPropertyEvent;
            SDKProgressCallbackEvent -= Camera_SDKProgressCallbackEvent;
            SDKObjectEvent -= Camera_SDKObjectEvent;

            if (CanonAPI.IsSDKInitialized)
            {
                MainThread.Invoke(() =>
                {
                    //Clear callbacks from Canon SDK
                    CanonSDK.EdsSetCameraStateEventHandler(CamRef, StateEventID.All, null!, CamRef);
                    CanonSDK.EdsSetObjectEventHandler(CamRef, ObjectEventID.All, null!, CamRef);
                    CanonSDK.EdsSetPropertyEventHandler(CamRef, PropertyEventID.All, null!, CamRef);
                });
            }
        }

        #endregion

        #region SDK Eventhandling

        private ErrorCode Camera_SDKObjectEvent(
            ObjectEventID inEvent,
            IntPtr inRef,
            IntPtr inContext
        )
        {
            ThreadPool.QueueUserWorkItem(
                (state) =>
                {
                    try
                    {
                        var DownloadReadyEvent = DownloadReady;

                        if (
                            inEvent == ObjectEventID.DirItemRequestTransfer
                            && DownloadReadyEvent != null
                        )
                        {
                            DownloadReadyEvent(this, new DownloadInfo(inRef));
                        }
                        else if (
                            inEvent == ObjectEventID.DirItemCreated
                            && saveFilm
                            && DownloadReadyEvent != null
                        )
                        {
                            saveFilm = false;
                            DownloadReadyEvent(this, new DownloadInfo(inRef));
                        }
                    }
                    catch (Exception ex) when (!IsDisposed && !ErrorHandler.ReportError(this, ex))
                    {
                        throw;
                    }

                    ObjectChanged?.Invoke(this, inEvent, inRef);
                }
            );
            return ErrorCode.OK;
        }

        private ErrorCode Camera_SDKProgressCallbackEvent(
            int inPercent,
            IntPtr inContext,
            ref bool outCancel
        )
        {
            ThreadPool.QueueUserWorkItem(
                (state) =>
                {
                    try
                    {
                        ProgressChanged?.Invoke(this, inPercent);
                    }
                    catch (Exception ex) when (!IsDisposed && !ErrorHandler.ReportError(this, ex))
                    {
                        throw;
                    }
                }
            );
            return ErrorCode.OK;
        }

        private ErrorCode Camera_SDKPropertyEvent(
            PropertyEventID inEvent,
            PropertyID inPropertyID,
            int inParameter,
            IntPtr inContext
        )
        {
            ThreadPool.QueueUserWorkItem(
                (state) =>
                {
                    try
                    {
                        if (
                            inPropertyID == PropertyID.Evf_OutputDevice
                            || inPropertyID == PropertyID.Record
                        )
                        {
                            lock (lvThreadLockObj)
                            {
                                EvfOutputDevice outDevice = GetEvf_OutputDevice();
                                Recording recordState = IsRecordAvailable
                                    ? ((Recording)GetInt32Setting(PropertyID.Record))
                                    : Recording.Off;

                                if (
                                    outDevice == EvfOutputDevice.PC
                                    || (
                                        recordState == Recording.Ready
                                        && outDevice == EvfOutputDevice.Filming
                                    )
                                    || (
                                        useFilmingPcLv
                                        && recordState == Recording.On
                                        && (
                                            outDevice == EvfOutputDevice.Filming
                                            || outDevice == EvfOutputDevice.Camera
                                        )
                                    )
                                )
                                {
                                    if (!KeepLVAlive)
                                    {
                                        KeepLVAlive = true;
                                        LVThread = STAThread.CreateThread(DownloadEvf);
                                        LVThread.Start();
                                    }
                                }
                                else if (KeepLVAlive)
                                {
                                    KeepLVAlive = false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!IsDisposed && !ErrorHandler.ReportError(this, ex))
                            throw;
                    }

                    PropertyChanged?.Invoke(this, inEvent, inPropertyID, inParameter);
                }
            );
            return ErrorCode.OK;
        }

        private ErrorCode Camera_SDKStateEvent(
            StateEventID inEvent,
            int inParameter,
            IntPtr inContext
        )
        {
            ThreadPool.QueueUserWorkItem(
                (state) =>
                {
                    try
                    {
                        if (inEvent == StateEventID.Shutdown)
                        {
                            SessionOpen = false;
                            Dispose(true);
                            CameraHasShutdown?.Invoke(this);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!IsDisposed && !ErrorHandler.ReportError(this, ex))
                            throw;
                    }

                    StateChanged?.Invoke(this, inEvent, inParameter);
                }
            );
            return ErrorCode.OK;
        }

        #endregion

        #region Methods

        #region Take Photo

        /// <summary>
        /// Takes a photo with the current camera settings with the TakePicture command
        /// </summary>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        public void TakePhoto()
        {
            CheckState();
            SendCommand(CameraCommand.TakePicture);
        }

        /// <summary>
        /// Takes a photo with the current camera settings asynchronously with the TakePicture command
        /// </summary>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        public Task TakePhotoAsync()
        {
            CheckState();
            return Task.Run(() =>
            {
                try
                {
                    SendCommand(CameraCommand.TakePicture);
                }
                catch (Exception ex)
                {
                    if (!ErrorHandler.ReportError(this, ex))
                        throw;
                }
            });
        }

        /// <summary>
        /// Takes a photo with the current camera settings with the PressShutterButton command
        /// </summary>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        public void TakePhotoShutter()
        {
            CheckState();
            MainThread.Invoke(() =>
            {
                SendCommand(CameraCommand.PressShutterButton, (int)ShutterButton.Completely);
                SendCommand(CameraCommand.PressShutterButton, (int)ShutterButton.OFF);
            });
        }

        /// <summary>
        /// Takes a photo with the current camera settings asynchronously with the PressShutterButton command
        /// </summary>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        public async Task TakePhotoShutterAsync()
        {
            CheckState();
            await Task.Run(() =>
            {
                try
                {
                    MainThread.Invoke(() =>
                    {
                        SendCommand(
                            CameraCommand.PressShutterButton,
                            (int)ShutterButton.Completely
                        );
                        SendCommand(CameraCommand.PressShutterButton, (int)ShutterButton.OFF);
                    });
                }
                catch (Exception ex)
                {
                    if (!ErrorHandler.ReportError(this, ex))
                        throw;
                }
            });
        }

        /// <summary>
        /// Takes a bulb photo with the current camera settings.
        /// The Tv camera value must be set to Bulb before calling this
        /// </summary>
        /// <param name="bulbTime">The time in ms for how long the shutter will be open</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        public void TakePhotoBulb(TimeSpan bulbTime)
        {
            CheckState();

            SendCommand(CameraCommand.BulbStart);

            Task.Delay(bulbTime).Wait();

            SendCommand(CameraCommand.BulbEnd);
        }

        /// <summary>
        /// Takes a bulb photo with the current camera settings asynchronously.
        /// The Tv camera value must be set to Bulb before calling this
        /// </summary>
        /// <param name="bulbTime">The time in ms for how long the shutter will be open</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        public async Task TakePhotoBulbAsync(TimeSpan bulbTime)
        {
            CheckState();

            try
            {
                SendCommand(CameraCommand.BulbStart);
                await Task.Delay(bulbTime);
                SendCommand(CameraCommand.BulbEnd);
            }
            catch (Exception ex)
            {
                if (!ErrorHandler.ReportError(this, ex))
                    throw;
            }
        }

        #endregion

        #region File Handling

        /// <summary>
        /// Downloads a file to given directory with the filename in the <see cref="DownloadInfo"/>
        /// </summary>
        /// <param name="Info">The <see cref="DownloadInfo"/> that is provided by the <see cref="DownloadReady"/> event</param>
        /// <param name="directory">The directory where the file will be saved to if it ends with a
        /// filename then this one will eb chosen else the one created in camera will be used</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="ArgumentNullException">The DownloadInfo is null</exception>
        public void DownloadFile(DownloadInfo Info, string directory)
        {
            CheckState();
            ArgumentNullException.ThrowIfNull(Info);

            if (directory == null || string.IsNullOrEmpty(directory.Trim()))
            {
                directory = ".";
            }

            string currentFile = directory;

            if (!Path.HasExtension(directory))
            {
                currentFile = Path.Combine(directory, Info.FileName);
            }

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            DownloadToFile(Info, currentFile);
        }

        /// <summary>
        /// Downloads a file into a stream
        /// </summary>
        /// <param name="Info">The <see cref="DownloadInfo"/> that is provided by the <see cref="DownloadReady"/> event</param>
        /// <returns>The stream containing the file data</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="ArgumentNullException">The DownloadInfo is null</exception>
        public Stream DownloadFile(DownloadInfo Info)
        {
            CheckState();
            ArgumentNullException.ThrowIfNull(Info);
            return DownloadToStream(Info);
        }

        /// <summary>
        /// Cancels the download of an image
        /// </summary>
        /// <param name="Info">The DownloadInfo that is provided by the "<see cref="DownloadReady"/>" event</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="ArgumentNullException">The DownloadInfo is null</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void CancelDownload(DownloadInfo Info)
        {
            CheckState();
            ArgumentNullException.ThrowIfNull(Info);

            MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(CanonSDK.EdsDownloadCancel(Info.Reference));
                ErrorHandler.CheckError(CanonSDK.EdsRelease(Info.Reference));
            });
        }

        /// <summary>
        /// Gets all volumes available on the camera
        /// </summary>
        /// <returns>An array of CameraFileEntries that are the volumes</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        public CameraFileEntry[] GetAllVolumes()
        {
            CheckState();
            return GetAllVolumesSub();
        }

        /// <summary>
        /// Gets all volumes, folders and files existing on the camera
        /// </summary>
        /// <returns>A <see cref="CameraFileEntry"/> that contains all the information</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        public CameraFileEntry GetAllEntries()
        {
            CheckState();
            return GetAllEntriesSub();
        }

        /// <summary>
        /// Gets all images saved on the cameras memory card(s)
        /// </summary>
        /// <returns>An array of CameraFileEntries that are the images</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        public CameraFileEntry[] GetAllImages()
        {
            CheckState();
            return GetAllImagesSub();
        }

        /// <summary>
        /// Formats a given camera volume. Get the available volumes with <see cref="GetAllVolumes"/>
        /// <para>Warning: All data on this volume will be lost!</para>
        /// </summary>
        /// <param name="volume">The volume that will get formatted</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="ArgumentException">CameraFileEntry is not a volume</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void FormatVolume(CameraFileEntry volume)
        {
            CheckState();
            if (!volume.IsVolume)
                throw new ArgumentException("CameraFileEntry must be a volume");
            MainThread.Invoke(() =>
                ErrorHandler.CheckError(this, CanonSDK.EdsFormatVolume(volume.Reference))
            );
        }

        /// <summary>
        /// Downloads all given files into a folder. To get all images on the camera, use
        /// <see cref="GetAllImages"/> or for all files <see cref="GetAllEntries"/>
        /// <para>
        /// Note: All given CameraFileEntries will be disposed after this.
        /// Call <see cref="GetAllImages"/> or <see cref="GetAllEntries"/> to get valid entries again.
        /// </para>
        /// </summary>
        /// <param name="folderPath">The path to the folder where the files will be saved to</param>
        /// <param name="files">The files that will be downloaded</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        public void DownloadFiles(string folderPath, CameraFileEntry[] files)
        {
            CheckState();
            if (files == null)
                return;
            if (folderPath == null || string.IsNullOrEmpty(folderPath.Trim()))
                folderPath = ".";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].IsFolder && !files[i].IsDisposed)
                {
                    var info = new DownloadInfo(files[i].Reference);
                    string CurrentFile = Path.Combine(folderPath, info.FileName);
                    DownloadToFile(info, CurrentFile);
                }
            }
        }

        /// <summary>
        /// Deletes all given files on the camera.
        /// To get all images on the camera, use <see cref="GetAllImages"/> or for all files <see cref="GetAllEntries"/>
        /// <para>Warning: All given files will be lost!</para>
        /// </summary>
        /// <param name="files">The images that will be deleted</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        public void DeleteFiles(CameraFileEntry[] files)
        {
            CheckState();
            if (files == null || files.Length == 0)
                return;

            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].IsFolder && !files[i].IsDisposed)
                {
                    ErrorHandler.CheckError(
                        this,
                        CanonSDK.EdsDeleteDirectoryItem(files[i].Reference)
                    );
                    files[i].IsDisposed = true;
                }
            }
        }

        #endregion

        #region Other

        /// <summary>
        /// Tells the camera how much space is available on the host PC
        /// </summary>
        /// <param name="bytesPerSector">Bytes per HD sector</param>
        /// <param name="numberOfFreeClusters">Number of free clusters on the HD</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void SetCapacity(int bytesPerSector, int numberOfFreeClusters)
        {
            CheckState();
            MainThread.Invoke(() =>
            {
                Capacity capacity = new(numberOfFreeClusters, bytesPerSector, true);
                ErrorHandler.CheckError(this, CanonSDK.EdsSetCapacity(CamRef, capacity));
            });
        }

        /// <summary>
        /// Locks or unlocks the camera UI
        /// </summary>
        /// <param name="lockState">True to lock, false to unlock</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void UILock(bool lockState)
        {
            if (lockState)
                SendStatusCommand(CameraStatusCommand.UILock);
            else
                SendStatusCommand(CameraStatusCommand.UIUnLock);
        }

        /// <summary>
        /// Gets the list of possible values for the current camera to set.
        /// Only the PropertyIDs "AEModeSelect", "ISO", "Av", "Tv", "MeteringMode" and "ExposureCompensation" are allowed.
        /// </summary>
        /// <param name="propId">The property ID</param>
        /// <returns>A list of available values for the given property ID</returns>
        public CameraValue[] GetSettingsList(PropertyID propId)
        {
            CheckState();

            if (
                propId == PropertyID.AEModeSelect
                || propId == PropertyID.ISO
                || propId == PropertyID.Av
                || propId == PropertyID.Tv
                || propId == PropertyID.MeteringMode
                || propId == PropertyID.ExposureCompensation
            )
            {
                CameraValue[] values = default!;
                PropertyDesc des = default;
                MainThread.Invoke(() =>
                    ErrorHandler.CheckError(
                        this,
                        CanonSDK.EdsGetPropertyDesc(CamRef, propId, out des)
                    )
                );
                values = new CameraValue[des.NumElements];
                for (int i = 0; i < values.Length; i++)
                    values[i] = new CameraValue(des.PropDesc![i], propId);
                return values;
            }
            else
                throw new ArgumentException($"Method cannot be used with Property ID {propId}");
        }

        /// <summary>
        /// Sends a command safely to the camera
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="inParam">Additional parameter</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void SendCommand(CameraCommand command, int inParam = 0)
        {
            CheckState();
            MainThread.Invoke(() =>
                ErrorHandler.CheckError(this, CanonSDK.EdsSendCommand(CamRef, command, inParam))
            );
        }

        /// <summary>
        /// Sends a Status Command to the camera
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="inParam">Additional parameter</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void SendStatusCommand(CameraStatusCommand command, int inParam = 0)
        {
            CheckState();
            MainThread.Invoke(() =>
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.EdsSendStatusCommand(CamRef, command, inParam)
                )
            );
        }

        /// <summary>
        /// Starts the live view
        /// </summary>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void StartLiveView()
        {
            CheckState();
            if (!IsLiveViewOn)
                SetSetting(PropertyID.Evf_OutputDevice, (int)EvfOutputDevice.PC);
        }

        /// <summary>
        /// Stops the live view
        /// </summary>
        /// <param name="LVOff">If true, the live view is shut off, if false, the live view will be shown on the cameras screen</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void StopLiveView(bool LVOff = true)
        {
            CheckState();
            if (LVOff)
                SetSetting(PropertyID.Evf_OutputDevice, (int)EvfOutputDevice.Off);
            else
                SetSetting(PropertyID.Evf_OutputDevice, (int)EvfOutputDevice.Camera);
        }

        /// <summary>
        /// Starts recording a video
        /// <para>Note: The camera has to be set into filming mode before using this (usually a physical switch on the camera)</para>
        /// <para>This works only on cameras that support filming</para>
        /// </summary>
        /// <param name="PCLiveView">If true, the live view will be transferred to the computer otherwise it's shown on the camera</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="InvalidOperationException">The camera is not in film mode</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void StartFilming(bool PCLiveView)
        {
            CheckState();

            Recording state = (Recording)GetInt32Setting(PropertyID.Record);
            if (state != Recording.On)
            {
                if (state != Recording.Ready)
                    throw new InvalidOperationException(
                        "The camera is not ready to film. The Record property has to be Recording.Ready"
                    );
                useFilmingPcLv = PCLiveView;
                //When recording videos, it has to be saved on the camera internal memory
                SetSetting(PropertyID.SaveTo, (int)SaveTo.Camera);
                //Start the video recording
                SetSetting(PropertyID.Record, (int)Recording.On);
            }
        }

        /// <summary>
        /// Stops recording a video
        /// </summary>
        /// <param name="saveFilm">If true, the <see cref="DownloadReady"/> event will fire as soon as the video file is created on the camera</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void StopFilming(bool saveFilm)
        {
            StopFilming(saveFilm, true);
        }

        /// <summary>
        /// Stops recording a video
        /// </summary>
        /// <param name="saveFilm">If true, the <see cref="DownloadReady"/> event will fire as soon as the video file is created on the camera</param>
        /// <param name="stopLiveView">If true, the PC live view will stop and will only be shown on the camera</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void StopFilming(bool saveFilm, bool stopLiveView)
        {
            CheckState();

            Recording state = (Recording)GetInt32Setting(PropertyID.Record);
            if (state == Recording.On)
            {
                this.saveFilm = saveFilm;
                //Stop video recording
                SetSetting(PropertyID.Record, (int)Recording.Off);
                useFilmingPcLv = false;
                if (IsLiveViewOn && stopLiveView)
                    StopLiveView(false);
            }
        }

        #endregion

        #region Set Settings

        /// <summary>
        /// Sets a value for the given property ID
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="value">The value which will be set</param>
        /// <param name="inParam">Additional property information</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void SetSetting(PropertyID propID, object value, int inParam = 0)
        {
            CheckState();

            MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.EdsGetPropertySize(
                        CamRef,
                        propID,
                        inParam,
                        out DataType propertyType,
                        out int propertySize
                    )
                );
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.EdsSetPropertyData(CamRef, propID, inParam, propertySize, value)
                );
            });
        }

        /// <summary>
        /// Sets a string value for the given property ID
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="value">The value which will be set</param>
        /// <param name="inParam">Additional property information</param>
        /// <param name="MAX">The maximum length of the string</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public void SetSetting(PropertyID propID, string value, int inParam = 0, int MAX = 32)
        {
            CheckState();

            value ??= string.Empty;

            if (value.Length > MAX - 1)
            {
                value = value[..(MAX - 1)];
            }

            byte[] propBytes = System.Text.Encoding.ASCII.GetBytes(value + '\0');
            MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.EdsSetPropertyData(
                        CamRef,
                        propID,
                        inParam,
                        propBytes.Length,
                        propBytes
                    )
                );
            });
        }

        /// <summary>
        /// Sets a struct value for the given property ID
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="value">The value which will be set</param>
        /// <param name="inParam">Additional property information</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        [Obsolete("Use SetSetting directly because internally it does the same")]
        public void SetStructSetting<T>(PropertyID propID, T value, int inParam = 0)
            where T : struct
        {
            SetSetting(propID, value, inParam);
        }

        #endregion

        #region Get Settings

        /// <summary>
        /// Gets the current setting of given property ID
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public bool GetBoolSetting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out bool property)
                );
                return property;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public byte GetByteSetting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out byte property)
                );
                return property;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public short GetInt16Setting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out short property)
                );
                return property;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public ushort GetUInt16Setting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out ushort property)
                );
                return property;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public uint GetUInt32Setting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out uint property)
                );
                return property;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public int GetInt32Setting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out int property)
                );
                return property;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID as a string
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public string GetStringSetting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out string property)
                );
                return property;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID as a integer array
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public bool[] GetBoolArrSetting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out bool[] data)
                );
                return data;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID as a integer array
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public short[] GetInt16ArrSetting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out short[] data)
                );
                return data;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID as a integer array
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public int[] GetInt32ArrSetting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out int[] data)
                );
                return data;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID as a integer array
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public byte[] GetByteArrSetting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out byte[] data)
                );
                return data;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID as a unsigned integer array
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public uint[] GetUInt32ArrSetting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out uint[] data)
                );
                return data;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID as a rational array
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public Rational[] GetRationalArrSetting(PropertyID propID, int inParam = 0)
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData(CamRef, propID, inParam, out Rational[] data)
                );
                return data;
            });
        }

        /// <summary>
        /// Gets the current setting of given property ID as a struct
        /// </summary>
        /// <param name="propID">The property ID</param>
        /// <param name="inParam">Additional property information</param>
        /// <typeparam name="T">One of the EDSDK structs</typeparam>
        /// <returns>The current setting of the camera</returns>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public T GetStructSetting<T>(PropertyID propID, int inParam = 0)
            where T : struct
        {
            CheckState();

            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetPropertyData<T>(CamRef, propID, inParam, out T property)
                );
                return property;
            });
        }

        /// <summary>
        /// There is an overflow bug in the SDK that messes up the Evf_OutputDevice property.
        /// With this method you can get the adjusted and correct value.
        /// </summary>
        /// <returns>The current output device of the live view</returns>
        public EvfOutputDevice GetEvf_OutputDevice()
        {
            int value = GetInt32Setting(PropertyID.Evf_OutputDevice);
            unchecked
            {
                if ((value > 2147483600 || value < -2147483640) && value != (int)0xFFFFFFFF)
                    return (EvfOutputDevice)(int.MinValue + value);
                else
                    return (EvfOutputDevice)value;
            }
            ;
        }

        #endregion

        #region Subroutines

        /// <summary>
        /// Downloads live view images and the live view metadata continuously
        /// </summary>
        private void DownloadEvf()
        {
            if (IsLiveViewOn)
            {
                return;
            }

            try
            {
                //Create variables
                IntPtr evfImageRef = IntPtr.Zero;
                ErrorCode err;

                //Create stream
                using var stream = new SDKStream(0L);
                IsLiveViewOn = true;

                //Run live view
                while (KeepLVAlive)
                {
                    //Get live view image
                    lock (STAThread.ExecLock)
                    {
                        err = CanonSDK.EdsCreateEvfImageRef(stream.Reference, out evfImageRef);
                        if (err == ErrorCode.OK)
                            err = CanonSDK.EdsDownloadEvfImage(CamRef, evfImageRef);
                    }

                    //Check for errors
                    if (err == ErrorCode.OBJECT_NOTREADY)
                    {
                        continue;
                    }
                    else if (err != ErrorCode.OK)
                    {
                        ErrorHandler.CheckError(err);
                        continue;
                    }

                    //Release current evf image
#pragma warning disable CA1806
                    CanonSDK.EdsRelease(evfImageRef);
#pragma warning restore CA1806

                    //Set stream position back to zero
                    stream.Position = 0;

                    //Update live view
                    LiveViewUpdated?.Invoke(this, stream);
                }
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException || !ErrorHandler.ReportError(this, ex))
                    throw;
            }
            finally
            {
                IsLiveViewOn = false;
                ThreadPool.QueueUserWorkItem((state) => LiveViewStopped?.Invoke(this));
            }
        }

        /// <summary>
        /// Downloads any data from the camera to the computer
        /// </summary>
        /// <param name="Info">The info about the object that will get downloaded</param>
        /// <param name="stream">The pointer to the stream where the data will be loaded into</param>
        /// <exception cref="SDKException">An SDK call failed</exception>
        protected void DownloadData(DownloadInfo Info, IntPtr stream)
        {
            MainThread.Invoke(() =>
            {
                try
                {
                    //Set the progress callback
                    ErrorHandler.CheckError(
                        this,
                        CanonSDK.EdsSetProgressCallback(
                            stream,
                            SDKProgressCallbackEvent,
                            ProgressOption.Periodically,
                            Info.Reference
                        )
                    );
                    //Check which SDK version is used and download the data with the correct method
                    if (CanonSDK.IsVerGE34)
                        ErrorHandler.CheckError(
                            this,
                            CanonSDK.EdsDownload(Info.Reference, Info.Size64, stream)
                        );
                    else
                        ErrorHandler.CheckError(
                            this,
                            CanonSDK.EdsDownload(Info.Reference, Info.Size, stream)
                        );
                }
                finally
                {
                    //Release all data
                    ErrorHandler.CheckError(CanonSDK.EdsDownloadComplete(Info.Reference));
                    ErrorHandler.CheckError(CanonSDK.EdsRelease(Info.Reference));
                }
            });
        }

        /// <summary>
        /// Downloads any data from the camera to the computer in a file
        /// </summary>
        /// <param name="Info">The info about the object that will get downloaded</param>
        /// <param name="filePath">The path with filename to where the data will be saved to</param>
        /// <exception cref="SDKException">An SDK call failed</exception>
        protected void DownloadToFile(DownloadInfo Info, string filePath)
        {
            using var stream = new SDKStream(
                filePath,
                FileCreateDisposition.CreateAlways,
                FileAccess.ReadWrite
            );

            DownloadData(Info, stream.Reference);
        }

        /// <summary>
        /// Downloads any data from the camera to the computer into a <see cref="Stream"/>
        /// </summary>
        /// <param name="Info">The info about the object that will get downloaded</param>
        /// <returns>A <see cref="Stream"/> containing the downloaded data</returns>
        /// <exception cref="SDKException">An SDK call failed</exception>
        public Stream DownloadToStream(DownloadInfo Info)
        {
            SDKStream stream = new(Info.Size64);
            DownloadData(Info, stream.Reference);
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Gets information about camera file entries recursively
        /// </summary>
        /// <param name="ptr">Pointer to the current file object</param>
        /// <returns>An array of file entries</returns>
        /// <exception cref="SDKException">An SDK call failed</exception>
        protected CameraFileEntry[] GetChildren(IntPtr ptr)
        {
            int ChildCount;
            lock (STAThread.ExecLock)
            {
                ErrorHandler.CheckError(this, CanonSDK.EdsGetChildCount(ptr, out ChildCount));
            }
            if (ChildCount > 0)
            {
                CameraFileEntry[] MainEntry = new CameraFileEntry[ChildCount];
                for (int i = 0; i < ChildCount; i++)
                {
                    MainEntry[i] = GetChild(ptr, i);

                    if (MainEntry[i].IsFolder)
                    {
                        CameraFileEntry[] returnValue = GetChildren(MainEntry[i].Reference);
                        if (returnValue is not null)
                        {
                            MainEntry[i].Entries = returnValue;
                        }
                    }
                }
                return MainEntry;
            }
            else
                return default!;
        }

        /// <summary>
        /// Gets information about children of a folder in the cameras file system
        /// </summary>
        /// <param name="ptr">Pointer to the folder</param>
        /// <param name="index">Index of the child in the folder</param>
        /// <returns>A camera file entry of this child</returns>
        /// <exception cref="SDKException">An SDK call failed</exception>
        protected CameraFileEntry GetChild(IntPtr ptr, int index)
        {
            return MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.EdsGetChildAtIndex(ptr, index, out nint ChildPtr)
                );
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.GetDirectoryItemInfo(ChildPtr, out DirectoryItemInfo ChildInfo)
                );

                CameraFileEntry outEntry = new(
                    ChildPtr,
                    ChildInfo.FileName,
                    ChildInfo.IsFolder,
                    false
                );
                if (!outEntry.IsFolder)
                {
                    using var stream = new SDKStream(0);
                    ErrorHandler.CheckError(
                        this,
                        CanonSDK.EdsDownloadThumbnail(ChildPtr, stream.Reference)
                    );
                    outEntry.SetThumb(stream.Reference);
                }
                return outEntry;
            });
        }

        /// <summary>
        /// Checks if the camera and SDK state is valid
        /// </summary>
        /// <param name="checkSession">If true, it checks if the session is open</param>
        /// <exception cref="ObjectDisposedException">Camera is disposed</exception>
        /// <exception cref="CameraSessionException">Session is closed</exception>
        /// <exception cref="SDKStateException">Canon SDK is not initialized</exception>
        protected void CheckState(bool checkSession = true)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            if (checkSession && !SessionOpen)
                throw new CameraSessionException("Session is closed");
            if (!CanonAPI.IsSDKInitialized)
                throw new SDKStateException("Canon SDK is not initialized");
        }

        /// <summary>
        /// Creates a bool value from an integer. 1 == true, else == false
        /// </summary>
        /// <param name="val">The integer value</param>
        /// <returns>The bool value</returns>
        protected static bool GetBool(int val)
        {
            return val == 1;
        }

        /// <summary>
        /// Gets all volumes, folders and files existing on the camera
        /// </summary>
        /// <returns>A <see cref="CameraFileEntry"/> that contains all the information</returns>
        /// <exception cref="SDKException">An SDK call failed</exception>
        protected CameraFileEntry GetAllEntriesSub()
        {
            CameraFileEntry MainEntry = new(IntPtr.Zero, DeviceName, true, false);
            CameraFileEntry[] VolumeEntries = GetAllVolumesSub();

            for (int i = 0; i < VolumeEntries.Length; i++)
                VolumeEntries[i].Entries = GetChildren(VolumeEntries[i].Reference);

            MainEntry.Entries = VolumeEntries;
            return MainEntry;
        }

        /// <summary>
        /// Gets all images saved on the cameras memory card(s)
        /// </summary>
        /// <returns>An array of CameraFileEntries that are the images</returns>
        /// <exception cref="SDKException">An SDK call failed</exception>
        protected CameraFileEntry[] GetAllImagesSub()
        {
            CameraFileEntry[] volumes = GetAllVolumesSub();
            List<CameraFileEntry> ImageList = [];

            for (int i = 0; i < volumes.Length; i++)
            {
                int ChildCount;
                lock (STAThread.ExecLock)
                {
                    ErrorHandler.CheckError(
                        this,
                        CanonSDK.EdsGetChildCount(volumes[i].Reference, out ChildCount)
                    );
                }
                CameraFileEntry[] fileEntries = [];

                for (int j = 0; j < ChildCount; j++)
                {
                    CameraFileEntry entry = GetChild(volumes[i].Reference, j);
                    if (entry.IsFolder && entry.Name == "DCIM")
                    {
                        fileEntries = GetChildren(entry.Reference);
                        break;
                    }
                }

                foreach (CameraFileEntry ve in fileEntries)
                {
                    if (ve.IsFolder && ve.Entries != null)
                        ImageList.AddRange(ve.Entries.Where(t => !t.IsFolder));
                }
            }

            return [.. ImageList];
        }

        /// <summary>
        /// Checks for all volumes available on the camera.
        /// </summary>
        /// <returns>An array of CameraFileEntries where each represents a volume (Note: content of volumes is not read)</returns>
        /// <exception cref="SDKException">An SDK call failed</exception>
        protected CameraFileEntry[] GetAllVolumesSub()
        {
            List<CameraFileEntry> VolumeEntries = [];

            MainThread.Invoke(() =>
            {
                ErrorHandler.CheckError(
                    this,
                    CanonSDK.EdsGetChildCount(CamRef, out int VolumeCount)
                );

                for (int i = 0; i < VolumeCount; i++)
                {
                    ErrorHandler.CheckError(
                        this,
                        CanonSDK.EdsGetChildAtIndex(CamRef, i, out nint ChildPtr)
                    );
                    ErrorHandler.CheckError(
                        this,
                        CanonSDK.EdsGetVolumeInfo(ChildPtr, out VolumeInfo volumeInfo)
                    );
                    if (volumeInfo.VolumeLabel != "HDD")
                        VolumeEntries.Add(
                            new CameraFileEntry(
                                ChildPtr,
                                "Volume" + i + "(" + volumeInfo.VolumeLabel + ")",
                                true,
                                true
                            )
                        );
                }
            });

            return [.. VolumeEntries];
        }

        #endregion

        #endregion
    }
}
