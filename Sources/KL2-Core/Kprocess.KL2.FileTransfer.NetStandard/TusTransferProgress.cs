using System;
using System.Diagnostics;

namespace Kprocess.KL2.FileTransfer
{
    public class TusTransferProgress : NotifiableObject
    {
        public Stopwatch stopWatch = new Stopwatch();

        public double PercentBytesTransferred
        {
            get
            {
                double transferred = Convert.ToDouble(BytesTransferred);
                double total = Convert.ToDouble(BytesTotal);
                double perc = transferred / total * 100.0;
                return perc;
            }
        }

        public long BytesTotal { get; set; }

        long _bytesTransferred;
        public long BytesTransferred
        {
            get => _bytesTransferred;
            set
            {
                if (_bytesTransferred != value)
                {
                    double seconds = stopWatch.IsRunning ? stopWatch.ElapsedMilliseconds / 1000 : 0;
                    long elapsedMs = stopWatch.ElapsedMilliseconds;
                    long speed = stopWatch.IsRunning && elapsedMs != 0 ? (value - _bytesTransferred) / elapsedMs * 1000 : 0;
                    stopWatch.Restart();
                    _bytesTransferred = value;
                    RaisePropertyChanged();
                    Speed = Math.Round((double)speed / 1024 / 1024, 2);
                }
            }
        }

        public int FilesTotal =>
            1;

        public int FilesTransferred =>
            0;

        /// <summary>
        /// Speed in Mo/s
        /// </summary>
        double _speed;
        public double Speed
        {
            get => _speed;
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
