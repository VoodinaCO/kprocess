using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ReencodeTool
{
    public class TranscodeTask : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisedPropertyChanged([CallerMemberName]string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        readonly DispatcherTimer taskWatcher = new DispatcherTimer();

        FileInfo _fileInfo;
        public FileInfo FileInfo
        {
            get => _fileInfo;
            set
            {
                if (_fileInfo != value)
                {
                    _fileInfo = value;
                    RaisedPropertyChanged();
                }
            }
        }

        Task<bool> _task;
        public Task<bool> Task
        {
            get => _task;
            set
            {
                if (_task != value)
                {
                    _task = value;
                    taskWatcher.Stop();
                    taskWatcher.Interval = TimeSpan.FromSeconds(1);
                    taskWatcher.Tick += (sender, args) => RaisedPropertyChanged(nameof(TranscodingTaskStatus));
                    taskWatcher.Start();
                    RaisedPropertyChanged();
                }
            }
        }

        public TranscodingTaskStatus TranscodingTaskStatus
        {
            get
            {
                switch (_task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        return _task.Result ? TranscodingTaskStatus.OK : TranscodingTaskStatus.Failed;
                    case TaskStatus.Faulted:
                        return TranscodingTaskStatus.Failed;
                    default:
                        return TranscodingTaskStatus.InProgress;
                }
            }
        }

        Progress<double> _progress;
        public Progress<double> Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    if (_progress != null)
                        _progress.ProgressChanged -= OnProgressChanged;
                    _progress = value;
                    if (_progress != null)
                        _progress.ProgressChanged += OnProgressChanged;
                    RaisedPropertyChanged();
                }
            }
        }

        double _progressValue;
        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    RaisedPropertyChanged();
                }
            }
        }

        void OnProgressChanged(object sender, double e)
        {
            ProgressValue = e;
            RaisedPropertyChanged(nameof(TranscodingTaskStatus));
        }
    }

    public enum TranscodingTaskStatus
    {
        InProgress,
        OK,
        Failed
    }
}
