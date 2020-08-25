using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;

namespace KProcess.KL2.Tablet.Setup
{
    class Program
    {
        public static string VersionToBuild = Tasks.GetVersionFromFile(@"bin\Release\Kprocess.KL2.TabletClient.exe").ToString();
        const string appName = "KL²® Field Services";
        const string manufacturer = "K-process";
        const string setupName = "KL²FieldServices_Setup.exe";
        const string regKey = "SOFTWARE\\K-process\\KL²FieldServices\\Install";

        static Guid UpgradeCode = Guid.Parse("7db5e246-4447-45d3-83fc-ef44d695ddb0");
        static Guid BootstrapperUpgradeCode = Guid.Parse("ef6e00cf-0172-4560-9d36-38c1b2a0a411");

        static void Main(string[] args)
        {
            var projectName = $"{appName} v{ToCompactVersionString(Version.Parse(VersionToBuild))}";
            //WriteLicenseRtf();

            /////////////// KL² Tablet MSI ///////////////
            ExeFileShortcut desktopShortcut = new ExeFileShortcut(appName, @"[INSTALLDIR]Kprocess.KL2.TabletClient.exe", "") { Condition = "INSTALLDESKTOPSHORTCUT=\"yes\"" };
            ExeFileShortcut startMenuLaunchShortcut = new ExeFileShortcut(appName, @"[INSTALLDIR]Kprocess.KL2.TabletClient.exe", "") { Condition = "INSTALLSTARTMENUSHORTCUT=\"yes\"" };
            ExeFileShortcut startMenuUninstallShortcut = new ExeFileShortcut($"Uninstall {appName}", $@"[CommonAppDataFolder]\Package Cache\[BUNDLE_ID]\{setupName}", "/uninstall") { WorkingDirectory = "CommonAppDataFolder", Condition = "INSTALLSTARTMENUSHORTCUT =\"yes\" AND BUNDLE_ID <> \"[BUNDLE_ID]\"" };
            Dir scanAppDir = ScanFiles("KL2FieldServices",
                                    System.IO.Path.GetFullPath(@"..\KL2-Core\Kprocess.KL2.TabletClient\bin\Release"),
                                    (file) => file.EndsWith(".pdb") || file.EndsWith(".xml") || file.EndsWith(".vshost.exe") || file.EndsWith(".vshost.exe.config") || file.EndsWith(".vshost.exe.manifest"));
            Dir desktopDir = new Dir("%Desktop%", desktopShortcut);
            Dir startMenuDir = new Dir($@"%ProgramMenu%\{manufacturer}", startMenuLaunchShortcut, startMenuUninstallShortcut);

            RegValue installReg = new RegValue(RegistryHive.LocalMachineOrUsers, regKey, "InstallLocation", "[INSTALLDIR]");
            RegValue desktopShortcutReg = new RegValue(RegistryHive.LocalMachineOrUsers, regKey, "DesktopShortcut", "[INSTALLDESKTOPSHORTCUT]");
            RegValue startMenuShortcutReg = new RegValue(RegistryHive.LocalMachineOrUsers, regKey, "StartMenuShortcut", "[INSTALLSTARTMENUSHORTCUT]");

            //ManagedAction restartIfNotAdmin = new ManagedAction(CustomActions.RestartIfNotAdmin, Return.check, When.Before, Step.AppSearch, WixSharp.Condition.NOT_Installed, Sequence.InstallUISequence);
            ManagedAction editConfig = new ManagedAction(CustomActions.EditConfig, Return.check, When.After, Step.InstallFinalize, WixSharp.Condition.NOT_Installed);
            ElevatedManagedAction uninstallService = new ElevatedManagedAction(CustomActions.UninstallService, Return.check, When.Before, Step.RemoveFiles, WixSharp.Condition.BeingUninstalled);

            var project = new ManagedProject(projectName, scanAppDir, desktopDir, startMenuDir, installReg, desktopShortcutReg, startMenuShortcutReg, /*restartIfNotAdmin, */editConfig, uninstallService)
            {
                GUID = Versions.List.Single(_ => _.Key.ToString() == VersionToBuild).Value,
                Version = Version.Parse(VersionToBuild),
                UpgradeCode = UpgradeCode,
                AttributesDefinition = $"Id={Versions.List.Single(_ => _.Key.ToString() == VersionToBuild).Value};Manufacturer={manufacturer}",
                Description = $"{projectName},{manufacturer}",
                InstallScope = InstallScope.perMachine,
                Properties = new[]
                {
                    new Property("LANGUAGE", "en-US"),
                    new Property("INSTALLDESKTOPSHORTCUT", "yes"),
                    new Property("INSTALLSTARTMENUSHORTCUT", "yes"),
                    new Property("API_LOCATION", "http://localhost:8081"),
                    new Property("FILESERVER_LOCATION", "http://localhost:8082"),
                    new Property("SYNCPATH", "[INSTALLDIR]\\SyncFiles"),
                    new Property("SYNC_INTERVAL", "3"),
                    new Property("BUNDLE_ID", "[BUNDLE_ID]")
                }
            };
            project.Package.AttributesDefinition = $"Id=*;InstallerVersion=500;Comments={projectName};Keywords={projectName},{manufacturer}";
            project.SetNetFxPrerequisite(new WixSharp.Condition(" (NETFRAMEWORK45 >= '#461308') "), "Please install .Net Framework 4.7.1 first.");
            project.ControlPanelInfo.ProductIcon = @"..\..\Assets\kl2_FieldService.ico";
            project.MajorUpgrade = new MajorUpgrade
            {
                AllowSameVersionUpgrades = true,
                DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
            };
            project.WixVariables.Add("WixUILicenseRtf", "License.rtf");
            project.BeforeInstall += Project_BeforeInstall;
            project.AfterInstall += Project_AfterInstall;

            // Save the list of files to check integrity
            void LogFiles(System.IO.StreamWriter writer, int rootCount, Dir root)
            {
                foreach (var file in root.Files)
                {
                    var splittedFileName = file.Name.Split('\\').ToList();
                    for (var i = 0; i < rootCount; i++)
                        splittedFileName.RemoveAt(0);
                    writer.WriteLine(string.Join("\\", splittedFileName.ToArray()));
                }
                foreach (var dir in root.Dirs)
                    LogFiles(writer, rootCount, dir);
            }
            using (var writer = System.IO.File.CreateText("FieldServices_FileList.txt"))
            {
                var rootCount = scanAppDir.Files.First().Name.Split('\\').Length - 1;
                LogFiles(writer, rootCount, scanAppDir);
            }

            project.BuildWxs(Compiler.OutputType.MSI, "KL2Tablet_project.wxs");
            string productMsi = project.BuildMsi("KL²FieldServices.msi");

            /////////////// BOOTSTRAPPER ///////////////
            var bootstrapper = new Bundle(projectName,
                new PackageGroupRef("NetFx471Redist"),
                new MsiPackage(productMsi)
                {
                    Id = "KL2FIELDSERVICES_MSI",
                    MsiProperties = $"BUNDLE_ID=[WixBundleProviderKey]; INSTALLDIR=[INSTALLDIR]; LANGUAGE=[LANGUAGE]; INSTALLDESKTOPSHORTCUT=[INSTALLDESKTOPSHORTCUT]; INSTALLSTARTMENUSHORTCUT=[INSTALLSTARTMENUSHORTCUT]; API_LOCATION=[API_LOCATION]; FILESERVER_LOCATION=[FILESERVER_LOCATION]; SYNCPATH=[SYNCPATH];"
                })
            {
                Version = project.Version,
                Manufacturer = manufacturer,
                UpgradeCode = BootstrapperUpgradeCode,
                AboutUrl = @"http://www.k-process.com/",
                IconFile = @"..\..\Assets\kl2_FieldService.ico",
                SuppressWixMbaPrereqVars = true,
                DisableModify = "yes",
                Application = new ManagedBootstrapperApplication(@"..\KProcess.KL2.Tablet.SetupUI\bin\Release\KProcess.KL2.Tablet.SetupUI.dll")
                {
                    Payloads = new[]
                    {
                        @"..\KProcess.KL2.Tablet.SetupUI\BootstrapperCore.config".ToPayload(),
                        @"..\KProcess.KL2.Tablet.SetupUI\bin\Release\BootstrapperCore.dll".ToPayload(),
                        @"..\KProcess.KL2.Tablet.SetupUI\bin\Release\Microsoft.Deployment.WindowsInstaller.dll".ToPayload(),
                        @"..\KProcess.KL2.Tablet.SetupUI\bin\Release\System.Windows.Interactivity.dll".ToPayload(),
                        @"..\KProcess.KL2.Tablet.SetupUI\bin\Release\ControlzEx.dll".ToPayload(),
                        @"..\KProcess.KL2.Tablet.SetupUI\bin\Release\MahApps.Metro.dll".ToPayload(),
                        @"..\KProcess.KL2.Tablet.SetupUI\bin\Release\MahApps.Metro.IconPacks.dll".ToPayload(),
                        @"..\KProcess.KL2.Tablet.SetupUI\bin\Release\WPF.Dialogs.dll".ToPayload(),
                        @"..\KProcess.KL2.Tablet.SetupUI\bin\Release\FreshDeskLib.dll".ToPayload(),
                        @"..\KProcess.KL2.Tablet.SetupUI\bin\Release\Newtonsoft.Json.dll".ToPayload()
                    },
                    PrimaryPackageId = "KL2FIELDSERVICES_MSI"
                }
            };
            bootstrapper.Variables = new[] {
                new Variable("INSTALLDIR", "dummy"),
                new Variable("INSTALLDESKTOPSHORTCUT", "yes"),
                new Variable("INSTALLSTARTMENUSHORTCUT", "yes"),
                new Variable("API_LOCATION", "http://localhost:8081"),
                new Variable("FILESERVER_LOCATION", "http://localhost:8082"),
                new Variable("SYNCPATH", @".\SyncFiles"),
                new Variable("SYNC_INTERVAL", "3"),
                new Variable("LANGUAGE", "en-US")
            };
            bootstrapper.PreserveTempFiles = true;
            bootstrapper.WixSourceGenerated += document =>
            {
                document.Root.Select("Bundle").Add(XDocument.Load("NET_Framework_Payload.wxs").Root.Elements());
                document.Root.Add(XDocument.Load("NET_Framework_Fragments.wxs").Root.Elements());
            };
            bootstrapper.Include(WixExtension.Util);
            bootstrapper.Build(setupName);
        }

