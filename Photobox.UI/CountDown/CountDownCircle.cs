using Microsoft.Extensions.Options;
using Photobox.Lib.ConfigModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Photobox.UI.CountDown
{
    public class CountDownCircle : ICountDown
    {
        public event TimerCallback? CountDownExpired;

        public event TimerCallback? CountDownEarly;

        private readonly TextBlock textBlockCountdown = new();

        private readonly DispatcherTimer timer = new();

        private readonly DispatcherTimer earlyTimer = new();

        private readonly DispatcherTimer expiredTimer = new();

        private readonly double circumference = 0;

        private double countDownTime = 0;

        private Point point;

        private readonly ArcSegment arc = new();

        private readonly Path path = new();

        private const double startAngle = 359;

        private double angle = 0;

        private readonly double earlySeconds = 0;

        private readonly Stopwatch stopwatch = new();

        private readonly TimeSpan totalTime;

        private readonly TimeSpan earlyTime;
        public Panel Panel
        {
            set
            {
                value.Loaded += (o, a) =>
                {
                    point = new Point(
                    Convert.ToInt32(value.Width / 2),
                    Convert.ToInt32(value.Height / 2)
                    );
                };
                field = value;
            }

            get => field;
        }


        /// <summary>
        /// This class is used to create a countdown which will be displayed on the screen
        /// </summary>
        /// <param name="Time">The time from which the countdown will start</param>
        /// <param name="early">When this time is remaining the early function is called</param>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="point">This point will be the middle point of the countdown on the canvas</param>
        /// <param name="canvas">The canvas on which the Countdown will be drawn</param>
        public CountDownCircle(IOptionsMonitor<PhotoboxConfig> config)
        {

            totalTime = TimeSpan.FromSeconds(config.CurrentValue.CountDown.TotalSeconds);
            
            earlyTime = totalTime - TimeSpan.FromSeconds(config.CurrentValue.CountDown.EarlySeconds);

            if (earlyTime > totalTime)
            {
                throw new ArgumentException("Early seconds cant be lager than the total countdown time!!");
            }

            angle = startAngle;

            circumference = 200.0d;

            timer.Interval = TimeSpan.FromSeconds(totalTime.TotalSeconds / angle);

            timer.Tick += (s, e) =>
            {
                if (stopwatch.Elapsed.Seconds !=  totalTime.Seconds - countDownTime)
                {
                    stopwatch.Stop();
                    countDownTime--;
                    textBlockCountdown.Text = countDownTime.ToString();
                    stopwatch.Start();
                }

                SetAngleBasedOnTime();

                if (stopwatch.Elapsed >= totalTime)
                {
                    timer.Stop();
                    CountDownExpired?.Invoke(this);
                    Panel.Children.Remove(textBlockCountdown);
                    Panel.Children.Remove(path);
                }
            };

            earlyTimer.Interval = earlyTime;
            earlyTimer.Tick += (s, e) =>
            {
                CountDownEarly?.Invoke(this);
                ((DispatcherTimer)s!).Stop();
            };

            expiredTimer.Interval = totalTime;

            expiredTimer.Tick += (s, e) =>
            {
                CountDownExpired?.Invoke(this);
                ((DispatcherTimer)s!).Stop();
            };
        }

        /// <summary>
        /// Starts the countdown
        /// </summary>
        public void StartCountDown()
        {
            ArgumentNullException.ThrowIfNull(Panel);

            angle = startAngle;

            countDownTime = totalTime.TotalSeconds;

            DrawTextBoxCountdown();

            CreateArc();

            StartTimers();

            stopwatch.Restart();
        }

        private void StartTimers()
        {
            timer.Start();
            earlyTimer.Start();
            expiredTimer.Start();
        }

        private void CreateArc()
        {
            // Define the start point on the perimeter of the circle, at the top center position relative to _point.
            var startPoint = new Point(point.X, point.Y - circumference);

            // Calculate the end point of the arc based on the current angle
            var endPoint = new Point(
                point.X + circumference * -Math.Sin(angle * Math.PI / 180),
                point.Y + circumference * -Math.Cos(angle * Math.PI / 180));

            // Update the ArcSegment properties
            arc.Point = endPoint;
            arc.Size = new Size(circumference, circumference);
            arc.RotationAngle = 0;
            arc.IsLargeArc = angle >= 180; // Determines if the arc is greater than 180 degrees
            arc.SweepDirection = SweepDirection.Counterclockwise;
            arc.IsStroked = true;

            // Update the PathFigure with the calculated start and end points
            var figure = new PathFigure { StartPoint = startPoint };
            figure.Segments.Add(arc);

            // Create and set the path geometry
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            path.Data = geometry;

            // Set the stroke (outline) color and thickness
            path.Stroke = Brushes.Black; // You can change this color as needed
            path.StrokeThickness = 20;   // You can change this thickness as needed

            // Position the path so it's centered on _point
            Canvas.SetLeft(path, point.X - circumference);
            Canvas.SetTop(path, point.Y - circumference);

            // Add the Path element to the canvas
            Panel.Children.Add(path);
        }

        private void SetAngleBasedOnTime()
        {
            // Calculate the elapsed time as a fraction of the total time
            double elapsedTime = stopwatch.Elapsed.TotalSeconds;
            double progress = elapsedTime / totalTime.TotalSeconds;

            // Calculate the new angle based on the progress (from 359 to 0 degrees)
            angle = startAngle * (1 - progress);

            // Calculate the end point based on the new angle
            var endPoint = new Point(
               point.X + circumference * -Math.Sin(angle * Math.PI / 180),
               point.Y + circumference * -Math.Cos(angle * Math.PI / 180));

            // Update the arc properties
            arc.Point = endPoint;
            arc.IsLargeArc = angle >= 180;

            Panel.UpdateLayout();
        }

        private void DrawTextBoxCountdown()
        {
            textBlockCountdown.VerticalAlignment = VerticalAlignment.Center;

            textBlockCountdown.HorizontalAlignment = HorizontalAlignment.Center;

            textBlockCountdown.FontSize = 150;

            textBlockCountdown.TextAlignment = TextAlignment.Center;

            textBlockCountdown.Text = countDownTime.ToString();

            textBlockCountdown.Loaded += TextBlockCountdown_Loaded;

            Panel.Children.Add(textBlockCountdown);
        }

        private void TextBlockCountdown_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(textBlockCountdown, point.X - textBlockCountdown.ActualWidth / 2);
            Canvas.SetTop(textBlockCountdown, point.Y - textBlockCountdown.ActualHeight / 2);
        }
    }
}
