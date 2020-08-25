using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;

namespace KProcess.KL2.Server.Setup
{
    class Program
    {
        //Common
        const string manufacturer = "K-process";
        static Guid BootstrapperUpgradeCode = Guid.Parse("52984f88-8634-4043-be36-25923e586b96");
        const string setupName = "KL²Server_Setup.exe";

        // API
        public static string API_VersionToBuild = Tasks.GetVersionFromFile(@"bin\Release\KProcess.KL2.API.exe").ToString();
        const string API_appName = "KL²® API";
        const string API_regKey = @"SOFTWARE\K-process\KL²API\Install";
        static Guid API_UpgradeCode = Guid.Parse("218f8d2a-6593-4b11-8852-e9e6aa8a0658");

        // FileServer
        public static string FileServer_VersionToBuild = Tasks.GetVersionFromFile(@"bin\Release\Kprocess.KL2.FileServer.exe").ToString();
        const string FileServer_appName = "KL²® File Server";
        const string FileServer_regKey = @"SOFTWARE\K-process\KL²FileServer\Install";
        static Guid FileServer_UpgradeCode = Guid.Parse("40395b9d-db11-4065-bc3b-957aee6cf8ce");

        // Notification
        public static string Notification_VersionToBuild = Tasks.GetVersionFromFile(@"bin\Release\KProcess.KL2.Notification.exe").ToString();
        const string Notification_appName = "KL²® Notification";
        const string Notification_regKey = @"SOFTWARE\K-process\KL²Notification\Install";
        static Guid Notification_UpgradeCode = Guid.Parse("2c0ac6ed-ecce-4987-8d39-8523c093ec91");

        // WebAdmin
        public static string WebAdmin_VersionToBuild = Tasks.GetVersionFromFile(@"bin\Release\KProcess.KL2.WebAdmin.dll").ToString();
        const string WebAdmin_appName = "KL²® Web Services";
        const string WebAdmin_regKey = @"SOFTWARE\K-process\KL²WebServices\Install";
        static Guid WebAdmin_UpgradeCode = Guid.Parse("d2a3e739-8aa9-446a-aaf7-083eb8ffcd10");

