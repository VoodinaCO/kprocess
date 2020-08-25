using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de fusion des référentiels.
    /// </summary>
    class ReferentialMergeViewModel : ViewModelBase, IReferentialMergeViewModel
    {

        #region Champs privés


        #endregion

        #region Surcharges

        /// <summary>
        /// Obtient le titre
        /// </summary>
        public override string Title
        {
            get { return LocalizationManagerExt.GetSafeDesignerString("VM_ReferentialMergeViewModel_Title"); }
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override Task OnLoading() =>
            Task.CompletedTask;

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            MainReferential = DesignData.GenerateRef1s().First();
            Referentials = DesignData.GenerateResources().ToArray();
            return Task.CompletedTask;
        }

        #endregion

        #region Propriétés

        private IActionReferential _mainReferential;
        /// <summary>
        /// Obtient ou définit le référentiel principal.
        /// </summary>
        public IActionReferential MainReferential
        {
            get { return _mainReferential; }
            set
            {
                if (_mainReferential != value)
                {
                    _mainReferential = value;
                    OnPropertyChanged("MainReferential");
                }
            }
        }

        private IActionReferential[] _referentials;
        /// <summary>
        /// Obtient ou définit les autres référentiels avec lesquels le principal peut être fusionné.
        /// </summary>
        public IActionReferential[] Referentials
        {
            get { return _referentials; }
            set
            {
                if (_referentials != value)
                {
                    _referentials = value;
                    OnPropertyChanged("Referentials");
                }
            }
        }

        private ReferentialsGroupSortDescription _groupSort;
        /// <summary>
        /// Obtient ou définit les régles de regroupement et de tri.
        /// </summary>
        public ReferentialsGroupSortDescription GroupSort
        {
            get { return _groupSort; }
            set
            {
                _groupSort = value;
                if (Referentials != null)
                {
                    var collectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(Referentials);
                    collectionView.GroupDescriptions.Add(_groupSort);
                    collectionView.CustomSort = _groupSort;
                }
            }
        }

        /// <summary>
        /// Obtient le résultat de la fenêtre.
        /// </summary>
        public bool Result { get; private set; }

        #endregion

        #region Commandes

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override void OnCancelCommandExecute()
        {
            this.Result = false;
            this.Shutdown();
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected override void OnValidateCommandExecute()
        {
            this.Result = true;
            this.Shutdown();
        }

        #endregion

    }
}
