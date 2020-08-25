using KProcess.KL2.WebAdmin.Models.Publications;
using KProcess.KL2.WebAdmin.Models.Qualification;
using KProcess.Ksmed.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KProcess.KL2.WebAdmin.Mapper
{
    public static class QualificationMapper
    {
        private static string GetTrainer(this Qualification qualification, List<User> users, IEnumerable<Publication> lastTrainingPublications)
        {
            var trainingPublication = lastTrainingPublications.Single(p => p.PublishedDate == qualification.Publication.PublishedDate);
            var training = trainingPublication.Trainings
                .Where(t => t.UserId == qualification.UserId
                            && !t.IsDeleted)
                .MaxBy(t => t.StartDate)
                .FirstOrDefault();

            if (training == null)
                return "";

            var lastStep = training.ValidationTrainings
                .Where(vt => !vt.IsDeleted)
                .MaxBy(vt => vt.EndDate)
                .FirstOrDefault();
            if (lastStep == null)
                return "";

            return users.First(u => u.UserId == lastStep.TrainerId).FullName;
        }


        private static DateTime? GetTrainingStartDate(this Qualification qualification, IEnumerable<Publication> lastTrainingPublications)
        {
            var trainingPublication = lastTrainingPublications.Single(p => p.PublishedDate == qualification.Publication.PublishedDate);
            var training = trainingPublication.Trainings
                .Where(t => t.UserId == qualification.UserId
                            && !t.IsDeleted)
                .MaxBy(t => t.StartDate)
                .FirstOrDefault();

            return training?.StartDate;
        }

        private static DateTime? GetTrainingEndDate(this Qualification qualification, IEnumerable<Publication> lastTrainingPublications)
        {
            var trainingPublication = lastTrainingPublications.Single(p => p.PublishedDate == qualification.Publication.PublishedDate);
            var training = trainingPublication.Trainings
                .Where(t => t.UserId == qualification.UserId
                            && !t.IsDeleted)
                .MaxBy(t => t.StartDate)
                .FirstOrDefault();

            return training?.EndDate != null ? training?.EndDate.Value : training?.EndDate;
        }

        static string GetQualifier(this Qualification qualification, List<User> users)
        {
            var lastStep = qualification.QualificationSteps
                .MaxBy(qs => qs.Date)
                .FirstOrDefault();
            if (lastStep == null)
                return "";

            return users.First(u => u.UserId == lastStep.QualifierId).FullName;
        }

        public async static Task<QualificationManageViewModel> ToQualificationManageViewModel(
                IEnumerable<Publication> lastTrainingPublications,
                IEnumerable<Publication> lastEvaluationPublications,
                List<User> users,
                int? userId)
        {
            var qualificationManageViewModel = new QualificationManageViewModel();
            List<QualificationViewModel> qualificationViewModel = new List<QualificationViewModel>();
            foreach (var evaluationPublication in lastEvaluationPublications)
            {
                List<Qualification> qualifications = new List<Qualification>();
                if (userId.HasValue)
                    qualifications = evaluationPublication.Qualifications
                        .Where(q => q.UserId == userId)
                        .ToList();
                else
                    qualifications = (await LicenseMapper.FilterByActivatedUser(evaluationPublication.Qualifications, q => q.UserId)).ToList();
                
                foreach (var qualification in qualifications.Where(q => q.IsQualified != null && !q.IsDeleted && q.EndDate != null))
                {
                    // Previous qualification not null means that the qualification has been done on a previous publication
                    // In that case, no need to display the qualification
                    if (qualification.PreviousVersionQualification != null)
                        continue;

                    qualificationViewModel.Add(new QualificationViewModel
                    {
                        QualificationId = qualification.QualificationId,
                        Folder = evaluationPublication.GetFolder(),
                        FolderPath = evaluationPublication.GetFolderPath(),
                        ProcessName = $"{evaluationPublication.Process.Label} (v{evaluationPublication.Version})",
                        Qualifier = qualification.GetQualifier(users),
                        Trainer = qualification.GetTrainer(users, lastTrainingPublications),
                        Operator = qualification.User.FullName,
                        TrainingStartDate = qualification.GetTrainingStartDate(lastTrainingPublications),
                        TrainingEndDate = qualification.GetTrainingEndDate(lastTrainingPublications),
                        PublicationName = evaluationPublication.Label,
                        Result = qualification.IsQualified ?? false,
                        QualificationDate = qualification.EndDate != null ? qualification.EndDate.Value : qualification.EndDate.Value,
                        Notes = qualification.Comment,
                        Teams = qualification.User.Teams.Select(t => t.Name).Distinct().ToList(),
                        PercentageResult = qualification.Result ?? 0,
                        Steps = qualification.QualificationSteps.Select(s => new QualificationStepViewModel
                        {
                            QualificationStepId = s.QualificationStepId,
                            Date = s.Date.ToShortDateString(),
                            Comment = s.Comment,
                            QualificationId = s.QualificationId,
                            Wbs = s.PublishedAction.WBS,
                            PublishedActionId = s.PublishedActionId,
                            QualifierId = s.QualifierId,
                            ActionLabel = s.PublishedAction.Label,
                            IsQualified = s.IsQualified,
                            QualifierName = s.User.FullName
                        }).ToList()
                    });
                }
            }
            qualificationManageViewModel.Qualifications = qualificationViewModel;
            return qualificationManageViewModel;
        }


        /// <summary>
        /// Create qualification view model from a qualification
        /// </summary>
        /// <param name="qualification"></param>
        /// <returns></returns>
        public static QualificationViewModel ToQualificationViewModel(
                Qualification qualification,
                IList<QualificationStep> steps,
                List<User> users,
                IEnumerable<Publication> trainingPublications)
        {
            var model = new QualificationViewModel
            {
                QualificationId = qualification.QualificationId,
                Folder = qualification.Publication.GetFolder(),
                FolderPath = qualification.Publication.GetFolderPath(),
                ProcessName = $"{qualification.Publication.Process.Label} (v{qualification.Publication.Version})",
                Qualifier = qualification.GetQualifier(users),
                Trainer = qualification.GetTrainer(users, trainingPublications),
                Operator = qualification.User.FullName,
                TrainingStartDate = qualification.GetTrainingStartDate(trainingPublications),
                TrainingEndDate = qualification.GetTrainingEndDate(trainingPublications),
                PublicationName = qualification.Publication.Label,
                Result = qualification.IsQualified ?? false,
                QualificationDate = qualification.EndDate.Value,
                Notes = qualification.Comment,
                Teams = qualification.User.Teams.Select(t => t.Name).Distinct().ToList(),
                PercentageResult = qualification.Result.Value,
                Steps = steps.Select(s => new QualificationStepViewModel
                {
                    QualificationStepId = s.QualificationStepId,
                    // If qualification step id, it means we are addin a parent task so we do not have any date
                    Date = s.QualificationStepId != 0 ? s.Date.ToShortDateString() : "",
                    Comment = s.IsParent != true ? s.Comment : "",
                    Wbs = s.PublishedAction.WBS,
                    QualificationId = s.QualificationId,
                    PublishedActionId = s.PublishedActionId,
                    QualifierId = s.QualifierId,
                    ActionLabel = s.PublishedAction.Label, 
                    IsQualified = s.IsQualified,
                    QualifierName = s.User != null ? s.User.FullName : "",
                    Level = s.Level,
                    IsParent = s.IsParent,
                    colorCondition = s.IsQualified == true && s.IsParent != true ? "readCell" : s.IsQualified == false && s.IsParent != true ? "notReadCell" : "",
                }).ToList()
            };


            return model;
        }

        public static List<QualificationViewModel> ToQualificationViewModels(IEnumerable<Qualification> qualifications, List<User> users, IEnumerable<Publication> trainingPublications)
        {
            var model = new List<QualificationViewModel>();
            foreach (var qualification in qualifications)
            {
                model.Add(new QualificationViewModel {
                    QualificationId = qualification.QualificationId,
                    Folder = qualification.Publication.GetFolder(),
                    FolderPath = qualification.Publication.GetFolderPath(),
                    ProcessName = $"{qualification.Publication.Process.Label} (v{qualification.Publication.Version})",
                    Qualifier = qualification.GetQualifier(users),
                    Trainer = qualification.GetTrainer(users, trainingPublications),
                    Operator = qualification.User.FullName,
                    TrainingStartDate = qualification.GetTrainingStartDate(trainingPublications),
                    TrainingEndDate = qualification.GetTrainingEndDate(trainingPublications),
                    PublicationName = qualification.Publication.Label,
                    Result = qualification.IsQualified ?? false,
                    QualificationDate = qualification.EndDate.Value,
                    Notes = qualification.Comment,
                    Teams = qualification.User.Teams.Select(t => t.Name).Distinct().ToList(),
                    PercentageResult = qualification.Result.Value,
                });
            }
            return model;
        }

    }
}