        static void Main(string[] args)
        {
            var projectName = $"KL² Server v{ToCompactVersionString(Version.Parse(API_VersionToBuild))}";

            /////////////// KL² API MSI ///////////////
            var API_projectName = $"{API_appName} v{ToCompactVersionString(Version.Parse(API_VersionToBuild))}";

            Dir API_scannAppDir = ScanFiles("API",
                                    System.IO.Path.GetFullPath(@"..\KL2-Core\KProcess.KL2.API\bin\Release"),
                                    (file) => file.EndsWith(".pdb") || (file.EndsWith(".xml") && !file.EndsWith("PublicKey.xml")) || file.EndsWith(".vshost.exe") || file.EndsWith(".vshost.exe.config") || file.EndsWith(".vshost.exe.manifest"));
            Dir API_appDir = new Dir(@"C:",
                new Dir("K-process",
                    new Dir("KL² Suite", API_scannAppDir)));

            RegValue API_installReg = new RegValue(RegistryHive.LocalMachineOrUsers, API_regKey, "InstallLocation", "[INSTALLDIR]");

            UrlReservation API_urlAcl = new UrlReservation("http://*:8081/", "*S-1-1-0", UrlReservationRights.all);
            FirewallException API_firewall = new FirewallException("KL2-API")
            {
                Scope = FirewallExceptionScope.any,
                IgnoreFailure = true,
                Port = "8081"
            };

            ManagedAction API_editConfig = new ManagedAction(CustomActions.API_EditConfig, Return.check, When.After, Step.InstallFinalize, WixSharp.Condition.NOT_Installed);
            ElevatedManagedAction API_uninstallService = new ElevatedManagedAction(CustomActions.API_UninstallService, Return.check, When.Before, Step.RemoveFiles, WixSharp.Condition.BeingUninstalled);

            var API_project = new ManagedProject(API_projectName, API_appDir, API_installReg, API_urlAcl, API_firewall, API_editConfig, API_uninstallService)
            {
                GUID = Versions.API_List.Single(_ => _.Key.ToString() == API_VersionToBuild).Value,
                Version = Version.Parse(API_VersionToBuild),
                UpgradeCode = API_UpgradeCode,
                AttributesDefinition = $"Id={Versions.API_List.Single(_ => _.Key.ToString() == API_VersionToBuild).Value};Manufacturer={manufacturer}",
                Description = $"{API_projectName},{manufacturer}",
                InstallScope = InstallScope.perMachine,
                Properties = new[]
                {
                    new Property("DATASOURCE", @"(LocalDb)\KL2"),
                    new Property("APPLICATION_URL", "http://*:8081"),
                    new Property("FILESERVER_LOCATION", "http://localhost:8082"),
                    new Property("BUNDLE_ID", "[BUNDLE_ID]")
                }
            };
            API_project.Package.AttributesDefinition = $"Id=*;InstallerVersion=500;Comments={API_projectName};Keywords={API_projectName},{manufacturer}";
            API_project.SetNetFxPrerequisite(new WixSharp.Condition(" (NETFRAMEWORK45 >= '#461308') "), "Please install .Net Framework 4.7.1 first.");
            API_project.ControlPanelInfo.ProductIcon = @"..\..\Assets\kl2_Suite.ico";
            API_project.MajorUpgrade = new MajorUpgrade
            {
                AllowSameVersionUpgrades = true,
                DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
            };
            API_project.Include(WixExtension.Util)
                .Include(WixExtension.Http);
            API_project.WixVariables.Add("WixUILicenseRtf", "License.rtf");
            API_project.BeforeInstall += API_Project_BeforeInstall;
            API_project.AfterInstall += API_Project_AfterInstall;

            // Save the list of files to check integrity
            void LogFiles(System.IO.StreamWriter writer, int rootCount, Dir root)
            {
                foreach (var file in root.Files)
                {
                    if (file.Name.EndsWith("WebServer.log"))
                        continue;
                    var splittedFileName = file.Name.Split('\\').ToList();
                    for (var i = 0; i < rootCount; i++)
                        splittedFileName.RemoveAt(0);
                    writer.WriteLine(string.Join("\\", splittedFileName.ToArray()));
                }
                foreach (var dir in root.Dirs)
                    LogFiles(writer, rootCount, dir);
            }
            using (var writer = System.IO.File.CreateText("API_FileList.txt"))
            {
                var rootCount = API_scannAppDir.Files.First().Name.Split('\\').Length - 1;
                LogFiles(writer, rootCount, API_scannAppDir);
            }

            API_project.BuildWxs(Compiler.OutputType.MSI, "KL2API_project.wxs");
            string API_productMsi = API_project.BuildMsi("KL²API.msi");

            /////////////// KL² FileServer MSI ///////////////
            var FileServer_projectName = $"{FileServer_appName} v{ToCompactVersionString(Version.Parse(FileServer_VersionToBuild))}";

            Dir FileServer_scannAppDir = ScanFiles("FileServer",
                                    System.IO.Path.GetFullPath(@"..\KL2-Core\Kprocess.KL2.FileServer\bin\Release"),
                                    (file) => file.EndsWith(".pdb") || file.EndsWith(".xml") || file.EndsWith(".vshost.exe") || file.EndsWith(".vshost.exe.config") || file.EndsWith(".vshost.exe.manifest"));
            Dir FileServer_appDir = new Dir(@"C:",
                new Dir("K-process",
                    new Dir("KL² Suite", FileServer_scannAppDir)));

            RegValue FileServer_installReg = new RegValue(RegistryHive.LocalMachineOrUsers, FileServer_regKey, "InstallLocation", "[INSTALLDIR]");

            UrlReservation FileServer_urlAcl = new UrlReservation("http://*:8082/", "*S-1-1-0", UrlReservationRights.all);
            FirewallException FileServer_firewall = new FirewallException("KL2-FilesServer")
            {
                Scope = FirewallExceptionScope.any,
                IgnoreFailure = true,
                Port = "8082"
            };

            ManagedAction FileServer_editConfig = new ManagedAction(CustomActions.FileServer_EditConfig, Return.check, When.After, Step.InstallFinalize, WixSharp.Condition.NOT_Installed);
            ElevatedManagedAction FileServer_uninstallService = new ElevatedManagedAction(CustomActions.FileServer_UninstallService, Return.check, When.Before, Step.RemoveFiles, WixSharp.Condition.BeingUninstalled);

            var FileServer_project = new ManagedProject(FileServer_projectName, FileServer_appDir, FileServer_installReg, FileServer_urlAcl, FileServer_firewall, FileServer_editConfig, FileServer_uninstallService)
            {
                GUID = Versions.FileServer_List.Single(_ => _.Key.ToString() == FileServer_VersionToBuild).Value,
                Version = Version.Parse(FileServer_VersionToBuild),
                UpgradeCode = FileServer_UpgradeCode,
                AttributesDefinition = $"Id={Versions.FileServer_List.Single(_ => _.Key.ToString() == FileServer_VersionToBuild).Value};Manufacturer={manufacturer}",
                Description = $"{FileServer_projectName},{manufacturer}",
                InstallScope = InstallScope.perMachine,
                Properties = new[]
                {
                    new Property("DATASOURCE", @"(LocalDb)\KL2"),
                    new Property("APPLICATION_URL", "http://*:8082"),
                    new Property("FILE_PROVIDER", "SFtp"),
                    new Property("SERVER", "127.0.0.1"),
                    new Property("PORT", "22"),
                    new Property("USER", "kl2"),
                    new Property("PASSWORD", "kl2"),
                    new Property("PUBLISHED_DIR", "/PublishedFiles"),
                    new Property("UPLOADED_DIR", "/UploadedFiles"),
                    new Property("BUNDLE_ID", "[BUNDLE_ID]")
                }
            };
            FileServer_project.Package.AttributesDefinition = $"Id=*;InstallerVersion=500;Comments={FileServer_projectName};Keywords={FileServer_projectName},{manufacturer}";
            FileServer_project.SetNetFxPrerequisite(new WixSharp.Condition(" (NETFRAMEWORK45 >= '#461308') "), "Please install .Net Framework 4.7.1 first.");
            FileServer_project.ControlPanelInfo.ProductIcon = @"..\..\Assets\kl2_Suite.ico";
            FileServer_project.MajorUpgrade = new MajorUpgrade
            {
                AllowSameVersionUpgrades = true,
                DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
            };
            FileServer_project.Include(WixExtension.Util)
                .Include(WixExtension.Http);
            FileServer_project.WixVariables.Add("WixUILicenseRtf", "License.rtf");
            FileServer_project.BeforeInstall += FileServer_Project_BeforeInstall;
            FileServer_project.AfterInstall += FileServer_Project_AfterInstall;

            // Save the list of files to check integrity
            using (var writer = System.IO.File.CreateText("FileServer_FileList.txt"))
            {
                var rootCount = FileServer_scannAppDir.Files.First().Name.Split('\\').Length - 1;
                LogFiles(writer, rootCount, FileServer_scannAppDir);
            }

            FileServer_project.BuildWxs(Compiler.OutputType.MSI, "KL2FileServer_project.wxs");
            string FileServer_productMsi = FileServer_project.BuildMsi("KL²FileServer.msi");

            /////////////// KL² Notification MSI ///////////////
            var Notification_projectName = $"{Notification_appName} v{ToCompactVersionString(Version.Parse(Notification_VersionToBuild))}";

            Dir Notification_scannAppDir = ScanFiles("Notification",
                                    System.IO.Path.GetFullPath(@"..\KProcess.KL2.Notification\bin\Release"),
                                    (file) => file.EndsWith(".pdb") || file.EndsWith(".xml") || file.EndsWith(".vshost.exe") || file.EndsWith(".vshost.exe.config") || file.EndsWith(".vshost.exe.manifest"));
            Dir Notification_appDir = new Dir(@"C:",
                new Dir("K-process",
                    new Dir("KL² Suite", Notification_scannAppDir)));

            RegValue Notification_installReg = new RegValue(RegistryHive.LocalMachineOrUsers, Notification_regKey, "InstallLocation", "[INSTALLDIR]");

            ManagedAction Notification_editConfig = new ManagedAction(CustomActions.Notification_EditConfig, Return.check, When.After, Step.InstallFinalize, WixSharp.Condition.NOT_Installed);
            ElevatedManagedAction Notification_uninstallService = new ElevatedManagedAction(CustomActions.Notification_UninstallService, Return.check, When.Before, Step.RemoveFiles, WixSharp.Condition.BeingUninstalled);

            var Notification_project = new ManagedProject(Notification_projectName, Notification_appDir, Notification_installReg, Notification_editConfig, Notification_uninstallService)
            {
                GUID = Versions.Notification_List.Single(_ => _.Key.ToString() == Notification_VersionToBuild).Value,
                Version = Version.Parse(Notification_VersionToBuild),
                UpgradeCode = Notification_UpgradeCode,
                AttributesDefinition = $"Id={Versions.Notification_List.Single(_ => _.Key.ToString() == Notification_VersionToBuild).Value};Manufacturer={manufacturer}",
                Description = $"{Notification_projectName},{manufacturer}",
                InstallScope = InstallScope.perMachine,
                Properties = new[]
                {
                    new Property("DATASOURCE", @"(LocalDb)\KL2"),
                    new Property("FILESERVER_LOCATION", "http://localhost:8082"),
                    new Property("INTERVAL", "3"),
                    new Property("BUNDLE_ID", "[BUNDLE_ID]")
                }
            };
            Notification_project.Package.AttributesDefinition = $"Id=*;InstallerVersion=500;Comments={Notification_projectName};Keywords={Notification_projectName},{manufacturer}";
            Notification_project.SetNetFxPrerequisite(new WixSharp.Condition(" (NETFRAMEWORK45 >= '#461308') "), "Please install .Net Framework 4.7.1 first.");
            Notification_project.ControlPanelInfo.ProductIcon = @"..\..\Assets\kl2_Suite.ico";
            Notification_project.MajorUpgrade = new MajorUpgrade
            {
                AllowSameVersionUpgrades = true,
                DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
            };
            Notification_project.WixVariables.Add("WixUILicenseRtf", "License.rtf");
            Notification_project.BeforeInstall += Notification_Project_BeforeInstall;
            Notification_project.AfterInstall += Notification_Project_AfterInstall;

            // Save the list of files to check integrity
            using (var writer = System.IO.File.CreateText("Notification_FileList.txt"))
            {
                var rootCount = Notification_scannAppDir.Files.First().Name.Split('\\').Length - 1;
                LogFiles(writer, rootCount, Notification_scannAppDir);
            }

            Notification_project.BuildWxs(Compiler.OutputType.MSI, "KL2Notification_project.wxs");
            string Notification_productMsi = Notification_project.BuildMsi("KL²Notification.msi");

            /////////////// KL² WebAdmin MSI ///////////////
            var WebAdmin_projectName = $"{WebAdmin_appName} v{ToCompactVersionString(Version.Parse(WebAdmin_VersionToBuild))}";

            Dir WebAdmin_scannAppDir = ScanWebFiles("KL2 Web Services", System.IO.Path.GetFullPath(@"..\KProcess.KL2.WebAdmin\bin\Release\PublishOutput"),
                new IISVirtualDir
                {
                    Name = "KL2 Web Services",
                    AppName = "KL2 Web Services",
                    WebSite = new WebSite("KL2 Web Services", "*:8080") { InstallWebSite = true },
                    WebAppPool = new WebAppPool("KL2AppPoolName", "Identity=applicationPoolIdentity")
                });
            Dir WebAdmin_appDir = new Dir(@"C:",
                            new Dir("inetpub", WebAdmin_scannAppDir)
                         );

            ElevatedManagedAction WebAdmin_editConfig = new ElevatedManagedAction(CustomActions.WebAdmin_EditConfig, Return.check, When.Before, Step.InstallFinalize, WixSharp.Condition.NOT_Installed)
            {
                UsesProperties = "D_INSTALLDIR=[INSTALLDIR];D_DATASOURCE=[DATASOURCE];D_API_LOCATION=[API_LOCATION];D_FILESERVER_LOCATION=[FILESERVER_LOCATION]"
            };

            UrlReservation WebAdmin_urlAcl = new UrlReservation("http://*:8080/", "*S-1-1-0", UrlReservationRights.all);
            FirewallException WebAdmin_firewall = new FirewallException("KL2-WebServices")
            {
                Scope = FirewallExceptionScope.any,
                IgnoreFailure = true,
                Port = "8080"
            };

            var WebAdmin_project = new ManagedProject(WebAdmin_projectName, WebAdmin_appDir, WebAdmin_editConfig, WebAdmin_urlAcl, WebAdmin_firewall)
            {
                GUID = Versions.WebAdmin_List.Single(_ => _.Key.ToString() == WebAdmin_VersionToBuild).Value,
                Version = Version.Parse(WebAdmin_VersionToBuild),
                UpgradeCode = WebAdmin_UpgradeCode,
                AttributesDefinition = $"Id={Versions.WebAdmin_List.Single(_ => _.Key.ToString() == WebAdmin_VersionToBuild).Value};Manufacturer={manufacturer}",
                Description = $"{WebAdmin_projectName},{manufacturer}",
                InstallScope = InstallScope.perMachine,
                Properties = new[]
                {
                    new Property("DATASOURCE", @"(LocalDb)\KL2"),
                    new Property("API_LOCATION", "http://localhost:8081"),
                    new Property("FILESERVER_LOCATION", "http://localhost:8082"),
                    new Property("BUNDLE_ID", "[BUNDLE_ID]")
                }
            };
            WebAdmin_project.Package.AttributesDefinition = $"Id=*;InstallerVersion=500;Comments={WebAdmin_projectName};Keywords={WebAdmin_projectName},{manufacturer}";
            WebAdmin_project.SetNetFxPrerequisite(new WixSharp.Condition(" (NETFRAMEWORK45 >= '#461308') "), "Please install .Net Framework 4.7.1 first.");
            WebAdmin_project.ControlPanelInfo.ProductIcon = @"..\..\Assets\kl2_WebServices.ico";
            WebAdmin_project.MajorUpgrade = new MajorUpgrade
            {
                AllowSameVersionUpgrades = true,
                DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
            };
            WebAdmin_project.WixVariables.Add("WixUILicenseRtf", "License.rtf");
            WebAdmin_project.DefaultDeferredProperties += "DATASOURCE=[DATASOURCE];API_LOCATION=[API_LOCATION];FILESERVER_LOCATION=[FILESERVER_LOCATION];";
            WebAdmin_project.BeforeInstall += WebAdmin_Project_BeforeInstall;

            // Save the list of files to check integrity
            using (var writer = System.IO.File.CreateText("WebAdmin_FileList.txt"))
            {
                var rootCount = WebAdmin_scannAppDir.Files.First().Name.Split('\\').Length - 1;
                LogFiles(writer, rootCount, WebAdmin_scannAppDir);
            }

            WebAdmin_project.BuildWxs(Compiler.OutputType.MSI, "KL2WebAdmin_project.wxs");
            string WebAdmin_productMsi = WebAdmin_project.BuildMsi("KL²WebServices.msi");

            /////////////// BOOTSTRAPPER ///////////////
            var bootstrapper = new Bundle(projectName,
                new PackageGroupRef("NetFx471Redist"),
                new MsiPackage(API_productMsi)
                {
                    Id = "KL2_API_MSI",
                    MsiProperties = $"BUNDLE_ID=[WixBundleProviderKey]; DATASOURCE=[DATASOURCE]; FILESERVER_LOCATION=[FILESERVER_LOCATION];"
                },
                new MsiPackage(FileServer_productMsi)
                {
                    Id = "KL2_FILESERVER_MSI",
                    MsiProperties = $"BUNDLE_ID=[WixBundleProviderKey]; DATASOURCE=[DATASOURCE]; FILE_PROVIDER=[FILE_PROVIDER]; SERVER=[SERVER]; PORT=[PORT]; USER=[USER]; PASSWORD=[PASSWORD]; PUBLISHED_DIR=[PUBLISHED_DIR]; UPLOADED_DIR=[UPLOADED_DIR];"
                },
                new MsiPackage(Notification_productMsi)
                {
                    Id = "KL2_NOTIFICATION_MSI",
                    MsiProperties = $"BUNDLE_ID=[WixBundleProviderKey]; DATASOURCE=[DATASOURCE]; FILESERVER_LOCATION=[FILESERVER_LOCATION]; INTERVAL=[INTERVAL];"
                },
                new MsiPackage(WebAdmin_productMsi)
                {
                    Id = "KL2_WEBSERVICES_MSI",
                    MsiProperties = $"BUNDLE_ID=[WixBundleProviderKey]; DATASOURCE=[DATASOURCE]; API_LOCATION=[API_LOCATION]; FILESERVER_LOCATION=[FILESERVER_LOCATION];"
                })
            {
                Version = API_project.Version,
                Manufacturer = manufacturer,
                UpgradeCode = BootstrapperUpgradeCode,
                AboutUrl = @"http://www.k-process.com/",
                IconFile = @"..\..\Assets\kl2_Suite.ico",
                SuppressWixMbaPrereqVars = true,
                DisableModify = "yes",
                Application = new ManagedBootstrapperApplication(@"..\KProcess.KL2.Server.SetupUI\bin\Release\KProcess.KL2.Server.SetupUI.dll")
                {
                    Payloads = new[]
                    {
                        @"..\KProcess.KL2.Server.SetupUI\BootstrapperCore.config".ToPayload(),
                        @"..\KProcess.KL2.Server.SetupUI\bin\Release\BootstrapperCore.dll".ToPayload(),
                        @"..\KProcess.KL2.Server.SetupUI\bin\Release\Microsoft.Deployment.WindowsInstaller.dll".ToPayload(),
                        @"..\KProcess.KL2.Server.SetupUI\bin\Release\System.Windows.Interactivity.dll".ToPayload(),
                        @"..\KProcess.KL2.Server.SetupUI\bin\Release\ControlzEx.dll".ToPayload(),
                        @"..\KProcess.KL2.Server.SetupUI\bin\Release\MahApps.Metro.dll".ToPayload(),
                        @"..\KProcess.KL2.Server.SetupUI\bin\Release\MahApps.Metro.IconPacks.dll".ToPayload(),
                        @"..\KProcess.KL2.Server.SetupUI\bin\Release\Ookii.Dialogs.Wpf.dll".ToPayload(),
                        @"..\KProcess.KL2.Server.SetupUI\bin\Release\Renci.SshNet.dll".ToPayload(),
                        @"..\KProcess.KL2.Server.SetupUI\bin\Release\KProcess.KL2.ConnectionSecurity.dll".ToPayload(),
                        @"..\KProcess.KL2.Server.SetupUI\bin\Release\FreshDeskLib.dll".ToPayload(),
                        @"..\KProcess.KL2.Server.SetupUI\bin\Release\Newtonsoft.Json.dll".ToPayload()
                    },
                    PrimaryPackageId = "KL2_API_MSI"
                }
            };
            bootstrapper.Variables = new[] {
                new Variable("DATASOURCE", @"(LocalDb)\KL2"),
                new Variable("API_LOCATION", "http://localhost:8081"),
                new Variable("FILESERVER_LOCATION", "http://localhost:8082"),
                new Variable("FILE_PROVIDER", "Local"),
                new Variable("SERVER", ""),
                new Variable("PORT", ""),
                new Variable("USER", ""),
                new Variable("PASSWORD", ""),
                new Variable("PUBLISHED_DIR", @"C:\PublishedFiles"),
                new Variable("UPLOADED_DIR", @"C:\UploadedFiles")
            };
            bootstrapper.PreserveTempFiles = true;
            bootstrapper.WixSourceGenerated += document =>
            {
                document.Root.Select("Bundle").Add(XDocument.Load("NET_Framework_Payload.wxs").Root.Elements());
                document.Root.Add(XDocument.Load("NET_Framework_Fragments.wxs").Root.Elements());
            };
            bootstrapper.Include(WixExtension.Util)
                .Include(WixExtension.Http);
            bootstrapper.Build(setupName);
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

        static Dir ScanWebFiles(string targetPath, string folderPath, IISVirtualDir iis)
        {
            var result = new Dir(targetPath, iis)
            {
                IsInstallDir = true
            };
            result.AddFiles(System.IO.Directory.EnumerateFiles(folderPath).Select(_ => new File(_)).ToArray());
            result.AddDirs(System.IO.Directory.EnumerateDirectories(folderPath).Select(_ => ScanFiles(_.Split('\\').Last(), _)).ToArray());

            // Add Logs
            var tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "WebServer.log");
            System.IO.File.Create(tempFilePath);
            result.AddDirs(new Dir[] { new Dir("Logs" , new DirPermission("Everyone", GenericPermission.All), new File(tempFilePath)) });

            return result;
        }

