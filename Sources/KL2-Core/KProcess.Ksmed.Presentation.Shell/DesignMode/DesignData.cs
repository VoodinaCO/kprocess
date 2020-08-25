using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Fournit des fausses données utilisées en mode design.
    /// </summary>
    public static class DesignData
    {

        /// <summary>
        /// Génère des utilisateurs.
        /// </summary>
        public static IEnumerable<User> GenerateUsers()
        {
            return new User[]
            {
                new User { Firstname = "Prénom", Name = "User 1", PhoneNumber = "0606060606", Email = "toto@titi.fr"},
                new User { Firstname = "Prénom", Name = "Fernand" },
                new User { Firstname = "Prénom", Name = "Bidule" },
            };
        }

        /// <summary>
        /// Génère des roles.
        /// </summary>
        public static IEnumerable<Role> GenerateRoles()
        {
            return new Role[]
            {
                new Role { ShortLabel = "Analyste"},
                new Role { ShortLabel = "Administrateur" },
                new Role { ShortLabel = "Lecteur" },
                new Role { ShortLabel = "Exporteur", RoleCode = KnownRoles.Exporter },
            };
        }

        /// <summary>
        /// Génère des UserRoleProcess.
        /// </summary>
        public static IEnumerable<UserRoleProcess> GenerateUserRoleProcesses(IEnumerable<User> users, IEnumerable<Role> roles)
        {
            return new UserRoleProcess[]
            {
                new UserRoleProcess { User = users.ElementAt(0), Role = roles.ElementAt(0) },
                new UserRoleProcess { User = users.ElementAt(1), Role = roles.ElementAt(2) },
                new UserRoleProcess { User = users.ElementAt(2), Role = roles.ElementAt(2) },
            };

        }

        /// <summary>
        /// Lie les utilisateurs et les rôles spécifiés.
        /// </summary>
        /// <param name="users">Les utilsateurs.</param>
        /// <param name="roles">les rôles.</param>
        public static void LinkUsersWithRoles(IEnumerable<User> users, IEnumerable<Role> roles)
        {
            bool b = true;
            foreach (User u in users)
            {
                foreach (Role role in roles)
                {
                    if (b)
                        u.Roles.Add(role);

                    b = !b;
                }
            }
        }

        /// <summary>
        /// Génère des ressources.
        /// </summary>
        public static IEnumerable<Resource> GenerateResources()
        {
            Procedure process1 = new Procedure { Label = "Process1" };
            Procedure process2 = new Procedure { Label = "Process2" };
            return new Resource[]
            {
                new Operator { Label = "Operateur 1", PaceRating=1.1, Process = process1},
                new Operator { Label = "Operateur 2", PaceRating=1.2},
                new Operator { Label = "Monsieur truc", PaceRating=1.3, Process = process2},
                new Equipment { Label = "Equipement 1", PaceRating=1.4, Process = process1}
            };
        }

        /// <summary>
        /// Génère des vidéos.
        /// </summary>
        public static IEnumerable<Video> GenerateVideos()
        {
            return new Video[]
            {
                new Video 
                {
                    CameraName = "Cam 1", 
                    Duration = 2398.3,
                    Format = "WMV blabla",
                    FilePath = @"D:\Projets\KPROCESS\POCs\20100914093510.wmv",
                    ResourceView = ResourceViewEnum.Intern
                },
                new Video { CameraName = "Cam 1" },
                new Video { CameraName = "Cam 2" },
            };
        }

        /// <summary>
        /// Génère des langues.
        /// </summary>
        public static IEnumerable<Language> GenerateLanguages()
        {
            return new Language[]
            {
                new Language() { Label = "Français", LanguageCode = "fr-FR" },
                new Language() { Label = "English", LanguageCode = "en-US" },
            };
        }

        /// <summary>
        /// Génère des objectifs.
        /// </summary>
        public static IEnumerable<Objective> GeneratesObjectives()
        {
            return new Objective[]
            {
                new Objective() { LongLabel = "Obj 1", },
                new Objective() { LongLabel = "MEttre en oeuvre truc muche",  },
                new Objective() { LongLabel = "Obj 3",  },
            };
        }

        /// <summary>
        /// Génère un projet avec ses détails.
        /// </summary>
        public static Project GenerateProjectWithDetails()
        {
            Resource defaultResource = GenerateResources().First();
            Procedure process = new Procedure()
            {
                Label = "Atelier 123",
                Videos = new TrackableCollection<Video>()
                {
                    new Video() { CameraName = "Cam 1", DefaultResource = defaultResource },
                    new Video() { CameraName = "Cam 2", DefaultResource = null},
                    new Video() { CameraName = "Cam 3", DefaultResource = null},
                },
            };
            return new Project()
            {
                Label = "Mon projet",
                Description = "BLABLABLABLABLA",
                Workshop = "Atelier 123",
                Process = new Procedure()
                {
                    Label = "Atelier 123",
                    Videos = new TrackableCollection<Video>()
                    {
                        new Video() { CameraName = "Cam 1", DefaultResource = defaultResource },
                        new Video() { CameraName = "Cam 2", DefaultResource = null},
                        new Video() { CameraName = "Cam 3", DefaultResource = null},
                    },
                    UserRoleProcesses = new TrackableCollection<UserRoleProcess>()
                    {
                        new UserRoleProcess() { User = new User() { Name = "Rémy", Firstname = "Boyer" }, Role = new Role() { ShortLabel = "Lecteur" }},
                        new UserRoleProcess() { User = new User() { Name = "QSDQSDQSD", Firstname = "QSDQSDSDSDS" }, Role = new Role() { ShortLabel = "Administrateur" }},
                        new UserRoleProcess() { User = new User() { Name = "Paul", Firstname = "Auchon" }, Role = new Role() { ShortLabel = "Analyste" }},
                    }
                }
            };
        }

        /// <summary>
        /// Génère des projets.
        /// </summary>
        public static IEnumerable<Project> GenerateProjects()
        {
            return new Project[]
            {
                new Project { Label = "Mon projet", Description = "BLABLABLABLABLA", },
                new Project { Label = "Mon projet 2", Description = "BLABLABLABLABLA", },
                new Project { Label = "Mon projet 3", Description = "BLABLABLABLABLA", },
            };
        }

        /// <summary>
        /// Génère des catégories d'action, des types d'action et des valorisations d'actions avec des liaisons entre eux.
        /// </summary>
        public static (IEnumerable<ActionCategory> Categories, IEnumerable<ActionType> ActionTypes, IEnumerable<ActionValue> ActionValues) GenerateActionCategories()
        {
            IEnumerable<ActionType> types = GenerateActionTypes();
            IEnumerable<ActionValue> values = GenerateActionValues();

            Procedure process1 = new Procedure { Label = "Process1" };
            Procedure process2 = new Procedure { Label = "Process2" };

            ActionCategory[] categories =
            {
                new ActionCategory() { Label = "Cat 1", },
                new ActionCategory() { Label = "Cat 2", },
                new ActionCategory() { Label = "Cat 3 P1", Process = process1 },
                new ActionCategory() { Label = "Cat 4 P2", Process = process2 },
            };

            Random rand = new Random();
            foreach (ActionCategory cat in categories)
            {
                cat.Type = types.ElementAt(rand.Next(types.Count()));
                cat.Value = values.ElementAt(rand.Next(values.Count()));
            }

            return (categories, types, values);
        }

        /// <summary>
        /// Génère des compétences d'action.
        /// </summary>
        public static IEnumerable<Skill> GenerateSkills()
        {
            Skill[] skills =
            {
                new Skill() { Label = "Compétence 1", },
                new Skill() { Label = "Compétence 2", }
            };

            return skills;
        }

        /// <summary>
        /// Génère des types d'action.
        /// </summary>
        public static IEnumerable<ActionType> GenerateActionTypes()
        {
            // I/E/S
            return new ActionType[]
            {
                new ActionType() { ActionTypeCode = "I" , ShortLabel = "Interne" },
                new ActionType() { ActionTypeCode = "E" , ShortLabel = "Externe" },
                new ActionType() { ActionTypeCode = "S" , ShortLabel = "S?" },
            };
        }

        /// <summary>
        /// Génère des valorisations d'action.
        /// </summary>
        public static IEnumerable<ActionValue> GenerateActionValues()
        {
            // VA/NVA/BNVA
            return new ActionValue[]
            {
                new ActionValue() { ActionValueCode = "VA" , ShortLabel = "Valeur ajoutée" },
                new ActionValue() { ActionValueCode = "NVA" , ShortLabel = "Non valeur ajoutée" },
                new ActionValue() { ActionValueCode = "BNVA" , ShortLabel = "BNVA ??" },
            };
        }

        /// <summary>
        /// Génère des outils.
        /// </summary>
        public static IEnumerable<Ref1> GenerateRef1s()
        {
            Procedure process1 = new Procedure { Label = "Process1" };
            Procedure process2 = new Procedure { Label = "Process2" };
            return new Ref1[]
            {
                new Ref1() { Label = "Clé à molette", Process = process1 }, 
                new Ref1() { Label = "Outil 1", Process = process2 },
                new Ref1() { Label = "Outil 2" },
            };
        }

        /// <summary>
        /// Génère des consommables.
        /// </summary>
        public static IEnumerable<Ref2> GenerateRef2s()
        {
            Procedure process1 = new Procedure { Label = "Process1" };
            Procedure process2 = new Procedure { Label = "Process2" };
            return new Ref2[]
            {
                new Ref2() { Label = "Consommable 1", Process = process1 }, 
                new Ref2() { Label = "Papier", Process = process2 },
                new Ref2() { Label = "Coca" },
            };
        }

        /// <summary>
        /// Génère des lieux.
        /// </summary>
        public static IEnumerable<Ref3> GenerateRef3s()
        {
            Procedure process1 = new Procedure { Label = "Process1" };
            Procedure process2 = new Procedure { Label = "Process2" };
            return new Ref3[]
            {
                new Ref3() { Label = "Salle à manger", Process = process1 }, 
                new Ref3() { Label = "Salle de travail", Process = process2 },
                new Ref3() { Label = "Machine à café" },
            };
        }

        /// <summary>
        /// Génère des documents.
        /// </summary>
        public static IEnumerable<Ref4> GenerateRef4s()
        {
            Procedure process1 = new Procedure { Label = "Process1" };
            Procedure process2 = new Procedure { Label = "Process2" };
            return new Ref4[]
            {
                new Ref4() { Label = "Document 1", Process = process1 }, 
                new Ref4() { Label = "Livre blanc", Process = process2 },
                new Ref4() { Label = "Livre noir" },
            };
        }

        /// <summary>
        /// Génère des états de scénrii.
        /// </summary>
        public static IEnumerable<ScenarioState> GenerateScenarioStates()
        {
            return new ScenarioState[]
            {
                new ScenarioState { ShortLabel = "State 1" },
                new ScenarioState { ShortLabel = "State 2" },
                new ScenarioState { ShortLabel = "State 3" },
            };
        }

        /// <summary>
        /// Génère des natures de scénrii.
        /// </summary>
        public static IEnumerable<ScenarioNature> GenerateScenarioNatures()
        {
            return new ScenarioNature[]
            {
                new ScenarioNature { ShortLabel = "Nature 1" },
                new ScenarioNature { ShortLabel = "Nature 2" },
                new ScenarioNature { ShortLabel = "Nature 3" },
            };
        }

        /// <summary>
        /// Génère des scénrii.
        /// </summary>
        public static IEnumerable<Scenario> GenerateScenarios(IEnumerable<ScenarioState> states, IEnumerable<ScenarioNature> natures)
        {
            return new Scenario[]
            {
                new Scenario { Label = "Scenario 1", Description = "description 1", Nature = natures.First(), State = states.First() },
                new Scenario { Label = "Scenario 2", Description = "description 2" },
                new Scenario { Label = "Scenario 3", Description = "description 3" },
            };
        }

        /// <summary>
        /// Génère un scénario avec des actions.
        /// </summary>
        public static Scenario GenerateScenarioWithActions(
            IEnumerable<Resource> resources,
            IEnumerable<ActionCategory> categories,
            IEnumerable<Skill> skills,
            IEnumerable<Video> videos)
        {
            Scenario scenario = new Scenario()
            {
                Label = "Scenario"
            };

            Resource resource1 = resources.ElementAt(0);
            Resource resource2 = resources.ElementAt(1);

            ActionCategory cat1 = categories.ElementAt(0);
            ActionCategory cat2 = categories.ElementAt(1);

            Skill skill1 = skills.ElementAt(0);
            Skill skill2 = skills.ElementAt(1);

            Video vid1 = videos.ElementAt(0);
            Video vid2 = videos.ElementAt(1);

            int duration1 = 120;
            int duration2 = 1000;
            int duration3 = 1500;

            KAction root = new KAction()
            {
                Label = "Root",
                Resource = resource1,
                Category = cat1,
                Video = vid1,
                Duration = duration1,
            };
            scenario.Actions.Add(root);

            KAction a1 = new KAction()
            {
                Label = "A1",
                Resource = resource1,
                Category = cat1,
                Video = vid1,
                Duration = duration2,
            };
            scenario.Actions.Add(a1);

            KAction a11 = new KAction()
            {
                Label = "A11",
                Resource = resource1,
                Category = cat1,
                Video = vid1,
                Duration = duration3,
            };
            scenario.Actions.Add(a11);

            KAction a2 = new KAction()
            {
                Label = "A2",
                Resource = resource2,
                Category = cat2,
                Video = vid2,
                Duration = duration2,
            };
            scenario.Actions.Add(a2);

            return scenario;
        }

        /// <summary>
        /// Génére des référentiels pour projet;
        /// </summary>
        public static IEnumerable<ProjectReferential> GenerateProjectReferentials()
        {
            return new ProjectReferential[]
            {
                new ProjectReferential 
                {
                    ReferentialId = ProcessReferentialIdentifier.Operator,
                    IsEnabled = true,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = true,
                },
                new ProjectReferential 
                {
                    ReferentialId = ProcessReferentialIdentifier.Equipment,
                    IsEnabled = true,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = true,
                },
                new ProjectReferential 
                {
                    ReferentialId = ProcessReferentialIdentifier.Category,
                    IsEnabled = true,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential 
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref1,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential 
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref2,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential 
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref3,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential 
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref4,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential 
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref5,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential 
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref6,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential 
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref7,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
            };
        }

    }
}
