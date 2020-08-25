using KProcess.Globalization;
using KProcess.Ksmed.Models.Security;
using System;

namespace Kprocess.KL2.Models
{
    /// <summary>
    /// Représente une fabrique de contextes Entity Framework
    /// </summary>
    public static class ContextFactory
    {

        static readonly Lazy<KsmedEntitiesExt> _context = new Lazy<KsmedEntitiesExt>(() 
            => new KsmedEntitiesExt(ConnectionStringsSecurity.GetConnectionString(KsmedEntities.ConnectionString)));

        /// <summary>
        /// Obtient un nouveau contexte.
        /// </summary>
        /// <returns>Un nouveau contexte.</returns>
        public static KsmedEntities GetNewContext() =>
            new KsmedEntitiesExt(ConnectionStringsSecurity.GetConnectionString(KsmedEntities.ConnectionString));

        /// <summary>
        /// Obtient un nouveau contexte avec le user courant
        /// </summary>
        /// <returns>Un nouveau contexte.</returns>
        public static KsmedEntities GetNewContext(SecurityUser user) =>
            new KsmedEntitiesExt(ConnectionStringsSecurity.GetConnectionString(KsmedEntities.ConnectionString), user);

        public static KsmedEntities GetNewContext(ILocalizationManager localizationManager) =>
            new KsmedEntitiesExt(ConnectionStringsSecurity.GetConnectionString(KsmedEntities.ConnectionString), localizationManager);

        public static KsmedEntities GetNewContext(SecurityUser user, ILocalizationManager localizationManager) =>
            new KsmedEntitiesExt(ConnectionStringsSecurity.GetConnectionString(KsmedEntities.ConnectionString), user, localizationManager);

        /// <summary>
        /// Obtient le contexte courant.
        /// </summary>
        /// <returns>Le context courant.</returns>
        public static KsmedEntities GetCurrentContext()
        {
            if (_context.Value.Configuration.LazyLoadingEnabled)
                _context.Value.Configuration.LazyLoadingEnabled = false;
            return _context.Value;
        }


    }
}
