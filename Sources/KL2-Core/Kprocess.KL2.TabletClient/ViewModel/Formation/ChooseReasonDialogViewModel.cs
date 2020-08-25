using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Kprocess.KL2.TabletClient.Models;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    public class ChooseReasonDialogViewModel : ViewModelBase
    {
        #region Events

        public event EventHandler<QualificationReason> OnClose;

        #endregion

        #region Attributs

        public string _userReason = string.Empty;
        public bool _showUserReason;

        #endregion

        #region Properties

        /// <summary>
        /// OObtient la commande permettant de valider la saisie de l'utilisateur
        /// </summary>
        ICommand _validateCommand;
        public ICommand ValidateCommand
        {
            get
            {
                if (_validateCommand == null)
                    _validateCommand = new RelayCommand(async () =>
                    {
                        if (SelectedReason.IsEditable)
                            return;

                        var reason = (QualificationReason)SelectedReason.Clone();

                        OnClose?.Invoke(this, reason);
                        await Locator.Navigation.PopModal();
                    }, () =>
                    {
                        return SelectedReason != null;
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
                        var reason = (QualificationReason)SelectedReason.Clone();
                        reason.Comment = string.IsNullOrEmpty(UserReason) ? reason.Comment : UserReason;

                        OnClose?.Invoke(this, reason);
                        await Locator.Navigation.PopModal();
                    });
                return _returnCommand;
            }
        }

        /// <summary>
        /// Obtient la commande exécutée lors du chargement
        /// </summary>
        ICommand _loadedCommand;
        public ICommand LoadedCommand
        {
            get
            {
                if (_loadedCommand == null)
                    _loadedCommand = new RelayCommand(async () =>
                    {
                        if (Reasons == null || Reasons.Count == 0)
                        {
                            if (Locator.APIManager.IsOnline == true) // Mode connecté
                                Reasons = new BindingList<QualificationReason>(
                                    await Locator.GetService<IPrepareService>().GetQualificationReasons());
                            else
                                Reasons = await OfflineFile.QualificationReasons.GetFromJson<BindingList<QualificationReason>>();
                        }
                    });
                return _loadedCommand;
            }
        }

        /// <summary>
        /// Obtient la liste des raisons
        /// </summary>
        BindingList<QualificationReason> _reasons;
        public BindingList<QualificationReason> Reasons
        {
            get => _reasons;
            private set
            {
                if (_reasons != value)
                {
                    _reasons = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la raison sélectionné par l'utilisateur
        /// </summary>
        QualificationReason _selectedReason;
        public QualificationReason SelectedReason
        {
            get => _selectedReason;
            set
            {
                if (_selectedReason != value)
                {
                    _selectedReason = value;
                    RaisePropertyChanged();
                }
                ShowUserReason = _selectedReason != null && _selectedReason.IsEditable;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserReason
        {
            get => _userReason;
            set
            {
                if (_userReason != value)
                {
                    _userReason = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowUserReason
        {
            get => _showUserReason;
            private set
            {
                if (_showUserReason != value)
                {
                    _showUserReason = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public ChooseReasonDialogViewModel()
        {
        }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public ChooseReasonDialogViewModel(int? previousId, string comment, BindingList<QualificationReason> reasons = null)
        {
            if (reasons != null)
            {
                //Anh Le TODO:
                //Move isEditable "Autre" to bottom should be change if have more than one
                var list = reasons.Where(x => !x.IsEditable).ToList();
                list.AddRange(reasons.Where(x => x.IsEditable));
                Reasons = new BindingList<QualificationReason>(list);
            }
            SelectedReason = previousId.HasValue ? Reasons.FirstOrDefault(x => x.Id == previousId) : null;
            if (SelectedReason != null && SelectedReason.IsEditable)
                UserReason = comment;
        }

        #endregion    
    }
}