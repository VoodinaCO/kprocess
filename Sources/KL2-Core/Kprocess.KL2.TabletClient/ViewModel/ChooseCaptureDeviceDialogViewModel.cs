using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static Kprocess.KL2.TabletClient.Models.FFMEWebcam;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    public class ChooseCaptureDeviceDialogViewModel : ViewModelBase
    {
        #region Properties

        Visibility _resolutionsVisibility = Visibility.Collapsed;
        public Visibility ResolutionsVisibility
        {
            get => _resolutionsVisibility;
            set
            {
                if (_resolutionsVisibility != value)
                {
                    _resolutionsVisibility = value;
                    RaisePropertyChanged();
                }
            }
        }

        CaptureDeviceInfo _selectedCaptureDevice;
        public CaptureDeviceInfo SelectedCaptureDevice
        {
            get => _selectedCaptureDevice;
            set
            {
                if (_selectedCaptureDevice != value)
                {
                    _selectedCaptureDevice = value;
                    RaisePropertyChanged();
                }
                SelectedVideoResolution = _selectedCaptureDevice.SupportedVideoResolutions.LastOrDefault();
            }
        }

        ResolutionInfo _selectedVideoResolution;
        public ResolutionInfo SelectedVideoResolution
        {
            get => _selectedVideoResolution;
            set
            {
                if (_selectedVideoResolution != value)
                {
                    _selectedVideoResolution = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        ICommand _showResolutionsCommand;
        public ICommand ShowResolutionsCommand
        {
            get
            {
                if (_showResolutionsCommand == null)
                    _showResolutionsCommand = new RelayCommand(() =>
                    {
                        ResolutionsVisibility = Visibility.Visible;
                    });
                return _showResolutionsCommand;
            }
        }

        /// <summary>
        /// OObtient la commande permettant de valider la saisie de l'utilisateur
        /// </summary>
        ICommand _validateCommand;
        public ICommand ValidateCommand
        {
            get
            {
                if(_validateCommand == null)
                    _validateCommand = new RelayCommand(async () =>
                    {
                        Locator.FFMEWebcam.InputSource = SelectedCaptureDevice.Name;
                        Locator.FFMEWebcam.InputVideoSize = SelectedVideoResolution.ToString(false);
                        await Locator.Navigation.PopModal();
                    }, () =>
                    {
                        return true;// !string.IsNullOrEmpty(SelectedReason);
                    });
                return _validateCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de revenir en arrière
        /// </summary>
        ICommand _returnCommand;
        public ICommand ReturnCommand
        {
            get
            {
                if (_returnCommand == null)
                    _returnCommand = new RelayCommand(async () =>
                    {
                        await Locator.Navigation.PopModal();
                    });
                return _returnCommand;
            }
        }

        #endregion
    }
}