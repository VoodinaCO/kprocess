using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;

namespace KProcess.KL2.Tablet.Setup
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
        public static ActionResult RestartIfNotAdmin(Session session)
        {
            session.Log($"Execute RestartIfNotAdmin custom action");
            try
            {
                if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                {
                    MessageBox.Show(session.GetMainWindow(), "You must start the msi file as admin");
                    return ActionResult.Failure;
                }
            }
            catch (Exception e)
            {
                LogExceptionWithInner(session, e);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult EditConfig(Session session)
        {
            session.Log($"Execute EditConfig custom action");
            session.Log($"API_LOCATION : {session["API_LOCATION"]}");
            session.Log($"FILESERVER_LOCATION : {session["FILESERVER_LOCATION"]}");
            session.Log($"SYNCPATH : {session["SYNCPATH"]}");
            session.Log($"SYNC_INTERVAL : {session["SYNC_INTERVAL"]}");
            try
            {
                // Tablet
                string configPath = $"{session["INSTALLDIR"]}Kprocess.KL2.TabletClient.exe.config";
                var doc = XDocument.Load(configPath);

                var elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri");
                elt.Attribute("value").SetValue(session["API_LOCATION"]);

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                elt.Attribute("value").SetValue(session["FILESERVER_LOCATION"]);

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SyncPath");
                elt.Attribute("value").SetValue(session["SYNCPATH"]);

                doc.Save(configPath);

                // SyncService
                configPath = $"{session["INSTALLDIR"]}Kprocess.KL2.SyncService.exe.config";
                doc = XDocument.Load(configPath);

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri");
                elt.Attribute("value").SetValue(session["API_LOCATION"]);

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SyncPath");
                elt.Attribute("value").SetValue(session["SYNCPATH"]);

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SyncInterval");
                elt.Attribute("value").SetValue(session["SYNC_INTERVAL"]);

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
        public static ActionResult UninstallService(Session session)
        {
            session.Log($"Execute UninstallService custom action");
            return session.HandleErrors(() =>
            {
                Process.Start("sc", "stop \"KL2 FS sync service\"").WaitForExit();
                Process.Start("sc", "delete \"KL2 FS sync service\"").WaitForExit();
            });
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
