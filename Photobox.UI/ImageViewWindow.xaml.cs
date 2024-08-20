using Photobox.LocalServer.RestApi.Api;
using Photobox.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Photobox
{
    /// <summary>
    /// Interaction logic for PhotoPrintPage.xaml
    /// </summary>
    public partial class ImageViewWindow : Window, IDisposable
    {
        private bool _disposed = false;

        private readonly string imagePath = default!;

        private readonly IPhotoboxApi photoboxApi = new PhotoboxApi("https://localhost:7176");

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageViewWindow"/> class.
        /// </summary>
		public ImageViewWindow(string showImagePath, IPhotoboxApi Api)
        {
            photoboxApi = Api;
            imagePath = showImagePath;
            InitializeComponent();

            Uri imageUri = new(showImagePath);

            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = imageUri;
            bitmap.EndInit();

            ImageViewer.Source = bitmap;
        }

        private async void BorderSave_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task task = photoboxApi.ApiPhotoboxSaveImagePathGetAsync(imagePath);
            Close();
            await task;
        }

        private async void BorderDelete_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task task = photoboxApi.ApiPhotoboxDeleteImagePathGetAsync(imagePath);
            Close();
            await task;
        }

        private async void BorderPrint_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task task = photoboxApi.ApiPhotoboxPrintImagePathGetAsync(imagePath);
            Close();
            await task;
        }

        /// <summary>
        /// Creates all the Buttons and adds them to the MainCanvas
        /// </summary>
		private void MainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            SetCanvasSize();
        }

        private void PhotoPrintPage_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
        }

        private void PhotoPrintPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetCanvasSize();
        }

        /// <summary>
        /// Method to set the canvas to the 3:2 aspect ratio
        /// </summary>
		private void SetCanvasSize()
        {
            // Calculate the desired canvas size based on the window's height
            double windowHeight = this.ActualHeight;
            double canvasHeight = windowHeight;
            double canvasWidth = (canvasHeight / 2) * 3; // 3:2 aspect ratio

            // Set the canvas size
            ImageViewer.Width = canvasWidth;
            ImageViewer.Height = canvasHeight;

            ImageViewer.SetValue(Canvas.LeftProperty, (this.ActualWidth - canvasWidth) / 2);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    this.SizeChanged -= PhotoPrintPage_SizeChanged;

                    if (ImageViewer.Source is BitmapImage bitmapImage)
                    {
                        bitmapImage.StreamSource?.Close();
                        bitmapImage.StreamSource?.Dispose();
                        bitmapImage.StreamSource = null;
                    }
                    ImageViewer.Source = null;
                }

                _disposed = true;
            }
        }

        // Finalizer
        ~ImageViewWindow()
        {
            Dispose(false);
        }
    }
}

