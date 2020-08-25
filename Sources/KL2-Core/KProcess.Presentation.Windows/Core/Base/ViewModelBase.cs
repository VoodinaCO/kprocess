// -----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using KProcess.Globalization;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'implémentation de base des ViewModels
    /// </summary>        
    public abstract class ViewModelBase : ValidatableObject, IViewModel, IBusiable
    {
        #region Attributs

        ViewModelStateEnum _viewModelState;
        IDisposer _disposer;
        bool _isBusy;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        protected ViewModelBase()
        {
            _disposer = new Disposer();

            if (!IsInDesignMode)
                this.TraceDebug("{0}.ctor()", GetType());

            _viewModelState = ViewModelStateEnum.Inactive;

            if (!IsInDesignMode)
                Initialize();

            InitializeDesigner();
        }

#if DEBUG

        /// <summary>
        /// Destructeur utilisé en debug pour s'assurer de la destruction de l'objet via le GC
        /// </summary>        
        ~ViewModelBase()
        {
            if (!IsInDesignMode)
                this.TraceDebug("{0} ({1}) ({2}) finalized", GetType().Name, Title, GetHashCode());
        }

#endif

        #endregion

        #region Designer

        /// <summary>
        /// Indique si le viewModel est en mode design
        /// </summary>
        protected bool IsInDesignMode
        {
            get { return DesignMode.IsInDesignMode; }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private async void InitializeDesigner()
        {
            if (IsInDesignMode)
                await OnInitializeDesigner();
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected virtual Task OnInitializeDesigner() =>
            Task.CompletedTask;

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient le disposer.
        /// </summary>
        protected IDisposer Disposer
        {
            get
            {
                if (_disposer == null)
                    _disposer = new Disposer();

                return _disposer;
            }
        }

        /// <summary>
        /// Obtient l'état du ViewModel
        /// </summary>
        public virtual ViewModelStateEnum ViewModelState
        {
            get => _viewModelState;
            protected set
            {
                if (_viewModelState != value)
                {
                    // Sauvegarde l'ancienne valeur
                    ViewModelStateEnum oldValue = _viewModelState;

                    _viewModelState = value;
                    OnPropertyChanged();

                    // Informe les bindings sur l'ancienne valeur
                    switch (oldValue)
                    {
                        case ViewModelStateEnum.Failed:
                            OnPropertyChanged("IsFailed");
                            break;
                        case ViewModelStateEnum.Loading:
                            OnPropertyChanged("IsLoading");
                            break;
                        case ViewModelStateEnum.Refreshing:
                            OnPropertyChanged("IsRefreshing");
                            break;
                        case ViewModelStateEnum.Ready:
                            OnPropertyChanged("IsReady");
                            break;
                        case ViewModelStateEnum.Waiting:
                            OnPropertyChanged("IsWaiting");
                            break;
                        case ViewModelStateEnum.ShuttingDown:
                            OnPropertyChanged("IsShuttingDown");
                            break;
                    }

                    // Informe les bindings sur la nouvelle valeur
                    switch (value)
                    {
                        case ViewModelStateEnum.Failed:
                            OnPropertyChanged("IsFailed");
                            break;
                        case ViewModelStateEnum.Loading:
                            OnPropertyChanged("IsLoading");
                            break;
                        case ViewModelStateEnum.Refreshing:
                            OnPropertyChanged("IsRefreshing");
                            break;
                        case ViewModelStateEnum.Ready:
                            OnPropertyChanged("IsReady");
                            break;
                        case ViewModelStateEnum.Waiting:
                            OnPropertyChanged("IsWaiting");
                            break;
                        case ViewModelStateEnum.ShuttingDown:
                            OnPropertyChanged("IsShuttingDown");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Obtient le titre
        /// </summary>
        public virtual string Title { get { return "ViewModelBase.Title"; } }

        /// <summary>
        /// Indique si le ViewModel est en cours de chargement
        /// </summary>
        public bool IsLoading
        {
            get { return _viewModelState == ViewModelStateEnum.Loading; }
        }

        /// <summary>
        /// Indique si le ViewModel est en cours de réactualisation d'une ou plusieurs propriétés
        /// </summary>
        public bool IsRefreshing
        {
            get { return _viewModelState == ViewModelStateEnum.Refreshing; }
        }

        /// <summary>
        /// Indique si le ViewModel est en état instable suite à une erreur
        /// </summary>
        public bool IsOnError
        {
            get { return _viewModelState == ViewModelStateEnum.Failed; }
        }

        /// <summary>
        /// Indique si le ViewModel est prêt à être utilisé
        /// </summary>
        public bool IsReady
        {
            get { return _viewModelState == ViewModelStateEnum.Ready; }
        }

        /// <summary>
        /// Indique si le ViewModel attend un action de l'utilisateur
        /// </summary>
        public bool IsWaiting
        {
            get { return _viewModelState == ViewModelStateEnum.Waiting; }
        }

        /// <summary>
        /// Indique si le ViewModel est en train de se fermer.
        /// </summary>
        public bool IsShuttingDown
        {
            get { return _viewModelState == ViewModelStateEnum.ShuttingDown; }
        }

        /// <summary>
        /// Indique si le ViewModel est en état d'édition
        /// </summary>
        public virtual bool IsEditing
        {
            get { return false; }
        }

        /// <summary>
        /// Obtient le bus d'événements
        /// </summary>
        [Import]
        protected IEventBus EventBus { get; set; }

        /// <summary>
        /// Obtient le bus de services
        /// </summary>
        [Import]
        protected IServiceBus ServiceBus { get; set; }

        /// <summary>
        /// Obtient la fabrique de boîtes de dialogues.
        /// </summary>
        [Import]
        protected IDialogFactory DialogFactory { get; set; }

        [Import]
        protected IExceptionHandler ExceptionHandler { get; set; }

        protected IEventSignalR EventSignalR => IoC.Resolve<IEventSignalR>();

        [Import]
        protected ISignalRFactory SignalRFactory { get; set; }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Rafraîchit l'état du ViewModel
        /// </summary>
        public async Task Refresh()
        {
            this.TraceDebug("Rafraîchissement du ViewModel {0}", GetType());

            _viewModelState = ViewModelStateEnum.Refreshing;
            await OnRefreshing();
            OnRefreshed();
            _viewModelState = ViewModelStateEnum.Ready;

            this.TraceDebug("ViewModel {0} rafraîchi", GetType());
        }

        /// <summary>
        /// Charge l'état du ViewModel
        /// </summary>
        public async Task Load()
        {
            this.TraceDebug("Chargement du ViewModel {0}", GetType());

            _viewModelState = ViewModelStateEnum.Loading;
            await OnLoading();
            OnLoaded();
            _viewModelState = ViewModelStateEnum.Ready;

            this.TraceDebug("ViewModel {0} chargé", GetType());
        }

        /// <summary>
        /// Indique si le ViewModel peut être fermé
        /// </summary>
        /// <returns>true s'il peut être fermé, false sinon</returns>
        public virtual bool CanClose()
        {
            return true;
        }

        #endregion

        #region Méthodes protégées

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected virtual Task OnLoading() =>
            Task.CompletedTask;

        /// <summary>
        /// Méthode appelée dés que le chargement est terminé
        /// </summary>
        protected virtual void OnLoaded()
        {
        }

        /// <summary>
        /// Méthode appelée lors du rafraîchissement
        /// </summary>
        protected virtual Task OnRefreshing() =>
            Task.CompletedTask;

        /// <summary>
        /// Méthode appelée dés que le rafraîchissement est terminé
        /// </summary>
        protected virtual void OnRefreshed()
        {
        }

        /// <summary>
        /// Gère l'affichage d'une erreur
        /// </summary>
        /// <param name="ex">exception levée</param>
        protected virtual void OnError(Exception ex)
        {
            this.TraceError(ex, ex.Message);
            ExceptionHandler.RaiseTaskException(ex);
        }

        /// <summary>
        /// Gère l'affichage d'une erreur
        /// </summary>
        /// <param name="message">message de l'erreur</param>
        protected virtual void OnError(string message)
        {
            this.TraceError(message);
            DialogFactory.GetDialogView<IErrorDialog>().Show(message, LocalizationManager.GetString("Common_Error"));
        }

        #endregion

        #region Commande de fermeture

        /// <summary>
        /// La commande de fermeture du ViewModel.
        /// </summary>
        protected IExtendedCommand _closeCommand;

        /// <summary>
        /// Obtient la commande de fermeture du ViewModel
        /// </summary>
        public IExtendedCommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new Command(OnCloseCommandExecute, OnCloseCommandCanExecute);

                return _closeCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CloseCommand
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected virtual bool OnCloseCommandCanExecute()
        {
            return CanClose();
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CloseCommand
        /// </summary>
        protected virtual void OnCloseCommandExecute()
        {
            EventBus.Publish<CloseViewRequestedEvent>(new CloseViewRequestedEvent(this));
        }

        #endregion

        #region Commande de rafraîchissement

        /// <summary>
        /// La commadne de rafraîchissement du ViewModel.
        /// </summary>
        protected IExtendedCommand _refreshCommand;

        /// <summary>
        /// Obtient la commande de rafraîchissement du ViewModel 
        /// </summary>
        public IExtendedCommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                    _refreshCommand = new Command(OnRefreshCommandExecute, OnRefreshCommandCanExecute);

                return _refreshCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande RefreshCommand
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected virtual bool OnRefreshCommandCanExecute()
        {
            return !IsLoading;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RefreshCommand
        /// </summary>
        protected virtual async void OnRefreshCommandExecute()
        {
            await Refresh();
        }

        #endregion

        #region Commande de suppression

        /// <summary>
        /// La commande de suppression du ViewModel.
        /// </summary>
        protected IExtendedCommand _removeCommand;
        protected IExtendedCommand _removeFolderCommand;
        protected IExtendedCommand _removeProcessCommand;
        protected IExtendedCommand _duplicateTaskCommand;

        /// <summary>
        /// Obtient la commande de duplication du ViewModel
        /// </summary>

        public IExtendedCommand DuplicateTaskCommand
        {
            get
            {
                if (_duplicateTaskCommand == null)
                    _duplicateTaskCommand = new Command(OnDuplicateTaskCommandExecute, OnDuplicateTaskCommandCanExecute);

                return _duplicateTaskCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande DuplicateTaskCommand
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        /// 

        protected virtual bool OnDuplicateTaskCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveCommand
        /// </summary>
        /// 
        protected virtual void OnDuplicateTaskCommandExecute()
        {

        }

        /// <summary>
        /// Obtient la commande de suppression du ViewModel
        /// </summary>
        public IExtendedCommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                    _removeCommand = new Command(OnRemoveCommandExecute, OnRemoveCommandCanExecute);

                return _removeCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande RemoveCommand
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected virtual bool OnRemoveCommandCanExecute()
        {
            return true;
        }


        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveCommand
        /// </summary>
        protected virtual void OnRemoveCommandExecute()
        {
        }

        /// <summary>
        /// Obtient la commande de suppression du ViewModel
        /// </summary>
        public IExtendedCommand RemoveFolderCommand
        {
            get
            {
                if (_removeFolderCommand == null)
                    _removeFolderCommand = new Command(OnRemoveFolderCommandExecute, OnRemoveFolderCommandCanExecute);

                return _removeFolderCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande RemoveFolderCommand
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected virtual bool OnRemoveFolderCommandCanExecute()
        {
            return true;
        }


        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveFolderCommand
        /// </summary>
        protected virtual void OnRemoveFolderCommandExecute()
        {
        }

        /// <summary>
        /// Obtient la commande de suppression du ViewModel
        /// </summary>
        public IExtendedCommand RemoveProcessCommand
        {
            get
            {
                if (_removeProcessCommand == null)
                    _removeProcessCommand = new Command(OnRemoveProcessCommandExecute, OnRemoveProcessCommandCanExecute);

                return _removeProcessCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande RemoveProcessCommand
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected virtual bool OnRemoveProcessCommandCanExecute()
        {
            return true;
        }


        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveProcessCommand
        /// </summary>
        protected virtual void OnRemoveProcessCommandExecute()
        {
        }

        #endregion

        #region Commande de création

        /// <summary>
        /// La commande de création du ViewModel.
        /// </summary>
        protected IExtendedCommand _addCommand;

        /// <summary>
        /// Obtient la commande de création du ViewModel
        /// </summary>
        public IExtendedCommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                    _addCommand = new Command(OnAddCommandExecute, OnAddCommandCanExecute);

                return _addCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande AddCommand
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected virtual bool OnAddCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande AddCommand
        /// </summary>
        protected virtual void OnAddCommandExecute()
        {
        }

        #endregion

        #region Commande d'édition

        /// <summary>
        /// La commande d'édition du ViewModel.
        /// </summary>
        protected IExtendedCommand _editCommand;

        /// <summary>
        /// Obtient la commande d'édition du ViewModel
        /// </summary>
        public IExtendedCommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                    _editCommand = new Command(OnEditCommandExecute, OnEditCommandCanExecute);

                return _editCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande EditCommand
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected virtual bool OnEditCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande EditCommand
        /// </summary>
        protected virtual void OnEditCommandExecute()
        {
        }

        #endregion

        #region Commande d'annulation

        /// <summary>
        /// La commande d'annulation du ViewModel.
        /// </summary>
        protected IExtendedCommand _cancelCommand;

        /// <summary>
        /// Obtient la commande d'annulation du ViewModel 
        /// </summary>
        public IExtendedCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new Command(OnCancelCommandExecute, OnCancelCommandCanExecute);

                return _cancelCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CancelCommand
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected virtual bool OnCancelCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected virtual void OnCancelCommandExecute()
        {
        }

        #endregion

        #region Commande de confirmation

        /// <summary>
        /// La commande de confirmation du ViewModel.
        /// </summary>
        protected IExtendedCommand _validateCommand;

        /// <summary>
        /// Obtient la commande de confirmation du ViewModel
        /// </summary>
        public IExtendedCommand ValidateCommand
        {
            get
            {
                if (_validateCommand == null)
                    _validateCommand = new Command(OnValidateCommandExecute, OnValidateCommandCanExecute);

                return _validateCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected virtual bool OnValidateCommandCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected virtual void OnValidateCommandExecute()
        {
        }

        #endregion

        #region ICleanable Members

        /// <summary>
        /// Survient lorsque le ViewModel est fermé.
        /// </summary>
        public event EventHandler Shuttingdown;

        /// <summary>
        /// Demande la fermeture du viewModel.
        /// </summary>
        public void Shutdown()
        {
            this.ViewModelState = ViewModelStateEnum.ShuttingDown;
            if (Shuttingdown != null)
                Shuttingdown(this, EventArgs.Empty);
            this.Cleanup();
        }

        /// <summary>
        /// Appelé afin de nettoyer les ressources.
        /// </summary>
        protected override void OnCleanup()
        {
            base.OnCleanup();

            _disposer.Dispose();

            if (EventBus != null)
            {
                // Désabonnement du bus d'événement
                EventBus.Unsubscribe(this);
            }
        }

        #endregion

        #region IBusiable Members

        private int _busyCount;
        private object _lockobjet = new object();

        /// <summary>
        /// Incrémente le compteur.
        /// </summary>
        public void IncrementBusyCount()
        {
            lock (_lockobjet)
            {
                _busyCount++;
                if (_busyCount == 1)
                    this.IsBusy = true;
            }
        }

        /// <summary>
        /// Décrémente le compteur.
        /// </summary>
        public void DecrementBusyCount()
        {
            lock (_lockobjet)
            {
                _busyCount--;
                if (_busyCount == 0)
                    this.IsBusy = false;
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le view model est actuellement occupé.
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            protected set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged("IsBusy");
                }
            }
        }

        #endregion
    }
}