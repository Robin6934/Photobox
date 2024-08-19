using Photobox.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Photobox
{
    /// <summary>
    /// Interaction logic for PhotoPrintPage.xaml
    /// </summary>
    public partial class ImageViewWindow : Window, IImageViewer, IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageViewWindow"/> class.
        /// </summary>
		public ImageViewWindow()
        {
            InitializeComponent();
        }


        private void BorderSave_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void BorderDelete_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void BorderPrint_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Creates all the Buttons and adds them to the MainCanvas
        /// </summary>
		private void MainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            SetCanvasSize();
        }

        /// <summary>
        /// Sets the Picture to be displayed
        /// </summary>
        /// <param name="imagePath">Path of the picture to be shown</param>
        public async Task ShowImage(string imagePath)
        {
            using var window = new ImageViewWindow();

            var tcs = new TaskCompletionSource<bool>();

            window.Closed += (o, e) => tcs.SetResult(true);

            Uri imageUri = new(imagePath);

            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = imageUri;
            bitmap.EndInit();

            window.ImageViewer.Source = bitmap;

            window.Show();

            await tcs.Task;
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

