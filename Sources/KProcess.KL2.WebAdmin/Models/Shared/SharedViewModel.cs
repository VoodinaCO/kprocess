using KProcess.KL2.JWT;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Models.Shared
{
    public class SharedViewModel
    {
        public string WebVersion;

        List<MenuItemViewModel> GetMenuRights(ILocalizationManager LocalizedStrings) => new List<MenuItemViewModel>
        {
            new MenuItemViewModel
            {
                Id = "MenuItem_Administration",
                Name = LocalizedStrings.GetString("Web_Menu_Administration"),
                Sprite = "fa fa-users",
                AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Documentalist },
                Childs = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_ManageUsers",
                        Name = LocalizedStrings.GetString("Web_Menu_Users"),
                        Action = (vm, url) => url.Action("Index", "User", new { partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_ManageTeams",
                        Name = LocalizedStrings.GetString("Web_Menu_ManageTeams"),
                        Action = (vm, url) => url.Action("Index", "Team", new { partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_ManageTimeSlot",
                        Name = LocalizedStrings.GetString("Web_Menu_ManageTimeSlot"),
                        Action = (vm, url) => url.Action("ManageTimeslots", "Schedule", new { partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_ManageReferentials",
                        Name = LocalizedStrings.GetString("Web_Menu_ManageReferentials"),
                        Action = (vm, url) => url.Action("Index", "Referentials", new { partial = true, refId = 1 }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Documentalist }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_NotificationSettings",
                        Name = LocalizedStrings.GetString("Web_Menu_NotificationSettings"),
                        Action = (vm, url) => url.Action("Index", "NotificationSetting", new { partial = true, type = 0, to = 1 }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_License",
                        Name = LocalizedStrings.GetString("Web_Menu_License"),
                        Action = (vm, url) => url.Action("Index", "License", new { partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator}
                    }
                }
            },
            new MenuItemViewModel
            {
                Id = "MenuItem_Documentation",
                Name = LocalizedStrings.GetString("Web_Menu_Documentation"),
                Sprite = "fa fa-pencil-alt",
                AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Documentalist },
                Childs = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_Publication",
                        Name = LocalizedStrings.GetString("Web_Menu_Publication"),
                        Action = (vm, url) => url.Action("Index", "Documentation", new { partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Documentalist }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_PublicationHistory",
                        Name = LocalizedStrings.GetString("Web_Menu_PublicationHistory"),
                        Action = (vm, url) => url.Action("History", "Documentation", new { partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Documentalist }
                    }
                }
            },
            new MenuItemViewModel
            {
                Id = "MenuItem_Training",
                Name = LocalizedStrings.GetString("Web_Menu_Training"),
                Sprite = "fa fa-graduation-cap",
                AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist },
                Childs = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_PublishedTrainingsList",
                        Name = LocalizedStrings.GetString("Web_Menu_PublishedTrainingsList"),
                        Action = (vm, url) => url.Action("Index", "Publication", new { PublishModeFilter = 1, partial = true}),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_ReadDocuments",
                        Name = LocalizedStrings.GetString("Web_Menu_ReadDocuments"),
                        Action = (vm, url) => vm.User.Roles.Any(r => new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer }.Contains(r))
                            ? url.Action("ReadPublication", "Publication", new { partial = true })
                            : url.Action("ReadPublication", "Publication", new { userId = vm.User.UserId, partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_TrainedOperators",
                        Name = LocalizedStrings.GetString("Web_Menu_TrainedOperators"),
                        Action = (vm, url) => vm.User.Roles.Any(r => new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Documentalist }.Contains(r))
                            ? url.Action("UptodateOperator", "Publication", new { partial = true })
                            : url.Action("UptodateOperator", "Publication", new { userId = vm.User.UserId, partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_SkillsEvaluation",
                        Name = LocalizedStrings.GetString("Web_Menu_SkillsEvaluation"),
                        Action = (vm, url) => vm.User.Roles.Any(r => new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Documentalist }.Contains(r))
                            ? url.Action("Index", "Qualification", new { partial = true })
                            : url.Action("Index", "Qualification", new { userId = vm.User.UserId, partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_SkillsMatrix",
                        Name = LocalizedStrings.GetString("Web_Menu_SkillsMatrix"),
                        Action = (vm, url) => vm.User.Roles.Any(r => new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer }.Contains(r))
                            ? url.Action("Competency", "Publication", new { partial = true })
                            : url.Action("Competency", "Publication", new { userId = vm.User.UserId, partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_SuccessRatePerTraining",
                        Name = LocalizedStrings.GetString("Web_Menu_SuccessRatePerTraining"),
                        // TODO : Personnal stats
                        Action = (vm, url) => vm.User.Roles.Any(r => new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Documentalist }.Contains(r))
                            ? url.Action("PublicationQualification", "Publication", new { partial = true })
                            : url.Action("PublicationQualification", "Publication", new { userId = vm.User.UserId, partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_SuccessRatePerOperator",
                        Name = LocalizedStrings.GetString("Web_Menu_SuccessRatePerOperator"),
                        // TODO : Personnal stats
                        Action = (vm, url) => vm.User.Roles.Any(r => new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Documentalist }.Contains(r))
                            ? url.Action("OperatorQualification", "Publication", new { partial = true })
                            : url.Action("OperatorQualification", "Publication", new { userId = vm.User.UserId, partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist }
                    }
                }
            },
            new MenuItemViewModel
            {
                Id = "MenuItem_Inspections",
                Name = LocalizedStrings.GetString("Web_Menu_Inspections"),
                Sprite = "fa fa-check",
                AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Technician, KnownRoles.Documentalist },
                Childs = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_PublishedInspectionsList",
                        Name = LocalizedStrings.GetString("Web_Menu_PublishedInspectionsList"),
                        Action = (vm, url) => url.Action("Index", "Publication", new { PublishModeFilter = 4, partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Technician, KnownRoles.Documentalist }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_InspectionScheduling",
                        Name = LocalizedStrings.GetString("Web_Menu_InspectionScheduling"),
                        Action = (vm, url) => url.Action("Index", "Schedule", new { partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor }
                    },
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_InspectionMonitoring",
                        Name = LocalizedStrings.GetString("Web_Menu_InspectionMonitoring"),
                        Action = (vm, url) => vm.User.Roles.Any(r => new[] { KnownRoles.Administrator, KnownRoles.Supervisor }.Contains(r))
                            ? url.Action("Index", "Inspection", new { partial = true })
                            : url.Action("Index", "Inspection", new { opeId = vm.User.UserId, partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Technician }
                    }
                }
            },
            new MenuItemViewModel
            {
                Id = "MenuItem_Audit",
                Name = LocalizedStrings.GetString("Web_Menu_Audit"),
                Sprite = "fa fa-list-alt",
                AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Technician, KnownRoles.Documentalist },
                Childs = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_Surveys",
                        Name = LocalizedStrings.GetString("Web_Menu_Surveys"),
                        Action = (vm, url) => url.Action("Index", "Questionnaire", new { partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor }
                    },new MenuItemViewModel
                    {
                        Id = "MenuItem_AuditsList",
                        Name = LocalizedStrings.GetString("Web_Menu_AuditsList"),
                        Action = (vm, url) => vm.User.Roles.Any(r => new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Documentalist }.Contains(r))
                            ? url.Action("Index", "Audit", new { partial = true })
                            : url.Action("Index", "Audit", new { userId = vm.User.UserId, partial = true}),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Technician, KnownRoles.Documentalist }
                    },
                    
                    new MenuItemViewModel
                    {
                        Id = "MenuItem_AuditMonitoring",
                        Name = LocalizedStrings.GetString("Web_Menu_AuditMonitoring"),
                        Action = (vm, url) => url.Action("AuditSummary", "Audit", new { partial = true }),
                        AuthorizedRoles = new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Documentalist }
                    }
                }
            }
        };

        public UserModel User { get; } = JwtTokenProvider.GetUserModel(System.Web.HttpContext.Current.Request.Cookies["token"].Value);
        public List<MenuItemViewModel> TreeNode { get; }

        void RemoveUnauthorizedItems(List<MenuItemViewModel> root)
        {
            root.RemoveWhere(_ => !_.IsAuthorized(User));
            foreach (var childs in root)
                RemoveUnauthorizedItems(childs.Childs);
        }

        public SharedViewModel(UrlHelper urlHelper)
        {
            var LocalizedStrings = DependencyResolver.Current.GetService<ILocalizationManager>();
            WebVersion = Assembly.GetExecutingAssembly().FullName
                .Split(',')
                .Single(_ => _.Contains("Version="))
                .Split('=')
                .Last();

            TreeNode = GetMenuRights(LocalizedStrings);
            RemoveUnauthorizedItems(TreeNode);
            foreach (var menuItem in TreeNode.Flatten(_ => _.Childs).Where(_ => _.Action != null))
                menuItem.Url = menuItem.Action(this, urlHelper);
        }
    }

    public class MenuItemViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsExpanded { get; set; }
        public List<MenuItemViewModel> Childs { get; set; } = new List<MenuItemViewModel>();
        public bool HasChild => Childs.Any();
        public string ParentId { get; set; }
        public string Url { get; set; }
        public Func<SharedViewModel, UrlHelper, string> Action { get; set; }
        public string Sprite { get; set; }
        public string[] AuthorizedRoles { get; set; }
        public Predicate<UserModel> IsAuthorized => u => u.Roles.Any(r => AuthorizedRoles.Contains(r));

        public Dictionary<string, string> NodeProperty { get; set; } = new Dictionary<string, string>();
    }
}