        private static void API_Project_BeforeInstall(SetupEventArgs e)
        {
            string backupConfigPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "KProcess.KL2.API.exe.config");

            Version installedVersion = AppSearch.GetProductVersionFromUpgradeCode(e.UpgradeCode);
            if (installedVersion != null && e.IsModifying && !e.IsUninstalling)
            {
                try
                {
                    string configPath = $"{e.Session["INSTALLDIR"]}KProcess.KL2.API.exe.config";
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

                var elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                e.Session["DATASOURCE"] = elt.Attribute("connectionString").Value.Split(';')
                    .Single(_ => _.StartsWith("Data Source")).Split('=')[1];

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApplicationUrl");
                e.Session["APPLICATION_URL"] = elt.Attribute("value").Value;

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                e.Session["FILESERVER_LOCATION"] = elt.Attribute("value").Value;

                System.IO.File.Delete(backupConfigPath);
            }
        }

        private static void FileServer_Project_BeforeInstall(SetupEventArgs e)
        {
            string backupConfigPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Kprocess.KL2.FileServer.exe.config");

            Version installedVersion = AppSearch.GetProductVersionFromUpgradeCode(e.UpgradeCode);
            if (installedVersion != null && e.IsModifying && !e.IsUninstalling)
            {
                try
                {
                    string configPath = $"{e.Session["INSTALLDIR"]}Kprocess.KL2.FileServer.exe.config";
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

                var elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                e.Session["DATASOURCE"] = elt.Attribute("connectionString").Value.Split(';')
                    .Single(_ => _.StartsWith("Data Source")).Split('=')[1];

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApplicationUrl");
                e.Session["APPLICATION_URL"] = elt.Attribute("value").Value;

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileProvider");
                e.Session["FILE_PROVIDER"] = elt.Attribute("value").Value;

                if (e.Session["FILE_PROVIDER"] == "SFtp")
                {
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_Server");
                    e.Session["SERVER"] = elt.Attribute("value").Value;

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_Port");
                    e.Session["PORT"] = elt.Attribute("value").Value;

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_User");
                    e.Session["USER"] = elt.Attribute("value").Value;

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_Password");
                    e.Session["PASSWORD"] = elt.Attribute("value").Value;

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_PublishedFilesDirectory");
                    e.Session["PUBLISHED_DIR"] = elt.Attribute("value").Value;

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_UploadedFilesDirectory");
                    e.Session["UPLOADED_DIR"] = elt.Attribute("value").Value;
                }
                else if (e.Session["FILE_PROVIDER"] == "Local")
                {
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "Local_PublishedFilesDirectory");
                    e.Session["PUBLISHED_DIR"] = elt.Attribute("value").Value;

                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "Local_UploadedFilesDirectory");
                    e.Session["UPLOADED_DIR"] = elt.Attribute("value").Value;
                }

                System.IO.File.Delete(backupConfigPath);
            }
        }

