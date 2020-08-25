using KProcess.Ksmed.Business;
using KProcess.Ksmed.Security;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Shell.Services
{
    /// <summary>
    /// Gestion de verrou en base de données partagée dans la couche présentation.
    /// </summary>
    class SharedDatabasePresentationService : ISharedDatabasePresentationService
    {

        private TimeSpan? _lockTimeout;
        /// <summary>
        /// Obtient la durée à partir de laquelle un verrou n'est plus valide.
        /// </summary>
        public TimeSpan LockTimeout
        {
            get
            {
                if (!_lockTimeout.HasValue)
                {
                    var seconds = ConfigurationManager.AppSettings["SharedDatabaseLockTimeout"];
                    _lockTimeout = TimeSpan.FromSeconds(Convert.ToDouble(seconds));
                }
                return _lockTimeout.Value;
            }
        }

        private TimeSpan? _lockUpdateFrequency;
        /// <summary>
        /// Obtient la fréquence de mise à jour du verrou.
        /// </summary>
        public TimeSpan LockUpdateFrequency
        {
            get
            {
                if (!_lockUpdateFrequency.HasValue)
                {
                    var seconds = ConfigurationManager.AppSettings["SharedDatabaseLockUpdateFrequency"];
                    _lockUpdateFrequency = TimeSpan.FromSeconds(Convert.ToDouble(seconds));
                }
                return _lockUpdateFrequency.Value;
            }
        }

        private bool? _isSharedDatabaseEnabled;
        /// <summary>
        /// Obtient une valeur indiquant si le système de base de données partagée est activé.
        /// </summary>
        private bool IsEnabled
        {
            get
            {
                if (!_isSharedDatabaseEnabled.HasValue)
                {
                    var settingsValue = ConfigurationManager.AppSettings["SharedDatabaseIsEnabled"];
                    bool value;

                    if (bool.TryParse(settingsValue, out value))
                        _isSharedDatabaseEnabled = value;
                    else
                        _isSharedDatabaseEnabled = false;
                }
                return _isSharedDatabaseEnabled.Value;
            }
        }

        Timer _timer;

        /// <summary>
        /// Initialise la gestion du verrou pour l'utilisateur actuellement connecté.
        /// </summary>
        public void Initialize()
        {
            if (IsEnabled)
            {
                _timer = new Timer(OnTick, null, 0, (int)LockUpdateFrequency.TotalMilliseconds);
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si la base de données est vérouillée pour l'utilisateur spécifié.
        /// </summary>
        /// <param name="username">le nom d'utilisateur.</param>
        public async Task<bool> IsLocked(string username)
        {
            if (IsEnabled)
            {
                try
                {
                    return await IoC.Resolve<IServiceBus>().Get<ISharedDatabaseService>().IsLocked(username);
                }
                catch (Exception e)
                {
                    this.TraceError(e, "Impossible de déterminer l'état IsLocked");
                    throw e;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// Met à jour le verrou pour l'utilisateur actuellement connecté.
        /// </summary>
        public async Task UpdateLock()
        {
            await UpdateLockInternal();
        }

        /// <summary>
        /// Libère le verrou pour l'utilisateur actuellement connecté.
        /// </summary>
        public async Task ReleaseLock()
        {
            if (IsEnabled && SecurityContext.CurrentUser != null)
            {
                try
                {
                    await IoC.Resolve<IServiceBus>().Get<ISharedDatabaseService>().ReleaseLock(SecurityContext.CurrentUser.Username);
                }
                catch (Exception e)
                {
                    this.TraceError(e, "Impossible de libérer le lock");
                }
            }
        }

        /// <summary>
        /// Appelé lorsque le Timer Ticks.
        /// </summary>
        /// <param name="state">l'état</param>
        private async void OnTick(object state)
        {
            await UpdateLockInternal();
        }

        /// <summary>
        /// Met à jour le verrou.
        /// </summary>
        private async Task UpdateLockInternal()
        {
            if (IsEnabled && SecurityContext.CurrentUser != null)
            {
                var username = SecurityContext.CurrentUser.Username;

                try
                {
                    await IoC.Resolve<IServiceBus>().Get<ISharedDatabaseService>().UpdateLock(username);
                }
                catch { } // Ne rien faire en cas d'erreur
            }
        }
    }
}