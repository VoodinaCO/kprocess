using KProcess.Business;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using System;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// Service de gestion de verrous en base de données partagée.
    /// </summary>
    public class SharedDatabaseService : IBusinessService, ISharedDatabaseService
    {
        private const string AppSettingKey_SingleUserLockUser = "SingleUserLockUser";
        private const string AppSettingKey_SingleUserLockDate = "SingleUserLockDate";
        private const string AppSettingKey_SingleUserLockComputer = "SingleUserLockComputer";

        /// <summary>
        /// Détermine si la base de données est vérouillée pour l'utilisateur spécifié.
        /// </summary>
        /// <param name="username">le nom d'utilisateur</param>
        public async Task<bool> IsLocked(string username) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    (AppSetting User, AppSetting Date, AppSetting Computer) lockStates = await ReleaseExpiredLocks(context);

                    if (lockStates.User != null && lockStates.Date != null && lockStates.Computer != null)
                    {
                        var dbusername = Encoding.UTF8.GetString(lockStates.User.Value);
                        var computer = Encoding.UTF8.GetString(lockStates.Computer.Value);

                        return !(string.Equals(dbusername, username) &&
                            string.Equals(computer, System.Net.Dns.GetHostName(), StringComparison.CurrentCultureIgnoreCase));
                    }
                    return false;
                }
            });

        /// <summary>
        /// Libère les verrous expirés.
        /// </summary>
        /// <param name="context">le contexte EF</param>
        /// <return name="lockUser">L'instance correspondant au verrou en base de données contenant le nom d'utilisateur, ou <c>null</c></return>
        /// <return name="lockDate">L'instance correspondant au verrou en base de données contenant la date, ou <c>null</c></return>
        /// <return name="lockComputer">L'instance correspondant au verrou en base de données contenant le nom du poste, ou <c>null</c></return>
        private async Task<(AppSetting User, AppSetting Date, AppSetting Computer)> ReleaseExpiredLocks(KsmedEntities context)
        {
            AppSetting lockUser = await context.AppSettings.FirstOrDefaultAsync(a => a.Key == AppSettingKey_SingleUserLockUser);
            AppSetting lockDate = await context.AppSettings.FirstOrDefaultAsync(a => a.Key == AppSettingKey_SingleUserLockDate);
            AppSetting lockComputer = await context.AppSettings.FirstOrDefaultAsync(a => a.Key == AppSettingKey_SingleUserLockComputer);

            if (lockUser != null && lockDate != null && lockComputer != null)
            {
                var date = new DateTime(BitConverter.ToInt64(lockDate.Value, 0), DateTimeKind.Utc);

                if ((DateTime.UtcNow - date) > IoC.Resolve<ISharedDatabaseSettingsService>().LockTimeout)
                {
                    context.AppSettings.DeleteObject(lockUser);
                    context.AppSettings.DeleteObject(lockDate);
                    context.AppSettings.DeleteObject(lockComputer);
                    await context.SaveChangesAsync();
                }
            }
            return (lockUser, lockDate, lockComputer);
        }

        /// <summary>
        /// Met à jour le verrou pour l'utilisateur spécifié.
        /// </summary>
        /// <param name="username">le nom d'utilisateur</param>
        public async Task UpdateLock(string username) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    AppSetting settingUser = await context.AppSettings.FirstOrDefaultAsync(a => a.Key == AppSettingKey_SingleUserLockUser);
                    AppSetting settingDate = await context.AppSettings.FirstOrDefaultAsync(a => a.Key == AppSettingKey_SingleUserLockDate);
                    AppSetting settingComputer = await context.AppSettings.FirstOrDefaultAsync(a => a.Key == AppSettingKey_SingleUserLockComputer);

                    if (settingUser == null)
                    {
                        settingUser = new AppSetting { Key = AppSettingKey_SingleUserLockUser };
                        context.AppSettings.ApplyChanges(settingUser);
                    }
                    if (settingDate == null)
                    {
                        settingDate = new AppSetting { Key = AppSettingKey_SingleUserLockDate };
                        context.AppSettings.ApplyChanges(settingDate);
                    }
                    if (settingComputer == null)
                    {
                        settingComputer = new AppSetting { Key = AppSettingKey_SingleUserLockComputer };
                        context.AppSettings.ApplyChanges(settingComputer);
                    }

                    settingUser.Value = Encoding.UTF8.GetBytes(username);
                    settingDate.Value = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
                    settingComputer.Value = Encoding.UTF8.GetBytes(System.Net.Dns.GetHostName());

                    await context.SaveChangesAsync();
                }
            });

        /// <summary>
        /// Libère le verrou pour l'utilisateur spécifié.
        /// </summary>
        /// <param name="username">le nom d'utilisateur</param>
        public async Task ReleaseLock(string username)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                AppSetting settingUser = await context.AppSettings.FirstOrDefaultAsync(a => a.Key == AppSettingKey_SingleUserLockUser);
                AppSetting settingDate = await context.AppSettings.FirstOrDefaultAsync(a => a.Key == AppSettingKey_SingleUserLockDate);
                AppSetting settingComputer = await context.AppSettings.FirstOrDefaultAsync(a => a.Key == AppSettingKey_SingleUserLockComputer);

                if (settingUser != null && settingComputer != null && settingDate != null)
                {
                    var dbusername = Encoding.UTF8.GetString(settingUser.Value);
                    var computer = Encoding.UTF8.GetString(settingComputer.Value);

                    if (string.Equals(dbusername, username) &&
                        string.Equals(computer, System.Net.Dns.GetHostName(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        context.AppSettings.DeleteObject(settingUser);
                        context.AppSettings.DeleteObject(settingDate);
                        context.AppSettings.DeleteObject(settingComputer);
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}