        private static void Project_BeforeInstall(SetupEventArgs e)
        {
            string backupConfigPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Kprocess.KL2.TabletClient.exe.config");
            string serviceBackupConfigPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Kprocess.KL2.SyncService.exe.config");

            Version installedVersion = AppSearch.GetProductVersionFromUpgradeCode(e.UpgradeCode);
            if (installedVersion != null && e.IsModifying && !e.IsUninstalling)
            {
                try
                {
                    // Tablet
                    string configPath = $"{e.Session["INSTALLDIR"]}Kprocess.KL2.TabletClient.exe.config";
                    e.Session.Log($"Load {configPath}");
                    var doc = XDocument.Load(configPath);

                    e.Session.Log($"Get API_LOCATION");
                    var elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri");
                    e.Session.Log($"API_LOCATION : {elt.Attribute("value").Value}");

                    e.Session.Log($"Get FILESERVER_LOCATION");
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                    e.Session.Log($"FILESERVER_LOCATION : {elt.Attribute("value").Value}");

                    e.Session.Log($"Get SYNCPATH");
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SyncPath");
                    e.Session.Log($"SYNCPATH : {elt.Attribute("value").Value}");

                    System.IO.File.Copy(configPath, backupConfigPath);

                    // SyncService
                    configPath = $"{e.Session["INSTALLDIR"]}Kprocess.KL2.SyncService.exe.config";
                    e.Session.Log($"Load {configPath}");
                    doc = XDocument.Load(configPath);

                    e.Session.Log($"Get API_LOCATION");
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri");
                    e.Session.Log($"API_LOCATION : {elt.Attribute("value").Value}");

                    e.Session.Log($"Get SYNCPATH");
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SyncPath");
                    e.Session.Log($"SYNCPATH : {elt.Attribute("value").Value}");

                    e.Session.Log($"Get SYNC_INTERVAL");
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SyncInterval");
                    e.Session.Log($"SYNC_INTERVAL : {elt.Attribute("value").Value}");

                    System.IO.File.Copy(configPath, serviceBackupConfigPath);
                }
                catch (Exception ex)
                {
                    e.Session.Log($"ERROR : {ex.Message}");
                }
            }
            else if (e.IsUpgrading && System.IO.File.Exists(backupConfigPath) && System.IO.File.Exists(serviceBackupConfigPath))
            {
                // Tablet
                e.Session.Log($"Load {backupConfigPath}");
                var doc = XDocument.Load(backupConfigPath);

                e.Session.Log($"Get API_LOCATION");
                var elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri");
                e.Session["API_LOCATION"] = elt.Attribute("value").Value;
                e.Session.Log($"API_LOCATION : {e.Session["API_LOCATION"]}");

                e.Session.Log($"Get FILESERVER_LOCATION");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                e.Session["FILESERVER_LOCATION"] = elt.Attribute("value").Value;
                e.Session.Log($"FILESERVER_LOCATION : {e.Session["FILESERVER_LOCATION"]}");

                e.Session.Log($"Get SYNCPATH");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SyncPath");
                e.Session["SYNCPATH"] = elt.Attribute("value").Value;
                e.Session.Log($"SYNCPATH : {e.Session["SYNCPATH"]}");

                System.IO.File.Delete(backupConfigPath);

                // SyncService
                e.Session.Log($"Load {serviceBackupConfigPath}");
                doc = XDocument.Load(serviceBackupConfigPath);

                e.Session.Log($"Get API_LOCATION");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri");
                e.Session["API_LOCATION"] = elt.Attribute("value").Value;
                e.Session.Log($"API_LOCATION : {e.Session["API_LOCATION"]}");

                e.Session.Log($"Get SYNCPATH");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SyncPath");
                e.Session["SYNCPATH"] = elt.Attribute("value").Value;
                e.Session.Log($"SYNCPATH : {e.Session["SYNCPATH"]}");

                e.Session.Log($"Get SYNC_INTERVAL");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SyncInterval");
                e.Session["SYNC_INTERVAL"] = elt.Attribute("value").Value;
                e.Session.Log($"SYNC_INTERVAL : {e.Session["SYNC_INTERVAL"]}");

                System.IO.File.Delete(serviceBackupConfigPath);
            }
        }

