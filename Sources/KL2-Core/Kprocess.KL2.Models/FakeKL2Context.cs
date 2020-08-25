using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace Kprocess.KL2.Models
{
    public class FakeKL2Context : IKL2Context
    {
        public DbSet<ActionCategory> ActionCategories { get; set; }
        public DbSet<ActionType> ActionTypes { get; set; }
        public DbSet<ActionValue> ActionValues { get; set; }
        public DbSet<AppResourceKey> AppResourceKeys { get; set; }
        public DbSet<AppResourceValue> AppResourceValues { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<KAction> KActions { get; set; }
        public DbSet<KActionReduced> KActionsReduced { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Objective> Objectives { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectDir> ProjectDirs { get; set; }
        public DbSet<ProjectReferential> ProjectReferentials { get; set; }
        public DbSet<Ref1> Refs1 { get; set; }
        public DbSet<Ref1Action> Ref1Actions { get; set; }
        public DbSet<Ref2> Refs2 { get; set; }
        public DbSet<Ref2Action> Ref2Actions { get; set; }
        public DbSet<Ref3> Refs3 { get; set; }
        public DbSet<Ref3Action> Ref3Actions { get; set; }
        public DbSet<Ref4> Refs4 { get; set; }
        public DbSet<Ref4Action> Ref4Actions { get; set; }
        public DbSet<Ref5> Refs5 { get; set; }
        public DbSet<Ref5Action> Ref5Actions { get; set; }
        public DbSet<Ref6> Refs6 { get; set; }
        public DbSet<Ref6Action> Ref6Actions { get; set; }
        public DbSet<Ref7> Refs7 { get; set; }
        public DbSet<Ref7Action> Ref7Actions { get; set; }
        public DbSet<Referential> Referentials { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<ScenarioNature> ScenarioNatures { get; set; }
        public DbSet<ScenarioState> ScenarioStates { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<UISetting> UISettings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRoleProject> UserRoleProjects { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<VideoNature> VideoNatures { get; set; }
        public DbSet<CutVideo> CutVideos { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<PublishedAction> PublishedActions { get; set; }
        public DbSet<PublishedActionCategory> PublishedActionCategories { get; set; }
        public DbSet<PublishedReferential> PublishedReferentials { get; set; }
        public DbSet<PublishedReferentialAction> PublishedReferentialActions { get; set; }
        public DbSet<PublishedResource> PublishedResources { get; set; }
        public DbSet<PublishedFile> PublishedFiles { get; set; }
        public virtual DbSet<Team> Teams { get; set; }

        public FakeKL2Context()
        {
            KActions = new FakeDbSet<KAction>(nameof(KAction.ActionId));
            KActionsReduced = new FakeDbSet<KActionReduced>(nameof(KActionReduced.ActionId));
            ActionTypes = new FakeDbSet<ActionType>(nameof(ActionType.ActionTypeCode));
            ActionValues = new FakeDbSet<ActionValue>(nameof(ActionValue.ActionValueCode));
            AppResourceKeys = new FakeDbSet<AppResourceKey>(nameof(AppResourceKey.ResourceId));
            AppResourceValues = new FakeDbSet<AppResourceValue>(nameof(AppResourceValue.ResourceId), nameof(AppResourceValue.LanguageCode));
            AppSettings = new FakeDbSet<AppSetting>(nameof(AppSetting.Key));
            Languages = new FakeDbSet<Language>(nameof(Language.LanguageCode));
            Objectives = new FakeDbSet<Objective>(nameof(Objective.ObjectiveCode));
            Projects = new FakeDbSet<Project>(nameof(Project.ProjectId));
            ProjectDirs = new FakeDbSet<ProjectDir>(nameof(ProjectDir.Id));
            ProjectReferentials = new FakeDbSet<ProjectReferential>(nameof(ProjectReferential.ProjectId), nameof(ProjectReferential.ReferentialId));
            Refs1 = new FakeDbSet<Ref1>(nameof(Ref1.RefId));
            Ref1Actions = new FakeDbSet<Ref1Action>(nameof(Ref1Action.ActionId), nameof(Ref1Action.RefId));
            Refs2 = new FakeDbSet<Ref2>(nameof(Ref2.RefId));
            Ref2Actions = new FakeDbSet<Ref2Action>(nameof(Ref2Action.ActionId), nameof(Ref2Action.RefId));
            Refs3 = new FakeDbSet<Ref3>(nameof(Ref3.RefId));
            Ref3Actions = new FakeDbSet<Ref3Action>(nameof(Ref3Action.ActionId), nameof(Ref3Action.RefId));
            Refs4 = new FakeDbSet<Ref4>(nameof(Ref4.RefId));
            Ref4Actions = new FakeDbSet<Ref4Action>(nameof(Ref4Action.ActionId), nameof(Ref4Action.RefId));
            Refs5 = new FakeDbSet<Ref5>(nameof(Ref5.RefId));
            Ref5Actions = new FakeDbSet<Ref5Action>(nameof(Ref5Action.ActionId), nameof(Ref5Action.RefId));
            Refs6 = new FakeDbSet<Ref6>(nameof(Ref6.RefId));
            Ref6Actions = new FakeDbSet<Ref6Action>(nameof(Ref6Action.ActionId), nameof(Ref6Action.RefId));
            Refs7 = new FakeDbSet<Ref7>(nameof(Ref7.RefId));
            Ref7Actions = new FakeDbSet<Ref7Action>(nameof(Ref7Action.ActionId), nameof(Ref7Action.RefId));
            ActionCategories = new FakeDbSet<ActionCategory>(nameof(ActionCategory.ActionCategoryId));
            Referentials = new FakeDbSet<Referential>(nameof(Referential.ReferentialId));
            Resources = new FakeDbSet<Resource>(nameof(Resource.ResourceId));
            Roles = new FakeDbSet<Role>(nameof(Role.RoleCode));
            Scenarios = new FakeDbSet<Scenario>(nameof(Scenario.ScenarioId));
            ScenarioNatures = new FakeDbSet<ScenarioNature>(nameof(ScenarioNature.ScenarioNatureCode));
            ScenarioStates = new FakeDbSet<ScenarioState>(nameof(ScenarioState.ScenarioStateCode));
            Solutions = new FakeDbSet<Solution>(nameof(Solution.SolutionId));
            UISettings = new FakeDbSet<UISetting>(nameof(UISetting.Key));
            Users = new FakeDbSet<User>(nameof(User.UserId));
            UserRoleProjects = new FakeDbSet<UserRoleProject>(nameof(UserRoleProject.UserId), nameof(UserRoleProject.RoleCode), nameof(UserRoleProject.ProjectId));
            Videos = new FakeDbSet<Video>(nameof(Video.VideoId));
            VideoNatures = new FakeDbSet<VideoNature>(nameof(VideoNature.VideoNatureCode));
        }

        public int SaveChangesCount { get; private set; }
        public int SaveChanges()
        {
            ++SaveChangesCount;
            return 1;
        }

        public Task<int> SaveChangesAsync()
        {
            ++SaveChangesCount;
            return Task<int>.Factory.StartNew(() => 1);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            ++SaveChangesCount;
            return Task<int>.Factory.StartNew(() => 1, cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose() =>
            Dispose(true);

        DbChangeTracker _changeTracker;
        public DbChangeTracker ChangeTracker => _changeTracker;

        DbContextConfiguration _configuration;
        public DbContextConfiguration Configuration => _configuration;

        Database _database;
        public Database Database => _database;

        public DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }
        public DbEntityEntry Entry(object entity)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<DbEntityValidationResult> GetValidationErrors()
        {
            throw new NotImplementedException();
        }
        public DbSet Set(Type entityType)
        {
            throw new NotImplementedException();
        }
        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            throw new NotImplementedException();
        }


        // Stored Procedures
        public int AddColumnIfNotExists(string tableName, string columnName, string @params) => 0;

        public int DeleteResource(string key) => 0;

        public int DropColumnWithConstraints(string tableName, string columnName) => 0;

        public List<GetDatabaseVersionReturnModel> GetDatabaseVersion() =>
            GetDatabaseVersion(out var procResult);

        public List<GetDatabaseVersionReturnModel> GetDatabaseVersion(out int procResult)
        {
            procResult = 0;
            return new List<GetDatabaseVersionReturnModel>();
        }

        public Task<List<GetDatabaseVersionReturnModel>> GetDatabaseVersionAsync() =>
            Task.FromResult(GetDatabaseVersion(out var procResult));

        public int InsertOrUpdateDatabaseVersion(string version) => 0;

        public int InsertOrUpdateResource(string key, string language, string value, string comment) => 0;
    }
}
