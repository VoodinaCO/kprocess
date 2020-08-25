using Kprocess.KL2.TabletClient.Extensions;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kprocess.KL2.TabletClient.Models
{
    public static class OfflineManager
    {
        const string OfflineFilesPath = "Offline";

        public static readonly Dictionary<OfflineFile, (object Lock, string File)> OfflineFiles = new Dictionary<OfflineFile, (object Lock, string File)>
        {
            [OfflineFile.TrainingsTree] = (new object(), "trainingsTree.json"),
            [OfflineFile.InspectionsTree] = (new object(), "inspectionsTree.json"),
            [OfflineFile.Training] = (new object(), "training.json"),
            [OfflineFile.Evaluation] = (new object(), "evaluation.json"),
            [OfflineFile.Inspection] = (new object(), "inspection.json"),
            [OfflineFile.Users] = (new object(), "users.json"),
            [OfflineFile.LastUser] = (new object(), "last_user.json"),
            [OfflineFile.QualificationReasons] = (new object(), "qualificationreasons.json"),
            [OfflineFile.Languages] = (new object(), "languages.json"),
            [OfflineFile.Token] = (new object(), "token.json")
        };

        public static string GetOfflinePath =>
            Path.Combine(FilesHelper.GetSyncFilesLocation(), OfflineFilesPath);

        public static void SaveToJson<T>(this OfflineFile offlineFile, T entity)
        {
            lock (OfflineFiles[offlineFile].Lock)
            {
                try
                {
                    Directory.CreateDirectory(GetOfflinePath);
                    using (var file = File.CreateText(Path.Combine(GetOfflinePath, OfflineFiles[offlineFile].File)))
                    {
                        var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
                        serializer.Serialize(file, entity);
                    }
                }
                catch (Exception e)
                {
                    Locator.TraceManager.TraceError(e, e.Message);
                    throw new JsonSerializationException($"Error when serializing {offlineFile}", e);
                }
            }
        }

        public static async Task<T> GetFromJson<T>(this OfflineFile offlineFile)
        {
            var result = default(T);
            if (File.Exists(Path.Combine(GetOfflinePath, OfflineFiles[offlineFile].File)))
            {
                lock (OfflineFiles[offlineFile].Lock)
                {
                    try
                    {
                        using (var file = File.OpenText(Path.Combine(GetOfflinePath, OfflineFiles[offlineFile].File)))
                        {
                            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
                            result = (T)serializer.Deserialize(file, typeof(T));
                        }
                    }
                    catch (Exception e)
                    {
                        Locator.TraceManager.TraceError(e, e.Message);
                        return result;
                    }
                }
                if (result is Publication publication)
                    await publication.UpdatePublication(false);
            }

            return result;
        }

        public static void DeleteJson(this OfflineFile offlineFile)
        {
            lock (OfflineFiles[offlineFile].Lock)
            {
                try
                {
                    string file = Path.Combine(GetOfflinePath, OfflineFiles[offlineFile].File);
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch { }
            }
        }
    }

    public enum OfflineFile
    {
        TrainingsTree,
        InspectionsTree,
        Training,
        Evaluation,
        Inspection,
        ScheduledInspection,
        Users,
        LastUser,
        QualificationReasons,
        Languages,
        Token
    }
}
