using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Extensions.Options;
using Photobox.UI.Lib.ConfigModels;

namespace Photobox.UI.CountDown
{
    public class CountDownCircle : ICountDown
    {
        public event TimerCallback? CountDownExpired;

        public event TimerCallback? CountDownEarly;

        private readonly TextBlock _textBlockCountdown = new();

        private readonly DispatcherTimer _timer = new();

        private readonly DispatcherTimer _earlyTimer = new();

        private readonly DispatcherTimer _expiredTimer = new();

        private readonly double _circumference = 0;

        private double _countDownTime = 0;

        private Point _point;

        private readonly ArcSegment _arc = new();

        private readonly Path _path = new();

        private const double StartAngle = 359;

        private double _angle = 0;

        private readonly Stopwatch _stopwatch = new();

        private readonly TimeSpan _totalTime;

        public Panel Panel
        {
            set
            {
                value.Loaded += (o, a) =>
                {
                    _point = new Point(
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
            _totalTime = TimeSpan.FromSeconds(config.CurrentValue.CountDown.TotalSeconds);

            TimeSpan earlyTime =
                _totalTime - TimeSpan.FromSeconds(config.CurrentValue.CountDown.EarlySeconds);

            if (earlyTime > _totalTime)
            {
                throw new ArgumentException(
                    "Early seconds cant be lager than the total countdown time!!"
                );
            }

            _angle = StartAngle;

            _circumference = 200.0d;

            _timer.Interval = TimeSpan.FromSeconds(_totalTime.TotalSeconds / _angle);

            _timer.Tick += (s, e) =>
            {
                if (_stopwatch.Elapsed.Seconds != _totalTime.Seconds - _countDownTime)
                {
                    _stopwatch.Stop();
                    _countDownTime--;
                    _textBlockCountdown.Text = _countDownTime.ToString();
                    _stopwatch.Start();
                }

                SetAngleBasedOnTime();
            };

            _earlyTimer.Interval = earlyTime;

            _earlyTimer.Tick += (s, e) =>
            {
                CountDownEarly?.Invoke(this);
                ((DispatcherTimer)s!).Stop();
            };

            _expiredTimer.Interval = _totalTime;

            _expiredTimer.Tick += (s, e) =>
            {
                ((DispatcherTimer)s!).Stop();
                CountDownExpired?.Invoke(this);
                Panel.Children.Remove(_textBlockCountdown);
                Panel.Children.Remove(_path);
            };
        }

        /// <summary>
        /// Starts the countdown
        /// </summary>
        public void StartCountDown()
        {
            ArgumentNullException.ThrowIfNull(Panel);

            _angle = StartAngle;

            _countDownTime = _totalTime.TotalSeconds;

            DrawTextBoxCountdown();

            CreateArc();

            StartTimers();

            _stopwatch.Restart();
        }

        private void StartTimers()
        {
            _timer.Start();
            _earlyTimer.Start();
            _expiredTimer.Start();
        }

        private void CreateArc()
        {
            // Define the start point on the perimeter of the circle, at the top center position relative to _point.
            var startPoint = new Point(_point.X, _point.Y - _circumference);

            // Calculate the end point of the arc based on the current angle
            var endPoint = new Point(
                _point.X + _circumference * -Math.Sin(_angle * Math.PI / 180),
                _point.Y + _circumference * -Math.Cos(_angle * Math.PI / 180)
            );

            // Update the ArcSegment properties
            _arc.Point = endPoint;
            _arc.Size = new Size(_circumference, _circumference);
            _arc.RotationAngle = 0;
            _arc.IsLargeArc = _angle >= 180; // Determines if the arc is greater than 180 degrees
            _arc.SweepDirection = SweepDirection.Counterclockwise;
            _arc.IsStroked = true;

            // Update the PathFigure with the calculated start and end points
            var figure = new PathFigure { StartPoint = startPoint };
            figure.Segments.Add(_arc);

            // Create and set the path geometry
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            _path.Data = geometry;

            // Set the stroke (outline) color and thickness
            _path.Stroke = Brushes.Black; // You can change this color as needed
            _path.StrokeThickness = 20; // You can change this thickness as needed

            // Position the path so it's centered on _point
            Canvas.SetLeft(_path, _point.X - _circumference);
            Canvas.SetTop(_path, _point.Y - _circumference);

            // Add the Path element to the canvas
            Panel.Children.Add(_path);
        }

        private void SetAngleBasedOnTime()
        {
            // Calculate the elapsed time as a fraction of the total time
            double elapsedTime = _stopwatch.Elapsed.TotalSeconds;
            double progress = elapsedTime / _totalTime.TotalSeconds;

            // Calculate the new angle based on the progress (from 359 to 0 degrees)
            _angle = StartAngle * (1 - progress);

            // Calculate the end point based on the new angle
            var endPoint = new Point(
                _point.X + _circumference * -Math.Sin(_angle * Math.PI / 180),
                _point.Y + _circumference * -Math.Cos(_angle * Math.PI / 180)
            );

            // Update the arc properties
            _arc.Point = endPoint;
            _arc.IsLargeArc = _angle >= 180;

            Panel.UpdateLayout();
        }

        private void DrawTextBoxCountdown()
        {
            _textBlockCountdown.VerticalAlignment = VerticalAlignment.Center;

            _textBlockCountdown.HorizontalAlignment = HorizontalAlignment.Center;

            _textBlockCountdown.FontSize = 150;

            _textBlockCountdown.TextAlignment = TextAlignment.Center;

            _textBlockCountdown.Text = _countDownTime.ToString();

            _textBlockCountdown.Loaded += TextBlockCountdown_Loaded;

            Panel.Children.Add(_textBlockCountdown);
        }

        private void TextBlockCountdown_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(_textBlockCountdown, _point.X - _textBlockCountdown.ActualWidth / 2);
            Canvas.SetTop(_textBlockCountdown, _point.Y - _textBlockCountdown.ActualHeight / 2);
        }
    }
}
