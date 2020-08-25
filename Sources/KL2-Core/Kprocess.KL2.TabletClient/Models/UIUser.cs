using KProcess.Ksmed.Models;
using MoreLinq;
using System;
using System.Linq;

namespace Kprocess.KL2.TabletClient.Models
{
    public class UIUser : User
    {
        #region Constants

        public static string EMPTY_QUALIFICATION = "-";

        #endregion

        #region Properties

        /// <summary>
        /// Obtient ou définit si l'utilisateur est selectionné ou non
        /// </summary>
        bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le nom de la dernière tâche validé par l'utilisateur
        /// </summary>
        string _lastValidationTask = "-";
        public string LastValidationTask
        {
            get => _lastValidationTask;
            set
            {
                if (_lastValidationTask != value)
                {
                    _lastValidationTask = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le nom du dernier formateur de l'utilisateur
        /// </summary>
        string _lastTrainer = string.Empty;
        public string LastTrainer
        {
            get => _lastTrainer;
            set
            {
                if (_lastTrainer != value)
                {
                    _lastTrainer = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le nom du dernier qualificateur de l'utilisateur
        /// </summary>
        string _lastQualifier = string.Empty;
        public string LastQualifier
        {
            get => _lastQualifier;
            set
            {
                if (_lastQualifier != value)
                {
                    _lastQualifier = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la date de la validation de la formation
        /// </summary>
        string _lastValidationDate;
        public string LastValidationDate
        {
            get => _lastValidationDate;
            set
            {
                if (_lastValidationDate != value)
                {
                    _lastValidationDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la date de la validation de l'évaluation
        /// </summary>
        string _lastQualificationDate;
        public string LastQualificationDate
        {
            get => _lastQualificationDate;
            set
            {
                if (_lastQualificationDate != value)
                {
                    _lastQualificationDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le résultat de la qualification
        /// </summary>
        int? _qualificationResult;
        public string QualificationResult
        {
            get => (_qualificationResult.HasValue) ? _qualificationResult.Value.ToString() : EMPTY_QUALIFICATION;
            set
            {
                if (value != null)
                {
                    if (Int32.TryParse(value, out int result))
                        _qualificationResult = result;
                    else
                        _qualificationResult = null;

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit un booléen indiquant si l'utilisateur à fini sa validation
        /// </summary>
        bool _finishQualification;
        public bool FinishQualification
        {
            get => _finishQualification;
            set
            {
                if (_finishQualification != value)
                {
                    _finishQualification = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit un booléen indiquant si l'utilisateur est qualifié
        /// </summary>
        bool _isQualified;
        public bool IsQualified
        {
            get => _isQualified;
            set
            {
                if (_isQualified != value)
                {
                    _isQualified = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le nom de la dernière tâche validé par l'utilisateur
        /// </summary>
        string _lastQualificationTask = "-";
        public string LastQualificationTask
        {
            get => _lastQualificationTask;
            set
            {
                if (_lastQualificationTask != value)
                {
                    _lastQualificationTask = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le statut de la dernière tâche validé par l'utilisateur
        /// </summary>
        QualificationStatus _lastQualificationStatus = QualificationStatus.NotBegun;
        public QualificationStatus LastQualificationStatus
        {
            get => _lastQualificationStatus;
            set
            {
                if (_lastQualificationStatus == value)
                    return;
                _lastQualificationStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Obtient ou définit si on affiche le résultat de l'évaluation
        /// </summary>
        bool _showQualificationResult;
        public bool ShowQualificationResult
        {
            get => _showQualificationResult;
            set
            {
                if (_showQualificationResult != value)
                {
                    _showQualificationResult = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        bool _canRestartTraining = true;
        public bool CanRestartTraining
        {
            get => _canRestartTraining;
            set
            {
                if (_canRestartTraining != value)
                {
                    _canRestartTraining = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Méthode permettant de créer un objet utilisateur graphique a partir d'un objet utilisateur
        /// </summary>
        /// <param name="user">Utilisateur dont on veut une représentation graphique</param>
        /// <returns>L'objet graphique</returns>
        public static UIUser Create(User user, Publication trainingPublication, Publication evaluationPublication)
        {
            UIUser result = new UIUser();
            if (user == null)
                return result;

            UpdateUIUserInfo(result, user);

            if (trainingPublication == null || evaluationPublication == null)
                return result;

            UpdateTraining(result, user, trainingPublication);

            UpdateQualification(result, user, evaluationPublication);

            return result;
        }

        static void UpdateUIUserInfo(UIUser uiUser, User user)
        {
            uiUser.UserId = user.UserId;
            uiUser.Username = user.Username;
            uiUser.Firstname = user.Firstname;
            uiUser.Name = user.Name;
            uiUser.OnPropertyChanged(nameof(FullName));
            uiUser.CanRestartTraining = true;

            uiUser.LastValidationTask = "-";
        }

        static void UpdateTraining(UIUser uiUser, User user, Publication trainingPublication)
        {
            var undeletedTrainings = trainingPublication.Trainings
                .Where(t => !t.IsDeleted && t.UserId == user.UserId);
            var training = undeletedTrainings
                .MaxBy(t => t.StartDate)
                .FirstOrDefault();
            ValidationTraining lastValidationTraining = training?.ValidationTrainings
                .Where(v => !v.IsDeleted && v.EndDate != null)
                .MaxBy(v => v.EndDate)
                .FirstOrDefault();
            if (training?.EndDate != null)
            {
                uiUser.LastValidationTask = Locator.LocalizationManager.GetString("TrainingCompleted");
                var lastTrainer = lastValidationTraining != null
                    ? Locator.Main.Users.SingleOrDefault(u => u.UserId == lastValidationTraining.TrainerId)
                    : null;
                uiUser.LastTrainer = lastTrainer != null
                    ? lastTrainer.FullName
                    : string.Empty;
                uiUser.LastValidationDate = training.EndDate.Value.ToShortDateString();
                uiUser.CanRestartTraining = false;
            }
            else if (training != null)
            {
                var lastInnerValidationTraining = trainingPublication.PublishedActions
                    .Where(pa => pa.LinkedPublication != null)
                    .Select(pa => pa.LinkedPublication)
                    .SelectMany(lp => lp.Trainings)
                    .Where(t => !t.IsDeleted && t.UserId == user.UserId)
                    .SelectMany(t => t.ValidationTrainings)
                    .Where(vt => !vt.IsDeleted && vt.EndDate != null)
                    .MaxBy(vt => vt.EndDate)
                    .FirstOrDefault();
                ValidationTraining toDisplay = null;
                if (lastValidationTraining == null)
                    toDisplay = lastInnerValidationTraining;
                else if (lastInnerValidationTraining == null)
                    toDisplay = lastValidationTraining;
                else if (lastValidationTraining.EndDate.Value >= lastInnerValidationTraining.EndDate.Value)
                    toDisplay = lastValidationTraining;
                else
                    toDisplay = lastInnerValidationTraining;

                PublishedAction publishedActionToDisplay = null;
                if (toDisplay != null)
                {
                    publishedActionToDisplay = toDisplay.PublishedAction ?? trainingPublication.PublishedActions.SingleOrDefault(_ => _.PublishedActionId == toDisplay.PublishedActionId);
                    if (publishedActionToDisplay == null)
                        trainingPublication.PublishedActions.Where(_ => _.LinkedPublication != null).SelectMany(_ => _.LinkedPublication.PublishedActions).First(_ => _.PublishedActionId == toDisplay.PublishedActionId);
                }
                uiUser.LastValidationTask = (toDisplay != null)
                    ? string.Concat(publishedActionToDisplay.WBS, " - ", publishedActionToDisplay.Label)
                    : "-";
                var lastTrainer = (toDisplay != null) ? Locator.Main.Users.SingleOrDefault(u => u.UserId == toDisplay.TrainerId) : null;
                uiUser.LastTrainer = (lastTrainer != null) ? lastTrainer.FullName : string.Empty;
                uiUser.LastValidationDate = (toDisplay?.EndDate != null) ? toDisplay.EndDate.Value.ToShortDateString() : training.StartDate.ToShortDateString();
            }
        }

        static void UpdateQualification(UIUser uiUser, User user, Publication evaluationPublication)
        {
            var undeletedQualifications = evaluationPublication.Qualifications
                .Where(q => !q.IsDeleted && q.UserId == user.UserId);
            var qualification = undeletedQualifications
                .MaxBy(q => q.StartDate)
                .FirstOrDefault();
            if (qualification == null)
                return;

            uiUser.LastValidationTask = qualification.EndDate.HasValue
                ? string.Concat($"{Locator.LocalizationManager.GetString("TrainingCompleted")} / ",
                    (qualification.IsQualified == true)
                    ? Locator.LocalizationManager.GetString("Common_Qualified")
                    : Locator.LocalizationManager.GetString("Common_NotQualified"))
                : $"{uiUser.LastValidationTask} / {Locator.LocalizationManager.GetString("QualificationInProgress")}";
            uiUser.CanRestartTraining = qualification.EndDate.HasValue && qualification.IsQualified == false;
            uiUser.QualificationResult = qualification.Result.HasValue ? qualification.Result.Value.ToString() : "0";
            uiUser.FinishQualification = qualification.EndDate.HasValue;
            uiUser.IsQualified = uiUser.FinishQualification && qualification.IsQualified == true;
            uiUser.ShowQualificationResult = true;

            var lastQualificationStep = qualification.QualificationSteps
                .Where(q => !q.IsDeleted && q.IsQualified != null)
                .MaxBy(_ => _.Date)
                .FirstOrDefault();
            var lastInnerQualificationStep = evaluationPublication.PublishedActions
                .Where(_ => _.LinkedPublication != null)
                .Select(_ => _.LinkedPublication)
                .SelectMany(_ => _.Qualifications)
                .Where(q => !q.IsDeleted && q.UserId == user.UserId)
                .SelectMany(_ => _.QualificationSteps)
                .Where(_ => !_.IsDeleted && _.IsQualified != null)
                .MaxBy(_ => _.Date)
                .FirstOrDefault();
            QualificationStep toDisplay = null;
            if (lastQualificationStep == null)
                toDisplay = lastInnerQualificationStep;
            else if (lastInnerQualificationStep == null)
                toDisplay = lastQualificationStep;
            else
                toDisplay = (new[] { lastQualificationStep, lastInnerQualificationStep })
                    .MaxBy(qs => qs.Date)
                    .FirstOrDefault();

            PublishedAction publishedActionToDisplay = null;
            if (toDisplay == null)
                return;

            if (toDisplay.Qualification.PublicationId == evaluationPublication.PublicationId) // Le step est sur le process parent
                publishedActionToDisplay = evaluationPublication.PublishedActions.FirstOrDefault(p => p.PublishedActionId == toDisplay.PublishedActionId);
            else // Le step est sur un process enfant
                publishedActionToDisplay = evaluationPublication.PublishedActions.FirstOrDefault(p => p.LinkedPublicationId == toDisplay.Qualification.PublicationId);

            var (lastQualificationStatus, lastQualificationTask) = GetStatusQualificationTask(qualification, evaluationPublication, publishedActionToDisplay);
            uiUser.LastQualificationStatus = lastQualificationStatus;
            uiUser.LastQualificationTask = lastQualificationTask;

            var lastQualifier = Locator.Main.Users.SingleOrDefault(u => u.UserId == toDisplay.QualifierId);
            uiUser.LastQualifier = (lastQualifier != null) 
                ? lastQualifier.FullName
                : string.Empty;
            uiUser._lastQualificationDate = toDisplay.Date.ToShortDateString();
        }

        static (QualificationStatus status, string task) GetStatusQualificationTask(Qualification qualification, Publication evaluationPublication, PublishedAction currentAction)
        {
            if (qualification.EndDate.HasValue)
                return (QualificationStatus.Completed, Locator.LocalizationManager.GetString("QualificationCompleted"));

            if (evaluationPublication.PublishedActions.All(pa => qualification.QualificationSteps.Any(qs => !qs.IsDeleted
                                                                                                            && qs.PublishedActionId == pa.PublishedActionId
                                                                                                            && qs.IsQualified != null)))
                return (QualificationStatus.WaitingValidation, Locator.LocalizationManager.GetString("WaitingValidation"));

            return (QualificationStatus.InProgress, $"{currentAction.WBS} - {currentAction.Label}");
        }

        #endregion

        public enum QualificationStatus
        {
            NotBegun,
            InProgress,
            WaitingValidation,
            Completed
        }
    }
}
