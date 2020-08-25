using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran d'activation.
    /// </summary>
    class ActivationFrameViewModel : FrameContentViewModelBase, IActivationFrameViewModel
    {

        #region Champs privés

        #endregion

        #region Surcharges

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override Task OnLoading() =>
            Task.CompletedTask;

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner() =>
            Task.CompletedTask;

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient le statut de l'activation sour forme d'énumération.
        /// </summary>
        public bool CanSendReport
        {
            get { return IoC.Resolve<IServiceBus>().Get<ISettingsService>().SendReport; }
            set
            {
                IoC.Resolve<IServiceBus>().Get<ISettingsService>().SendReport = value;
                OnPropertyChanged("CanSendReport");
            }
        }

        #endregion

        #region Commandes

        private Command _openWindowCommand;
        public ICommand OpenWindowCommand
        {
            get
            {
                if (_openWindowCommand == null)
                    _openWindowCommand = new Command(() =>
                    {
                        IModalWindowView view = UXFactory.GetView(out IActivationViewModel vm) as IModalWindowView;
                        vm.Load();

                        view.ShowDialog();
                    });
                return _openWindowCommand;
            }
        }

        #endregion

    }
}
