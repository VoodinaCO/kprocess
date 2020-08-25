using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml.Linq;
using WixSharp;

namespace KProcess.KL2.Server.Setup
{
    public class CustomActions
    {
        static void LogExceptionWithInner(Session session, Exception e)
        {
            session.Log($"ERROR : {e.Message}");
            if (e.InnerException != null)
                LogExceptionWithInner(session, e.InnerException);
        }

        [CustomAction]
        public static ActionResult API_EditConfig(Session session)
        {
            session.Log($"Execute API_EditConfig custom action");
            session.Log($"DATASOURCE : {session["DATASOURCE"]}");
            session.Log($"APPLICATION_URL : {session["APPLICATION_URL"]}");
            session.Log($"FILESERVER_LOCATION : {session["FILESERVER_LOCATION"]}");
            try
            {
                string configPath = $"{session["INSTALLDIR"]}KProcess.KL2.API.exe.config";
                var doc = XDocument.Load(configPath);

                var elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                elt.Attribute("connectionString").SetValue(System.Net.WebUtility.HtmlDecode($@"metadata=          res://KProcess.Ksmed.Data/KL2.csdl|          res://KProcess.Ksmed.Data/KL2.ssdl|          res://KProcess.Ksmed.Data/KL2.msl;          provider=System.Data.SqlClient;          provider connection string=&quot;Data Source={session["DATASOURCE"]};Initial Catalog=KProcess.KL2;User ID=KL2User;Password=B4A60FC44BCBEDC6267B80118219D235;MultipleActiveResultSets=True&quot;"));

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApplicationUrl");
                elt.Attribute("value").SetValue(session["APPLICATION_URL"]);

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                elt.Attribute("value").SetValue(session["FILESERVER_LOCATION"]);

                doc.Save(configPath);
            }
            catch (Exception e)
            {
                LogExceptionWithInner(session, e);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult API_UninstallService(Session session)
        {
            session.Log($"Execute UninstallService custom action");
            return session.HandleErrors(() =>
            {
                StartHideProcess("sc", $"stop \"KL2 API\"");
                StartHideProcess("sc", $"delete \"KL2 API\"");
            });
        }

        [CustomAction]
        public static ActionResult FileServer_EditConfig(Session session)
        {
            session.Log($"Execute FileServer_EditConfig custom action");
            session.Log($"DATASOURCE : {session["DATASOURCE"]}");
            session.Log($"APPLICATION_URL : {session["APPLICATION_URL"]}");
            session.Log($"FILE_PROVIDER : {session["FILE_PROVIDER"]}");
            if (session["FILE_PROVIDER"] == "SFtp")
            {
                session.Log($"SERVER : {session["SERVER"]}");
                session.Log($"PORT : {session["PORT"]}");
                session.Log($"USER : {session["USER"]}");
                session.Log($"PASSWORD : {session["PASSWORD"]}");
                session.Log($"PUBLISHED_DIR : {session["PUBLISHED_DIR"]}");
                session.Log($"UPLOADED_DIR : {session["UPLOADED_DIR"]}");
            }
            else if (session["FILE_PROVIDER"] == "Local")
            {
                session.Log($"PUBLISHED_DIR : {session["PUBLISHED_DIR"]}");
                session.Log($"UPLOADED_DIR : {session["UPLOADED_DIR"]}");
            }
            try
            {
                string configPath = $"{session["INSTALLDIR"]}Kprocess.KL2.FileServer.exe.config";
                var doc = XDocument.Load(configPath);

                var elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                elt.Attribute("connectionString").SetValue(System.Net.WebUtility.HtmlDecode($@"metadata=          res://KProcess.Ksmed.Data/KL2.csdl|          res://KProcess.Ksmed.Data/KL2.ssdl|          res://KProcess.Ksmed.Data/KL2.msl;          provider=System.Data.SqlClient;          provider connection string=&quot;Data Source={session["DATASOURCE"]};Initial Catalog=KProcess.KL2;User ID=KL2User;Password=B4A60FC44BCBEDC6267B80118219D235;MultipleActiveResultSets=True&quot;"));

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApplicationUrl");
                elt.Attribute("value").SetValue(session["APPLICATION_URL"]);

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileProvider");
                elt.Attribute("value").SetValue(session["FILE_PROVIDER"]);

                if (session["FILE_PROVIDER"] == "SFtp")
                {
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_Server");
                    elt.Attribute("value").SetValue(session["SERVER"]);

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_Port");
                    elt.Attribute("value").SetValue(session["PORT"]);

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_User");
                    elt.Attribute("value").SetValue(session["USER"]);

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_Password");
                    elt.Attribute("value").SetValue(session["PASSWORD"]);

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_PublishedFilesDirectory");
                    elt.Attribute("value").SetValue(session["PUBLISHED_DIR"]);

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_UploadedFilesDirectory");
                    elt.Attribute("value").SetValue(session["UPLOADED_DIR"]);
                }
                else if (session["FILE_PROVIDER"] == "Local")
                {
                    SecurityIdentifier networkService = new SecurityIdentifier("S-1-5-20");
                    IdentityReference networkServiceIdentity = networkService.Translate(typeof(NTAccount));
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "Local_PublishedFilesDirectory");
                    elt.Attribute("value").SetValue(session["PUBLISHED_DIR"]);
                    AddDirectorySecurity(session["PUBLISHED_DIR"], networkServiceIdentity, FileSystemRights.FullControl, AccessControlType.Allow);

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "Local_UploadedFilesDirectory");
                    elt.Attribute("value").SetValue(session["UPLOADED_DIR"]);
                    AddDirectorySecurity(session["UPLOADED_DIR"], networkServiceIdentity, FileSystemRights.FullControl, AccessControlType.Allow);
                }

                doc.Save(configPath);
            }
            catch (Exception e)
            {
                LogExceptionWithInner(session, e);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        public static void AddDirectorySecurity(string FileName, IdentityReference Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(FileName);

            // Get a DirectorySecurity object that represents the 
            // current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account,
                                                            Rights,
                                                            ControlType));

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);

        }

