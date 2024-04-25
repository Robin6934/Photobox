using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Photobox
{
	/// <summary>
	/// Interaction logic for PhotoPrintPage.xaml
	/// </summary>
	public partial class PhotoPrintPage : Window, IDisposable
	{
		private string _imagePath = "";

		public ConfigLoader? ConfigLoader { get; set;}

        private bool _disposed = false;

        private readonly PhotoBoothLib _photoBoothLib;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoPrintPage"/> class.
        /// </summary>
		public PhotoPrintPage()
		{
			InitializeComponent();

			this.WindowStyle = WindowStyle.None;

			this.WindowState = WindowState.Maximized;

            _photoBoothLib = PhotoBoothLib.Instance;
        }

        /// <summary>
        /// Creates all the Buttons and adds them to the MainCanvas
        /// </summary>
		private void MainCanvas_Loaded(object sender, RoutedEventArgs e)
		{
            // Create the Print button
            Button buttonPrint = new Button();
            buttonPrint.Name = "ButtonPrint";
            buttonPrint.Content = "Print";
            buttonPrint.Height = 150;
            buttonPrint.Width = 150;
            buttonPrint.FontSize = 40;
            buttonPrint.Click += ButtonPrint_Click;
            Canvas.SetLeft(buttonPrint, (MainCanvas.ActualWidth - buttonPrint.Width) / 2); // Centered
            Canvas.SetBottom(buttonPrint, 50);
            buttonPrint.SetValue(Button.TemplateProperty, GetRoundButtonTemplate(buttonPrint));

            // Create the Save button
            Button buttonSave = new Button();
            buttonSave.Name = "ButtonSave";
            buttonSave.Content = "Save";
            buttonSave.Height = 150;
            buttonSave.Width = 150;
            buttonSave.FontSize = 40;
            buttonSave.Click += ButtonSave_Click;
            Canvas.SetLeft(buttonSave, (MainCanvas.ActualWidth - buttonPrint.Width) / 2 - 180); // Centered
            Canvas.SetBottom(buttonSave, 50);
            buttonSave.SetValue(Button.TemplateProperty, GetRoundButtonTemplate(buttonSave));

            // Create the Delete button
            Button buttonDelete = new Button();
            buttonDelete.Name = "ButtonDelete";
            buttonDelete.Content = "Delete";
            buttonDelete.Height = 150;
            buttonDelete.Width = 150;
            buttonDelete.FontSize = 40;
            buttonDelete.Click += ButtonDelete_Click;
            Canvas.SetLeft(buttonDelete, (MainCanvas.ActualWidth - buttonPrint.Width) / 2 + 180); // Centered
            Canvas.SetBottom(buttonDelete, 50);
            buttonDelete.SetValue(Button.TemplateProperty, GetRoundButtonTemplate(buttonDelete));

            // Add the buttons to the canvas
            MainCanvas.Children.Add(buttonPrint);
			MainCanvas.Children.Add(buttonSave);
			MainCanvas.Children.Add(buttonDelete);

			SetCanvasSize();
		}

        /// <summary>
        /// Creates a template for a Round Button
        /// </summary>
        /// <param name="button">The Button on which to use the Template</param>
        /// <returns></returns>
        private static ControlTemplate GetRoundButtonTemplate(Button button)
        {
            ControlTemplate template = new ControlTemplate(typeof(Button));

            FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.Name = "border";
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(button.Width / 2));
            borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(BackgroundProperty));

            FrameworkElementFactory contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentPresenterFactory);

            template.VisualTree = borderFactory;
            return template;
        }

        /// <summary>
        /// Sets the Picture to be displayed
        /// </summary>
        /// <param name="imagePath">Path of the picture to be shown</param>
        public void DisplayImage(string imagePath)
		{
			_imagePath = imagePath;
			this.SizeChanged += PhotoPrintPage_SizeChanged;

            Uri imageUri = new Uri(imagePath);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
			bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = imageUri;
            bitmap.EndInit();

            ImageViewer.Source = bitmap;
			
			MainCanvas.Loaded += MainCanvas_Loaded;
            this.Closing += PhotoPrintPage_Closing;
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
        /// Eventhandler for the Save Button click event Saves the picture
        /// in the Photobox path
        /// </summary>
		private void ButtonSave_Click(object sender, RoutedEventArgs e)
		{
			_photoBoothLib.DoPhotoboxThings(_imagePath, PhotoBoothLib.PictureOptions.Save); // Call the method on the instance
			Close();
        }

        /// <summary>
        /// Eventhandler for the Print Button click event Saves the picture
        /// in the Photobox path and prints it
        /// </summary>
        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
		{
			_photoBoothLib.DoPhotoboxThings(_imagePath, PhotoBoothLib.PictureOptions.Print); // Call the method on the instance
			Close();
        }

        /// <summary>
        /// Eventhandler for the Delete Button click event Copies the
        /// picture into the delete folder
        /// </summary>
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
		{
			_photoBoothLib.DoPhotoboxThings(_imagePath, PhotoBoothLib.PictureOptions.Delete); // Call the method on the instance
			Close();
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
        ~PhotoPrintPage()
        {
            Dispose(false);
        }

    }
}

