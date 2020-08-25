using KProcess.KL2.WebAdmin.Models.Publications;
using KProcess.Ksmed.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KProcess.KL2.WebAdmin.Mapper
{
    public  class PublicationMapper
    {
        public static ReadPublicationViewModel ToReadPublicationViewModel(
            IEnumerable<Publication> lastPublications,
            List<User> Users)
        {
            List<PublicationViewModel> publicationsViewModel = new List<PublicationViewModel>();
            List<Publication> allLastPublications = new List<Publication>();

            foreach (var lastPublication in lastPublications)
            {
                if (lastPublication.Process.ProjectDir == null)
                {
                    allLastPublications.Add(lastPublication);
                    publicationsViewModel.Add(new PublicationViewModel
                    {
                        PublicationId = lastPublication.PublicationId.ToString(),
                        Label = lastPublication.Process.Label,
                        FolderId = 0,
                        Folder = " ",
                        FolderPath = " ",
                        Version = lastPublication.Version,
                        IsMajor = lastPublication.IsMajor
                    });
                    continue;
                }
                var FolderId = lastPublication.Process.ProjectDir.Id;
                var Folder = lastPublication.Process.ProjectDir.Name;
                var path = "/" + Folder;
                var parent = lastPublication.Process.ProjectDir.Parent;
                while (parent != null)
                {   
                    path = "/" + parent.Name + path;
                    parent = parent.Parent;
                }
                allLastPublications.Add(lastPublication);
                publicationsViewModel.Add(new PublicationViewModel
                {
                    PublicationId = lastPublication.PublicationId.ToString(),
                    Label = lastPublication.Process.Label,
                    FolderId = FolderId,
                    Folder = Folder,
                    FolderParent = Path.GetDirectoryName(path),
                    FolderPath = path,
                    Version = lastPublication.Version,
                    IsMajor = lastPublication.IsMajor
                });
            }

            allLastPublications = allLastPublications.OrderBy(p => p.Process.ProjectDir != null ? p.Process.ProjectDir.Name : string.Empty).ToList();

            var users = Users.Select(u =>
            {
                return new UserReadPublicationViewModel
                {
                    UserId = u.UserId,
                    Fullname = u.FullName,
                    TenuredDisplay = u.Tenured.HasValue ? u.Tenured.Value ? "Titulaire" : "Intérimaire" : "",
                    Teams = u.Teams.Select(t => t.Name).ToList(),
                    TeamsString = string.Join(", ", u.Teams.Select(t => t.Name).ToList()),
                    HasRead = allLastPublications.Select(p => p.Readers.Any(r => r.UserId == u.UserId && r.ReadDate != null)).ToList(),
                    HasReadPreviousVersion = allLastPublications.Select(p => p.Readers.Any(r => r.UserId == u.UserId && r.PreviousVersionPublicationId != null)).ToList(),
                    HasReadPreviousMajorVersion = allLastPublications.Select(p => p.LastMajorReadDates.Any(r => r.Key == u.UserId)).ToList()
                };
            });
            List<UserReadPublicationViewModel> usersRead = new List<UserReadPublicationViewModel>();
            foreach (var user in users)
            {
                user.ReadDate = new List<string>();
                var pubCount = 0;
                foreach (var read in user.HasRead)
                {
                    if (read == true)
                        user.ReadDate.Add(allLastPublications.ElementAt(pubCount).Readers.Where(r => r.UserId == user.UserId).Select(r => r.ReadDate).First().Value.ToShortDateString());
                    else if (user.HasReadPreviousVersion[pubCount])
                        user.ReadDate.Add(allLastPublications.ElementAt(pubCount).Readers.Where(r => r.UserId == user.UserId).Select(r => r.PreviousVersionPublication.Readers.Where(r2 => r2.UserId == user.UserId).Select(r2 => r2.ReadDate).First()).First().Value.ToShortDateString());
                    else if (user.HasReadPreviousMajorVersion[pubCount])
                        user.ReadDate.Add(allLastPublications.ElementAt(pubCount).LastMajorReadDates[user.UserId].ToShortDateString());
                    else
                        user.ReadDate.Add("");
                    pubCount++;
                }
                usersRead.Add(user);
            }
            return new ReadPublicationViewModel
            {
                UserReadPublicationViewModel = usersRead,
                PublicationViewModel = publicationsViewModel.OrderBy(p => p.Folder),
                PublicationCount = allLastPublications.Count(),
                DirectoryList = publicationsViewModel.Select(p => p.Folder).Distinct(),
                DirectoryListPath = publicationsViewModel.DistinctBy(p => p.Folder).Select(p => p.FolderParent)
            };
        }
        public static UptodateOperatorsViewModel ToUptodateOperatorsViewModel(
            IEnumerable<Publication> lastPublications,
            IEnumerable<Training> trainings,
            List<User> Users)
        {
            List<PublicationViewModel> publicationsViewModel = new List<PublicationViewModel>();
            lastPublications = lastPublications.OrderBy(p => p.Process.ProjectDir != null ? p.Process.ProjectDir.Name : string.Empty).ToList();
            foreach (var lastPublication in lastPublications)
            {
                if (lastPublication.Process.ProjectDir == null)
                {
                    publicationsViewModel.Add(new PublicationViewModel
                    {
                        PublicationId = lastPublication.PublicationId.ToString(),
                        Label = lastPublication.Process.Label,
                        FolderId = 0,
                        Folder = " ",
                        FolderPath = " ",
                        Version = lastPublication.Version,
                        IsMajor = lastPublication.IsMajor
                    });
                    continue;
                }

                publicationsViewModel.Add(new PublicationViewModel
                {
                    PublicationId = lastPublication.PublicationId.ToString(),
                    Label = lastPublication.Process.Label,
                    FolderId = lastPublication.Process.ProjectDir.Id,
                    Folder = lastPublication.Process.ProjectDir.Name,
                    FolderParent = Path.GetDirectoryName(lastPublication.GetFolderPath()),
                    FolderPath = lastPublication.GetFolderPath(),
                    Version = lastPublication.Version,
                    IsMajor = lastPublication.IsMajor
                });
            }
            var users = Users.Select(u =>
            {
                return new OperatorViewModel
                {
                    UserId = u.UserId,
                    Fullname = u.FullName,
                    TenuredDisplay = u.Tenured.HasValue ? u.Tenured.Value ? "Titulaire" : "Intérimaire" : "",
                    Teams = u.Teams.Select(t => t.Name).ToList(),
                    TeamsString = string.Join(", ", u.Teams.Select(t => t.Name).ToList()),
                    Uptodate = new List<bool>(),
                    UptodatePreviousVersion = new List<bool>(),
                    UptodatePreviousMajorVersion = new List<bool>(),
                    UptodateDate = new List<string>()
                };
            });
            List<OperatorViewModel> operatorTrain = new List<OperatorViewModel>();
            foreach (var user in users)
            {
                if (trainings == null)
                {
                    operatorTrain.Add(user);
                    continue;
                }
                foreach (var pub in lastPublications)
                {
                    user.Uptodate.Add(trainings.Any(t => t.PublicationId == pub.PublicationId && t.UserId == user.UserId));
                    user.UptodatePreviousVersion.Add(trainings.Any(t => t.PublicationId == pub.PublicationId && t.UserId == user.UserId && t.PreviousVersionTrainingId != null));
                    user.UptodatePreviousMajorVersion.Add(pub.LastMajorTrainedDates.Any(mt => mt.Key == user.UserId));
                    if (trainings.Any(t => t.PublicationId == pub.PublicationId && t.UserId == user.UserId))
                    {
                        var lastTrainingAncestor = GetAncestorTraining(trainings.FirstOrDefault(t => t.PublicationId == pub.PublicationId && t.UserId == user.UserId));
                        user.UptodateDate.Add(lastTrainingAncestor.EndDate.Value.ToShortDateString());
                    }
                    else if (pub.LastMajorTrainedDates.Any(mt => mt.Key == user.UserId))
                    {
                        var ancestorLastMajorTraining = pub.LastMajorAncestorTrainings.Where(q => q.UserId == user.UserId).OrderByDescending(q => q.EndDate).FirstOrDefault();
                        user.UptodateDate.Add(ancestorLastMajorTraining.EndDate.Value.ToShortDateString());
                    }   
                    else
                        user.UptodateDate.Add("");
                }
                operatorTrain.Add(user);
            }
            return new UptodateOperatorsViewModel()
            {
                OperatorsViewModel = operatorTrain.ToList(),
                PublicationsViewModel = publicationsViewModel.OrderBy(p => p.Folder).ToList(),
                PublicationCount = publicationsViewModel.Count(),
                DirectoryList = publicationsViewModel.Select(p => p.Folder).Distinct(),
                DirectoryListPath = publicationsViewModel.DistinctBy(p => p.Folder).Select(p => p.FolderParent)
            };
            
        }

        public static PublicationQualificationsViewModel ToPublicationQualificationsViewModel(
            IEnumerable<Publication> lastPublications,
            (User[] Users, Role[] Roles, Language[] Languages, Team[] Teams) usersRolesLanguages)
        {
            List<PublicationQualificationViewModel> publicationQualificationViewModel = new List<PublicationQualificationViewModel>();
            int total = 0;
            int successUser = 0;
            int failedUser = 0;

            var lastPublicationPerProcess = lastPublications.GroupBy(p => p.Process);
            foreach (var publicationGroup in lastPublicationPerProcess)
            {
                foreach (var publication in publicationGroup)
                {
                    if (publication.Qualifications.Any())
                    {
                        foreach (var qualification in publication.Qualifications.Where(q => !q.IsDeleted && q.EndDate != null && q.PreviousVersionQualification == null))
                        {
                            if (!usersRolesLanguages.Users.Any(u => u.UserId == qualification.User.UserId))
                                continue;
                            if (qualification.IsQualified == true)
                                successUser++;
                            else if (qualification.IsQualified == false)
                                failedUser++;
                            total++;
                        }
                    }
                }
                if (total == 0)
                    continue;

                var maxVersion = publicationGroup.Select(_ => new Version(_.Version)).OrderByDescending(_ => _).First();

                publicationQualificationViewModel.Add(new PublicationQualificationViewModel
                {
                    ProcessId = publicationGroup.Key.ProcessId.ToString(),
                    Folder = publicationGroup.FirstOrDefault().GetFolder(),
                    FolderPath = publicationGroup.FirstOrDefault().GetFolderPath(),
                    Label = $"{publicationGroup.Key.Label} (v{maxVersion.Major}.0 -> {maxVersion.Major}.{maxVersion.Minor})",
                    Success = successUser,
                    Failed = failedUser,
                    Total = successUser + failedUser,
                    PercentageRate = successUser + failedUser != 0 ? Math.Round((double)(successUser / ((double)successUser + (double)failedUser)), 2) * 100 : -1
                });
                successUser = 0;
                failedUser = 0;
                total = 0;
            }
            return new PublicationQualificationsViewModel
            {
                PublicationQualificationViewModel = publicationQualificationViewModel.ToList()
            };
        }

        public static PublicationQualificationsViewModel ToPublicationQualificationsViewModel(
            IEnumerable<Publication> lastPublications,
            (User[] Users, Team[] Teams) usersTeams,
            string team,
            string position)
        {
            List<PublicationQualificationViewModel> publicationQualificationViewModel = new List<PublicationQualificationViewModel>();
            var teamQualifications = usersTeams.Users.SelectMany(u => u.Qualifications).Distinct().ToList();
            var teams = usersTeams.Teams.Select(t => t.Id).ToList();
            List<string> positions = new List<string>() { "True", "False" };
            int successUser = 0;
            int failedUser = 0;
            int total = 0;
            string notNullPosition = position == "All" ? "false" : position;

            var lastPublicationPerProcess = lastPublications.GroupBy(p => p.Process);
            foreach (var publicationGroup in lastPublicationPerProcess)
            {
                foreach (var publication in publicationGroup)
                {
                    if (publication.Qualifications.Any())
                    {
                        if (teamQualifications.Where(q => q.EndDate != null).Select(q => q.PublicationId).Contains(publication.PublicationId))
                        {
                            foreach (var qualification in teamQualifications.Where(q => q.PublicationId == publication.PublicationId && !q.IsDeleted && q.EndDate != null && q.PreviousVersionQualification == null))
                            {
                                if (!usersTeams.Users.Any(u => u.UserId == qualification.User.UserId))
                                    continue;
                                if (qualification.IsQualified == true)
                                    successUser++;
                                else if (qualification.IsQualified == false)
                                    failedUser++;
                                total++;
                            }
                        }
                    }
                }
                if (total == 0)
                    continue;

                var maxVersion = publicationGroup.Select(_ => new Version(_.Version)).OrderByDescending(_ => _).First();

                publicationQualificationViewModel.Add(new PublicationQualificationViewModel
                {
                    ProcessId = publicationGroup.Key.ProcessId.ToString(),
                    Folder = publicationGroup.FirstOrDefault().GetFolder(),
                    FolderPath = publicationGroup.FirstOrDefault().GetFolderPath(),
                    Label = $"{publicationGroup.Key.Label} (v{maxVersion.Major}.0 -> {maxVersion.Major}.{maxVersion.Minor})",
                    Success = successUser,
                    Failed = failedUser,
                    Total = successUser + failedUser,
                    PercentageRate = successUser + failedUser != 0 ? Math.Round((double)(successUser / ((double)successUser + (double)failedUser)), 2) * 100 : -1
                });
                successUser = 0;
                failedUser = 0;
                total = 0;
            }
            return new PublicationQualificationsViewModel
            {
                PublicationQualificationViewModel = publicationQualificationViewModel.ToList(),
                selectedIndexTeam = (team == "0") ? 0 : teams.IndexOf(Convert.ToInt16(team)) + 1,
                selectedIndexPosition = (position == "All") ? 0 : positions.IndexOf(position) + 1
            };
        }
        public static OperatorQualificationsViewModel ToOperatorQualificationsViewModel(
            IEnumerable<Publication> lastPublications,
            (User[] Users, Role[] Roles, Language[] Languages, Team[] Teams) usersRolesLanguages)
        {
            List<OperatorQualificationViewModel> operatorQualificationViewModel = new List<OperatorQualificationViewModel>();
            int success = 0;
            int failed = 0;

            foreach (var user in usersRolesLanguages.Users.OrderBy(u => u.FullName))
            {
                foreach (var publication in lastPublications)
                {
                    foreach (var qualification in publication.Qualifications.Where(q => !q.IsDeleted && q.EndDate != null 
                        &&  q.UserId == user.UserId && q.EndDate != null && q.PreviousVersionQualification == null))
                    {
                        if (qualification.IsQualified == true) success++;
                        else if (qualification.IsQualified == false) failed++;
                    }
                }

                operatorQualificationViewModel.Add(new OperatorQualificationViewModel
                {
                    UserId = user.UserId,
                    Fullname = user.FullName,
                    Success = success,
                    Failed = failed,
                    Total = success + failed,
                    PercentageRate = success + failed != 0 ? Math.Round((double)(success / ((double)success + (double)failed)), 2) * 100 : -1
                });
                success = 0;
                failed = 0;
            }
            return new OperatorQualificationsViewModel
            {
                OperatorQualificationViewModel = operatorQualificationViewModel
            };
        }
        public static OperatorQualificationsViewModel ToOperatorQualificationsViewModel(
            IEnumerable<Publication> lastPublications,
            (User[] Users, Team[] Teams) usersTeams,
            string team,
            string position)
        {
            List<OperatorQualificationViewModel> operatorQualificationViewModel = new List<OperatorQualificationViewModel>();
            int success = 0;
            int failed = 0;
            var teams = usersTeams.Teams.Select(t => t.Id).ToList();
            List<string> positions = new List<string>() { "True", "False" };

            var users = usersTeams.Users.ToList();
            foreach (var user in usersTeams.Users.OrderBy(u => u.FullName).ToList())
            {
                var qualifications = lastPublications.SelectMany(p => p.Qualifications).Where(q => q.UserId == user.UserId && q.EndDate != null && !q.IsDeleted);
                foreach (var qualification in qualifications)
                {
                    if (qualification.IsQualified == true) success++;
                    else if (qualification.IsQualified == false) failed++;
                }
                operatorQualificationViewModel.Add(new OperatorQualificationViewModel
                {
                    UserId = user.UserId,
                    Fullname = user.FullName,
                    Success = success,
                    Failed = failed,
                    Total = success + failed,
                    PercentageRate = success + failed != 0 ? Math.Round((double)(success / ((double)success + (double)failed)), 2) * 100 : -1
                });
                success = 0;
                failed = 0;
            }
            return new OperatorQualificationsViewModel
            {
                OperatorQualificationViewModel = operatorQualificationViewModel,
                selectedIndexTeam = (team == "0") ? 0 : teams.IndexOf(Convert.ToInt16(team)) + 1,
                selectedIndexPosition = (position == "All") ? 0 : positions.IndexOf(position) + 1
            };
        }

        public static CompetenciesViewModel ToCompetenciesViewModel(
            Skill[] skills,
            IEnumerable<PublishedAction> publishedActions,
            IEnumerable<Publication> lastPublicationSkills,
            List<User> Users)
        {
            List<UserCompetencyViewModel> userCompetencyViewModel = new List<UserCompetencyViewModel>();
            List<ProcessCompetencyViewModel> processCompetencyViewModel = new List<ProcessCompetencyViewModel>();
            List<TaskCompetencyViewModel> taskCompetencyViewModel = new List<TaskCompetencyViewModel>();

            foreach (var skill in skills)
            {       
                //Get last publications per Process
                var publications = skill.PublishedActions.Select(p => p.Publication).Where(p => p.PublishMode == PublishModeEnum.Evaluation).GroupBy(a => a.ProcessId).Select(p => p.MaxBy(x => x.PublishedDate).First());
                
                taskCompetencyViewModel.Add(new TaskCompetencyViewModel
                {
                    SkillId = skill.SkillId,
                    Label = skill.Label,
                    Publications = publications
                });
            }
            foreach (var publication in lastPublicationSkills)
            {
                processCompetencyViewModel.Add(new ProcessCompetencyViewModel
                {
                    PublicationId = publication.PublicationId.ToString(),
                    Label = publication.Process.Label,
                    Publication = publication,
                });
            }

            userCompetencyViewModel = Users.Select(u => {
                var hasCompetencyProcess = new List<bool>();
                var hasCompetencyPreviousVersionProcess = new List<bool>();
                var hasCompetencyPreviousMajorVersionProcess = new List<bool>();
                var hasCompetencyProcessData = new List<string>();
                foreach (var process in processCompetencyViewModel)
                {
                    hasCompetencyProcess.Add(process.Publication.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false));
                    hasCompetencyPreviousVersionProcess.Add(process.Publication.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false && q.PreviousVersionQualificationId != null));
                    hasCompetencyPreviousMajorVersionProcess.Add(process.Publication.LastMajorQualifiedDates.Any(q => q.Key == u.UserId));

                    if (process.Publication.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false))
                    {
                        var ancestorQualification = GetAncestorQualification(process.Publication.Qualifications.FirstOrDefault(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false));
                        hasCompetencyProcessData.Add(ancestorQualification.EndDate.Value.ToShortDateString());
                    }
                    else if (process.Publication.LastMajorQualifiedDates.Any(q => q.Key == u.UserId))
                    {
                        var ancestorLastMajorQualifications = process.Publication.LastMajorAncestorQualifications.Where(q => q.UserId == u.UserId).OrderByDescending(q => q.EndDate).FirstOrDefault();
                        hasCompetencyProcessData.Add(ancestorLastMajorQualifications.EndDate.Value.ToShortDateString());
                    }
                    else
                        hasCompetencyProcessData.Add("");
                }

                //percentage is used only for task competency because its linked with many qualification per skill
                var hasCompetencyTask = new List<bool>();
                var hasCompetencyTaskPreviousVersion = new List<bool>();

                var hasCompetencyTaskData = new List<string>();
                foreach (var task in taskCompetencyViewModel)
                {
                    hasCompetencyTask.Add(task.Publications.Any() && task.Publications.All(p => p.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false)));
                    hasCompetencyTaskPreviousVersion.Add(task.Publications.Any() && task.Publications.All(p => p.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false && q.PreviousVersionQualificationId != null)));
                    
                    if (task.Publications.Any() && task.Publications.All(p => p.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false)))
                    {
                        hasCompetencyTaskData.Add(task.Publications.Select(p => GetAncestorQualification(p.Qualifications.OrderByDescending(q => q.EndDate).FirstOrDefault(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false)).EndDate.Value.ToShortDateString()).FirstOrDefault());
                    }
                    else
                    {
                        float success = 0, failOrUnqualified = 0;
                        int percentage;
                        foreach (var publication in task.Publications)
                        {
                            if (publication.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false))
                                success++;
                            else failOrUnqualified++;
                        }

                        percentage = success + failOrUnqualified == 0 ? 0 : Convert.ToInt16((success / (success + failOrUnqualified)) * 100);
                        hasCompetencyTaskData.Add(percentage + " %");
                    }
                }

                return new UserCompetencyViewModel {
                    UserId = u.UserId,
                    Fullname = u.FullName,
                    HasCompetency = hasCompetencyProcess.Concat(hasCompetencyTask).ToList(),
                    HasCompetencyPreviousVersion = hasCompetencyPreviousVersionProcess.Concat(hasCompetencyTaskPreviousVersion).ToList(),
                    HasCompetencyPreviousMajorVersion = hasCompetencyPreviousMajorVersionProcess,
                    HasCompetencyData = hasCompetencyProcessData.Concat(hasCompetencyTaskData).ToList(),
                };
            }).ToList();

            return new CompetenciesViewModel
            {
                UserCompetencyViewModel = userCompetencyViewModel,
                ProcessCompetencyViewModel = processCompetencyViewModel,
                TaskCompetencyViewModel = taskCompetencyViewModel
            };
        }
        public static CompetenciesViewModel ToCompetenciesViewModel(
            Skill[] skills,
            IEnumerable<PublishedAction> publishedActions,
            IEnumerable<Publication> lastPublicationSkills,
            (User[] Users, Team[] Teams) usersTeams,
            string team,
            string position)
        {
            List<UserCompetencyViewModel> userCompetencyViewModel = new List<UserCompetencyViewModel>();
            List<ProcessCompetencyViewModel> processCompetencyViewModel = new List<ProcessCompetencyViewModel>();
            List<TaskCompetencyViewModel> taskCompetencyViewModel = new List<TaskCompetencyViewModel>();
            var teams = usersTeams.Teams.Select(t => t.Id).ToList();
            List<string> positions = new List<string>() { "True", "False" };

            foreach (var skill in skills)
            {
                //Get last publications per Process
                var publications = skill.PublishedActions.Select(p => p.Publication).Where(p => p.PublishMode == PublishModeEnum.Evaluation).GroupBy(a => a.ProcessId).Select(p => p.OrderByDescending(x => x.PublishedDate).First());

                taskCompetencyViewModel.Add(new TaskCompetencyViewModel
                {
                    SkillId = skill.SkillId,
                    Label = skill.Label,
                    Publications = publications
                });
            }
            if (lastPublicationSkills != null)
            {
                foreach (var publication in lastPublicationSkills)
                {
                    processCompetencyViewModel.Add(new ProcessCompetencyViewModel
                    {
                        PublicationId = publication.PublicationId.ToString(),
                        Label = publication.Process.Label,
                        Publication = publication
                    });
                }
            }

            userCompetencyViewModel = usersTeams.Users.Select(u => {
                var hasCompetencyProcess = new List<bool>();
                var hasCompetencyPreviousVersionProcess = new List<bool>();
                var hasCompetencyPreviousMajorVersionProcess = new List<bool>();
                var hasCompetencyProcessData = new List<string>();
                foreach (var process in processCompetencyViewModel)
                {
                    hasCompetencyProcess.Add(process.Publication.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false));
                    hasCompetencyPreviousVersionProcess.Add(process.Publication.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false && q.PreviousVersionQualificationId != null));
                    hasCompetencyPreviousMajorVersionProcess.Add(process.Publication.LastMajorQualifiedDates.Any(q => q.Key == u.UserId));

                    if (process.Publication.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false))
                    {
                        var ancestorQualification = GetAncestorQualification(process.Publication.Qualifications.FirstOrDefault(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false));
                        hasCompetencyProcessData.Add(ancestorQualification.EndDate.Value.ToShortDateString());
                    }   
                    else if (process.Publication.LastMajorQualifiedDates.Any(q => q.Key == u.UserId))
                    {
                        var ancestorLastMajorQualifications = process.Publication.LastMajorAncestorQualifications.Where(q => q.UserId == u.UserId).OrderByDescending(q => q.EndDate).FirstOrDefault();
                        hasCompetencyProcessData.Add(ancestorLastMajorQualifications.EndDate.Value.ToShortDateString());
                    }
                    else
                        hasCompetencyProcessData.Add("");
                }

                //percentage is used only for task competency because its linked with many qualification per skill
                var hasCompetencyTask = new List<bool>();
                var hasCompetencyTaskData = new List<string>();
                var hasCompetencyTaskPreviousVersion = new List<bool>();

                foreach (var task in taskCompetencyViewModel)
                {
                    hasCompetencyTaskPreviousVersion.Add(task.Publications.Any() && task.Publications.All(p => p.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false && q.PreviousVersionQualificationId != null)));

                    if (task.Publications.Any() && task.Publications.All(p => p.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false)))
                    {
                        hasCompetencyTask.Add(true);
                        hasCompetencyTaskData.Add(task.Publications.Select(p => GetAncestorQualification(p.Qualifications.OrderByDescending(q => q.EndDate).FirstOrDefault(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false)).EndDate.Value.ToShortDateString()).FirstOrDefault());
                    }
                    else
                    {
                        float success = 0, failOrUnqualified = 0;
                        int percentage;
                        foreach (var publication in task.Publications)
                        {
                            if (publication.Qualifications.Any(q => q.IsQualified == true && q.UserId == u.UserId && q.EndDate != null && q.IsDeleted == false))
                                success++;
                            else failOrUnqualified++;
                        }
                        percentage = success + failOrUnqualified == 0 ? 0 : Convert.ToInt16((success / (success + failOrUnqualified)) * 100);
                        hasCompetencyTask.Add(false);
                        hasCompetencyTaskData.Add(percentage + " %");
                    }
                }
                return new UserCompetencyViewModel
                {
                    UserId = u.UserId,
                    Fullname = u.FullName,
                    HasCompetency = hasCompetencyProcess.Concat(hasCompetencyTask).ToList(),
                    HasCompetencyPreviousVersion = hasCompetencyPreviousVersionProcess.Concat(hasCompetencyTaskPreviousVersion).ToList(),
                    HasCompetencyPreviousMajorVersion = hasCompetencyPreviousMajorVersionProcess,
                    HasCompetencyData = hasCompetencyProcessData.Concat(hasCompetencyTaskData).ToList()
                };
            }).ToList();

            return new CompetenciesViewModel
            {
                UserCompetencyViewModel = userCompetencyViewModel,
                ProcessCompetencyViewModel = processCompetencyViewModel,
                TaskCompetencyViewModel = taskCompetencyViewModel,
                selectedIndexTeam = (team == "0") ? 0 : teams.IndexOf(Convert.ToInt16(team)) + 1,
                selectedIndexPosition = (position == "All") ? 0 : positions.IndexOf(position) + 1
            };
        }

        public static Qualification GetAncestorQualification(Qualification qualification)
        {
            if (qualification.PreviousVersionQualification == null)
                return qualification;
            else
                return GetAncestorQualification(qualification.PreviousVersionQualification);
        }

        public static Training GetAncestorTraining(Training training)
        {
            if (training.PreviousVersionTraining == null)
                return training;
            else
                return GetAncestorTraining(training.PreviousVersionTraining);
        }
    }
}