        [CustomAction]
        public static ActionResult FileServer_UninstallService(Session session)
        {
            session.Log($"Execute UninstallService custom action");
            return session.HandleErrors(() =>
            {
                StartHideProcess("sc", "stop \"KL2 File Server\"");
                StartHideProcess("sc", "delete \"KL2 File Server\"");
            });
        }

        [CustomAction]
        public static ActionResult Notification_EditConfig(Session session)
        {
            session.Log($"Execute Notification_EditConfig custom action");
            session.Log($"DATASOURCE : {session["DATASOURCE"]}");
            session.Log($"FILESERVER_LOCATION : {session["FILESERVER_LOCATION"]}");
            session.Log($"INTERVAL : {session["INTERVAL"]}");
            try
            {
                string configPath = $"{session["INSTALLDIR"]}KProcess.KL2.Notification.exe.config";
                var doc = XDocument.Load(configPath);

                var elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                elt.Attribute("connectionString").SetValue(System.Net.WebUtility.HtmlDecode($@"metadata=          res://KProcess.Ksmed.Data/KL2.csdl|          res://KProcess.Ksmed.Data/KL2.ssdl|          res://KProcess.Ksmed.Data/KL2.msl;          provider=System.Data.SqlClient;          provider connection string=&quot;Data Source={session["DATASOURCE"]};Initial Catalog=KProcess.KL2;User ID=KL2User;Password=B4A60FC44BCBEDC6267B80118219D235;MultipleActiveResultSets=True&quot;"));

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                elt.Attribute("value").SetValue(session["FILESERVER_LOCATION"]);

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SendNotificationInterval");
                elt.Attribute("value").SetValue(session["INTERVAL"]);

                doc.Save(configPath);
            }
            catch (Exception e)
            {
                LogExceptionWithInner(session, e);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult Notification_UninstallService(Session session)
        {
            session.Log($"Execute UninstallService custom action");
            return session.HandleErrors(() =>
            {
                StartHideProcess("sc", "stop \"KL2 Notification\"");
                StartHideProcess("sc", "delete \"KL2 Notification\"");
            });
        }

        [CustomAction]
        public static ActionResult WebAdmin_EditConfig(Session session)
        {
            session.Log($"Execute WebAdmin_EditConfig custom action");
            session.Log($"DATASOURCE : {session.Property("D_DATASOURCE")}");
            session.Log($"API_LOCATION : {session.Property("D_API_LOCATION")}");
            session.Log($"FILESERVER_LOCATION : {session.Property("D_FILESERVER_LOCATION")}");
            try
            {
                string configPath = $"{session.Property("D_INSTALLDIR")}Web.config";
                var doc = XDocument.Load(configPath);

                var elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                elt.Attribute("connectionString").SetValue(System.Net.WebUtility.HtmlDecode($@"metadata=          res://KProcess.Ksmed.Data/KL2.csdl|          res://KProcess.Ksmed.Data/KL2.ssdl|          res://KProcess.Ksmed.Data/KL2.msl;          provider=System.Data.SqlClient;          provider connection string=&quot;Data Source={session.Property("D_DATASOURCE")};Initial Catalog=KProcess.KL2;User ID=KL2User;Password=B4A60FC44BCBEDC6267B80118219D235;MultipleActiveResultSets=True&quot;"));

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri");
                elt.Attribute("value").SetValue(session.Property("D_API_LOCATION"));

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                elt.Attribute("value").SetValue(session.Property("D_FILESERVER_LOCATION"));

                doc.Save(configPath);
            }
            catch (Exception e)
            {
                LogExceptionWithInner(session, e);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        public static void StartHideProcess(string proc, string args)
        {
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p = Process.Start(proc, args, null, null, null);
            p.WaitForExit();
        }
    }
}
