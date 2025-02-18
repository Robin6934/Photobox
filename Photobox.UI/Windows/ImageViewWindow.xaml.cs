using Photobox.Lib;
using Photobox.Lib.Helper;
using Photobox.UI.ImageViewer;
using Photobox.WpfHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Photobox.UI.Windows;


public delegate void ResultHander(object? sender, ImageViewResult r);

public partial class ImageViewWindow : Window, IDisposable
{
    private bool _disposed = false;

    private readonly Image<Rgb24> image = default!;

    public new event ResultHander Closed = default!;
    
    private readonly AspectRatio _imageAspectRatio;


    /// <summary>
    /// Initializes a new instance of the <see cref="ImageViewWindow"/> class.
    /// </summary>
    public ImageViewWindow(Image<Rgb24> showImage, bool printingEnabled)
    {
        image = showImage;
        InitializeComponent();

        ImageViewer.Source = showImage.ToBitmapSource();

        _imageAspectRatio = image.GetAspectRatio();

        if (!printingEnabled)
        {
            BorderPrint.Visibility = Visibility.Hidden;
        }
    }

    private void BorderSave_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Close();
        Closed?.Invoke(this, ImageViewResult.Save);
    }

    private void BorderDelete_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Close();
        Closed?.Invoke(this, ImageViewResult.Delete);
    }

    private void BorderPrint_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Close();
        Closed?.Invoke(this, ImageViewResult.Print);
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
        double canvasWidth = canvasHeight * _imageAspectRatio.Ratio;

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
                }
                ImageViewer.Source = null;
            }

            _disposed = true;
        }
    }

    ~ImageViewWindow()
    {
        Dispose(false);
    }
}

