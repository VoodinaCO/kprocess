using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de gestion des référentiels d'un projet.
    /// </summary>
    partial class PrepareReferentialsViewModel : FrameContentViewModelBase, IPrepareReferentialsViewModel
    {
        #region Champs privés

        static readonly ProcessReferentialIdentifier[] _referentialsCannotSelectMultiple =
        {
            ProcessReferentialIdentifier.Operator,
            ProcessReferentialIdentifier.Equipment,
            ProcessReferentialIdentifier.Category,
            ProcessReferentialIdentifier.Skill
        };

        static readonly ProcessReferentialIdentifier[] _referentialsCannotSelectQty =
        {
            ProcessReferentialIdentifier.Operator,
            ProcessReferentialIdentifier.Equipment,
            ProcessReferentialIdentifier.Category,
            ProcessReferentialIdentifier.Skill
        };

        static readonly ProcessReferentialIdentifier[] _referentialsCannotKeepSelection =
        {
            ProcessReferentialIdentifier.Category,
            ProcessReferentialIdentifier.Skill
        };

        static readonly ProcessReferentialIdentifier[] _referentialsCannotUnuse =
        {
            ProcessReferentialIdentifier.Operator,
            ProcessReferentialIdentifier.Equipment,
            ProcessReferentialIdentifier.Category,
            ProcessReferentialIdentifier.Skill
        };

        int _projectId;


        /// <summary>
        /// Si définit, une navigation se produit à l'issue d'une sauvegarde
        /// </summary>
        IFrameNavigationToken _navigationToken;

        #endregion

        #region Surcharges

        /// <inheritdoc />
        protected override async Task OnLoading()
        {
            _projectId = ServiceBus.Get<IProjectManagerService>().CurrentProject.ProjectId;

            ShowSpinner();

            try
            {
                var prepareService = ServiceBus.Get<IPrepareService>();
                Project project = await prepareService.GetReferentials(_projectId);
                CurrentProject = prepareService.GetProjectSync(project.ProjectId);

                var service = IoC.Resolve<IReferentialsUseService>();
                foreach (var refe in project.Referentials)
                    refe.Label = service.GetLabel(refe.ReferentialId);

                RegisterToStateChanged(project);
                Project = project;

                Project.StartTracking();
                foreach (var refe in Project.Referentials)
                    refe.StartTracking();

                HideSpinner();
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
            await base.OnLoading();
        }

        /// <inheritdoc />
        protected override Task OnInitializeDesigner()
        {
            Project = new Project
            {
                CustomNumericLabel = "custom 1",
                CustomTextLabel3 = "custom 3",
                Referentials = new TrackableCollection<ProjectReferential>(DesignData.GenerateProjectReferentials()),
            };
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override void OnEntityStateChanged(ObjectState newState)
        {
            HasChanged |= newState != ObjectState.Unchanged;
        }

        #endregion

        #region Propriétés

        private IEnumerable<ExtendedProjectReferential> _referentials;
        /// <summary>
        /// Obtient toutes les references du projet courant
        /// </summary>
        public IEnumerable<ExtendedProjectReferential> Referentials
        {
            get { return _referentials; }
            private set
            {
                if (_referentials != value)
                {
                    _referentials = value;
                    OnPropertyChanged();
                }
            }
        }

        private Project _project;
        public Project Project
        {
            get { return _project; }
            private set
            {
                if (_project != value)
                {
                    _project = value;
                    OnPropertyChanged();
                    UpdateReferentialsWithCurrentProject();
                }
            }
        }

        Project _currentProject;
        public Project CurrentProject
        {
            get => _currentProject;
            set
            {
                if (_currentProject == value)
                    return;
                _currentProject = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le projet courant est en lecture seule.
        /// </summary>
        public bool IsReadOnly =>
            CurrentProject?.IsReadOnly ?? true;

        private ProjectReferential _selectedReferential;
        /// <summary>
        /// Obtient ou définit le référentiel sélectionné.
        /// </summary>
        public ProjectReferential SelectedReferential
        {
            get { return _selectedReferential; }
            set
            {
                if (_selectedReferential != value)
                {
                    var old = _selectedReferential;
                    _selectedReferential = value;
                    OnPropertyChanged();
                    OnCurrentReferentialChanged(old, value);
                }
            }
        }

        private bool _hasChanged;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si des valeurs ont changé.
        /// </summary>
        public bool HasChanged
        {
            get { return _hasChanged; }
            private set
            {
                if (_hasChanged != value)
                {
                    _hasChanged = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _canChangeSelectionMultiple;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si "Sélection multiple" peut être changé.
        /// </summary>
        public bool CanChangeSelectMultiple
        {
            get { return _canChangeSelectionMultiple; }
            private set
            {
                if (_canChangeSelectionMultiple != value)
                {
                    _canChangeSelectionMultiple = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _canChangeQuantity;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si "Quantité" peut être changé.
        /// </summary>
        public bool CanChangeQuantity
        {
            get { return _canChangeQuantity; }
            private set
            {
                if (_canChangeQuantity != value)
                {
                    _canChangeQuantity = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _canChangeKeepSelection;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si "Conserver sélection" peut être changé.
        /// </summary>
        public bool CanChangeKeepSelection
        {
            get { return _canChangeKeepSelection; }
            private set
            {
                if (_canChangeKeepSelection != value)
                {
                    _canChangeKeepSelection = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _canChangeIsEnabled;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si la propriété "IsEnabled" peut être changée.
        /// </summary>
        public bool CanChangeIsEnabled
        {
            get { return _canChangeIsEnabled; }
            private set
            {
                if (_canChangeIsEnabled != value)
                {
                    _canChangeIsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commandes

        /// <inheritdoc />
        protected override bool OnValidateCommandCanExecute()
        {
            return HasChanged;
        }

        /// <inheritdoc />
        protected override async void OnValidateCommandExecute()
        {
            // Valider l'objet
            if (!ValidateReferentials())
                return;

            if (!await SaveReferentials())
                return;
            var service = IoC.Resolve<IReferentialsUseService>();
            foreach (var refe in Project.Referentials)
            {
                refe.StopTracking();
                refe.Label = service.GetLabel(refe.ReferentialId);
                refe.StartTracking();
            }

            HasChanged = false;
            if (_navigationToken?.IsValid == true)
                _navigationToken.Navigate();
            else
                RegisterToStateChanged(Project);
        }

        /// <inheritdoc />
        protected override bool OnCancelCommandCanExecute()
        {
            return HasChanged;
        }

        /// <inheritdoc />
        protected override void OnCancelCommandExecute()
        {
            foreach (var refe in Project.Referentials)
            {
                refe.CancelChanges();
            }
            Project.CancelChanges();

            HasChanged = false;
            HideValidationErrors();
        }

        /// <inheritdoc />
        public override Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            if (HasChanged)
            {
                MessageDialogResult answer = this.AskNavigatingAwayValidationConfirmation(() => _navigationToken = token);
                return Task.FromResult(answer == MessageDialogResult.No);
            }

            return Task.FromResult(true);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Met à jour la propriété Referentials à partir du projet courant
        /// </summary>
        void UpdateReferentialsWithCurrentProject()
        {
            if (_project != null)
            {
                Referentials = _project.Referentials
                            .Select(referential => new ExtendedProjectReferential
                            {
                                Referential = referential,
                                IsResource = _project.Referentials.IndexOf(referential) < 2
                            });
            }
            else
            {
                Referentials = null;
            }
        }

        /// <summary>
        /// Appelé lorsque la sélection a changé.
        /// </summary>
        /// <param name="oldRef">L'ancienne sélection.</param>
        /// <param name="newRef">La nouvelle sélection.</param>
        void OnCurrentReferentialChanged(ProjectReferential oldRef, ProjectReferential newRef)
        {
            RegisterToStateChanged(oldRef, newRef);

            CanChangeKeepSelection = Array.IndexOf(_referentialsCannotKeepSelection, newRef.ReferentialId) == -1;
            CanChangeQuantity = Array.IndexOf(_referentialsCannotSelectQty, newRef.ReferentialId) == -1;
            CanChangeSelectMultiple = Array.IndexOf(_referentialsCannotSelectMultiple, newRef.ReferentialId) == -1;
            CanChangeIsEnabled = Array.IndexOf(_referentialsCannotUnuse, newRef.ReferentialId) == -1;
        }

        /// <summary>
        /// Valide les référentiels.
        /// </summary>
        /// <returns><c>true</c> si les référentiels sont valides.</returns>
        bool ValidateReferentials()
        {
            var entities = new List<ValidatableObject>()
            {
                Project,
            };

            Project.Validate();

            bool areValid = Project.IsValid.GetValueOrDefault();

            foreach (var refe in Project.Referentials)
            {
                refe.Validate();
                areValid &= refe.IsValid.GetValueOrDefault();
                entities.Add(refe);
            }

            RefreshValidationErrors(entities);

            if (!areValid)
            {
                // Active la validation auto
                Project.EnableAutoValidation = true;
                foreach (var refe in Project.Referentials)
                    refe.EnableAutoValidation = true;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sauvegarde les référentiels.
        /// </summary>
        async Task<bool> SaveReferentials()
        {
            ShowSpinner();
            try
            {
                Project = await ServiceBus.Get<IPrepareService>().SaveReferentials(Project);

                HideSpinner();
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                IoC.Resolve<IReferentialsUseService>().UpdateProjectReferentials(Project.Referentials);
                return true;
            }
            catch (Exception e)
            {
                base.OnError(e);
                return false;
            }
        }

        #endregion
    }
}