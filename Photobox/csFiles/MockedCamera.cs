using EOSDigital.API;
using EOSDigital.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using EDSKLib.API;
using System.Windows.Threading;
using Moq;
using System.IO;

namespace Photobox
{
    internal class MockedCamera : ICamera
    {
        public event LiveViewUpdate? LiveViewUpdated;

        public event DownloadHandler? DownloadReady;

        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private readonly DispatcherTimer _timerLVUpdate = new DispatcherTimer();

        private const int _framesPerSecond = 1;

        private const double _timeToFocus = 1;

        public MockedCamera()
        {
            _timer.Interval = TimeSpan.FromSeconds(_timeToFocus);

            _timer.Tick += Timer_Tick;

            _timerLVUpdate.Interval = TimeSpan.FromSeconds(1 / _framesPerSecond);

            _timerLVUpdate.Tick += TimerLVUpdate_Tick;
        }

        private void TimerLVUpdate_Tick(object? sender, EventArgs e)
        {
            //using (Stream stream = File.OpenRead("./Resources/Test.JPG"))
            //{
            //    LiveViewUpdated(this, stream);
            //}
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _timer.Stop();

            if(DownloadReady is null) { return; }

            DownloadReady(this, new DownloadInfo() { FileName = "./Resources/Test.JPG"}) ;
        }

        public void CloseSession()
        {
            Debug.WriteLine("CloseSession called");
        }

        public void Dispose()
        {
            Debug.WriteLine("Dispose called");
        }

        public void OpenSession()
        {
            Debug.WriteLine("OpenSession called");
        }

        public void SendCommand(CameraCommand command, int inParam = 0)
        {
            Debug.WriteLine($"SendCommand {command} {inParam}");
        }

        public void SetCapacity(int bytesPerSector, int numberOfFreeClusters)
        {
            Debug.WriteLine($"SetCapacity {bytesPerSector} {numberOfFreeClusters}");
        }

        public void SetSetting(PropertyID propID, object value, int inParam = 0)
        {
            Debug.WriteLine($"SetSetting {propID} {value} {inParam}");
        }

        public void StartLiveView()
        {
            Debug.WriteLine("StartLiveView called");

            _timerLVUpdate.Start();
        }

        public void TakePhoto()
        {
            Debug.WriteLine("TakePhoto called");
            _timer.Start();
        }

        public void TakePhotoAsync()
        {
            Debug.WriteLine("TakePhoto called");
            _timer.Start();
        }

        public void DownloadFile(DownloadInfo Info, string directory)
        {
            string filename = Path.GetFileName(Info.FileName);
            string filepath = Path.Combine(directory, filename);
            File.Copy(Info.FileName, filepath, true);
        }
    }
}
