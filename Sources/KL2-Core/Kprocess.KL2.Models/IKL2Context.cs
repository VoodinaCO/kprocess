using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading.Tasks;

namespace Kprocess.KL2.Models
{
    public interface IKL2Context : IDisposable
    {
        DbSet<ActionCategory> ActionCategories { get; set; }
        DbSet<ActionType> ActionTypes { get; set; }
        DbSet<ActionValue> ActionValues { get; set; }
        DbSet<AppResourceKey> AppResourceKeys { get; set; }
        DbSet<AppResourceValue> AppResourceValues { get; set; }
        DbSet<AppSetting> AppSettings { get; set; }
        DbSet<KAction> KActions { get; set; }
        DbSet<KActionReduced> KActionsReduced { get; set; }
        DbSet<Procedure> Procedures { get; set; }
        DbSet<Language> Languages { get; set; }
        DbSet<Objective> Objectives { get; set; }
        DbSet<Project> Projects { get; set; }
        DbSet<ProjectDir> ProjectDirs { get; set; }
        DbSet<ProjectReferential> ProjectReferentials { get; set; }
        DbSet<Ref1> Refs1 { get; set; }
        DbSet<Ref1Action> Ref1Actions { get; set; }
        DbSet<Ref2> Refs2 { get; set; }
        DbSet<Ref2Action> Ref2Actions { get; set; }
        DbSet<Ref3> Refs3 { get; set; }
        DbSet<Ref3Action> Ref3Actions { get; set; }
        DbSet<Ref4> Refs4 { get; set; }
        DbSet<Ref4Action> Ref4Actions { get; set; }
        DbSet<Ref5> Refs5 { get; set; }
        DbSet<Ref5Action> Ref5Actions { get; set; }
        DbSet<Ref6> Refs6 { get; set; }
        DbSet<Ref6Action> Ref6Actions { get; set; }
        DbSet<Ref7> Refs7 { get; set; }
        DbSet<Ref7Action> Ref7Actions { get; set; }
        DbSet<Referential> Referentials { get; set; }
        DbSet<Resource> Resources { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Scenario> Scenarios { get; set; }
        DbSet<ScenarioNature> ScenarioNatures { get; set; }
        DbSet<ScenarioState> ScenarioStates { get; set; }
        DbSet<Solution> Solutions { get; set; }
        DbSet<UISetting> UISettings { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<UserRoleProject> UserRoleProjects { get; set; }
        DbSet<Video> Videos { get; set; }
        DbSet<VideoNature> VideoNatures { get; set; }
        DbSet<CutVideo> CutVideos { get; set; }
        DbSet<Publication> Publications { get; set; }
        DbSet<PublishedAction> PublishedActions { get; set; }
        DbSet<PublishedActionCategory> PublishedActionCategories { get; set; }
        DbSet<PublishedReferential> PublishedReferentials { get; set; }
        DbSet<PublishedReferentialAction> PublishedReferentialActions { get; set; }
        DbSet<PublishedResource> PublishedResources { get; set; }
        DbSet<PublishedFile> PublishedFiles { get; set; }
        DbSet<Team> Teams { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken);
        DbChangeTracker ChangeTracker { get; }
        DbContextConfiguration Configuration { get; }
        Database Database { get; }
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DbEntityEntry Entry(object entity);
        IEnumerable<DbEntityValidationResult> GetValidationErrors();
        DbSet Set(Type entityType);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        string ToString();

        // Stored Procedures
        int AddColumnIfNotExists(string tableName, string columnName, string @params);
        // AddColumnIfNotExistsAsync cannot be created due to having out parameters, or is relying on the procedure result (int)

        int DeleteResource(string key);
        // DeleteResourceAsync cannot be created due to having out parameters, or is relying on the procedure result (int)

        int DropColumnWithConstraints(string tableName, string columnName);
        // DropColumnWithConstraintsAsync cannot be created due to having out parameters, or is relying on the procedure result (int)

        List<GetDatabaseVersionReturnModel> GetDatabaseVersion();
        List<GetDatabaseVersionReturnModel> GetDatabaseVersion(out int procResult);
        Task<List<GetDatabaseVersionReturnModel>> GetDatabaseVersionAsync();

        int InsertOrUpdateDatabaseVersion(string version);
        // InsertOrUpdateDatabaseVersionAsync cannot be created due to having out parameters, or is relying on the procedure result (int)

        int InsertOrUpdateResource(string key, string language, string value, string comment);
        // InsertOrUpdateResourceAsync cannot be created due to having out parameters, or is relying on the procedure result (int)
    }
}
