using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.Models;
using Kprocess.KL2.TabletClient.Views;
using KProcess.Ksmed.Models;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MoreLinq;
using KProcess.Ksmed.Business.ActionsManagement;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    public class SelectFormationOperatorViewModel : ViewModelBase, IRefreshViewModel
    {
        #region Constants

        string START_FORMATION = Locator.LocalizationManager.GetString("Start_Training"); 
        string CONTINUE_FORMATION = Locator.LocalizationManager.GetString("Continue_Training");
        string SHOW_FORMATION = Locator.LocalizationManager.GetString("See_Training_Label"); 

        #endregion

        #region Attributs

        public SfDataGrid OperatorsDataGrid { get; set; }

        ICommand _returnCommand;
        ICommand _startCommand;

        List<UIUser> _operators = new List<UIUser>();

        string _startOrShowFormation = Locator.LocalizationManager.GetString("Start_Training");

        #endregion

        #region Properties

        /// <summary>
        /// Obtient la commande pour revenir en arrière
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
        /// Obtient la commande permettant de démarrer la formation
        /// </summary>
        public ICommand StartCommand
        {
            get
            {
                if (_startCommand == null)
                    _startCommand = new RelayCommand(async () =>
                    {
                        Locator.Main.IsLoading = true;
                        Locator.Main.ShowDisconnectedMessage = false;

                        try
                        {
                            var operators = OperatorsDataGrid.SelectedItems.Cast<UIUser>().ToList();
                            List<(int UserId, string WBS_Level)> levels = new List<(int UserId, string WBS_Level)>();

                            // Vérification si toutes les personnes ont fini la formation
                            bool allFinish = operators.All(op => !op.CanRestartTraining);

                            if (!allFinish)
                            {
                                foreach (var op in operators)
                                {
                                    // Inner Formation
                                    if (Publication.PublishedActions.Any(p => p.LinkedPublication != null))
                                        foreach (Publication publication in Publication.PublishedActions.Where(p => p.LinkedPublication != null).Select(p => p.LinkedPublication).Distinct())
                                            ManageTraining(publication, op.UserId);

                                    levels.AddRange(ManageTraining(Publication, op.UserId));
                                }

                                // Vérification que les deux personnes sont au même niveau
                                if (levels.Count > 1)
                                {
                                    string minWBS = levels.Select(_ => _.WBS_Level)
                                        .OrderBy(WBSHelper.GetParts, new WBSHelper.WBSComparer())
                                        .First();

                                    // On invalide les validations des personnes étant plus haut
                                    var tooHighOperatorsId = levels.Where(l => l.WBS_Level != minWBS)
                                        .Select(l => l.UserId);
                                    if (tooHighOperatorsId != null && operators.Any())
                                    {
                                        foreach (var opId in tooHighOperatorsId)
                                        {
                                            var training = Publication.Trainings.FirstOrDefault(t => !t.IsDeleted && t.UserId == opId && t.EndDate == null);
                                            if (training.ValidationTrainings?.Any() == true)
                                            {
                                                var validationTraining = training.ValidationTrainings.OrderBy(v => v.PublishedAction, new WBSComparer())
                                                    .FirstOrDefault(v => v.PublishedAction.WBS == minWBS);
                                                int index = -1;
                                                if (validationTraining != null)
                                                    index = training.ValidationTrainings.IndexOf(validationTraining);

                                                for (int i = index + 1; i < training.ValidationTrainings.Count; i++)
                                                    training.ValidationTrainings[i].EndDate = null;
                                            }
                                        }
                                    }
                                }

                                OfflineFile.Training.SaveToJson(Publication);
                            }

                            var viewModel = new FormationViewModel
                            {
                                Publication = Publication,
                                Operators = operators
                            };
                            await Locator.Navigation.Push<Formation, FormationViewModel>(viewModel);
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de la création de la formation.");
                        }
                        finally
                        {
                            Locator.Main.IsLoading = false;
                        }
                    }, () =>
                    {
                        if (OperatorsDataGrid?.SelectedItems?.Any() == true)
                        {
                            var trainings = Publication.Trainings
                                .Where(t => !t.IsDeleted && OperatorsDataGrid.SelectedItems.Cast<UIUser>()
                                                .Any(u => u.UserId == t.UserId))
                                .ToList();
                            StartOrShowFormation = trainings.Count == OperatorsDataGrid.SelectedItems.Count
                                ? CONTINUE_FORMATION
                                : START_FORMATION;
                        }
                        else
                            StartOrShowFormation = START_FORMATION;
                        return OperatorsDataGrid?.SelectedItems?.Any() == true;
                    });

                return _startCommand;
            }
        }

        /// <summary>
        /// Obtient ou définit la publication courante
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

                Operators = Locator.Main.Users.Where(u => u.UserId != Locator.Main.SelectedUser.UserId).Select(u =>
                {
                    var user = UIUser.Create(u, value, Locator.Main.EvaluationPublication);
                    user.IsSelected = (Operators.FirstOrDefault(o => o.UserId == u.UserId) != null) && Operators.FirstOrDefault(o => o.UserId == u.UserId).IsSelected;
                    user.PropertyChanged += User_PropertyChanged;

                    return user;
                }).ToList();
            }
        }

        /// <summary>
        /// Obtient la liste des personnes
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

        /// <summary>
        /// Obtient le texte pour le bouton permettant d'accéder à la formation
        /// </summary>
        public string StartOrShowFormation
        {
            get => _startOrShowFormation;
            private set
            {
                if (_startOrShowFormation != value)
                {
                    _startOrShowFormation = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public SelectFormationOperatorViewModel()
        {
            Locator.Main.IsLoading = true;
            Locator.Main.ShowDisconnectedMessage = false;

            try
            {
                var selectedUsers = Locator.Main.SelectedFormationUsers;
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors du chargement de la page permettant la sélection des personnes à former.");
            }
            finally
            {
                Locator.Main.IsLoading = false;
            }
        }

        #endregion

        #region Event Methods

        void User_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UIUser.IsSelected))
            {
                Locator.Main.SelectedFormationUsers = Operators.Where(o => o.IsSelected == true).ToList();

                if(Operators.FirstOrDefault(o => o.IsSelected) == null)
                    StartOrShowFormation = START_FORMATION;
                else
                    StartOrShowFormation = (Operators.FirstOrDefault(u => u.IsSelected && u.LastValidationTask != Locator.LocalizationManager.GetString("TrainingCompleted")) != null) ? START_FORMATION : SHOW_FORMATION;

                ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Public Methods

        public async Task Refresh()
        {
            Publication = await OfflineFile.Training.GetFromJson<Publication>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Méthode appelée lorsque l'utilisateur veut revenir sur la page précédente
        /// </summary>
        Task ExecuteReturnCommand() =>
            Locator.Navigation.Pop();

        List<(int UserId, string WBS_Level)> ManageTraining(Publication publication, int userId)
        {
            List<(int UserId, string WBS_Level)> result = new List<(int UserId, string WBS_Level)>();

            var training = publication.Trainings.FirstOrDefault(t => !t.IsDeleted && t.UserId == userId && t.EndDate == null);
            if (training == null)
            {
                var newTraining = new Training()
                {
                    PublicationId = publication.PublicationId,
                    UserId = userId,
                    StartDate = DateTime.Now,
                    ValidationTrainings = new TrackableCollection<ValidationTraining>()
                };

                publication.Trainings.Add(newTraining);

                result.Add((userId, "0"));
            }
            else
            {
                ValidationTraining toDisplay = training.ValidationTrainings
                    .Where(v => v.EndDate != null)
                    .MaxBy(v => v.EndDate).FirstOrDefault();
                if (toDisplay != null)
                {
                    PublishedAction publishedActionToDisplay = publication.PublishedActions.SingleOrDefault(_ => _.PublishedActionId == toDisplay.PublishedActionId);
                    if (publishedActionToDisplay == null)
                        publishedActionToDisplay = publication.PublishedActions
                            .FirstOrDefault(_ => _.LinkedPublication != null
                                                 && _.LinkedPublication.PublishedActions.Any(lpa => lpa.PublishedActionId == toDisplay.PublishedActionId));
                    result.Add((userId, publishedActionToDisplay.WBS));
                }
                else
                    result.Add((userId, "0"));
            }

            return result;
        }

        #endregion
    }
}