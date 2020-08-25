using KProcess.KL2.Languages;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Data
{
    public class KsmedEntitiesExt : KsmedEntities
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
            ObjectMaterialized += OnObjectMaterialized;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="KsmedEntitiesExt"/>.
        /// </summary>
        public KsmedEntitiesExt(EntityConnection connection)
            : base(connection)
        {
            ObjectMaterialized += OnObjectMaterialized;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="KsmedEntitiesExt"/>.
        /// </summary>
        public KsmedEntitiesExt(EntityConnection connection, ILocalizationManager localizationManager)
            : base(connection)
        {
            _localizationManager = localizationManager;
            ObjectMaterialized += OnObjectMaterialized;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="KsmedEntitiesExt"/>.
        /// </summary>
        public KsmedEntitiesExt(EntityConnection connection, SecurityUser user)
            : base(connection)
        {
            _currentUser = user;
            ObjectMaterialized += OnObjectMaterialized;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="KsmedEntitiesExt"/>.
        /// </summary>
        public KsmedEntitiesExt(EntityConnection connection, SecurityUser user, ILocalizationManager localizationManager)
            : base(connection)
        {
            _currentUser = user;
            _localizationManager = localizationManager;
            ObjectMaterialized += OnObjectMaterialized;
        }


        /// <summary>
        /// Appelé lorsqu'un objet est matérialisé après une requête;
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="ObjectMaterializedEventArgs"/> contenant les données de l'évènement.</param>
        void OnObjectMaterialized(object sender, ObjectMaterializedEventArgs e) =>
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

            IEnumerable<ObjectStateEntry> stateEntries = ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified)
                      .Where(e => (e.Entity as IAuditable)?.CustomAudit == false);

            int userId = GetUserId(CurrentEditingUsername);

            foreach (ObjectStateEntry stateEntry in stateEntries)
                UpdateAuditInformation(stateEntry, userId);

#if DEBUG
            // Utile pour inspection en mode débug uniquement
            KeyValuePair<EntityState, object>[] allchangedEntities = ObjectStateManager.GetObjectStateEntries(
                              EntityState.Added |
                              EntityState.Modified |
                              EntityState.Deleted)
                              .Where(e => e.Entity is IObjectWithChangeTracker)
                              .Select(entry => new KeyValuePair<EntityState, object>(entry.State, entry.Entity))
                              .ToArray();

            IGrouping<EntityKey, ObjectStateEntry>[] duplicatedKeys = ObjectStateManager.GetObjectStateEntries(EntityState.Added |
                                  EntityState.Modified |
                                  EntityState.Unchanged |
                                  EntityState.Deleted)
                                  .Where(e => e.Entity is IObjectWithChangeTracker)
                                  .GroupBy(e => e.EntityKey)
                                  .Where(g => g.Count() > 1)
                                  .ToArray();
#endif

            IObjectWithChangeTracker[] allEntities = ObjectStateManager.GetObjectStateEntries(
                              EntityState.Added |
                              EntityState.Modified |
                              EntityState.Unchanged |
                              EntityState.Deleted)
                              .Select(entry => entry.Entity)
                              .OfType<IObjectWithChangeTracker>()
                              .ToArray();

            int ret = await base.SaveChangesAsync();

            // Passer tout en unchanged
            foreach (IObjectWithChangeTracker entity in allEntities)
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
        void UpdateAuditInformation(ObjectStateEntry stateEntry, int userId)
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


    }
}