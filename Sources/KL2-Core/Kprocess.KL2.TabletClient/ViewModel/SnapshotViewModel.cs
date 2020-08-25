using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Kprocess.KL2.TabletClient.Dialog;
using Kprocess.KL2.TabletClient.Views;
using KProcess.Ksmed.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Unosquare.FFME;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SnapshotViewModel : ViewModelBase, IMediaElementViewModel, IRefreshViewModel
    {
        #region Attributs

        ChooseCaptureDeviceDialogViewModel _chooseCaptureDeviceDialogViewModel;
        bool _isCapturing;

        #endregion

        public Unosquare.FFME.MediaElement MediaElement
        {
            get => Locator.Main.MediaElement;
            set
            {
                if (Locator.Main.MediaElement != value)
                {
                    Locator.Main.MediaElement = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool _returnAfterPhoto;
        public bool ReturnAfterPhoto
        {
            get => _returnAfterPhoto;
            set
            {
                if (_returnAfterPhoto != value)
                {
                    _returnAfterPhoto = value;
                    RaisePropertyChanged();
                }
            }
        }

        MemoryStream _captureStream;
        public MemoryStream CaptureStream
        {
            get => _captureStream;
            set
            {
                if (_captureStream != value)
                {
                    _captureStream = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la publication courante
        /// </summary>
        public Publication Publication
        {
            get => Locator.Main.InspectionPublication;
            set
            {
                if (Locator.Main.InspectionPublication != value)
                {
                    Locator.Main.InspectionPublication = value;
                    RaisePropertyChanged();
                }
            }
        }

        ICommand _loadedCommand;
        public ICommand LoadedCommand
        {
            get
            {
                if (_loadedCommand == null)
                    _loadedCommand = new RelayCommand(async () =>
                    {
                        Locator.Main.IsLoading = true;
                        Locator.Main.ShowDisconnectedMessage = false;

                        if (!Locator.FFMEWebcam.CaptureDevices.Any(_ => _.Name == Locator.FFMEWebcam.InputSource))
                            ((RelayCommand)ShowChooseCaptureDeviceDialogCommand).Execute(null);
                        else
                            await MediaElement.Open(Locator.FFMEWebcam.InputUriSource);

                        Locator.Main.IsLoading = false;
                    });
                return _loadedCommand;
            }
        }

        ICommand _showChooseCaptureDeviceDialogCommand;
        public ICommand ShowChooseCaptureDeviceDialogCommand
        {
            get
            {
                if (_showChooseCaptureDeviceDialogCommand == null)
                    _showChooseCaptureDeviceDialogCommand = new RelayCommand(async () =>
                    {
                        _chooseCaptureDeviceDialogViewModel = new ChooseCaptureDeviceDialogViewModel();
                        await Locator.Navigation.PushDialog<ChooseCaptureDeviceDialog, ChooseCaptureDeviceDialogViewModel>(_chooseCaptureDeviceDialogViewModel);
                    });
                return _showChooseCaptureDeviceDialogCommand;
            }
        }

        ICommand _takeSnapshotCommand;
        public ICommand TakeSnapshotCommand
        {
            get
            {
                if (_takeSnapshotCommand == null)
                    _takeSnapshotCommand = new RelayCommand<MediaElement>(mediaElement =>
                    {
                        mediaElement.RenderingVideo += MediaElement_RenderingVideo;
                    });
                return _takeSnapshotCommand;
            }
        }

        ICommand _returnCommand;
        public ICommand ReturnCommand
        {
            get
            {
                if (_returnCommand == null)
                    _returnCommand = new RelayCommand(async () =>
                    {
                        await Locator.Navigation.Pop();
                    });
                return _returnCommand;
            }
        }

        ICommand _playPauseCommand;
        public ICommand PlayPauseCommand
        {
            get
            {
                if (_playPauseCommand == null)
                    _playPauseCommand = new RelayCommand<MediaElement>(mediaElement =>
                    {
                    });
                return _playPauseCommand;
            }
        }

        ICommand _muteCommand;
        public ICommand MuteCommand
        {
            get
            {
                if (_muteCommand == null)
                    _muteCommand = new RelayCommand<MediaElement>(mediaElement =>
                    {
                    });
                return _muteCommand;
            }
        }

        ICommand _maximizeCommand;
        public ICommand MaximizeCommand
        {
            get
            {
                if (_maximizeCommand == null)
                    _maximizeCommand = new RelayCommand(() =>
                    {
                    });
                return _maximizeCommand;
            }
        }

        async void MediaElement_RenderingVideo(object sender, Unosquare.FFME.Common.RenderingVideoEventArgs e)
        {
            if (!_isCapturing && sender is Unosquare.FFME.MediaElement mediaElement)
            {
                _isCapturing = true;
                mediaElement.RenderingVideo -= MediaElement_RenderingVideo;
                await MediaElement.Stop();
                using (_captureStream = new MemoryStream())
                {
                    e.Bitmap.CreateDrawingBitmap().Save(_captureStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                _isCapturing = false;
                await MediaElement.Close();
                MediaElement = null;
                DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                {
                    if (ReturnAfterPhoto)
                    {
                        await Locator.Navigation.PopWithResult(CaptureStream);
                    }
                    else
                    {
                        var vm = new AddInspectionAnomalyViewModel()
                        {
                            CaptureStream = CaptureStream
                        };
                        await Locator.Navigation.Push<AddInspectionAnomaly, AddInspectionAnomalyViewModel>(vm);
                    }
                });
            }
        }

        public async Task Refresh()
        {
            await MediaElement.Open(Locator.FFMEWebcam.InputUriSource);
        }
    }
}