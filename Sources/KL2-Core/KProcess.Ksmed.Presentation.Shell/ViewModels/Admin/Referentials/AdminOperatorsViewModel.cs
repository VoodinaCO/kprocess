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
    /// Représente le modèle de vue de l'écran de gestion des opérateurs des projets.
    /// </summary>
    class AppOperatorsViewModel : ViewModelBase, IAdminOperatorsViewModel
    {

        #region Champs privés

        #endregion

        #region Propriétés

        /// <inheritdoc />
        public IAdminReferentialsViewModel ParentViewModel { get; set; }

        /// <inheritdoc />
        public bool HasExtraFeatures { get { return true; } }

        #endregion

        #region Surcharges

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override async Task OnInitializeDesigner()
        {
            await base.OnInitializeDesigner();
        }

        #endregion

        #region Implémentation

        /// <inheritdoc />
        public async Task LoadItems()
        {
            var (Operators, Processes) = await ServiceBus.Get<IReferentialsService>().LoadOperators();
            ParentViewModel.SetItemsSource(Operators, Processes);
        }

        /// <inheritdoc />
        public async Task SaveItems(IEnumerable<IActionReferential> items)
        {
            await ServiceBus.Get<IReferentialsService>().SaveResources(items.Cast<Operator>());
            await LoadItems();
        }

        /// <inheritdoc />
        public IActionReferential CreateStandardReferential() =>
            new Operator();

        /// <inheritdoc />
        public IActionReferentialProcess CreateProcessReferential()
        {
            var processesCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(ParentViewModel.Processes);
            return new Operator()
            {
                Process = (Procedure)processesCollectionView.GetItemAt(0)
            };
        }

        /// <inheritdoc />
        public void OnCurrentItemChanged(IActionReferential oldValue, IActionReferential newValue)
        {
        }

        /// <inheritdoc />
        public void UninitializeRemovedItem(Models.IActionReferential item)
        {
        }

        #endregion



    }
}