        private static void Notification_Project_BeforeInstall(SetupEventArgs e)
        {
            string backupConfigPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "KProcess.KL2.Notification.exe.config");

            Version installedVersion = AppSearch.GetProductVersionFromUpgradeCode(e.UpgradeCode);
            if (installedVersion != null && e.IsModifying && !e.IsUninstalling)
            {
                try
                {
                    string configPath = $"{e.Session["INSTALLDIR"]}KProcess.KL2.Notification.exe.config";
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

                var elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                e.Session["DATASOURCE"] = elt.Attribute("connectionString").Value.Split(';')
                    .Single(_ => _.StartsWith("Data Source")).Split('=')[1];

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                e.Session["FILESERVER_LOCATION"] = elt.Attribute("value").Value;

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SendNotificationInterval");
                e.Session["INTERVAL"] = elt.Attribute("value").Value;

                System.IO.File.Delete(backupConfigPath);
            }
        }

        private static void WebAdmin_Project_BeforeInstall(SetupEventArgs e)
        {
            string backupConfigPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Web.config");

            Version installedVersion = AppSearch.GetProductVersionFromUpgradeCode(e.UpgradeCode);
            if (installedVersion != null && e.IsModifying && !e.IsUninstalling)
            {
                try
                {
                    string configPath = $"{e.Session["INSTALLDIR"]}Web.config";
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

                var elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                e.Session["DATASOURCE"] = elt.Attribute("connectionString").Value.Split(';')
                    .Single(_ => _.StartsWith("Data Source")).Split('=')[1];

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri");
                e.Session["API_LOCATION"] = elt.Attribute("value").Value;

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                e.Session["FILESERVER_LOCATION"] = elt.Attribute("value").Value;

                System.IO.File.Delete(backupConfigPath);
            }
        }

        private static void API_Project_AfterInstall(SetupEventArgs e)
        {
            if (!e.IsUninstalling)
            {
                // Install and run
                try
                {
                    CustomActions.StartHideProcess(System.IO.Path.Combine(e.Session.Property("INSTALLDIR"), "KProcess.KL2.API.exe"), "install");
                    Tasks.StartService("KL2 API", false);
                }
                catch (Exception ex)
                {
                    e.Session.Log($"Error AfterInstall : {ex.Message}");
                }
            }
        }

        private static void FileServer_Project_AfterInstall(SetupEventArgs e)
        {
            if (!e.IsUninstalling)
            {
                // Install and run
                try
                {
                    CustomActions.StartHideProcess(System.IO.Path.Combine(e.Session.Property("INSTALLDIR"), "Kprocess.KL2.FileServer.exe"), "install");
                    Tasks.StartService("KL2 File Server", false);
                }
                catch (Exception ex)
                {
                    e.Session.Log($"Error AfterInstall : {ex.Message}");
                }
            }
        }

        private static void Notification_Project_AfterInstall(SetupEventArgs e)
        {
            if (!e.IsUninstalling)
            {
                try
                {
                    CustomActions.StartHideProcess(System.IO.Path.Combine(e.Session.Property("INSTALLDIR"), "KProcess.KL2.Notification.exe"), "install");
                    Tasks.StartService("KL2 Notification", false);
                }
                catch (Exception ex)
                {
                    e.Session.Log($"Error AfterInstall : {ex.Message}");
                }
            }
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