        private static void Project_AfterInstall(SetupEventArgs e)
        {
            if (!e.IsUninstalling)
            {
                try
                {
                    CustomActions.StartHideProcess(System.IO.Path.Combine(e.Session.Property("INSTALLDIR"), "Kprocess.KL2.SyncService.exe"), "install");
                    Tasks.StartService("KL2 FS sync service", false);
                }
                catch (Exception ex)
                {
                    e.Session.Log($"Error AfterInstall : {ex.Message}");
                }
            }
        }

        static string ToCompactVersionString(Version version)
        {
            if (version.Revision != 0)
                return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            else if (version.Build != 0)
                return $"{version.Major}.{version.Minor}.{version.Build}";
            else if (version.Minor != 0)
                return $"{version.Major}.{version.Minor}";
            else
                return $"{version.Major}";
        }

        static void WriteLicenseRtf()
        {
            string inputFile = @"..\KProcess.KL2.Tablet.SetupUI\Resources\License_en-US.xaml";
            string outputFile = "License_CopyPaste_To_New_License.rtf";

            var test = new XamlReader();
            FlowDocument doc = XamlReader.Load(System.IO.File.OpenRead(inputFile)) as FlowDocument;
            doc.FontSize = 12;
            doc.Foreground = Brushes.Black;
            var content = new TextRange(doc.ContentStart, doc.ContentEnd);
            content.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
            using (var fileStream = System.IO.File.Create(outputFile))
                content.Save(fileStream, DataFormats.Rtf);
        }

        static Dir ScanFiles(string targetPath, string folderPath, Predicate<string> exclude = null)
        {
            var result = new Dir(targetPath);
            if (exclude == null)
                result.AddFiles(System.IO.Directory.EnumerateFiles(folderPath).Select(_ => new File(_)).ToArray());
            else
                result.AddFiles(System.IO.Directory.EnumerateFiles(folderPath).Where(_ => !exclude(_)).Select(_ => new File(_)).ToArray());
            result.AddDirs(System.IO.Directory.EnumerateDirectories(folderPath).Select(_ => ScanFiles(_.Split('\\').Last(), _, exclude)).ToArray());
            return result;
        }
    }
}
