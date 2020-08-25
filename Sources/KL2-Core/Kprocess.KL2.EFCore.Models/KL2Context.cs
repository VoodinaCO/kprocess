using KProcess.Ksmed.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace Kprocess.KL2.EFCore.Models
{
    public class KL2Context : DbContext, IKL2Context
    {
        public DbSet<KAction> KActions { get; set; }
        public DbSet<KActionReduced> KActionsReduced { get; set; }
        public DbSet<ActionType> ActionTypes { get; set; }
        public DbSet<ActionValue> ActionValues { get; set; }
        public DbSet<AppResourceKey> AppResourceKeys { get; set; }
        public DbSet<AppResourceValue> AppResourceValues { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
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
        public DbSet<ActionCategory> ActionCategories { get; set; }
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

        static KL2Context()
        {
            //Database.SetInitializer<KL2Context>(null);
        }

        public KL2Context()
            : base(new DbContextOptionsBuilder().UseSqlServer("Name=KL2Entities").Options)
        {
        }

        public KL2Context(string connectionString)
            : base(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
        {
        }

        public KL2Context(string connectionString, IModel model)
            : base(new DbContextOptionsBuilder().UseSqlServer(connectionString).UseModel(model).Options)
        {
        }

        public bool IsSqlParameterNull(SqlParameter param)
        {
            var sqlValue = param.SqlValue;
            if (sqlValue is INullable nullableValue)
                return nullableValue.IsNull;
            return (sqlValue == null || sqlValue == DBNull.Value);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Types<IObjectWithChangeTracker>().Configure(c => c.Ignore(p => p.ChangeTracker)); // Doesn't work for Ref (inheritance, IObjectWithChangeTracker used twice)

            CreateModel(modelBuilder);
        }

        public static ModelBuilder CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new KActionConfiguration());
            modelBuilder.ApplyConfiguration(new ActionPredecessorSuccessorConfiguration());
            modelBuilder.ApplyConfiguration(new KActionReducedConfiguration());
            modelBuilder.ApplyConfiguration(new ActionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ActionValueConfiguration());
            modelBuilder.ApplyConfiguration(new AppResourceKeyConfiguration());
            modelBuilder.ApplyConfiguration(new AppResourceValueConfiguration());
            modelBuilder.ApplyConfiguration(new AppSettingConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageConfiguration());
            modelBuilder.ApplyConfiguration(new ObjectiveConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectDirConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectReferentialConfiguration());
            modelBuilder.ApplyConfiguration(new Ref1Configuration());
            modelBuilder.ApplyConfiguration(new Ref1ActionConfiguration());
            modelBuilder.ApplyConfiguration(new Ref2Configuration());
            modelBuilder.ApplyConfiguration(new Ref2ActionConfiguration());
            modelBuilder.ApplyConfiguration(new Ref3Configuration());
            modelBuilder.ApplyConfiguration(new Ref3ActionConfiguration());
            modelBuilder.ApplyConfiguration(new Ref4Configuration());
            modelBuilder.ApplyConfiguration(new Ref4ActionConfiguration());
            modelBuilder.ApplyConfiguration(new Ref5Configuration());
            modelBuilder.ApplyConfiguration(new Ref5ActionConfiguration());
            modelBuilder.ApplyConfiguration(new Ref6Configuration());
            modelBuilder.ApplyConfiguration(new Ref6ActionConfiguration());
            modelBuilder.ApplyConfiguration(new Ref7Configuration());
            modelBuilder.ApplyConfiguration(new Ref7ActionConfiguration());
            modelBuilder.ApplyConfiguration(new ActionCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new EquipmentConfiguration());
            modelBuilder.ApplyConfiguration(new ReferentialConfiguration());
            modelBuilder.ApplyConfiguration(new OperatorConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new ScenarioConfiguration());
            modelBuilder.ApplyConfiguration(new ScenarioNatureConfiguration());
            modelBuilder.ApplyConfiguration(new ScenarioStateConfiguration());
            modelBuilder.ApplyConfiguration(new SolutionConfiguration());
            modelBuilder.ApplyConfiguration(new UISettingConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleProjectConfiguration());
            modelBuilder.ApplyConfiguration(new VideoConfiguration());
            return modelBuilder;
        }

        #region Stored Procedures

        public int AddColumnIfNotExists(string tableName, string columnName, string @params)
        {
            SqlParameter tableNameParam = new SqlParameter { ParameterName = "@tableName", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = tableName, Size = 100 };
            if (tableNameParam.Value == null)
                tableNameParam.Value = DBNull.Value;

            SqlParameter columnNameParam = new SqlParameter { ParameterName = "@columnName", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = columnName, Size = 100 };
            if (columnNameParam.Value == null)
                columnNameParam.Value = DBNull.Value;

            SqlParameter @paramsParam = new SqlParameter { ParameterName = "@params", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = @params, Size = -1 };
            if (@paramsParam.Value == null)
                @paramsParam.Value = DBNull.Value;

            SqlParameter procResultParam = new SqlParameter { ParameterName = "@procResult", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            Database.ExecuteSqlCommand("EXEC @procResult = [dbo].[AddColumnIfNotExists] @tableName, @columnName, @params", tableNameParam, columnNameParam, @paramsParam, procResultParam);

            return (int)procResultParam.Value;
        }

        public int DeleteResource(string key)
        {
            SqlParameter keyParam = new SqlParameter { ParameterName = "@key", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = key, Size = 100 };
            if (keyParam.Value == null)
                keyParam.Value = DBNull.Value;

            SqlParameter procResultParam = new SqlParameter { ParameterName = "@procResult", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            Database.ExecuteSqlCommand("EXEC @procResult = [dbo].[DeleteResource] @key", keyParam, procResultParam);

            return (int)procResultParam.Value;
        }

        public int DropColumnWithConstraints(string tableName, string columnName)
        {
            SqlParameter tableNameParam = new SqlParameter { ParameterName = "@tableName", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = tableName, Size = 100 };
            if (tableNameParam.Value == null)
                tableNameParam.Value = DBNull.Value;

            SqlParameter columnNameParam = new SqlParameter { ParameterName = "@columnName", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = columnName, Size = 100 };
            if (columnNameParam.Value == null)
                columnNameParam.Value = DBNull.Value;

            SqlParameter procResultParam = new SqlParameter { ParameterName = "@procResult", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            Database.ExecuteSqlCommand("EXEC @procResult = [dbo].[DropColumnWithConstraints] @tableName, @columnName", tableNameParam, columnNameParam, procResultParam);

            return (int)procResultParam.Value;
        }

        public Version GetDatabaseVersion()
        {
            using (var context = new KL2Context())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC [dbo].[GetDatabaseVersion]";
                context.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    Version currentDatabaseVersion = null;
                    if (reader.Read())
                        currentDatabaseVersion = Version.Parse((string)reader[0]);
                    return currentDatabaseVersion;
                }
            }
        }

        public async Task<Version> GetDatabaseVersionAsync()
        {
            using (var context = new KL2Context())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC [dbo].[GetDatabaseVersion]";
                await context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    Version currentDatabaseVersion = null;
                    if (await reader.ReadAsync())
                        currentDatabaseVersion = Version.Parse((string)reader[0]);
                    return currentDatabaseVersion;
                }
            }
        }

        public int InsertOrUpdateDatabaseVersion(string version)
        {
            SqlParameter versionParam = new SqlParameter { ParameterName = "@version", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = version, Size = 30 };
            if (versionParam.Value == null)
                versionParam.Value = DBNull.Value;

            return Database.ExecuteSqlCommand("EXEC [dbo].[InsertOrUpdateDatabaseVersion] @version", versionParam);
        }

        public Task<int> InsertOrUpdateDatabaseVersionAsync(string version)
        {
            SqlParameter versionParam = new SqlParameter { ParameterName = "@version", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = version, Size = 30 };
            if (versionParam.Value == null)
                versionParam.Value = DBNull.Value;

            return Database.ExecuteSqlCommandAsync("EXEC [dbo].[InsertOrUpdateDatabaseVersion] @version", versionParam);
        }

        public int InsertOrUpdateResource(string key, string language, string value, string comment)
        {
            SqlParameter keyParam = new SqlParameter { ParameterName = "@key", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = key, Size = 100 };
            if (keyParam.Value == null)
                keyParam.Value = DBNull.Value;

            SqlParameter languageParam = new SqlParameter { ParameterName = "@language", SqlDbType = SqlDbType.NChar, Direction = ParameterDirection.Input, Value = language, Size = 5 };
            if (languageParam.Value == null)
                languageParam.Value = DBNull.Value;

            SqlParameter valueParam = new SqlParameter { ParameterName = "@value", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = value, Size = 500 };
            if (valueParam.Value == null)
                valueParam.Value = DBNull.Value;

            SqlParameter commentParam = new SqlParameter { ParameterName = "@comment", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = comment, Size = 500 };
            if (commentParam.Value == null)
                commentParam.Value = DBNull.Value;

            return Database.ExecuteSqlCommand("EXEC [dbo].[InsertOrUpdateResource] @key, @language, @value, @comment", keyParam, languageParam, valueParam, commentParam);
        }

        public Task<int> InsertOrUpdateResourceAsync(string key, string language, string value, string comment)
        {
            SqlParameter keyParam = new SqlParameter { ParameterName = "@key", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = key, Size = 100 };
            if (keyParam.Value == null)
                keyParam.Value = DBNull.Value;

            SqlParameter languageParam = new SqlParameter { ParameterName = "@language", SqlDbType = SqlDbType.NChar, Direction = ParameterDirection.Input, Value = language, Size = 5 };
            if (languageParam.Value == null)
                languageParam.Value = DBNull.Value;

            SqlParameter valueParam = new SqlParameter { ParameterName = "@value", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = value, Size = 500 };
            if (valueParam.Value == null)
                valueParam.Value = DBNull.Value;

            SqlParameter commentParam = new SqlParameter { ParameterName = "@comment", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = comment, Size = 500 };
            if (commentParam.Value == null)
                commentParam.Value = DBNull.Value;

            return Database.ExecuteSqlCommandAsync("EXEC [dbo].[InsertOrUpdateResource] @key, @language, @value, @comment", keyParam, languageParam, valueParam, commentParam);
        }

        #endregion
    }
}
