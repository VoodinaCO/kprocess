using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de gestion des outils.
    /// </summary>
    class AdminRef1ViewModel : ViewModelBase, IAdminRef1ViewModel
    {

        #region Champs privés

        #endregion

        #region Propriétés

        /// <inheritdoc />
        public IAdminReferentialsViewModel ParentViewModel { get; set; }

        /// <inheritdoc />
        public bool HasExtraFeatures { get { return false; } }

        #endregion

        #region Surcharges

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner() =>
            base.OnInitializeDesigner();

        #endregion

        #region Implémentation

        /// <inheritdoc />
        public async Task LoadItems()
        {
            var (Referentials, Processes) = await ServiceBus.Get<IReferentialsService>().GetReferentials(ProcessReferentialIdentifier.Ref1);
            ParentViewModel.SetItemsSource(Referentials, Processes);
        }

        /// <inheritdoc />
        public async Task SaveItems(IEnumerable<IActionReferential> items)
        {
            await ServiceBus.Get<IReferentialsService>().SaveReferentials(items.Cast<Ref1>());
            await LoadItems();
        }

        /// <inheritdoc />
        public IActionReferential CreateStandardReferential() =>
            new Ref1();

        /// <inheritdoc />
        public IActionReferentialProcess CreateProcessReferential()
        {
            var processesCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(ParentViewModel.Processes);
            return new Ref1()
            {
                Process = (Procedure)processesCollectionView.GetItemAt(0)
            };
        }

        /// <inheritdoc />
        public void OnCurrentItemChanged(IActionReferential oldValue, IActionReferential newValue)
        {
        }

        /// <inheritdoc />
        public void UninitializeRemovedItem(IActionReferential item)
        {
        }

        #endregion



    }
}