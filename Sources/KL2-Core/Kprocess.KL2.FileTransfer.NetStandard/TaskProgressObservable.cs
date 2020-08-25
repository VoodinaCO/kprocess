using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kprocess.KL2.FileTransfer
{
    public class TaskProgressObservable
    {
        public Task Task { get; set; }

        Progress<double> _progress;
        public Progress<double> Progress
        {
            get => _progress;
            set
            {
                if (_progress != null)
                    _progress.ProgressChanged -= OnProgressChanged;
                _progress = value;
                if (_progress != null)
                    _progress.ProgressChanged += OnProgressChanged;
            }
        }

        public double ProgressValue { get; private set; }

        public CancellationToken CancellationToken { get; set; }

        void OnProgressChanged(object sender, double e)
        {
            ProgressValue = e;
        }
    }

    public class TaskProgressObservable<T>
    {
        public Task<T> Task { get; set; }

        Progress<double> _progress;
        public Progress<double> Progress
        {
            get => _progress;
            set
            {
                if (_progress != null)
                    _progress.ProgressChanged -= OnProgressChanged;
                _progress = value;
                if (_progress != null)
                    _progress.ProgressChanged += OnProgressChanged;
            }
        }

        public double ProgressValue { get; private set; }

        public CancellationToken CancellationToken { get; set; }

        void OnProgressChanged(object sender, double e)
        {
            ProgressValue = e;
        }
    }
}
