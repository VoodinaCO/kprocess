using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.Models;
using Kprocess.KL2.TabletClient.Views;
using KProcess.Ksmed.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    public class SelectQualificationOperatorViewModel : ViewModelBase, IRefreshViewModel
    {
        #region Constants

        string START_QUALIFICATION = Locator.LocalizationManager.GetString("Start_Qualification"); 
        string CONTINUE_QUALIFICATION = Locator.LocalizationManager.GetString("Continue_Qualification"); 
        string SHOW_QUALIFICATION = Locator.LocalizationManager.GetString("Show_Qualification"); 
        string SHOW_REQUALIFICATION = Locator.LocalizationManager.GetString("Show_Requalification"); 

        #endregion

        #region Attributs

        ICommand _returnCommand;
        ICommand _startCommand;

        List<UIUser> _operators = new List<UIUser>();
        UIUser _operator;

        string _startOrShowQualification = Locator.LocalizationManager.GetString("Start_Qualification");

        bool _showOperatorsList;

        #endregion

        #region Properties

        public UIUser Operator
        {
            get => _operator;
            set
            {
                if (_operator != value)
                {
                    _operator = value;
                    RaisePropertyChanged();
                }

                if (value != null)
                {
                    Operators.ForEach(o => o.IsSelected = false);
                    Operators.First(o => o.UserId == value.UserId).IsSelected = true;
                    RaisePropertyChanged(nameof(Operators));

                    StartOrShowQualification = (value.QualificationResult == UIUser.EMPTY_QUALIFICATION) ?
                        START_QUALIFICATION :
                        (value.FinishQualification ?
                            (value.IsQualified ?
                                SHOW_QUALIFICATION :
                                SHOW_REQUALIFICATION) :
                            CONTINUE_QUALIFICATION);
                    ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la commande pour revenir en arrière
        /// </summary>
        public ICommand ReturnCommand
        {
            get
            {
                if (_returnCommand == null)
                    _returnCommand = new RelayCommand(async () => await Locator.Navigation.Pop());

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
                            var op = Operators.FirstOrDefault(o => o.IsSelected);
                            bool isFinish = Publication.Qualifications
                                                .Where(q => !q.IsDeleted && q.UserId == op.UserId)
                                                .MaxBy(q => q.StartDate)
                                                .FirstOrDefault(q => q.EndDate.HasValue && q.IsQualified == true) 
                                            != null;

                            if (!isFinish)
                            {
                                ManageQualification(Publication, op.UserId);

                                Publication.PublishedActions
                                    .Where(p => p.LinkedPublication != null)
                                    .Select(p => p.LinkedPublication)
                                    .ForEach(innerPublication => ManageQualification(innerPublication, op.UserId));
                            }

                            OfflineFile.Evaluation.SaveToJson(Publication);

                            var viewModel = new QualificationViewModel
                            {
                                Publication = Publication,
                                Operator = op
                            };
                            await Locator.Navigation.Push<QualificationView, QualificationViewModel>(viewModel);
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de la création de la qualification.");
                        }
                        finally
                        {
                            Locator.Main.IsLoading = false;
                        }
                    }, () => _operator != null);

                return _startCommand;
            }
        }

        /// <summary>
        /// Obtient ou définit la publication courante
        /// </summary>
        public Publication Publication
        {
            get => Locator.Main.EvaluationPublication;
            set
            {
                if (Locator.Main.EvaluationPublication != value)
                {
                    Locator.Main.EvaluationPublication = value;
                    RaisePropertyChanged();
                }

                if (Publication != null)
                {
                    try
                    {
                        var trainedUsers = Locator.Main.TrainingPublication.Trainings
                            .Where(t => !t.IsDeleted && t.EndDate.HasValue && t.UserId != Locator.Main.SelectedUser.UserId)
                            .Select(t => t.UserId);
                        var toQualifiedUsers = Locator.Main.Users.Where(u => trainedUsers.Contains(u.UserId));

                        if (toQualifiedUsers.Any())
                        {
                            Operators = toQualifiedUsers.Select(u =>
                            {
                                var user = UIUser.Create(u, Locator.Main.TrainingPublication, value);
                                user.IsSelected = Operators.FirstOrDefault(o => o.UserId == u.UserId)?.IsSelected ?? false;
                                user.PropertyChanged += User_PropertyChanged;

                                return user;
                            }).ToList();

                            ShowOperatorsList = Operators.Any();
                        }
                    }
                    catch (Exception ex)
                    {
                        Locator.TraceManager.TraceError(ex, "Erreur lors de la création de la sélection des utilisateurs.");
                    }
                }
            }
        }

        /// <summary>
        /// Obtient la liste des personnes
        /// </summary>
        public List<UIUser> Operators
        {
            get => _operators;
            private set
            {
                if (_operators != value)
                {
                    _operators = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient le texte pour le bouton permettant d'accèder à l'évaluation
        /// </summary>
        public string StartOrShowQualification
        {
            get => _startOrShowQualification;
            private set
            {
                if (_startOrShowQualification != value)
                {
                    _startOrShowQualification = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient un booléen indiquant si on doit afficher la liste des personnes
        /// </summary>
        public bool ShowOperatorsList
        {
            get => _showOperatorsList;
            private set
            {
                if (_showOperatorsList != value)
                {
                    _showOperatorsList = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public SelectQualificationOperatorViewModel()
        {
            Locator.Main.IsLoading = true;
            Locator.Main.ShowDisconnectedMessage = false;

            try
            {
                var selectedUsers = Locator.Main.SelectedFormationUsers;
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors du chargement de la page permettant la sélection des personnes à évaluer.");
            }
            finally
            {
                Locator.Main.IsLoading = false;
            }
        }

        #endregion

        #region Event Methods

        /*void UpdateSelectedUser(UIUser selectedUser)
        {
            if (selectedUser != null)
            {
                Operators.ForEach(u => u.IsSelected = false);
                Operators.First(o => o.UserId == selectedUser.UserId).IsSelected = true;
                RaisePropertyChanged(nameof(Operators));
                //Locator.Main.SelectedFormationUsers = Operators.Where(o => o.IsSelected == true).ToList();

                if(Operators.FirstOrDefault(o => o.IsSelected) == null)
                {
                    StartOrShowQualification = START_QUALIFICATION;
                }
                else
                {
                    StartOrShowQualification = (Operators.FirstOrDefault(u => u.IsSelected && u.LastValidationTask != Locator.LocalizationManager.GetString("TrainingCompleted")) != null) ? START_QUALIFICATION : SHOW_QUALIFICATION;
                }

                ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            }
        }*/

        #endregion

        #region Event Methods

        void User_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UIUser.IsSelected))
            {
                var user = (UIUser)sender;

                foreach (var op in Operators)
                {
                    if (op.UserId != user.UserId)
                    {
                        op.PropertyChanged -= User_PropertyChanged;
                        op.IsSelected = false;
                        op.PropertyChanged += User_PropertyChanged;
                    }
                }
                
                StartOrShowQualification = (user.QualificationResult == UIUser.EMPTY_QUALIFICATION) ?
                    START_QUALIFICATION :
                    (user.FinishQualification ?
                        (user.IsQualified ?
                                SHOW_QUALIFICATION :
                                SHOW_REQUALIFICATION) :
                        CONTINUE_QUALIFICATION);
                ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Public Methods

        public async Task Refresh()
        {
            Publication = await OfflineFile.Evaluation.GetFromJson<Publication>();
        }

        #endregion

        #region Private Methods

        void ManageQualification(Publication publication, int userId)
        {
            var lastQualification = publication.Qualifications
                .Where(q => !q.IsDeleted
                            && q.UserId == userId)
                .MaxBy(q => q.StartDate)
                .FirstOrDefault(q => q.EndDate == null || q.IsQualified != true);

            if (lastQualification != null && lastQualification.EndDate == null)
                return;

            //First qualify or re-qualify
            Qualification newQualification = CreateNewQualification(userId, lastQualification);

            bool isRequalify = lastQualification?.EndDate != null && lastQualification.IsQualified != true;

            if (isRequalify)
            {
                lastQualification.QualificationSteps.ForEach(s =>
                {
                    var newStep = CreateNewQualificationStep(newQualification.QualificationId, userId, s);
                    newQualification.QualificationSteps.Add(newStep);
                    publication.PublishedActions.Single(_ => _.PublishedActionId == newStep.PublishedActionId).QualificationStep = newStep;
                    newStep.PublishedAction = publication.PublishedActions.Single(_ => _.PublishedActionId == newStep.PublishedActionId);
                });
            }

            publication.Qualifications.Add(newQualification);
        }

        QualificationStep CreateNewQualificationStep(int qualificationId, int userId, QualificationStep step) =>
            new QualificationStep
            {
                QualificationId = qualificationId,
                Date = DateTime.Now,
                Comment = step.Comment,
                QualifierId = userId,
                IsQualified = step.IsQualified,
                PublishedActionId = step.PublishedActionId,
                QualificationReasonId = step.QualificationReasonId
            };

        Qualification CreateNewQualification(int userId, Qualification previousQualification) =>
            new Qualification
            {
                PublicationId = Publication.PublicationId,
                UserId = userId,
                StartDate = DateTime.Now,
                Comment = previousQualification?.Comment,
                QualificationSteps = new TrackableCollection<QualificationStep>(),
                IsQualified = previousQualification?.IsQualified,
                Result = previousQualification?.Result
            };

        #endregion
    }
}