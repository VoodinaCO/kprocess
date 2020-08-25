using KProcess.Globalization;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TrackableEntities;
using TrackableEntities.Common;

namespace Kprocess.KL2.Models
{
    public class KsmedEntitiesExt : KsmedEntities, IKL2Context
    {
        private readonly SecurityUser _currentUser;
        private readonly ILocalizationManager _localizationManager;
        Dictionary<string, int> _usersCache;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="KsmedEntitiesExt"/>.
        /// </summary>
        public KsmedEntitiesExt(
            string connectionString)
            : base(connectionString)
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += OnEntityMaterialized;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="KsmedEntitiesExt"/>.
        /// </summary>
        public KsmedEntitiesExt(EntityConnection connection)
            : base(connection)
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += OnEntityMaterialized;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="KsmedEntitiesExt"/>.
        /// </summary>
        public KsmedEntitiesExt(EntityConnection connection, ILocalizationManager localizationManager)
            : base(connection)
        {
            _localizationManager = localizationManager;
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += OnEntityMaterialized;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="KsmedEntitiesExt"/>.
        /// </summary>
        public KsmedEntitiesExt(EntityConnection connection, SecurityUser user)
            : base(connection)
        {
            _currentUser = user;
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += OnEntityMaterialized;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="KsmedEntitiesExt"/>.
        /// </summary>
        public KsmedEntitiesExt(EntityConnection connection, SecurityUser user, ILocalizationManager localizationManager)
            : base(connection)
        {
            _currentUser = user;
            _localizationManager = localizationManager;
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += OnEntityMaterialized;
        }


        /// <summary>
        /// Appelé lorsqu'un objet est matérialisé après une requête;
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="ObjectMaterializedEventArgs"/> contenant les données de l'évènement.</param>
        void OnEntityMaterialized(object sender, ObjectMaterializedEventArgs e) =>
            LoadLocalizedLabels(e.Entity);

        /// <summary>
        /// Charge les labels localisés d'une entité.
        /// </summary>
        /// <param name="entity">L'entité.</param>
        void LoadLocalizedLabels(object entity)
        {
            if (entity is ILocalizedLabels localizable)
                LocalizableLabels.LoadLabel(localizable, _localizationManager);
        }

        /// <summary>
        /// Sauvegarde les entités
        /// </summary>
        /// <returns>Le nombre d'entités modifiées.</returns>
        public override async Task<int> SaveChangesAsync()
        {
            UpdateCurrentEditingUsername();

            IEnumerable<DbEntityEntry> stateEntries = ChangeTracker.Entries()
                .Where(_ => (_.State == EntityState.Added || _.State == EntityState.Modified) && (_.Entity as IAuditable)?.CustomAudit == false);

            int userId = GetUserId(CurrentEditingUsername);

            foreach (DbEntityEntry stateEntry in stateEntries)
                UpdateAuditInformation(stateEntry, userId);

#if DEBUG
            // Utile pour inspection en mode débug uniquement
            KeyValuePair<EntityState, object>[] allchangedEntities = ChangeTracker.Entries()
                              .Where(_ => (_.State == EntityState.Added || _.State == EntityState.Modified || _.State == EntityState.Deleted) && _.Entity is ITrackable)
                              .Select(entry => new KeyValuePair<EntityState, object>(entry.State, entry.Entity))
                              .ToArray();

            IGrouping<int, DbEntityEntry>[] duplicatedKeys = ChangeTracker.Entries()
                                  .Where(_ => (_.State == EntityState.Added || _.State == EntityState.Modified || _.State == EntityState.Unchanged || _.State == EntityState.Deleted) && _.Entity is ITrackable)
                                  .GroupBy(e => e.Entity.GetHashCode())
                                  .Where(g => g.Count() > 1)
                                  .ToArray();
#endif

            ITrackable[] allEntities = ChangeTracker.Entries()
                              .Where(_ => _.State == EntityState.Added || _.State == EntityState.Modified || _.State == EntityState.Unchanged || _.State == EntityState.Deleted)
                              .Select(entry => entry.Entity)
                              .OfType<ITrackable>()
                              .ToArray();

            int ret = await base.SaveChangesAsync();

            // Passer tout en unchanged
            foreach (ITrackable entity in allEntities)
                entity.AcceptChanges();

            return ret;
        }

        /// <summary>
        /// Met à jour le nom de l'utilisateur qui fait la miste à jour.
        /// </summary>
        void UpdateCurrentEditingUsername()
        {
            // If current provided when construction of context
            if (_currentUser != null)
                CurrentEditingUsername = _currentUser.Username;
            else
                throw new Exception("Security context needs to migrated to ISecurityContext. Review the call to ContextFactory");
            //else
            //    CurrentEditingUsername = SecurityContext.CurrentUser.Username;
        }

        /// <summary>
        /// Met à jour les informations d'audit.
        /// </summary>
        /// <param name="stateEntry">L'<see cref="ObjectStateEntry"/> de l'entité.</param>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        void UpdateAuditInformation(DbEntityEntry stateEntry, int userId)
        {
            IAuditable entity = stateEntry.Entity as IAuditable;
            IAuditableUserOptional entityUserOptional = entity as IAuditableUserOptional;
            IAuditableUserRequired entityUserRequired = entity as IAuditableUserRequired;

            DateTime now = DateTime.Now;

            if (stateEntry.State == EntityState.Added)
            {
                if (entityUserOptional != null)
                    entityUserOptional.CreatedByUserId = userId;
                else if (entityUserRequired != null)
                    entityUserRequired.CreatedByUserId = userId;

                entity.CreationDate = now;
            }

            if (entityUserOptional != null)
                entityUserOptional.ModifiedByUserId = userId;
            else if (entityUserRequired != null)
                entityUserRequired.ModifiedByUserId = userId;

            entity.LastModificationDate = now;
        }

        /// <summary>
        /// Obtient l'identifiant de l'utilisateur à partir de son login.
        /// </summary>
        /// <param name="username">Le login.</param>
        /// <returns>L'identifiant.</returns>
        int GetUserId(string username)
        {
            if (_usersCache == null)
                _usersCache = new Dictionary<string, int>();

            if (!_usersCache.ContainsKey(username))
            {
                int userId = Users.Single(u => !u.IsDeleted && u.Username == username).UserId;
                _usersCache[username] = userId;
            }

            return _usersCache[username];
        }

        #region Stored Procedures

        public int AddColumnIfNotExists(string tableName, string columnName, string @params)
        {
            var tableNameParam = new SqlParameter { ParameterName = "@tableName", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = tableName, Size = 100 };
            if (tableNameParam.Value == null)
                tableNameParam.Value = DBNull.Value;

            var columnNameParam = new SqlParameter { ParameterName = "@columnName", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = columnName, Size = 100 };
            if (columnNameParam.Value == null)
                columnNameParam.Value = DBNull.Value;

            var @paramsParam = new SqlParameter { ParameterName = "@params", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = @params, Size = -1 };
            if (@paramsParam.Value == null)
                @paramsParam.Value = DBNull.Value;

            var procResultParam = new SqlParameter { ParameterName = "@procResult", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "EXEC @procResult = [dbo].[AddColumnIfNotExists] @tableName, @columnName, @params", tableNameParam, columnNameParam, @paramsParam, procResultParam);

            return (int)procResultParam.Value;
        }

        public int DeleteResource(string key)
        {
            var keyParam = new SqlParameter { ParameterName = "@key", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = key, Size = 100 };
            if (keyParam.Value == null)
                keyParam.Value = DBNull.Value;

            var procResultParam = new SqlParameter { ParameterName = "@procResult", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "EXEC @procResult = [dbo].[DeleteResource] @key", keyParam, procResultParam);

            return (int)procResultParam.Value;
        }

        public int DropColumnWithConstraints(string tableName, string columnName)
        {
            var tableNameParam = new SqlParameter { ParameterName = "@tableName", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = tableName, Size = 100 };
            if (tableNameParam.Value == null)
                tableNameParam.Value = DBNull.Value;

            var columnNameParam = new SqlParameter { ParameterName = "@columnName", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = columnName, Size = 100 };
            if (columnNameParam.Value == null)
                columnNameParam.Value = DBNull.Value;

            var procResultParam = new SqlParameter { ParameterName = "@procResult", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "EXEC @procResult = [dbo].[DropColumnWithConstraints] @tableName, @columnName", tableNameParam, columnNameParam, procResultParam);

            return (int)procResultParam.Value;
        }

        public List<GetDatabaseVersionReturnModel> GetDatabaseVersion() =>
            GetDatabaseVersion(out var procResult);

        public List<GetDatabaseVersionReturnModel> GetDatabaseVersion(out int procResult)
        {
            var procResultParam = new SqlParameter { ParameterName = "@procResult", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            var procResultData = Database.SqlQuery<GetDatabaseVersionReturnModel>("EXEC @procResult = [dbo].[GetDatabaseVersion] ", procResultParam).ToList();

            procResult = (int)procResultParam.Value;
            return procResultData;
        }

        public async Task<List<GetDatabaseVersionReturnModel>> GetDatabaseVersionAsync() =>
            await Database.SqlQuery<GetDatabaseVersionReturnModel>("EXEC [dbo].[GetDatabaseVersion] ").ToListAsync();

        public int InsertOrUpdateDatabaseVersion(string version)
        {
            var versionParam = new SqlParameter { ParameterName = "@version", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = version, Size = 30 };
            if (versionParam.Value == null)
                versionParam.Value = DBNull.Value;

            var procResultParam = new SqlParameter { ParameterName = "@procResult", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "EXEC @procResult = [dbo].[InsertOrUpdateDatabaseVersion] @version", versionParam, procResultParam);

            return (int)procResultParam.Value;
        }

        public int InsertOrUpdateResource(string key, string language, string value, string comment)
        {
            var keyParam = new SqlParameter { ParameterName = "@key", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = key, Size = 100 };
            if (keyParam.Value == null)
                keyParam.Value = DBNull.Value;

            var languageParam = new SqlParameter { ParameterName = "@language", SqlDbType = SqlDbType.NChar, Direction = ParameterDirection.Input, Value = language, Size = 5 };
            if (languageParam.Value == null)
                languageParam.Value = DBNull.Value;

            var valueParam = new SqlParameter { ParameterName = "@value", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = value, Size = 500 };
            if (valueParam.Value == null)
                valueParam.Value = DBNull.Value;

            var commentParam = new SqlParameter { ParameterName = "@comment", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = comment, Size = 500 };
            if (commentParam.Value == null)
                commentParam.Value = DBNull.Value;

            var procResultParam = new SqlParameter { ParameterName = "@procResult", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "EXEC @procResult = [dbo].[InsertOrUpdateResource] @key, @language, @value, @comment", keyParam, languageParam, valueParam, commentParam, procResultParam);

            return (int)procResultParam.Value;
        }

        #endregion
    }
}