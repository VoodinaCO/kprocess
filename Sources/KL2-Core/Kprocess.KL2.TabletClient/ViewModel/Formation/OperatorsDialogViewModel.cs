using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.Models;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    public class OperatorsDialogViewModel : ViewModelBase
    {
        #region Attributs

        ICommand _returnCommand;

        List<UIUser> _operators = new List<UIUser>();

        #endregion

        #region Properties

        /// <summary>
        /// Obtient la commande permettant de revenir en arri�re
        /// </summary>
        public ICommand ReturnCommand
        {
            get
            {
                if (_returnCommand == null)
                    _returnCommand = new RelayCommand(async () => await ExecuteReturnCommand());

                return _returnCommand;
            }
        }

        /// <summary>
        /// Obtient ou d�finit la publication courante
        /// </summary>
        public Publication Publication
        {
            get => Locator.Main.TrainingPublication;
            set
            {
                if (Locator.Main.TrainingPublication != value)
                {
                    Locator.Main.TrainingPublication = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la liste des personnes s�lectionn�es
        /// </summary>
        public List<UIUser> Operators
        {
            get => _operators;
            set
            {
                if (_operators != value)
                {
                    _operators = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// M�thode permettant de revenir sur l'�cran pr�c�dent
        /// </summary>
        Task ExecuteReturnCommand() =>
            Locator.Navigation.PopModal();

        #endregion        
    }
}