using KProcess.Business;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// Représente un service de gestion de l'état de l'IHM.
    /// </summary>
    public class UISettingsService : IBusinessService, IUISettingsService
    {
        private const string ColumnSettingKeyFormat = "{0}_#Col_{1}";
        private const string ColumnRecoveryRegex = @".*_#Col_(?<ColumnKey>.*)";

        private readonly ITraceManager _traceManager;
        private readonly ISecurityContext _securityContext;
        private readonly ILocalizationManager _localizationManager;

        public UISettingsService(
            ISecurityContext securityContext,
            ILocalizationManager localizationManager,
            ITraceManager traceManager)
        {
            _securityContext = securityContext;
            _localizationManager = localizationManager;
            _traceManager = traceManager;
        }

        public async Task SaveColumnsInfo(string uiPartCode, IDictionary<string, double> columnInfo)
        {
            foreach (var info in columnInfo)
            {
                try
                {
                    await SaveSetting(string.Format(ColumnSettingKeyFormat, uiPartCode, info.Key), info.Value);
                    _traceManager.TraceDebug("Columns Width Persisted for {0}", uiPartCode);
                }
                catch (Exception e)
                {
                    _traceManager.TraceError(e, "PersistDatagridColumnWidthBehavior: {0} Column persistance failed", uiPartCode);
                }
            }
        }

        public async Task<Dictionary<string, double>> GetColumnsInfo(string uiPartCode)
        {
            UISetting[] appSettings = await GetSettingStartingWith(string.Format(ColumnSettingKeyFormat, uiPartCode, string.Empty));
            return appSettings.ToDictionary(
                setting => Regex.Match(setting.Key, ColumnRecoveryRegex).Groups["ColumnKey"].Value,
                setting => BitConverter.ToDouble(setting.Value, 0));
            /*return appSettings.Select(setting => new KeyValuePair<string, double>(
                    Regex.Match(setting.Key, ColumnRecoveryRegex).Groups["ColumnKey"].Value,
                    BitConverter.ToDouble(setting.Value, 0)))
                .ToArray();*/
        }

        private async Task SaveSetting(string key, string value) =>
            await SaveSetting(key, Encoding.UTF8.GetBytes(value));

        private async Task SaveSetting(string key, double value) =>
            await SaveSetting(key, BitConverter.GetBytes(value));

        private async Task<string> GetSettingString(string key) =>
            Encoding.UTF8.GetString(await GetSetting(key));

        private async Task<double> GetSettingDouble(string key) =>
            BitConverter.ToDouble(await GetSetting(key), 0);

        private async Task SaveSetting(string key, byte[] value) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    var existingSetting = await context.UISettings.FirstOrDefaultAsync(setting => setting.Key == key);
                    if (existingSetting != null)
                    {
                        existingSetting.Value = value;
                        existingSetting.MarkAsModified();
                    }
                    else
                    {
                        existingSetting = new UISetting
                        {
                            Key = key,
                            Value = value
                        };
                    }

                    context.UISettings.ApplyChanges(existingSetting);
                    await context.SaveChangesAsync();
                }
            });

        private async Task<byte[]> GetSetting(string key) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    UISetting existingSetting = await context.UISettings.FirstOrDefaultAsync(setting => setting.Key == key);
                    return existingSetting?.Value;
                }
            });

        private async Task<UISetting[]> GetSettingStartingWith(string startingKey) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await context.UISettings.Where(setting => setting.Key.StartsWith(startingKey)).ToArrayAsync();
                }
            });
    }
}