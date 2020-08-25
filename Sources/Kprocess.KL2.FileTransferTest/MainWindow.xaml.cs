using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.FileTransfer;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Kprocess.KL2.FileTransferTest
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        const string DownloadOperationTestName = "Test download";
        const string UploadOperationTestName = "Test upload";

        bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        DownloadOperation _downloadOperation;
        public DownloadOperation DownloadOperation
        {
            get => _downloadOperation;
            set
            {
                if (_downloadOperation != value)
                {
                    if (_downloadOperation != null)
                        _downloadOperation.OnTransferFinished -= DownloadOperation_OnTransferFinished_Typed;
                    _downloadOperation = value;
                    if (_downloadOperation != null)
                        _downloadOperation.OnTransferFinished += DownloadOperation_OnTransferFinished_Typed;
                    RaisePropertyChanged();
                }
            }
        }

        UploadOperation _uploadOperation;
        public UploadOperation UploadOperation
        {
            get => _uploadOperation;
            set
            {
                if (_uploadOperation != value)
                {
                    if (_uploadOperation != null)
                        _uploadOperation.OnTransferFinished -= UploadOperation_OnTransferFinished;
                    _uploadOperation = value;
                    if (_uploadOperation != null)
                        _uploadOperation.OnTransferFinished += UploadOperation_OnTransferFinished;
                    RaisePropertyChanged();
                }
            }
        }

        TusOperation _tusDownloadOperation;
        public TusOperation TusDownloadOperation
        {
            get => _tusDownloadOperation;
            set
            {
                if (_tusDownloadOperation != value)
                {
                    if (_tusDownloadOperation != null)
                        _tusDownloadOperation.OnTransferFinished -= DownloadOperation_OnTransferFinished_Typed;
                    _tusDownloadOperation = value;
                    if (_tusDownloadOperation != null)
                        _tusDownloadOperation.OnTransferFinished += DownloadOperation_OnTransferFinished_Typed;
                    RaisePropertyChanged();
                }
            }
        }

        TusOperation _tusUploadOperation;
        public TusOperation TusUploadOperation
        {
            get => _tusUploadOperation;
            set
            {
                if (_tusUploadOperation != value)
                {
                    if (_tusUploadOperation != null)
                        _tusUploadOperation.OnTransferFinished -= UploadOperation_OnTransferFinished;
                    _tusUploadOperation = value;
                    if (_tusUploadOperation != null)
                        _tusUploadOperation.OnTransferFinished += UploadOperation_OnTransferFinished;
                    RaisePropertyChanged();
                }
            }
        }

        void DownloadOperation_OnTransferFinished_Typed(object sender, JobType e)
        {
            DownloadOperation = null;
            MessageBox.Show("Le téléchargement est terminé");
        }

        void UploadOperation_OnTransferFinished(object sender, JobType e)
        {
            UploadOperation = null;
            MessageBox.Show("L'envoi est terminé");
        }

        void DownloadOperation_OnTransferFinished_Typed(object sender, EventArgs e)
        {
            DownloadOperation = null;
            MessageBox.Show("Le téléchargement est terminé");
        }

        void UploadOperation_OnTransferFinished(object sender, EventArgs e)
        {
            UploadOperation = null;
            MessageBox.Show("L'envoi est terminé");
        }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                DataContext = this;
                _manager = new FileTransferManager();
                _tusManager = TusFileTransferManager.Instance;

                DownloadOperation = _manager.DownloadOperations.ContainsKey(DownloadOperationTestName) ? (DownloadOperation)_manager.DownloadOperations[DownloadOperationTestName] : null;
                DownloadOperation?.Resume();

                UploadOperation = _manager.UploadOperations.ContainsKey(UploadOperationTestName) ? (UploadOperation)_manager.UploadOperations[UploadOperationTestName] : null;
                UploadOperation?.Resume();
            };
        }

        FileTransferManager _manager;
        TusFileTransferManager _tusManager;

        ICommand _downloadCommand;
        public ICommand DownloadCommand
        {
            get
            {
                if (_downloadCommand == null)
                    _downloadCommand = new RelayCommand(() =>
                    {
                        if (DownloadOperation != null)
                        {
                            DownloadOperation.Resume();
                        }
                        else
                        {
                            /*DownloadOperation = _manager.CreateDownload(DownloadOperationTestName,
                                (@"http://localhost:8082/GetFile/test1.mp4", @"C:\Temp\test1.mp4"),
                                (@"http://localhost:8082/GetFile/test2.wmv", @"C:\Temp\test2.wmv"));*/
                            DownloadOperation = _manager.CreateDownload(DownloadOperationTestName,
                                (@"http://localhost:8082/GetFile/download.mkv", @"C:\Temp\download.mkv"));
                            DownloadOperation.Resume();
                        }
                    });
                return _downloadCommand;
            }
        }

        ICommand _downloadAsyncCommand;
        public ICommand DownloadAsyncCommand
        {
            get
            {
                if (_downloadAsyncCommand == null)
                    _downloadAsyncCommand = new RelayCommand(async () =>
                    {
                        if (DownloadOperation != null)
                        {
                            DownloadOperation.Resume();
                        }
                        else
                        {
                            /*DownloadOperation = _manager.CreateDownload(DownloadOperationTestName,
                                (@"http://localhost:8082/GetFile/test1.mp4", @"C:\Temp\test1.mp4"),
                                (@"http://localhost:8082/GetFile/test2.wmv", @"C:\Temp\test2.wmv"));*/
                            DownloadOperation = _manager.CreateDownload(DownloadOperationTestName,
                                (@"http://localhost:8082/GetFile/download.mkv", @"C:\Temp\download.mkv"));
                            DownloadOperation.Resume();
                        }
                        IsLoading = true;
                        await DownloadOperation.WaitTransferFinished();
                        IsLoading = false;
                    });
                return _downloadAsyncCommand;
            }
        }

        ICommand _pauseDownloadCommand;
        public ICommand PauseDownloadCommand
        {
            get
            {
                if (_pauseDownloadCommand == null)
                    _pauseDownloadCommand = new RelayCommand(() =>
                    {
                        DownloadOperation.Pause();
                    });
                return _pauseDownloadCommand;
            }
        }

        ICommand _cancelDownloadCommand;
        public ICommand CancelDownloadCommand
        {
            get
            {
                if (_cancelDownloadCommand == null)
                    _cancelDownloadCommand = new RelayCommand(() =>
                    {
                        DownloadOperation.Cancel();
                        DownloadOperation = null;
                    });
                return _cancelDownloadCommand;
            }
        }

        ICommand _uploadCommand;
        public ICommand UploadCommand
        {
            get
            {
                if (_uploadCommand == null)
                    _uploadCommand = new RelayCommand(() =>
                    {
                        if (UploadOperation != null)
                        {
                            UploadOperation.Resume();
                        }
                        else
                        {
                            UploadOperation = _manager.CreateUpload(UploadOperationTestName,
                                @"http://localhost:8082/files/test.ts",
                                @"C:\Temp\test.ts");
                            UploadOperation.Resume();
                        }
                    });
                return _uploadCommand;
            }
        }

        ICommand _uploadAsyncCommand;
        public ICommand UploadAsyncCommand
        {
            get
            {
                if (_uploadAsyncCommand == null)
                    _uploadAsyncCommand = new RelayCommand(async () =>
                    {
                        if (UploadOperation != null)
                        {
                            UploadOperation.Resume();
                        }
                        else
                        {
                            UploadOperation = _manager.CreateUpload(DownloadOperationTestName,
                                @"http://localhost:8082/test/test.ts",
                                @"C:\Temp\test.ts");
                            UploadOperation.Resume();
                        }
                        IsLoading = true;
                        await UploadOperation.WaitTransferFinished();
                        IsLoading = false;
                        UploadOperation = null;
                    });
                return _uploadAsyncCommand;
            }
        }

        ICommand _pauseUploadCommand;
        public ICommand PauseUploadCommand
        {
            get
            {
                if (_pauseUploadCommand == null)
                    _pauseUploadCommand = new RelayCommand(() =>
                    {
                        UploadOperation.Pause();
                    });
                return _pauseUploadCommand;
            }
        }

        ICommand _cancelUploadCommand;
        public ICommand CancelUploadCommand
        {
            get
            {
                if (_cancelUploadCommand == null)
                    _cancelUploadCommand = new RelayCommand(() =>
                    {
                        UploadOperation.Cancel();
                        UploadOperation = null;
                    });
                return _cancelUploadCommand;
            }
        }

        ICommand _tusDownloadCommand;
        public ICommand TusDownloadCommand
        {
            get
            {
                if (_tusDownloadCommand == null)
                    _tusDownloadCommand = new RelayCommand(() =>
                    {
                        var fileInfo = new FileInfo(@"C:\Temp\test\test.ts");
                        //TusDownloadOperation = _tusManager.Download(@"http://localhost:8082/files/test.ts", fileInfo);
                    });
                return _tusDownloadCommand;
            }
        }

        ICommand _tusDownloadAsyncCommand;
        public ICommand TusDownloadAsyncCommand
        {
            get
            {
                if (_tusDownloadAsyncCommand == null)
                    _tusDownloadAsyncCommand = new RelayCommand(async () =>
                    {
                        if (DownloadOperation != null)
                        {
                            DownloadOperation.Resume();
                        }
                        else
                        {
                            DownloadOperation = _manager.CreateDownload(DownloadOperationTestName,
                                (@"http://localhost:8082/GetFile/test1.mp4", @"C:\Temp\test1.mp4"),
                                (@"http://localhost:8082/GetFile/test2.wmv", @"C:\Temp\test2.wmv"));
                            DownloadOperation.Resume();
                        }
                        IsLoading = true;
                        await DownloadOperation.WaitTransferFinished();
                        IsLoading = false;
                    });
                return _tusDownloadAsyncCommand;
            }
        }

        ICommand _tusPauseDownloadCommand;
        public ICommand TusPauseDownloadCommand
        {
            get
            {
                if (_tusPauseDownloadCommand == null)
                    _tusPauseDownloadCommand = new RelayCommand(() =>
                    {
                        DownloadOperation.Pause();
                    });
                return _tusPauseDownloadCommand;
            }
        }

        ICommand _tusCancelDownloadCommand;
        public ICommand TusCancelDownloadCommand
        {
            get
            {
                if (_tusCancelDownloadCommand == null)
                    _tusCancelDownloadCommand = new RelayCommand(() =>
                    {
                        DownloadOperation.Cancel();
                        DownloadOperation = null;
                    });
                return _tusCancelDownloadCommand;
            }
        }

        ICommand _tusUploadCommand;
        public ICommand TusUploadCommand
        {
            get
            {
                if (_tusUploadCommand == null)
                    _tusUploadCommand = new RelayCommand(() =>
                    {
                        var fileInfo = new FileInfo(@"C:\Temp\test.ts");
                        TusUploadOperation = _tusManager.Upload(@"http://localhost:8082/files", fileInfo, false);
                    });
                return _tusUploadCommand;
            }
        }

        ICommand _tusUploadAsyncCommand;
        public ICommand TusUploadAsyncCommand
        {
            get
            {
                if (_tusUploadAsyncCommand == null)
                    _tusUploadAsyncCommand = new RelayCommand(async () =>
                    {
                        var fileInfo = new FileInfo(@"C:\Temp\test.ts");
                        TusUploadOperation = _tusManager.Upload(@"http://localhost:8082/files", fileInfo, false);
                        IsLoading = true;
                        await TusUploadOperation.WaitTransferFinished();
                        IsLoading = false;
                        TusUploadOperation = null;
                    });
                return _tusUploadAsyncCommand;
            }
        }

        ICommand _tusPauseUploadCommand;
        public ICommand TusPauseUploadCommand
        {
            get
            {
                if (_tusPauseUploadCommand == null)
                    _tusPauseUploadCommand = new RelayCommand(() =>
                    {
                        TusUploadOperation.Pause();
                    });
                return _tusPauseUploadCommand;
            }
        }

        ICommand _tusCancelUploadCommand;
        public ICommand TusCancelUploadCommand
        {
            get
            {
                if (_tusCancelUploadCommand == null)
                    _tusCancelUploadCommand = new RelayCommand(() =>
                    {
                        TusUploadOperation.Cancel();
                        TusUploadOperation = null;
                    });
                return _tusCancelUploadCommand;
            }
        }
    }
}
