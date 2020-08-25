using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Validation;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Extensions;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de gestion des référentiels.
    /// </summary>
    public partial class AdminReferentialsViewModel : FrameContentExtensibleViewModelBase<AdminReferentialsViewModel, IAdminReferentialsViewModel>, IAdminReferentialsViewModel
    {
        #region Champs privés

        private IAdminSubReferentialViewModel _currentViewModel;
        private IView _currentSubView;
        private Dictionary<string, Action> _viewsFactory;
        private ExtendedViewItem[] _views;
        private string _selectedView;
        private ProcessReferentialIdentifier _selectedReferential;
        private ProcessReferentialIdentifier _lastModifiedReferential;

        private Procedure[] _processes;
        private BulkObservableCollection<IActionReferential> _items;
        private IActionReferential _currentItem;
        private List<IActionReferential> _itemsToDelete;
        private Visibility _processesListVisibility;
        private ICollectionView _collectionView;
        private bool _hasChanged;
        private ColorConverter _colorConverter;
        private ReferentialsGroupSortDescription _groupSort;

        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            EnableAutoValidation = true;

            _colorConverter = new ColorConverter();
            _groupSort = new ReferentialsGroupSortDescription();
        }

        /// <inheritdoc />
        protected override Task OnInitializeDesigner()
        {
            Procedure process1 = new Procedure { Label = "Process 1" };
            Procedure process2 = new Procedure { Label = "Process 2" };
            //var groupIndex = 0;
            //this.Views = new string[] { "Catégories", "Lieux" }.GroupBy(key => "group" + groupIndex++).ToArray(); // TODO
            //this.SelectedView = this.Views[0].ToArray()[0];
            Items = new BulkObservableCollection<IActionReferential>
            {
                new Operator
                {
                    Label = "Referential1",
                    Color = "#FFFF0000",
                    Process = process1
                },
                new Operator
                {
                    Label = "And Referential2",
                    Color = "#FF00FF00",
                    Process = process2
                }
            };
            return Task.CompletedTask;
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override async Task OnLoading()
        {
            ShowSpinner();

            var selectedViewBackup = SelectedView; // Necessaire à cause d'une regression probable via User Story 4370:Ajouter le mot ressource autour des references. Le temps imparti n'a pas permis d'investiguer d'avantage
            CreateMenu();
            SelectedView = selectedViewBackup;

            if (SelectedView == null)
                SelectedView = Views.First().Key;
            else if (_currentViewModel != null)
                await _currentViewModel.Refresh();
        }

        /// <summary>
        /// Appelé afin de nettoyer les ressources.
        /// </summary>
        protected override void OnCleanup()
        {
            base.OnCleanup();

            if (_currentViewModel != null)
                _currentViewModel.Shutdown();

            if (_items != null)
                foreach (var item in _items)
                    UnregisterItem(item);

            this.CurrentItem = null;
            this.Items = null;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Crée la CollectionView pour les éléments.
        /// </summary>
        public void SetItemsSource(IActionReferential[] items, Procedure[] processes)
        {
            Processes = processes;

            if (!DesignMode.IsInDesignMode)
            {
                foreach (var item in items.OfType<IActionReferentialProcess>())
                {
                    if (item.ProcessId != null)
                        item.Process = Processes.Single(p => p.ProcessId == item.ProcessId);
                    item.MarkAsUnchanged();
                }
            }

            foreach (var item in items)
                item.IsEditable = false;

            // Filtrer les éléments en fonction des autorisations

            if (IsAdmin)
                foreach (var item in items)
                    item.IsEditable = true;
            else if (IsAnalyst)
            {
                foreach (var item in items)
                {
                    if (item is IActionReferentialProcess refProcess && refProcess.ProcessId != null)
                    {
                        // Prendre le process chargé avec les détails
                        var process = Processes.First(p => p.ProcessId == refProcess.ProcessId);

                        // Un analyste peut modifier un référentiel process s'il a les droits d'écriture sur ce process
                        refProcess.IsEditable = process.CanWrite(SecurityContext.CurrentUser?.User);
                    }
                }
            }

            _items = new BulkObservableCollection<IActionReferential>(items);
            var collectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(_items);
            collectionView.GroupDescriptions.Add(_groupSort);
            collectionView.CustomSort = _groupSort;
            _collectionView = collectionView;
            OnPropertyChanged(nameof(Items));

            // On filtre la liste des processes sélectionnables. Il ne devra y avoir que les processes modifiables par l'utilisateur actuellement connecté
            var processesCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(Processes);
            processesCollectionView.Filter = o =>
            {
                var process = ((Procedure)o);
                return process.CanWrite(SecurityContext.CurrentUser.User);
            };

            HideSpinner();
            HasChanged = false;
        }

        #endregion

        #region Propriétés

        public bool IsAdmin => SecurityContext.HasCurrentUserRole(KnownRoles.Administrator)
            && SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.All);

        public bool IsAnalyst => SecurityContext.HasCurrentUserRole(KnownRoles.Analyst)
            && SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.All);

        public ProcessReferentialIdentifier SelectedReferential
        {
            get { return _selectedReferential; }
            private set
            {
                if (_selectedReferential != value)
                {
                    _selectedReferential = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la vue courante.
        /// </summary>
        public IView CurrentView
        {
            get { return _currentSubView; }
            private set
            {
                if (_currentSubView != value)
                {
                    _currentSubView = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les vues.
        /// </summary>
        public ExtendedViewItem[] Views
        {
            get { return _views; }
            private set
            {
                if (_views != value)
                {
                    _views = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la vue sélectionnée
        /// </summary>
        public string SelectedView
        {
            get { return _selectedView; }
            set
            {
                if (_selectedView != value)
                {
                    var originalValue = _selectedView;
                    _selectedView = value;
                    OnPropertyChanged();
                    OnSelectedViewChanged(originalValue, value);
                }
            }
        }

        /// <summary>
        /// Obtient les process disponibles.
        /// </summary>
        public Procedure[] Processes
        {
            get => _processes;
            private set
            {
                if (_processes == value)
                    return;
                _processes = value;
                OnPropertyChanged();
            }
        }

        private string _currentReferentialLabel;
        /// <summary>
        /// Obtient ou définit le libellé du référentiel.
        /// </summary>
        [LocalizableRequired(ErrorMessageResourceName = "Validation_Referential_Label_Required")]
        [LocalizableStringLength(Referential.LabelMaxLength, ErrorMessageResourceName = "Validation_Referential_Label_Required")]
        public string CurrentReferentialLabel
        {
            get { return _currentReferentialLabel; }
            set
            {
                if (_currentReferentialLabel != value)
                {
                    _currentReferentialLabel = value;
                    OnPropertyChanged();
                    OnCurrentReferentialLabelChanged();
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le libellé du référentiel peut être changé.
        /// </summary>
        public bool CanEditReferentialLabel
        {
            get { return SecurityContext.CurrentUser != null && SecurityContext.HasCurrentUserRole(KnownRoles.Administrator); }
        }

        /// <summary>
        /// Obtient les éléments.
        /// </summary>
        public BulkObservableCollection<IActionReferential> Items
        {
            get { return _items; }
            private set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged("Items");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit l'élément courant.
        /// </summary>
        public IActionReferential CurrentItem
        {
            get { return _currentItem; }
            set
            {
                if (_currentItem != value)
                {
                    var old = _currentItem;
                    _currentItem = value;
                    OnCurrentItemChanged(old, value);
                    OnPropertyChanged("CurrentItem");
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si une des éléments de la liste a changé.
        /// </summary>
        public bool HasChanged
        {
            get { return _hasChanged; }
            private set
            {
                if (_hasChanged != value)
                {
                    _hasChanged = value;
                    OnPropertyChanged("HasChanged");
                }
            }
        }

        /// <summary>
        /// Obtient la visibilité de la liste des projets.
        /// </summary>
        public Visibility ProcessesListVisibility
        {
            get { return _processesListVisibility; }
            private set
            {
                if (_processesListVisibility != value)
                {
                    _processesListVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color _currentActionColor;
        /// <summary>
        /// Obtient ou définit la couleur de l'action courante.
        /// </summary>
        public Color CurrentActionColor
        {
            get { return _currentActionColor; }
            set
            {
                if (_currentActionColor != value)
                {
                    _currentActionColor = value;
                    OnPropertyChanged();
                    if (CurrentItem != null)
                    {
                        if (CurrentItem.Color == null
                            || CurrentItem.Color.ToString().ToUpper() != value.ToString().ToUpper())
                            CurrentItem.Color = value.ToString();
                    }
                }
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Crées le menu
        /// </summary>
        private void CreateMenu()
        {
            if (!DesignMode.IsInDesignMode)
            {
                var service = IoC.Resolve<IReferentialsUseService>();

                _viewsFactory = new Dictionary<string, Action>()
                {
                    { service.GetLabel(ProcessReferentialIdentifier.Operator), async () => await ShowSubMenu<IAdminOperatorsViewModel>(ProcessReferentialIdentifier.Operator) },
                    { service.GetLabel(ProcessReferentialIdentifier.Equipment), async () => await ShowSubMenu<IAdminEquipmentsViewModel>(ProcessReferentialIdentifier.Equipment) },
                    { service.GetLabel(ProcessReferentialIdentifier.Category), async () => await ShowSubMenu<IAdminActionCategoriesViewModel>(ProcessReferentialIdentifier.Category) },
                    { service.GetLabel(ProcessReferentialIdentifier.Skill), async () => await ShowSubMenu<IAdminSkillsViewModel>(ProcessReferentialIdentifier.Skill) },
                    { service.GetLabel(ProcessReferentialIdentifier.Ref1), async () => await ShowSubMenu<IAdminRef1ViewModel>(ProcessReferentialIdentifier.Ref1) },
                    { service.GetLabel(ProcessReferentialIdentifier.Ref2), async () => await ShowSubMenu<IAdminRef2ViewModel>(ProcessReferentialIdentifier.Ref2) },
                    { service.GetLabel(ProcessReferentialIdentifier.Ref3), async () => await ShowSubMenu<IAdminRef3ViewModel>(ProcessReferentialIdentifier.Ref3) },
                    { service.GetLabel(ProcessReferentialIdentifier.Ref4), async () => await ShowSubMenu<IAdminRef4ViewModel>(ProcessReferentialIdentifier.Ref4) },
                    { service.GetLabel(ProcessReferentialIdentifier.Ref5), async () => await ShowSubMenu<IAdminRef5ViewModel>(ProcessReferentialIdentifier.Ref5) },
                    { service.GetLabel(ProcessReferentialIdentifier.Ref6), async () => await ShowSubMenu<IAdminRef6ViewModel>(ProcessReferentialIdentifier.Ref6) },
                    { service.GetLabel(ProcessReferentialIdentifier.Ref7), async () => await ShowSubMenu<IAdminRef7ViewModel>(ProcessReferentialIdentifier.Ref7) },
                };

                var resources = _viewsFactory.Keys.Take(2).ToList();
                var newViews = _viewsFactory.Keys.Select(key => new ExtendedViewItem { Key = key, IsResource = resources.Contains(key) }).ToArray();
                if (this.Views == null)
                    this.Views = newViews;
                else//Trouver la vue qui vient d'être modifié et modifier uniquement la clé
                {


                    foreach (var view in this.Views)
                    {
                        if (!_viewsFactory.Keys.Contains(view.Key))
                        {
                            view.Key = service.GetLabel(_lastModifiedReferential);
                            //OnPropertyChanged("Views");
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Affiche le sous-menu spécifié.
        /// </summary>
        /// <typeparam name="TViewModel">Le type du VM.</typeparam>
        /// <param name="id">L'identifiant du référentiel.</param>
        private async Task ShowSubMenu<TViewModel>(ProcessReferentialIdentifier id)
            where TViewModel : IAdminSubReferentialViewModel
        {
            if (_currentViewModel != null)
                _currentViewModel.Shutdown();

            var vm = UXFactory.GetViewModel<TViewModel>();

            IView view = null;
            if (vm.HasExtraFeatures)
            {
                view = UXFactory.GetView<TViewModel>();
                view.DataContext = vm;
            }

            _currentViewModel = vm;
            vm.ParentViewModel = this;

            await vm.Load();

            CurrentView = view;

            SelectedReferential = id;
            CurrentReferentialLabel = IoC.Resolve<IReferentialsUseService>().GetLabel(id);

            // Charger les référentiels
            ShowSpinner();

            if (_items != null)
                foreach (var item in _items)
                    UnregisterItem(item);

            try
            {
                await _currentViewModel.LoadItems();
                HideSpinner();
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        /// <summary>
        /// Supprime les abonnements sur l'élément.
        /// </summary>
        /// <param name="item">L'élément.</param>
        private void UnregisterItem(IActionReferential item)
        {
            if (item is IActionReferentialProcess process && process.ProcessId != null)
                process.ProcessChanged -= OnItemProcessChanged;
        }

        private bool _ignoreSelectedViewChanged;

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="SelectedView"/> a changé.
        /// </summary>
        private async void OnSelectedViewChanged(string oldValue, string newValue)
        {
            if (!DesignMode.IsInDesignMode && !_ignoreSelectedViewChanged)
            {
                if (await OnNavigatingAway(null))
                {
                    OnCleanup();
                    _viewsFactory[SelectedView]();
                }
                else
                {
                    _ignoreSelectedViewChanged = true;
                    await Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
                    {
                        SelectedView = oldValue;
                        _ignoreSelectedViewChanged = false;
                    }), DispatcherPriority.Background);
                }
            }
        }

        /// <summary>
        /// Initialise une nouvelle instance de l'élément lors de l'ajout.
        /// </summary>
        /// <param name="item">L'élément.</param>
        private void InitializeNewItem(IActionReferential item)
        {
            item.Color = ColorsHelper.GetRandomColor(ColorsHelper.StandardColorsExcludedGreenYellowOrangeRed).ToString();
        }

        /// <summary>
        /// A lieu lorsque l'élément courant a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
        protected virtual void OnCurrentItemChanged(IActionReferential oldValue, IActionReferential newValue)
        {
            ProcessesListVisibility =
                (this.CurrentItem != null &&
                this.CurrentItem is IActionReferentialProcess &&
                (this.CurrentItem as IActionReferentialProcess)?.ProcessId != null &&
                ((IActionReferentialProcess)this.CurrentItem).IsMarkedAsAdded) ? Visibility.Visible : Visibility.Collapsed;

            base.RegisterToStateChanged(oldValue, newValue);

            if (newValue != null)
            {
                if (newValue.Color != null)
                {
                    try
                    {
                        this.CurrentActionColor = (Color)_colorConverter.ConvertFrom(newValue.Color);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            _currentViewModel.OnCurrentItemChanged(oldValue, newValue);
        }

        /// <summary>
        /// Appelé lorsque l'état de l'entité courante (spécifiée via RegisterToStateChanged) a changé.
        /// </summary>
        /// <param name="newState">le nouvel état.</param>
        protected override void OnEntityStateChanged(ObjectState newState)
        {
            if (newState != ObjectState.Unchanged)
                this.HasChanged = true;
        }

        /// <summary>
        /// Appelé lorsque la valeur de la propriété <see cref="Procedure"/> d'un référentiel a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="Models.PropertyChangedEventArgs&lt;Procedure&gt;"/> contenant les données de l'évènement.</param>
        private void OnItemProcessChanged(object sender, PropertyChangedEventArgs<Procedure> e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke((Action)_collectionView.Refresh, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Valide les éléments.
        /// </summary>
        /// <returns><c>true</c> si les éléments sont tous valides.</returns>
        private bool ValidateItems()
        {
            var itemsAsValidable = _items.Cast<ValidatableObject>();
            foreach (var refe in itemsAsValidable)
                refe.Validate();

            Validate();

            RefreshValidationErrors(Enumerable.Empty<ValidatableObject>()
                .Concat(this)
                .Concat(itemsAsValidable));

            if (itemsAsValidable.Any(u => !u.IsValid.GetValueOrDefault())
                || !IsValid.GetValueOrDefault())
            {
                foreach (var refe in itemsAsValidable)
                    refe.EnableAutoValidation = true;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Appelé lorsque la refraichissement des erreurs de validation est demandé.
        /// Dans une méthode dérivée, appeler RefreshValidationErrors.
        /// </summary>
        protected override void OnRefreshValidationErrorsRequested()
        {
            ValidateItems();
        }

        /// <summary>
        /// Sauvegarde les éléments.
        /// </summary>
        private async Task Save()
        {
            _lastModifiedReferential = _selectedReferential;
            var itemsToSave = _items
                .Where(u => u.ChangeTracker.State != ObjectState.Unchanged)
                .ToList();

            if (_itemsToDelete != null)
            {
                foreach (var category in _itemsToDelete)
                    category.MarkAsDeleted();
                itemsToSave.AddRange(_itemsToDelete);
            }

            var currentReferential = _selectedReferential;
            var oldRefLabel = IoC.Resolve<IReferentialsUseService>().GetLabel(currentReferential);
            var currentRefLabel = CurrentReferentialLabel;

            ShowSpinner();
            try
            {
                await _currentViewModel.SaveItems(itemsToSave);

                if (currentRefLabel != oldRefLabel)
                {
                    await ServiceBus.Get<IReferentialsService>().UpdateReferentialLabel(currentReferential, currentRefLabel);

                    //IoC.Resolve<IReferentialsUseService>().UpdateReferentialLabel(currentReferential, currentRefLabel);
                    _itemsToDelete?.Clear();
                    HideSpinner();
                    HasChanged = false;
                    // _ignoreSelectedViewChanged = true;

                    OnCleanup();
                    CreateMenu();

                    _ignoreSelectedViewChanged = false;

                    if (_selectedReferential == currentReferential)
                        _viewsFactory[currentRefLabel]();
                    else
                        _viewsFactory[CurrentReferentialLabel]();

                    // this.SelectedView = this.CurrentReferentialLabel;
                }
                else
                {
                    _itemsToDelete?.Clear();
                    HideSpinner();
                    HasChanged = false;
                }
            }
            catch (BLLException e)
            {
                switch (e.ErrorCode)
                {
                    case KnownErrorCodes.ReferentialNameAlreadyUsed:
                    case KnownErrorCodes.UpdateException:
                        DialogFactory.GetDialogView<IMessageDialog>().Show(
                            string.Format(
                                LocalizationManager.GetString("VM_AdminReferentials_ReferentialNameAlreadyUsed"),
                                currentRefLabel
                            ),
                            LocalizationManager.GetString("Common_Error"),
                            MessageDialogButton.OK, MessageDialogImage.Error);
                        HideSpinner();
                        return;
                }
                base.OnError(e);
            }
            catch(Exception e)
            {
                base.OnError(e);
            }
        }

        /// <summary>
        /// Recharge le contenu.
        /// </summary>
        private async Task Reload()
        {
            // rafraichir le contenu depuis la base de données
            await OnLoading();

            HideValidationErrors();

            HasChanged = false;

            _itemsToDelete?.Clear();

            if (_items != null)
            {
                foreach (var item in _items)
                    UnregisterItem(item);
            }

            _viewsFactory[SelectedView]();
        }

        /// <summary>
        /// Appelé lorsque la valeur de <see cref="CurrentReferentialLabel"/> a changé.
        /// </summary>
        private void OnCurrentReferentialLabelChanged()
        {
            if (this.CurrentReferentialLabel != IoC.Resolve<IReferentialsUseService>().GetLabel(_selectedReferential))
                this.HasChanged = true;
        }

        #endregion

        #region Commandes

        private Command _addStandardCommand;
        /// <summary>
        /// Obtient la commande permettant d'ajouter un nouveau référentiel standard.
        /// </summary>
        public ICommand AddStandardCommand
        {
            get
            {
                if (_addStandardCommand == null)
                    _addStandardCommand = new Command(() =>
                    {
                        var item = _currentViewModel.CreateStandardReferential();
                        item.IsEditable = true;

                        InitializeNewItem(item);

                        _items.Add(item);
                        this.CurrentItem = item;
                        this.HasChanged = true;
                    }, () => SecurityContext.CurrentUser != null && IsAdmin);
                return _addStandardCommand;
            }
        }

        private Command _addProcessCommand;
        /// <summary>
        /// Obtient la commande permettant d'ajouter un nouveau référentiel process.
        /// </summary>
        public ICommand AddProcessCommand
        {
            get
            {
                if (_addProcessCommand == null)
                    _addProcessCommand = new Command(() =>
                    {
                        var item = _currentViewModel.CreateProcessReferential();
                        item.IsEditable = true;

                        InitializeNewItem(item);

                        _items.Add(item);
                        CurrentItem = item;
                        item.ProcessChanged += new EventHandler<PropertyChangedEventArgs<Procedure>>(OnItemProcessChanged);
                        HasChanged = true;
                    }, () =>
                    {
                        if (Processes == null)
                            return false;
                        var processesCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(Processes);
                        if (processesCollectionView.Count == 0)
                            return false;
                        return SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.All);
                    });
                return _addProcessCommand;
            }
        }

        private Command _mergeCommand;
        /// <summary>
        /// Obtient la commande permettant de fusionner deux référentiels.
        /// </summary>
        public ICommand MergeCommand
        {
            get
            {
                if (_mergeCommand == null)
                    _mergeCommand = new Command(() =>
                    {
                        IChildWindow view = (IChildWindow)UXFactory.GetView(out IReferentialMergeViewModel viewModel);

                        // Filtrer les référentiels pouvant être cochés en fonction de ceux présents
                        List<IActionReferential> filtered = new List<IActionReferential>();
                        bool isCurrentStandard = CurrentItem is IActionReferentialProcess && (CurrentItem as IActionReferentialProcess).ProcessId == null;
                        bool isCurrentProcess = CurrentItem is IActionReferentialProcess && (CurrentItem as IActionReferentialProcess).ProcessId != null;
                        Procedure currentProcess = isCurrentProcess ? (CurrentItem as IActionReferentialProcess).Process : null;

                        foreach (IActionReferential item in Items)
                        {
                            if (item.IsEditable &&
                                item != CurrentItem &&
                                (isCurrentStandard ||
                                (isCurrentProcess && (item as IActionReferentialProcess)?.Process == currentProcess)))
                            {
                                item.IsSelected = false;
                                filtered.Add(item);
                            }
                        }

                        viewModel.MainReferential = CurrentItem;
                        viewModel.Referentials = filtered.ToArray();
                        viewModel.GroupSort = _groupSort;
                        viewModel.Load();

                        ServiceBus.Get<IChildWindowService>().ShowDialog(
                            ServiceBus.Get<IViewHandleService>().Resolve(this),
                            view,
                            onClosed: async r => await OnMergeClosed(viewModel));

                    }, () => CurrentItem != null && CurrentItem.IsEditable && !HasChanged);
                return _mergeCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de définir l'uri par un fichier.
        /// </summary>
        private Command _browseCommand;
        public ICommand BrowseCommand
        {
            get
            {
                if (_browseCommand == null)
                    _browseCommand = new Command(async () =>
                    {
                        var file = DialogFactory.GetDialogView<IOpenFileDialog>()
                            .Show(LocalizationManager.GetString("View_AdminReferentials_Uri_Browse_Caption"));

                        if (file?.Any() == true)
                        {
                            var fileName = file.First();
                            using (var ms = new MemoryStream())
                            using (var fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                            {
                                await fileStream.CopyToAsync(ms);
                                CurrentItem.CloudFile = new CloudFile(ms.ToArray(), Path.GetExtension(fileName));
                            }
                            OnPropertyChanged(nameof(CurrentItem));
                        }
                    });
                return _browseCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de supprimer le fichier d'un référentiel.
        /// </summary>
        private Command _deleteDocumentCommand;
        public ICommand DeleteDocumentCommand
        {
            get
            {
                if (_deleteDocumentCommand == null)
                    _deleteDocumentCommand = new Command(() =>
                    {
                        CurrentItem.CloudFile = null;
                        OnPropertyChanged(nameof(CurrentItem));
                    });
                return _deleteDocumentCommand;
            }
        }

        private Command<Uri> _openUriCommand;
        /// <summary>
        /// Obtient une commande permettant d'ouvrir le fichier.
        /// </summary>
        public ICommand OpenUriCommand
        {
            get
            {
                if (_openUriCommand == null)
                    _openUriCommand = new Command<Uri>(uri =>
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(uri.OriginalString);
                        }
                        catch (Exception e)
                        {
                            TraceManager.TraceError(e, e.Message);
                            IoC.Resolve<IDialogFactory>().GetDialogView<IErrorDialog>().Show(
                                LocalizationManager.GetString("View_AdminReferentials_Uri_Open_Error"),
                                LocalizationManager.GetString("Common_Error"), e);
                        }
                    });
                return _openUriCommand;
            }
        }

        /// <summary>
        /// Appelé lorsque la fenêtre de fusion est fermée.
        /// </summary>
        /// <param name="result">le choix de l'utilisateur.</param>
        /// <param name="viewModel">Le VM utilisé.</param>
        private async Task OnMergeClosed(IReferentialMergeViewModel viewModel)
        {
            if (viewModel.Result)
            {
                var selectedItems = viewModel.Referentials.Where(r => r.IsSelected);
                if (selectedItems.Any())
                {
                    ShowSpinner();
                    try
                    {
                        await ServiceBus.Get<IReferentialsService>().MergeReferentials(CurrentItem, selectedItems.ToArray());

                        HideSpinner();

                        // rafraichir le contenu depuis la base de données
                        await Reload();
                    }
                    catch (Exception e)
                    {
                        base.OnError(e);
                    }
                }
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande RemoveCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnRemoveCommandCanExecute()
        {
            return this.CurrentItem != null && this.CurrentItem.IsEditable;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveCommand
        /// </summary>
        protected override async void OnRemoveCommandExecute()
        {
            if (CurrentItem.ChangeTracker.State == ObjectState.Added)
            {
                if ((CurrentItem as IActionReferentialProcess)?.ProcessId != null)
                    (CurrentItem as IActionReferentialProcess).Process = null;

                _currentViewModel.UninitializeRemovedItem(CurrentItem);

                CurrentItem.ChangeTracker.State = ObjectState.Unchanged;
                CurrentItem.ChangeTracker.ChangeTrackingEnabled = false;

                UnregisterItem(CurrentItem);
                _items.Remove(CurrentItem);
                return;
            }

            if (await ServiceBus.Get<IReferentialsService>().ReferentialUsed(CurrentItem.ProcessReferentialId, CurrentItem.Id))
            {
                DialogFactory.GetDialogView<IMessageDialog>().Show(
                    LocalizationManager.GetString("VM_AppActionsReferentialsViewModelBase_Message_CannotDeleteAttachedActions"),
                    LocalizationManager.GetString("Common_Error"), MessageDialogButton.OK, MessageDialogImage.Error);
                return;
            }

            if (_itemsToDelete == null)
                _itemsToDelete = new List<IActionReferential>();
            _itemsToDelete.Add(CurrentItem);
            UnregisterItem(CurrentItem);
            _items.Remove(CurrentItem);
            HasChanged = true;

            CancelCommand.Invalidate();
            ValidateCommand.Invalidate();
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnValidateCommandCanExecute()
        {
            return this.HasChanged;
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CancelCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnCancelCommandCanExecute()
        {
            return this.HasChanged;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected override async void OnValidateCommandExecute()
        {
            // Fix process
            foreach (var item in _items.OfType<IActionReferentialProcess>())
            {
                item.ProcessChanged -= OnItemProcessChanged;
                if (item.Process != null)
                {
                    item.ChangeTracker.ChangeTrackingEnabled = false;
                    var processId = item.ProcessId;
                    item.Process = null;
                    item.ProcessId = processId;
                    item.ChangeTracker.ChangeTrackingEnabled = true;
                }
            }
            if (ValidateItems())
                await Save();
        }

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        public override async Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            if (_items == null)
                return true;

            if (HasChanged)
            {
                var result = DialogFactory.GetDialogView<IMessageDialog>().Show(
                    LocalizationManager.GetString("VM_AppActionsReferentialsViewModelBase_Message_WantToSave"),
                    LocalizationManager.GetString("Common_Confirmation"),
                    MessageDialogButton.YesNoCancel,
                    MessageDialogImage.Question);

                switch (result)
                {
                    case MessageDialogResult.Yes:

                        // Valider, sauvegarder, quitter
                        if (ValidateItems())
                        {
                            await Save();
                            return true;
                        }
                        else
                            return false;

                    case MessageDialogResult.No:
                        // Quitter sans rien faire
                        return true;
                    default:
                        // Ne pas quitter
                        return false;
                }

            }
            else
                return true;

        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override async void OnCancelCommandExecute()
        {
            await Reload();
        }

        #endregion

    }
}