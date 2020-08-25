using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    class AdminActionCategoriesViewModel : ViewModelBase, IAdminActionCategoriesViewModel
    {

        #region Champs privés

        private ActionValue[] _values;
        private SelectionNullWrapper<ActionCategory, ActionType> _types;

        #endregion

        #region Propriétés

        /// <inheritdoc />
        public IAdminReferentialsViewModel ParentViewModel { get; set; }

        /// <inheritdoc />
        public bool HasExtraFeatures { get { return true; } }

        /// <summary>
        /// Obtient les types d'actions.
        /// </summary>
        public SelectionNullWrapper<ActionCategory, ActionType> Types
        {
            get { return _types; }
            private set
            {
                if (_types != value)
                {
                    _types = value;
                    OnPropertyChanged("Types");
                }
            }
        }

        /// <summary>
        /// Obtient les valorisations.
        /// </summary>
        public ActionValue[] Values
        {
            get { return _values; }
            private set
            {
                if (_values != value)
                {
                    _values = value;
                    OnPropertyChanged("Values");
                }
            }
        }

        #endregion

        #region Surcharges

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override async Task OnInitializeDesigner()
        {
            await base.OnInitializeDesigner();
            var (Categories, ActionTypes, ActionValues) = DesignData.GenerateActionCategories();
            this.Types = new SelectionNullWrapper<ActionCategory, ActionType>(
                        (ActionCategory)this.ParentViewModel.CurrentItem,
                        c => c.Type,
                        ActionTypes,
                        new ActionType()
                        {
                            ShortLabel = "Aucun",
                            LongLabel = "Aucun"
                        },
                        NullItemPosition.Bottom);
            this.Values = ActionValues.ToArray();
        }

        #endregion

        #region Implémentation

        /// <inheritdoc />
        public async Task LoadItems()
        {
            var (Categories, ActionValues, ActionTypes, Processes) = await ServiceBus.Get<IReferentialsService>().LoadCategories();
            Values = ActionValues;

            // Ajouter la valeur null
            Types = new SelectionNullWrapper<ActionCategory, ActionType>(
                (ActionCategory)ParentViewModel.CurrentItem,
                c => c.Type,
                ActionTypes,
                new ActionType()
                {
                    ShortLabel = LocalizationManager.GetString("View_AppActionCategories_ActionType_None"),
                    LongLabel = LocalizationManager.GetString("View_AppActionCategories_ActionType_None")
                },
                NullItemPosition.Bottom);

            ParentViewModel.SetItemsSource(Categories, Processes);
        }

        /// <inheritdoc />
        public async Task SaveItems(IEnumerable<IActionReferential> items)
        {
            await ServiceBus.Get<IReferentialsService>().SaveCategories(items.Cast<ActionCategory>());
            await LoadItems();
        }

        /// <inheritdoc />
        public IActionReferential CreateStandardReferential() =>
            new ActionCategory
            {
                Value = Values[1]
            };

        /// <inheritdoc />
        public IActionReferentialProcess CreateProcessReferential()
        {
            var processesCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(ParentViewModel.Processes);
            return new ActionCategory()
            {
                Process = (Procedure)processesCollectionView.GetItemAt(0),
                Value = Values[1]
            };
        }

        /// <inheritdoc />
        public void OnCurrentItemChanged(IActionReferential oldValue, IActionReferential newValue)
        {
            this.Types.Container = (ActionCategory)newValue;
        }

        /// <inheritdoc />
        public void UninitializeRemovedItem(Models.IActionReferential item)
        {
            var cat = (ActionCategory)item;
            cat.Value = null;
            cat.Type = null;
        }

        #endregion
    }
}