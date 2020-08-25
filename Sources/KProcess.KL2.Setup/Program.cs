using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;

namespace KProcess.KL2.Setup
{
    class Program
    {
        public static string VersionToBuild = Tasks.GetVersionFromFile(@"bin\Release\KL².exe").ToString();
        const string appName = "KL²® Video Analyst";
        const string manufacturer = "K-process";
        const string setupName = "KL²VideoAnalyst_Setup.exe";
        const string regKey = @"SOFTWARE\K-process\KL²VideoAnalyst\Install";

        static Guid UpgradeCode = Guid.Parse("b7e92242-e349-47d7-8cc3-aba7a5869868");
        static Guid BootstrapperUpgradeCode = Guid.Parse("55ed5ef5-8516-4600-8e72-01189d875ea6");

        static void Main(string[] args)
        {
            var projectName = $"{appName} v{ToCompactVersionString(Version.Parse(VersionToBuild))}";
            //WriteLicenseRtf();

            Compiler.LightOptions = "-sice:ICE57";

            /////////////// KL² MSI ///////////////
            ExeFileShortcut desktopShortcut = new ExeFileShortcut(appName, @"[INSTALLDIR]KL².exe", "") { Condition = "INSTALLDESKTOPSHORTCUT=\"yes\"" };
            ExeFileShortcut startMenuLaunchShortcut = new ExeFileShortcut(appName, @"[INSTALLDIR]KL².exe", "") { Condition = "INSTALLSTARTMENUSHORTCUT=\"yes\"" };
            ExeFileShortcut startMenuUninstallShortcut = new ExeFileShortcut($"Uninstall {appName}", $@"[AppDataFolder]\Package Cache\[BUNDLE_ID]\{setupName}", "/uninstall") { WorkingDirectory = "AppDataFolder", Condition = "INSTALLSTARTMENUSHORTCUT =\"yes\" AND BUNDLE_ID <> \"[BUNDLE_ID]\"" };
            Dir scanAppDir = ScanFiles("KL2VideoAnalyst",
                                    System.IO.Path.GetFullPath(@"..\KL2-Core\KProcess.Ksmed.Presentation.Shell\bin\Release"),
                                    (file) => file.EndsWith(".pdb") || file.EndsWith(".xml") || file.EndsWith(".vshost.exe") || file.EndsWith(".vshost.exe.config") || file.EndsWith(".vshost.exe.manifest"));
            //scannAppDir.AddDir(new Dir("Extensions"));
            /*scannAppDir.AddDir(new Dir("ExportBuffer",
                                        new Dir("SQL")
                                        {
                                            Permissions = new DirPermission[]
                                        {
                                            new DirPermission("Users", GenericPermission.All),
                                            new DirPermission("NetworkService", GenericPermission.All)
                                        }
                                        }));*/
            Dir desktopDir = new Dir("%DesktopFolder%", desktopShortcut);
            Dir startMenuDir = new Dir($@"%ProgramMenuFolder%\{manufacturer}", startMenuLaunchShortcut, startMenuUninstallShortcut);

            RegValue installReg = new RegValue(RegistryHive.LocalMachine, regKey, "InstallLocation", "[INSTALLDIR]");
            RegValue desktopShortcutReg = new RegValue(RegistryHive.LocalMachine, regKey, "DesktopShortcut", "[INSTALLDESKTOPSHORTCUT]");
            RegValue startMenuShortcutReg = new RegValue(RegistryHive.LocalMachine, regKey, "StartMenuShortcut", "[INSTALLSTARTMENUSHORTCUT]");

            ManagedAction editConfig = new ManagedAction(CustomActions.EditConfig, Return.check, When.After, Step.InstallFinalize, WixSharp.Condition.NOT_Installed);

            var project = new ManagedProject(projectName, scanAppDir, desktopDir, startMenuDir, installReg, desktopShortcutReg, startMenuShortcutReg, editConfig)
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
                    new Property("API_LOCATION", ""),
                    new Property("FILESERVER_LOCATION", ""),
                    new Property("SYNCPATH", "[INSTALLDIR]\\SyncFiles"),
                    new Property("SENDREPORT", "yes"),
                    new Property("MUTE", "no"),
                    new Property("BUNDLE_ID", "[BUNDLE_ID]")
                }
            };
            project.Package.AttributesDefinition = $"Id=*;InstallerVersion=500;Comments={projectName};Keywords={projectName},{manufacturer}";
            project.SetNetFxPrerequisite(new WixSharp.Condition(" (NETFRAMEWORK45 >= '#461308') "), "Please install .Net Framework 4.7.1 first.");
            project.ControlPanelInfo.ProductIcon = @"..\..\Assets\kl2_VideoAnalyst.ico";
            project.MajorUpgrade = new MajorUpgrade
            {
                AllowSameVersionUpgrades = true,
                DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
            };
            project.WixVariables.Add("WixUILicenseRtf", "License.rtf");
            project.BeforeInstall += Project_BeforeInstall;

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
            using (var writer = System.IO.File.CreateText("VideoAnalyst_FileList.txt"))
            {
                var rootCount = scanAppDir.Files.First().Name.Split('\\').Length - 1;
                LogFiles(writer, rootCount, scanAppDir);
            }

            project.BuildWxs(Compiler.OutputType.MSI, "KL2_project.wxs");
            string productMsi = project.BuildMsi("KL²VideoAnalyst.msi");

