using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Linq;
using System.Xml.Linq;

namespace KProcess.KL2.Setup
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
        public static ActionResult EditConfig(Session session)
        {
            session.Log($"Execute EditConfig custom action");
            session.Log($"API_LOCATION : {session["API_LOCATION"]}");
            session.Log($"FILESERVER_LOCATION : {session["FILESERVER_LOCATION"]}");
            session.Log($"SYNCPATH : {session["SYNCPATH"]}");
            session.Log($"SENDREPORT : {session["SENDREPORT"]}");
            session.Log($"MUTE : {session["MUTE"]}");
            try
            {
                string configPath = $"{session["INSTALLDIR"]}KL².exe.config";
                var doc = XDocument.Load(configPath);

                var elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri");
                elt.Attribute("value").SetValue(session["API_LOCATION"]);

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                elt.Attribute("value").SetValue(session["FILESERVER_LOCATION"]);

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SyncPath");
                elt.Attribute("value").SetValue(session["SYNCPATH"]);

                elt = doc.Root.Element("userSettings").Element("KProcess.Ksmed.Presentation.Shell.Settings").Elements("setting").Single(x => x.Attribute("name").Value == "SendReport");
                elt.Element("value").SetValue(session["SENDREPORT"] == "yes" ? "True" : "False");

                elt = doc.Root.Element("userSettings").Element("KProcess.Ksmed.Presentation.Shell.Settings").Elements("setting").Single(x => x.Attribute("name").Value == "Mute");
                elt.Element("value").SetValue(session["MUTE"] == "yes" ? "True" : "False");

                doc.Save(configPath);
            }
            catch (Exception e)
            {
                LogExceptionWithInner(session, e);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }
    }
}
