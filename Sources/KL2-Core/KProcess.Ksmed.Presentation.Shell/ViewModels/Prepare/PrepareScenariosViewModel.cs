using KProcess.Globalization;
using KProcess.KL2.APIClient;
using KProcess.KL2.SignalRClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos.Export;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Extensions;
using KProcess.Presentation.Windows;
using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran Preparation - Scénarios.
    /// </summary>
    class PrepareScenariosViewModel : FrameContentExtensibleViewModelBase<PrepareScenariosViewModel, IPrepareScenariosViewModel>, IPrepareScenariosViewModel, ISignalRHandle<AnalyzeEventArgs>
    {
        public PrepareScenariosViewModel()
        {
            EventSignalR.Subscribe(this);
        }

        ~PrepareScenariosViewModel()
        {
            EventSignalR.Unsubscribe(this);
        }

        #region Champs privés

        private Project _currentProject;
        private BulkObservableCollection<Scenario> _scenarios;
        private Scenario _currentScenario;
        private ScenarioNature[] _natures;
        private ScenarioState[] _states;
        private bool _canChange = true;

        /// <summary>
        /// Si définit, une navigation se produit à l'issue d'une sauvegarde
        /// </summary>
        private IFrameNavigationToken _navigationToken = null;

        #endregion

        #region Surcharges

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override async Task OnLoading()
        {
            var projectId = ServiceBus.Get<IProjectManagerService>().CurrentProject.ProjectId;
            var prepareService = ServiceBus.Get<IPrepareService>();
            _currentProject = await prepareService.GetProject(projectId);
            CurrentProjectInfo = prepareService.GetProjectSync(projectId);

            IsCurrentUserAdmin = Security.SecurityContext.HasCurrentUserRole(Security.KnownRoles.Administrator)
                                 && Security.SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.All);

            IsCurrentUserExporter = Security.SecurityContext.HasCurrentUserRole(Security.KnownRoles.Exporter)
                                    && Security.SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.All);

            IsRunningReadOnlyVersion = Security.SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.ReadOnly);

            ShowSpinner();

            try
            {
                ScenariosData data = await ServiceBus.Get<IPrepareService>().GetScenarios(_currentProject.ProjectId);

                LoadScenarios(data);

                var current = ServiceBus.Get<IProjectManagerService>().CurrentScenario;
                if (current != null)
                    CurrentScenario = Scenarios.First(s => s.ScenarioId == current.Id);

                HideSpinner();
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            States = DesignData.GenerateScenarioStates().ToArray();
            Natures = DesignData.GenerateScenarioNatures().ToArray();
            Scenarios = DesignData.GenerateScenarios(States, Natures).ToBulkObservableCollection();
            CurrentScenario = Scenarios.First();
            UpdateLockedStates();
            return Task.CompletedTask;
        }

        #endregion

        #region Propriétés

        Project _currentProjectInfo;
        public Project CurrentProjectInfo
        {
            get => _currentProjectInfo;
            set
            {
                if (_currentProjectInfo == value)
                    return;
                _currentProjectInfo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le projet courant est en lecture seule.
        /// </summary>
        public bool IsReadOnly =>
            CurrentProjectInfo?.IsReadOnly ?? true;

        /// <summary>
        /// Obtient les scénarios.
        /// </summary>
        public BulkObservableCollection<Scenario> Scenarios
        {
            get { return _scenarios; }
            private set
            {
                if (_scenarios != value)
                {
                    _scenarios = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le scénario courant.
        /// </summary>
        public Scenario CurrentScenario
        {
            get { return _currentScenario; }
            set
            {
                if (_currentScenario != value)
                {
                    var old = _currentScenario;
                    _currentScenario = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanChangeStatut));
                    OnCurrentScenarioChanged(old, value);
                }
            }
        }

        /// <summary>
        /// Obtient les différentes natures des scénarios.
        /// </summary>
        public ScenarioNature[] Natures
        {
            get { return _natures; }
            private set
            {
                if (_natures != value)
                {
                    _natures = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les états des scénarios.
        /// </summary>
        public ScenarioState[] States
        {
            get { return _states; }
            private set
            {
                if (_states != value)
                {
                    _states = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si la sélection peut être changée.
        /// </summary>
        public bool CanChange
        {
            get { return _canChange; }
            private set
            {
                if (_canChange != value)
                {
                    _canChange = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le statut peut être changé.
        /// </summary>
        public bool CanChangeStatut
        {
            get
            {
                if (!CanCurrentUserWrite)
                    return false;
                if (_currentScenario?.NatureCode == KnownScenarioNatures.Realized)
                    return !ServiceBus.Get<IPrepareService>().HasDocumentationDraftSync(_currentScenario.ScenarioId);
                return _scenarios?.Any(s => s.NatureCode == KnownScenarioNatures.Realized) == false;
            }
        }

        private ScenarioCriticalPath[] _summary;
        /// <summary>
        /// Obtient la synthèse.
        /// </summary>
        public ScenarioCriticalPath[] Summary
        {
            get { return _summary; }
            private set
            {
                if (_summary != value)
                {
                    _summary = value;
                    OnPropertyChanged();
                    if (this.EventBus != null)
                        this.EventBus.Publish(new RefreshRequestedEvent(this));
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si l'utilisateur courant est un administrateur.
        /// </summary>
        public bool IsCurrentUserAdmin { get; private set; }

        /// <summary>
        /// Obtient une valeur indiquant si l'utilisateur courant est un exporteur.
        /// </summary>
        public bool IsCurrentUserExporter { get; private set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la licence actuelle est en lecture seule.
        /// </summary>
        public bool IsRunningReadOnlyVersion { get; private set; }

        /// <summary>
        /// Obtient une valeur indiquant si les projets peuvent être exportés.
        /// </summary>
        public bool CanExportImportVideoDecomposition =>
            IsRunningReadOnlyVersion
            || IsCurrentUserAdmin
            || IsCurrentUserExporter;

        #endregion

        #region Commandes

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande AddCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnAddCommandCanExecute()
        {
            if (Scenarios != null && Scenarios.Any(s => s.NatureCode == KnownScenarioNatures.Realized))
                return false;
            return CanChange && !IsReadOnly
                && _currentProject != null
                && Security.SecurityContext.CurrentUser != null
                && CanCurrentUserWrite
                && ((Scenarios != null && !Scenarios.Any())
                    || (CurrentScenario != null && CurrentScenario.NatureCode == KnownScenarioNatures.Initial)
                    || (CurrentScenario != null && CurrentScenario.NatureCode == KnownScenarioNatures.Target));
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande AddCommand
        /// </summary>
        protected override async void OnAddCommandExecute()
        {
            bool createInitial = Scenarios == null || !Scenarios.Any();
            if (createInitial)
            {
                ShowSpinner();
                try
                {
                    Scenario scenario = await ServiceBus.Get<IPrepareService>().CreateInitialScenario(_currentProject.ProjectId);

                    // Remapper l'état et la nature
                    scenario.Nature = Natures.First(n => n.ScenarioNatureCode == scenario.NatureCode);
                    scenario.State = States.First(n => n.ScenarioStateCode == scenario.StateCode);
                    scenario.AcceptChanges();

                    Scenarios.Add(scenario);

                    var service = ServiceBus.Get<IProjectManagerService>();
                    service.SyncScenarios(Scenarios);

                    CurrentScenario = scenario;
                    UpdateLockedStates();

                    if (await SaveCurrentScenario())
                        CanChange = true;
                    HideSpinner();
                }
                catch (Exception e)
                {
                    base.OnError(e);
                }
            }
            else
            {
                if (CurrentScenario.NatureCode == KnownScenarioNatures.Target &&
                    CurrentScenario.StateCode == KnownScenarioStates.Validated &&
                    Scenarios.Any(s => s.NatureCode == KnownScenarioNatures.Realized))
                {
                    // il ne peut y avoir qu'un seul scénario de validation
                    DialogFactory.GetDialogView<IMessageDialog>().Show(
                        LocalizationManager.GetString("VM_PrepareScenarios_Message_CannotCreateMultipleRealizedScenario"),
                        "", image: MessageDialogImage.Exclamation);
                    return;
                }

                var keepVideoForUnchanged = false;
                if (CurrentScenario.NatureCode == KnownScenarioNatures.Target
                    && CurrentScenario.StateCode == KnownScenarioStates.Validated)
                {
                    var result = DialogFactory.GetDialogView<IMessageDialog>().Show(
                        LocalizationManager.GetString("VM_PrepareScenarios_Message_KeepVideoValidation"),
                        LocalizationManager.GetString("VM_PrepareScenarios_Message_Title_KeepVideoValidation"),
                        MessageDialogButton.YesNoCancel,
                        MessageDialogImage.Question);

                    if (result == MessageDialogResult.Cancel || result == MessageDialogResult.None)
                        return;

                    keepVideoForUnchanged = result == MessageDialogResult.Yes;
                }

                ShowSpinner();

                try
                {
                    Scenario s = await ServiceBus.Get<IPrepareService>().CreateScenario(_currentProject.ProjectId, CurrentScenario, keepVideoForUnchanged);

                    // Remapper l'état et la nature
                    //s.Nature = Natures.First(n => n.ScenarioNatureCode == s.NatureCode);
                    //s.State = States.First(n => n.ScenarioStateCode == s.StateCode);
                    Scenarios.Add(s);
                    CurrentScenario = s;
                    CanChange = false;
                    UpdateLockedStates();
                    if (await SaveCurrentScenario())
                    {
                        CanChange = true;
                        CurrentScenario.MarkAsUnchanged();
                    }
                    HideSpinner();
                }
                catch (Exception e)
                {
                    base.OnError(e);
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
            if (CurrentScenario == null)
                return false;
            if (!CanChange)
                return false;
            if (IsReadOnly)
                return false;
            if (!CanCurrentUserWrite)
                return false;
            if (IoC.Resolve<ISecurityContext>().CurrentUser == null)
                return false;
            if (ServiceBus.Get<IPrepareService>().HasDocumentationDraftSync(CurrentScenario.ScenarioId))
                return false;
            return true;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveCommand
        /// </summary>
        protected override async void OnRemoveCommandExecute()
        {
            // Deletion is forbidden if a draft already exists
            if (await ServiceBus.Get<IPrepareService>().HasDocumentationDraft(CurrentScenario.ScenarioId))
            {
                OnError("Impossible de supprimer/défiger le scénario car un brouillon de documentation est en cours d'édition.");
                return;
            }

            if (CurrentScenario.IsMarkedAsAdded)
            {
                CurrentScenario.MarkAsUnchanged();
                Scenarios.Remove(CurrentScenario);
            }
            else
                await DeleteScenario(CurrentScenario);
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnValidateCommandCanExecute() =>
            !CanChange
            && CurrentScenario != null
            && CanCurrentUserWrite;

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CancelCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnCancelCommandCanExecute() =>
            !CanChange
            && CurrentScenario != null;

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected override async void OnValidateCommandExecute()
        {
            // Vérfier si l'objet a changé
            if (CurrentScenario.IsNotMarkedAsUnchanged)
            {
                // Valider l'objet
                if (!ValidateCurrentScenario())
                    return;

                if (await SaveCurrentScenario())
                {
                    CurrentScenario.MarkAsUnchanged();
                    CanChange = true;
                    if (_navigationToken?.IsValid == true)
                        _navigationToken.Navigate();
                }
            }
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override void OnCancelCommandExecute()
        {
            // Si la vidéo est nouvelle, simplement le supprimer
            if (this.CurrentScenario.IsMarkedAsAdded)
                this.RemoveCommand.Execute(null);
            else
                this.CancelChanges();
            this.CanChange = true;
            this.HideValidationErrors();
        }

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        public override Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            if (CurrentScenario != null &&
                (!CanChange || CurrentScenario.IsNotMarkedAsUnchanged && !CurrentScenario.IsValid.GetValueOrDefault()))
            {
                MessageDialogResult answer = this.AskNavigatingAwayValidationConfirmation(() => _navigationToken = token);
                return Task.FromResult(answer == MessageDialogResult.No);
            }

            return Task.FromResult(true);
        }

        private Command _exportVideoDecompositionCommand;
        /// <summary>
        /// Obtient la commande permettant d'exporter la décomposition d'une vidéo.
        /// </summary>
        public ICommand ExportVideoDecompositionCommand
        {
            get
            {
                if (_exportVideoDecompositionCommand == null)
                    _exportVideoDecompositionCommand = new Command(async () =>
                    {
                        if (CurrentScenario.UsedVideos == null || !CurrentScenario.UsedVideos.Any())
                            return;

                        var result = DialogFactory.GetDialogView<IExportDialog>().ExportVideoDecomposition(CurrentScenario.UsedVideos);

                        if (result != null)
                        {
                            try
                            {
                                ShowSpinner();

                                try
                                {
                                    Stream s = await ServiceBus.Get<IImportExportService>().ExportVideoDecomposition(_currentProject.ProjectId, CurrentScenario.ScenarioId, result.VideoId);

                                    using (var fs = File.Create(result.Filename))
                                    {
                                        await s.CopyToAsync(fs, StreamExtensions.BufferSize);
                                    }

                                    HideSpinner();
                                }
                                catch (Exception e)
                                {
                                    base.OnError(e);
                                }
                            }
                            catch (Exception e)
                            {
                                TraceManager.TraceError(e, e.Message);
                                DialogFactory.GetDialogView<IErrorDialog>().Show(
                                    LocalizationManager.GetString("VM_PrepareScenarios_Message_ErrorDuringExport"),
                                    LocalizationManager.GetString("Common_Error"), e);
                            }
                        }

                    }, () => CanChange && CurrentScenario != null && CanExportImportVideoDecomposition);
                return _exportVideoDecompositionCommand;
            }
        }

        private Command _importVideoDecompositionCommand;
        /// <summary>
        /// Obtient la commande permettant d'importer un projet.
        /// </summary>
        public ICommand ImportVideoDecompositionCommand
        {
            get
            {
                if (_importVideoDecompositionCommand == null)
                    _importVideoDecompositionCommand = new Command(async () =>
                    {
                        var result = DialogFactory.GetDialogView<IExportDialog>().ImportVideoDecomposition();

                        if (result.Accepts)
                        {
                            try
                            {
                                ShowSpinner();

                                var file = File.OpenRead(result.Filename);

                                var projectId = ServiceBus.Get<IProjectManagerService>().CurrentProject.ProjectId;

                                try
                                {
                                    VideoDecompositionImport vdi = await ServiceBus.Get<IImportExportService>().PredictMergedReferentialsVideoDecomposition(projectId, file);

                                    file.Close();

                                    bool mergeReferentials;

                                    if (vdi.ProjectReferentialsMergeCandidates.Count > 0 || vdi.StandardReferentialsMergeCandidates.Count > 0)
                                    {
                                        var mergedReferentialsLabels = string.Join(Environment.NewLine,
                                            vdi.ProjectReferentialsMergeCandidates.Keys.Select(r => r.Label)
                                            .Union(vdi.StandardReferentialsMergeCandidates.Keys.Select(r => r.Label))
                                            .Distinct()
                                            .OrderBy(l => l));

                                        var dialogResult = DialogFactory.GetDialogView<IMessageDialog>().Show(
                                            LocalizationManager.GetStringFormat("VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials", mergedReferentialsLabels),
                                            LocalizationManager.GetString("VM_PrepareScenarios_Message_ImportVideoDecompositionMergeReferentials_Title"),
                                            MessageDialogButton.YesNoCancel,
                                            MessageDialogImage.Question);

                                        mergeReferentials = dialogResult == MessageDialogResult.Yes;
                                    }
                                    else
                                        mergeReferentials = false;

                                    try
                                    {
                                        bool p = await ServiceBus.Get<IImportExportService>().ImportVideoDecomposition(vdi, mergeReferentials, result.VideosFolder, projectId);

                                        HideSpinner();
                                        await Load();
                                    }
                                    catch (Exception e)
                                    {
                                        base.OnError(e);
                                    }
                                }
                                catch (Exception e)
                                {
                                    file.Close();
                                    base.OnError(e);
                                }
                            }
                            catch (Exception e)
                            {
                                TraceManager.TraceError(e, e.Message);
                                DialogFactory.GetDialogView<IErrorDialog>().Show(
                                    LocalizationManager.GetString("VM_PrepareScenarios_Message_ErrorDuringimport"),
                                    LocalizationManager.GetString("Common_Error"), e);
                            }
                        }

                    }, () =>
                    {
                        if (!CanChange || Scenarios == null || !CanExportImportVideoDecomposition)
                            return false;

                        var initial = Scenarios.FirstOrDefault(s => s.NatureCode == KnownScenarioNatures.Initial);
                        if (initial == null)
                            return false;

                        if (initial.StateCode != KnownScenarioStates.Draft)
                            return false;

                        return true;
                    });
                return _importVideoDecompositionCommand;
            }
        }

        private Command _convertToNewProjectCommand;
        /// <summary>
        /// Obtient la commande permettant de convertir le scénario en un nouveau scénario dans un nouveau projet.
        /// </summary>
        public ICommand ConvertToNewProjectCommand
        {
            get
            {
                if (_convertToNewProjectCommand == null)
                    _convertToNewProjectCommand = new Command(async () =>
                    {
                        ShowSpinner();

                        try
                        {
                            int p = await ServiceBus.Get<IPrepareService>().CreateNewProjectFromValidatedScenario(_currentProject.ProjectId, CurrentScenario);
                            HideSpinner();
                            await ServiceBus.Get<INavigationService>().TryNavigate(KnownMenus.Prepare, KnownMenus.PrepareProject);
                        }
                        catch (Exception e)
                        {
                            base.OnError(e);
                        }
                    }, () => CanChange
                             && CurrentScenario != null
                             && CurrentScenario.Project != null
                             && CurrentScenario.Project.Process != null
                             && ProjectSecurity.CanAdd(CurrentScenario.Project.Process)
                             // && CurrentScenario.NatureCode == KnownScenarioNatures.Realized
                             && CurrentScenario.StateCode == KnownScenarioStates.Validated);
                return _convertToNewProjectCommand;
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Charge les scénarios.
        /// </summary>
        /// <param name="data">Les données contenant les scénarios.</param>
        private void LoadScenarios(ScenariosData data)
        {
            // Il faut masque Intermédiaire pour l'instant (WI 829)
            if (this.Scenarios != null)
                foreach (var scenario in this.Scenarios)
                    UnregisterScenario(scenario);

            this.Scenarios = null;

            this.Natures = data.ScenarioNatures
                .OrderBy(s => s.ScenarioNatureCode, new KnownScenarioNatures.ScenarioNatureDefaultOrderComparer()).ToArray();
            this.States = data.ScenarioStates;
            this.Scenarios = data.Scenarios.ToBulkObservableCollection();

            this.Summary = data.Summary;

            this.UpdateLockedStates();

        }

        private void RegisterScenario(Scenario scenario)
        {
            if (scenario != null)
            {
                scenario.StartTracking();

                base.RegisterToStateChanged(scenario);
            }
        }

        private void UnregisterScenario(Scenario scenario)
        {
            if (scenario != null)
            {
                scenario.StopTracking();

                base.UnregisterToStateChanged(scenario);
            }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CurrentScenario"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne sélection.</param>
        /// <param name="newValue">La nouvelle sélection.</param>
        private void OnCurrentScenarioChanged(Scenario oldValue, Scenario newValue)
        {
            if (oldValue != null)
                oldValue.StateChanged -= OnCurrentScenarioStateChanged;
            if (newValue != null)
                newValue.StateChanged += OnCurrentScenarioStateChanged;
            UnregisterScenario(oldValue);
            RegisterScenario(newValue);

            if (newValue != null && base.ServiceBus != null)
                base.ServiceBus.Get<IProjectManagerService>().SelectScenario(newValue);
        }

        // Permet de setter la date de cloture du projet si il s'agit d'un scénario de validation
        void OnCurrentScenarioStateChanged(object sender, PropertyChangedEventArgs<ScenarioState> e)
        {
            var scenario = (Scenario)sender;
            if (scenario.NatureCode == KnownScenarioNatures.Realized && e.NewValue != null)
            {
                if (e.NewValue.ScenarioStateCode == KnownScenarioStates.Draft)
                    scenario.Project.RealEndDate = null;
                if (e.NewValue.ScenarioStateCode == KnownScenarioStates.Validated)
                    scenario.Project.RealEndDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Appelé lorsque l'état de l'entité courante (spécifiée via RegisterToStateChanged) a changé.
        /// </summary>
        /// <param name="newState">le nouvel état.</param>
        protected override void OnEntityStateChanged(ObjectState newState)
        {
            CanChange &= newState == ObjectState.Unchanged;
        }

        /// <summary>
        /// Valide le scénario courant.
        /// </summary>
        /// <returns><c>true</c> si le scénario est valides.</returns>
        private bool ValidateCurrentScenario()
        {
            this.CurrentScenario.Validate();
            base.RefreshValidationErrors(this.CurrentScenario);

            if (!this.CurrentScenario.IsValid.GetValueOrDefault())
            {
                // Active la validation auto
                this.CurrentScenario.EnableAutoValidation = true;

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
            if (CurrentScenario != null)
            {
                CurrentScenario.Validate();
                RefreshValidationErrors(CurrentScenario);
            }
        }

        /// <summary>
        /// Sauvegarde les données
        /// </summary>
        private async Task<bool> SaveCurrentScenario()
        {
            var oldCurrentScenario = CurrentScenario;

            ShowSpinner();
            try
            {
                ScenariosData scenarios = await ServiceBus.Get<IPrepareService>().SaveScenario(_currentProject.ProjectId, CurrentScenario);

                await SignalRFactory.GetSignalR<IKL2AnalyzeHubConnect>().RaiseScenarioUpdated();

                HideSpinner();

                LoadScenarios(scenarios);

                CurrentScenario = Scenarios.First(sc => sc.ScenarioId == oldCurrentScenario.ScenarioId);

                var service = ServiceBus.Get<IProjectManagerService>();
                service.SyncScenarios(Scenarios);
                service.SelectScenario(CurrentScenario);

                UpdateLockedStates();

                return true;
            }
            catch (BLLException e)
            {
                switch(e.ErrorCode)
                {
                    case KnownErrorCodes.CannotValidateMoreThanOneScenarioOfSameNature:

                        DialogFactory.GetDialogView<IMessageDialog>().Show(
                            string.Format(
                                LocalizationManager.GetString("VM_AnalyzeBuild_Message_CannotValidateScenario"),
                                (string)e.Data[KnownErrorCodes.CannotValidateMoreThanOneScenarioOfSameNature_ScenarioNameKey]
                            ),
                            LocalizationManager.GetString("Common_Error"),
                            MessageDialogButton.OK, MessageDialogImage.Error);
                        HideSpinner();
                        return false;


                    case KnownErrorCodes.CannotInvalidateAScenarioWhenHavingRealizedScenario:

                        DialogFactory.GetDialogView<IMessageDialog>().Show(
                                LocalizationManager.GetString("VM_AnalyzeBuild_Message_CannotInvalidateATargetScenarioWhenHavingRealizedScenario"
                            ),
                            LocalizationManager.GetString("Common_Error"),
                            MessageDialogButton.OK, MessageDialogImage.Error);
                        HideSpinner();
                        return false;
                }
                base.OnError(e);
            }
            catch(Exception e)
            {
                base.OnError(e);
            }
            return false;
        }

        /// <summary>
        /// Supprime le scénario spécifié.
        /// </summary>
        /// <param name="scenario">Le scénario.</param>
        private async Task DeleteScenario(Scenario scenario)
        {
            if (!DialogFactory.GetDialogView<ICommonDialog>().ShowSureToDelete())
                return;

            ShowSpinner();

            try
            {
                var result_summary = await ServiceBus.Get<IPrepareService>().DeleteScenario(_currentProject.ProjectId, scenario);

                if (!result_summary.Result)
                {
                    DialogFactory.GetDialogView<IMessageDialog>().Show(
                        LocalizationManager.GetString("VM_PrepareScenarios_CannotDeleteScenarioBecauseLinked"),
                        "", MessageDialogButton.OK, MessageDialogImage.Error
                        );
                    HideSpinner();
                }
                else
                {
                    await SignalRFactory.GetSignalR<IKL2AnalyzeHubConnect>().RaiseScenarioUpdated();

                    try
                    {
                        ScenariosData data = await ServiceBus.Get<IPrepareService>().GetScenarios(_currentProject.ProjectId);

                        LoadScenarios(data);
                        var service = ServiceBus.Get<IProjectManagerService>();
                        service.SyncScenarios(Scenarios);
                        UpdateLockedStates();
                        CanChange = true;
                        HideSpinner();
                    }
                    catch (Exception e)
                    {
                        base.OnError(e);
                    }
                }
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        /// <summary>
        /// Annule les changements réalisés sur les données en local.
        /// </summary>
        private void CancelChanges()
        {
            ShowSpinner();

            ObjectWithChangeTrackerExtensions.CancelChanges(
                Scenarios,
                Natures,
                States
                );

            if (Scenarios != null)
                foreach (var s in Scenarios)
                    s.StartTracking();

            UpdateLockedStates();
            HideSpinner();
        }

        /// <summary>
        /// Met à jour l'état "Figé" des scénarios.
        /// </summary>
        private void UpdateLockedStates()
        {
            var service = ServiceBus.Get<IProjectManagerService>();

            var hasValidationScenario = Scenarios.Any(sc => sc.NatureCode == KnownScenarioNatures.Realized);

            foreach (var scenario in Scenarios)
            {
                var scenarioDescription = service.Scenarios.FirstOrDefault(sc => sc.Id == scenario.ScenarioId);
                if (scenarioDescription != null)
                    scenario.IsLocked = scenarioDescription.IsLocked;
                else
                    scenario.IsLocked = scenario.StateCode == KnownScenarioStates.Validated ||
                        (scenario.NatureCode != KnownScenarioNatures.Realized && hasValidationScenario);
            }
        }

        #endregion

        public Task SignalRHandler(AnalyzeEventArgs args)
        {
            if (args != null && args.Messages == "RaiseDocumentationUpdated")
            {
                OnPropertyChanged(nameof(CanChangeStatut));
            }
            return Task.CompletedTask;
        }

    }
}