            /////////////// BOOTSTRAPPER ///////////////
            var bootstrapper = new Bundle(projectName,
                new PackageGroupRef("NetFx471Redist"),
                new MsiPackage(productMsi)
                {
                    Id = "KL2VIDEOANALYST_MSI",
                    MsiProperties = $"BUNDLE_ID=[WixBundleProviderKey]; INSTALLDIR=[INSTALLDIR]; LANGUAGE=[LANGUAGE]; INSTALLDESKTOPSHORTCUT=[INSTALLDESKTOPSHORTCUT]; INSTALLSTARTMENUSHORTCUT=[INSTALLSTARTMENUSHORTCUT]; API_LOCATION=[API_LOCATION]; FILESERVER_LOCATION=[FILESERVER_LOCATION]; SYNCPATH=[SYNCPATH]; SENDREPORT=[SENDREPORT]; MUTE=[MUTE];"
                })
            {
                Version = project.Version,
                Manufacturer = manufacturer,
                UpgradeCode = BootstrapperUpgradeCode,
                AboutUrl = @"http://www.k-process.com/",
                IconFile = @"..\..\Assets\kl2_VideoAnalyst.ico",
                SuppressWixMbaPrereqVars = true,
                DisableModify = "yes",
                Application = new ManagedBootstrapperApplication(@"..\KProcess.KL2.SetupUI\bin\Release\KProcess.KL2.SetupUI.dll")
                {
                    Payloads = new[]
                    {
                        @"..\KProcess.KL2.SetupUI\BootstrapperCore.config".ToPayload(),
                        @"..\KProcess.KL2.SetupUI\bin\Release\BootstrapperCore.dll".ToPayload(),
                        @"..\KProcess.KL2.SetupUI\bin\Release\Microsoft.Deployment.WindowsInstaller.dll".ToPayload(),
                        @"..\KProcess.KL2.SetupUI\bin\Release\System.Windows.Interactivity.dll".ToPayload(),
                        @"..\KProcess.KL2.SetupUI\bin\Release\ControlzEx.dll".ToPayload(),
                        @"..\KProcess.KL2.SetupUI\bin\Release\MahApps.Metro.dll".ToPayload(),
                        @"..\KProcess.KL2.SetupUI\bin\Release\MahApps.Metro.IconPacks.dll".ToPayload(),
                        @"..\KProcess.KL2.SetupUI\bin\Release\WPF.Dialogs.dll".ToPayload(),
                        @"..\KProcess.KL2.SetupUI\bin\Release\KProcess.KL2.ConnectionSecurity.dll".ToPayload(),
                        @"..\KProcess.KL2.SetupUI\bin\Release\FreshDeskLib.dll".ToPayload(),
                        @"..\KProcess.KL2.SetupUI\bin\Release\Newtonsoft.Json.dll".ToPayload()
                    },
                    PrimaryPackageId = "KL2VIDEOANALYST_MSI"
                }
            };
            bootstrapper.Variables = new[] {
                new Variable("MSIINSTALLPERUSER", "0"),
                new Variable("INSTALLDIR", "dummy"),
                new Variable("INSTALLDESKTOPSHORTCUT", "yes"),
                new Variable("INSTALLSTARTMENUSHORTCUT", "yes"),
                new Variable("API_LOCATION", "http://localhost:8081"),
                new Variable("FILESERVER_LOCATION", "http://localhost:8082"),
                new Variable("SYNCPATH", @".\SyncFiles"),
                new Variable("SENDREPORT", "yes"),
                new Variable("MUTE", "no"),
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
            string backupConfigPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "KL².exe.config");

            Version installedVersion = AppSearch.GetProductVersionFromUpgradeCode(e.UpgradeCode);
            e.Session.Log($"Installed version : {installedVersion}");
            if (installedVersion != null && e.IsModifying && !e.IsUninstalling)
            {
                try
                {
                    string configPath = $"{e.Session["INSTALLDIR"]}KL².exe.config";
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

                    e.Session.Log($"Get SENDREPORT");
                    elt = doc.Root.Element("userSettings").Element("KProcess.Ksmed.Presentation.Shell.Settings").Elements("setting").Single(x => x.Attribute("name").Value == "SendReport");
                    e.Session.Log($"SENDREPORT : {(elt.Element("value").Value == "True" ? "yes" : "no")}");

                    e.Session.Log($"Get MUTE");
                    elt = doc.Root.Element("userSettings").Element("KProcess.Ksmed.Presentation.Shell.Settings").Elements("setting").Single(x => x.Attribute("name").Value == "Mute");
                    e.Session.Log($"MUTE : {(elt.Element("value").Value == "True" ? "yes" : "no")}");

                    System.IO.File.Copy(configPath, backupConfigPath);
                }
                catch (Exception ex)
                {
                    e.Session.Log($"ERROR : {ex.Message}");
                }
            }
            else if (e.IsUpgrading && System.IO.File.Exists(backupConfigPath))
            {
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

                e.Session.Log($"Get SENDREPORT");
                elt = doc.Root.Element("userSettings").Element("KProcess.Ksmed.Presentation.Shell.Settings").Elements("setting").Single(x => x.Attribute("name").Value == "SendReport");
                e.Session["SENDREPORT"] = elt.Element("value").Value == "True" ? "yes" : "no";
                e.Session.Log($"SENDREPORT : {e.Session["SENDREPORT"]}");

                e.Session.Log($"Get MUTE");
                elt = doc.Root.Element("userSettings").Element("KProcess.Ksmed.Presentation.Shell.Settings").Elements("setting").Single(x => x.Attribute("name").Value == "Mute");
                e.Session["MUTE"] = elt.Element("value").Value == "True" ? "yes" : "no";
                e.Session.Log($"MUTE : {e.Session["MUTE"]}");

                System.IO.File.Delete(backupConfigPath);
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
            string inputFile = @"..\KProcess.KL2.SetupUI\Resources\License_en-US.xaml";
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
