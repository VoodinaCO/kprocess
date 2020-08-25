using Kprocess.PackIconKprocess;
using KProcess.KL2.APIClient;
using KProcess.KL2.JWT;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models;
using KProcess.KL2.WebAdmin.Models.Users;
using KProcess.KL2.WebAdmin.Utils;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Activation;
using log4net;
using log4net.Appender;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Operator, KnownRoles.Technician, KnownRoles.Documentalist)]
    [SettingUserContextFilter]
    public class UtilsController : LocalizedController
    {
        readonly ITraceManager _traceManager;
        ILocalizedStrings _localizedStrings;
        IApplicationUsersService _applicationUsersService;
        readonly IAPIHttpClient _apiHttpClient;

        public UtilsController(IApplicationUsersService applicationUsersService, ITraceManager traceManager, ILocalizedStrings localizedStrings, IAPIHttpClient apiHttpClient,
            ILocalizationManager localizationManager)
            : base(localizationManager)
        {
            _applicationUsersService = applicationUsersService;
            _traceManager = traceManager;
            _localizedStrings = localizedStrings;
            _apiHttpClient = apiHttpClient;
        }

        private (string fileName, byte[] data) GetLogInternal()
        {
            var logFileLocation = LogManager.GetRepository().GetAppenders().OfType<FileAppender>().FirstOrDefault()?.File;

            var tmpFileName = Path.GetTempFileName();
            System.IO.File.Copy(logFileLocation, tmpFileName, true);
            var fileBytes = System.IO.File.ReadAllBytes(tmpFileName);
            System.IO.File.Delete(tmpFileName);

            return (Path.GetFileName(logFileLocation), fileBytes);
        }

        [CustomAuthorize(KnownRoles.Administrator)]
        public ActionResult GetLog()
        {
            byte[] compressedBytes;

            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    var (fileName, fileBytes) = GetLogInternal();
                    var fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                    using (var entryStream = fileInArchive.Open())
                    {
                        using (var fileToCompressStream = new MemoryStream(fileBytes))
                        {
                            fileToCompressStream.CopyTo(entryStream);
                        }
                    }
                }
                compressedBytes = outStream.ToArray();
            }
            return File(compressedBytes, "application/zip", "WebAdmin_Log.zip");
        }

        [CustomAuthorize(KnownRoles.Administrator)]
        public ActionResult GetAllLogs()
        {
            var version = Assembly.GetExecutingAssembly().FullName
                .Split(',')
                .Single(_ => _.Contains("Version="))
                .Split('=')
                .Last();
            var logFiles = new List<(string fileName, byte[] data)>
            {
                GetLogInternal() // WebAdmin
            };
            var apiLog = _apiHttpClient.GetLog(KL2_Server.API);
            if (!string.IsNullOrEmpty(apiLog.fileName) && apiLog.data != null)
                logFiles.Add(apiLog);
            var fileServerLog = _apiHttpClient.GetLog(KL2_Server.File);
            if (!string.IsNullOrEmpty(fileServerLog.fileName) && fileServerLog.data != null)
                logFiles.Add(fileServerLog);
            //TODO : Get Logs from Notification

            byte[] compressedBytes;

            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    foreach (var (fileName, data) in logFiles)
                    {
                        var fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                        using (var entryStream = fileInArchive.Open())
                        using (var fileToCompressStream = new MemoryStream(data))
                        {
                            fileToCompressStream.CopyTo(entryStream);
                        }
                    }
                }
                compressedBytes = outStream.ToArray();
            }
            return File(compressedBytes, "application/zip", $"WS-log-v{version}.zip");
        }
    }
}