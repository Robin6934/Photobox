#define Dev
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EOSDigital.API;
using EOSDigital.SDK;
using static Photobox.PhotoBoothLib;
using System.Windows.Threading;

namespace Photobox
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

        #region Variables

        readonly CanonAPI APIHandler = new CanonAPI();

        Camera MainCamera;

        readonly static ImageBrush bgbrush = new ImageBrush();

        readonly Action<BitmapImage> SetImageAction = (BitmapImage img) => { bgbrush.ImageSource = img; };

        int ErrCount;

        private bool secondTick = false;

        readonly object ErrLock = new object();

		static readonly string userName = Environment.UserName;

		public string dir = $"C:\\Users\\{userName}\\Pictures\\PhotoBox";

        public const string jsonFilePath = "Resources\\config.json";

        private ConfigLoader Config { get; set; }

		readonly DispatcherTimer KeepAliveTimer = new DispatcherTimer()
		{
			Interval = TimeSpan.FromMinutes(1)
		};

		private readonly FileSystemWatcher fileSystemWatcherConfigFile = new FileSystemWatcher();

        private CountDown countDown;

#endregion

        public MainWindow()
		{
			try
			{
                InitJsonReader();

                InitializeComponent();

                Config = ConfigLoader.LoadFromJsonFile(jsonFilePath, this);

                CreateFilePaths();

				ErrorHandler.SevereErrorHappened += ErrorHandler_SevereErrorHappened;

				ErrorHandler.NonSevereErrorHappened += ErrorHandler_NonSevereErrorHappened;

				GetMainCamera(out MainCamera);

                InitTimer();

				InitWindow();
				
				MainCamera?.OpenSession();

                MainCamera.LiveViewUpdated += MainCamera_LiveViewUpdated;

				MainCamera.SetSetting(PropertyID.SaveTo, (int)SaveTo.Both);

				MainCamera.SetCapacity(4096, int.MaxValue);

				StartLV();
#if !Dev
				_ = PowerStatusWatcher.StartDCWatcher();

                RestApiMethods.Init();

                RestApiMethods.StartPolingForPicture(this, dir);
#endif
                SetCanvasSize();				
			}
			catch (DllNotFoundException) { ReportError("Canon DLLs not found!"); }
			catch (Exception ex) { ReportError(ex.Message); }
		}

        private void SettingsChangesHandler(object sender, FileSystemEventArgs e)
        {
            ReadJson();
        }

        private void ReadJson() => Config = ConfigLoader.LoadFromJsonFile(jsonFilePath, this);

        private void InitJsonReader()
        {
            string? absolutePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), jsonFilePath);

			string FileName = System.IO.Path.GetFileName(absolutePath);

			absolutePath = System.IO.Path.GetDirectoryName(absolutePath);

            ReadJson();

            fileSystemWatcherConfigFile.Path = absolutePath ?? "";

			fileSystemWatcherConfigFile.Filter = FileName;

            fileSystemWatcherConfigFile.Changed += SettingsChangesHandler;

            fileSystemWatcherConfigFile.NotifyFilter = NotifyFilters.LastWrite;

            fileSystemWatcherConfigFile.EnableRaisingEvents = true;
        }

        private void TakePicture()
		{
			try
			{
                MainCamera.TakePhotoAsync();
			}
			catch (Exception ex) { ReportError(ex.Message); }
		}
		

		/// <summary>
		/// Refreshes the connected cameras list and selects the first camera.
		/// </summary>
		private void GetMainCamera(out Camera mainCamera)
		{
			int cnt = 0;
			bool CameraFound = false;
			List<Camera> CamList = [];
			mainCamera = null!;

            while (!CameraFound)
			{
				try { CamList = APIHandler.GetCameraList(); }
				catch (Exception ex) { ReportError(ex.Message); }
				if (CamList.Count > 0)
				{
					mainCamera = CamList[0];
					CameraFound = true;
				}
				if(cnt>1000)
				{
					ReportError("No camera found");
					break;
				}
				cnt++;
			}

		}
		
		private void TakePictureButton_Click(object sender, RoutedEventArgs e)
		{
			BorderText.Visibility = Visibility.Hidden;

            Debug.WriteLine("TakePictureButton Pressed");

            TriggerPicture();
        }

        /// <summary>
        /// To trigger the camera using the Spacebar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			// Check if the pressed key is the Spacebar
			if (e.Key == Key.Space)
			{
				TriggerPicture();
			}
		}		

		/// <summary>
		/// Creates an instance of the ImagePreviewWindow class and displays the image.
		/// </summary>
		/// <param name="imagePath">the ImagePath of the Image to be shown</param>
		private void ShowImageViewer(string imagePath)
		{
			string imageName = Path.GetFileName(imagePath);

			string TempPath = dir+"\\ShowTemp\\"+imageName;

			AddTextForPreview(imagePath, TempPath, Config);

			WaitForFileToUnlock(TempPath, TimeSpan.FromSeconds(10));

			Dispatcher.Invoke((Action)(() =>
			{
                PhotoPrintPage imageViewerWindow = new PhotoPrintPage
                {
                    ConfigLoader = Config
                };

                imageViewerWindow.DisplayImage(TempPath);

                imageViewerWindow.Show();
			}));
        }

        #region Initialisations


        /// <summary>
        /// Sets the window to fullscreen and removes the border, also adds the eventlisteners of the MainWindow
        /// </summary>
        private void InitWindow()
		{
#if !Dev
			this.WindowState = WindowState.Maximized;
			this.WindowStyle = WindowStyle.None;
			this.ResizeMode = ResizeMode.NoResize;
#endif
			// Subscribe to the PreviewKeyDown event
			this.PreviewKeyDown += MainWindow_PreviewKeyDown;

            this.Loaded += MainWindow_Loaded;

			// Set the window to focusable so that it can receive keyboard events
			this.Focusable = true;

			this.Focus();

			this.SizeChanged += MainWindow_SizeChanged;

			this.Closing += Window_Closing;
		}

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Point point = new System.Windows.Point(
                Convert.ToInt32(LVCanvas.Width / 2),
                Convert.ToInt32(LVCanvas.Height / 2));

            countDown = new CountDown(Config.CountDown, 1.0d, 200,
                point, LVCanvas);

            countDown.CountDownEarly += CountDown_CountDownEarly;
        }

        /// <summary>
        /// Initialises the Timer for the Countdown and the KeepAliveTimer so the camera wont go to sleep
        /// </summary>
        private void InitTimer()
		{
			KeepAliveTimer.Tick += KeepAliveTimer_Tick;
			KeepAliveTimer.Start();
        }

        #endregion

        #region LiveView

        /// <summary>
        /// Start the Live view and set the background of the canvas to the LiveView
        /// </summary>
        private void StartLV()
		{
			try
			{
				LVCanvas.Background = bgbrush;
				MainCamera.StartLiveView();
			}
			catch (Exception ex) { ReportError(ex.Message); }

		}

		/// <summary>
		/// Subscribtion to update the Liveview
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="img"></param>
		private void MainCamera_LiveViewUpdated(Camera sender, Stream img)
		{	
			try
			{
                using WrapStream s = new WrapStream(img);
                img.Position = 0;
                BitmapImage EvfImage = new BitmapImage();
                EvfImage.BeginInit();
                EvfImage.StreamSource = s;
                EvfImage.CacheOption = BitmapCacheOption.OnLoad;
                EvfImage.EndInit();
                EvfImage.Freeze();

                Application.Current.Dispatcher.BeginInvoke(SetImageAction, EvfImage);
            }
			catch (Exception ex) { ReportError(ex.Message); }
		}


        #endregion

        #region EventListeners
		private void KeepAliveTimer_Tick(object sender, EventArgs e)
		{
			if (secondTick)
            {
                MainCamera.SendCommand(CameraCommand.DriveLensEvf, (int)DriveLens.Near1);
            }
			else
			{
                MainCamera.SendCommand(CameraCommand.DriveLensEvf, (int)DriveLens.Far1);
            }
            Debug.WriteLine("Keepalive Timer Ticked " + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            secondTick ^= true;
        }

		/// <summary>
		/// Eventlistener for when a picture is ready to be downloaded from the camera, the picture will be downloaded to the Temp folder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="Info"></param>
		private void MainCamera_DownloadReady(Camera sender, DownloadInfo Info)
		{
            MainCamera.DownloadReady -= MainCamera_DownloadReady;

            string DownloadDir = dir + "\\Temp\\";

            string fileName = Path.GetFileName(Info.FileName);
			
			string TotalPath = Path.Combine(DownloadDir, fileName);

            try
			{
				sender.DownloadFile(Info, DownloadDir);

                WaitForFileToUnlock(TotalPath, TimeSpan.FromSeconds(10));

                ShowImageViewer(TotalPath);

				Dispatcher.Invoke(new Action(() =>
				{
					BorderText.Visibility = Visibility.Visible;
				}));
            }
            catch (Exception ex) { ReportError(ex.Message); }
		}

        private void CountDown_CountDownEarly()
        {
            TakePictureButton.Click += TakePictureButton_Click;

            KeepAliveTimer.Start();

			TakePicture();
        }

        #endregion

        #region CameraInteractions

        /// <summary>
        /// Trigger the camera to take a picture staticly
        /// </summary>
        public static void TriggerPictureStatic()
		{
            Application.Current.Dispatcher.Invoke(() => 
			((MainWindow)Application.Current.MainWindow).TriggerPicture());
        }

		/// <summary>
		/// Triggers the picture, starts the timer and disables the button
		/// </summary>
		public void TriggerPicture()
		{
            KeepAliveTimer.Stop();
            MainCamera.DownloadReady += MainCamera_DownloadReady;

			countDown.StartCountdown();

			TakePictureButton.Click -= TakePictureButton_Click;
		}

        #endregion

        #region ErrorHandling

		/// <summary>
		/// Shows an error message in a MessageBox
		/// </summary>
		/// <param name="message">The message that gets shown in the created messagebox</param>
		public void ReportError(string message)
		{
			int errc;
			lock (ErrLock) { errc = ++ErrCount; }

			if (errc < 4) MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else if (errc == 4) MessageBox.Show("Many errors happened!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

			lock (ErrLock) { ErrCount--; }
		}

		private void ErrorHandler_NonSevereErrorHappened(object sender, ErrorCode ex)
		{
			ReportError($"SDK Error code: {ex} ({(int)ex:X})");
		}

		private void ErrorHandler_SevereErrorHappened(object sender, Exception ex)
		{
			ReportError(ex.Message);
		}

        #endregion

        #region WindowSize and Closing

		/// <summary>
		/// Eventlistener for when the window size changes, it will update the canvas size
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// Update the canvas size when the window size changes
			SetCanvasSize();
		}

		/// <summary>
		/// Sets the Canvas to be in the middle of the screen
		/// </summary>
		private void SetCanvasSize()
		{
			// Calculate the desired canvas size based on the window's height
			double windowHeight = this.ActualHeight;
			double canvasHeight = windowHeight;
			double canvasWidth = (canvasHeight / 2) * 3; // 3:2 aspect ratio

			// Set the canvas size
			LVCanvas.Width = canvasWidth;
			LVCanvas.Height = canvasHeight;
		}

		/// <summary>
		/// Eventlistener for closing of the Mainwindow, it will dispose the camera and tell the Spring application to shutdown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Window_Closing(object sender, CancelEventArgs e)
		{
			try
			{
				MainCamera?.Dispose();
				APIHandler?.Dispose();
				MainCamera.LiveViewUpdated -= MainCamera_LiveViewUpdated;

				PowerStatusWatcher.StopWatcher();

				RestApiMethods.StopPolingForPicture();

				await RestApi.RestApiGet("http://localhost:80/PhotoBoothApi/Shutdown");
			}
			catch (Exception ex) { ReportError(ex.Message); }
		}

        #endregion
    }
}
