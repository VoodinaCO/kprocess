using KProcess.KL2.Languages;
using KProcess.Ksmed.Models.Security;
using KProcess.Supervision.Log4net;
using System;

namespace KProcess.Ksmed.Data
{
    /// <summary>
    /// Représente une fabrique de contextes Entity Framework
    /// </summary>
    public static class ContextFactory
    {

        static readonly Lazy<KsmedEntitiesExt> _context = new Lazy<KsmedEntitiesExt>(() 
            => new KsmedEntitiesExt(ConnectionStringsSecurity.GetConnectionString(GetTraceManager())));

        /// <summary>
        /// Obtient un nouveau contexte.
        /// </summary>
        /// <returns>Un nouveau contexte.</returns>
        public static KsmedEntities GetNewContext() =>
            new KsmedEntitiesExt(ConnectionStringsSecurity.GetConnectionString(GetTraceManager()));

        /// <summary>
        /// Obtient un nouveau contexte avec le user courant
        /// </summary>
        /// <returns>Un nouveau contexte.</returns>
        public static KsmedEntities GetNewContext(SecurityUser user) =>
            new KsmedEntitiesExt(ConnectionStringsSecurity.GetConnectionString(GetTraceManager()), user);

        public static KsmedEntities GetNewContext(ILocalizationManager localizationManager) =>
            new KsmedEntitiesExt(ConnectionStringsSecurity.GetConnectionString(GetTraceManager()), localizationManager);

        public static KsmedEntities GetNewContext(SecurityUser user, ILocalizationManager localizationManager) =>
            new KsmedEntitiesExt(ConnectionStringsSecurity.GetConnectionString(GetTraceManager()), user, localizationManager);

        /// <summary>
        /// Obtient le contexte courant.
        /// </summary>
        /// <returns>Le context courant.</returns>
        public static KsmedEntities GetCurrentContext()
        {
            if (_context.Value.ContextOptions.LazyLoadingEnabled)
                _context.Value.ContextOptions.LazyLoadingEnabled = false;
            return _context.Value;
        }

        public static ITraceManager GetTraceManager()
        {
            log4net.Config.XmlConfigurator.Configure();
            ITraceManager traceManager = new Log4netTraceManager(new Log4netWrapper());
            return traceManager;
        }
    }
}
