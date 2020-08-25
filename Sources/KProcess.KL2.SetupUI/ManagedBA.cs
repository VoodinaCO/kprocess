using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace KProcess.KL2.SetupUI
{
    public class ManagedBA : BootstrapperApplication, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Log(string e) =>
            Engine.Log(LogLevel.Verbose, e);

        public void LogExceptionWithInner(Exception e)
        {
            Engine.Log(LogLevel.Verbose, $"ERROR : {e.Message}");
            if (e.InnerException != null)
                LogExceptionWithInner(e.InnerException);
        }

        // global dispatcher
        static public Dispatcher BootstrapperDispatcher { get; private set; }

        public static ManagedBA Instance;

        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        private double _PackageProgress = 0;
        public double PackageProgress
        {
            get { return _PackageProgress; }
            set
            {
                if (_PackageProgress != value)
                {
                    _PackageProgress = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _GlobalProgress = 0;
        public double GlobalProgress
        {
            get { return _GlobalProgress; }
            set
            {
                if (_GlobalProgress != value)
                {
                    _GlobalProgress = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _CurrentlyProcessingPackageName = null;
        public string CurrentlyProcessingPackageName
        {
            get { return _CurrentlyProcessingPackageName; }
            set
            {
                if (_CurrentlyProcessingPackageName != value)
                {
                    _CurrentlyProcessingPackageName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public LaunchAction launchAction { get; set; }

        private ActionResult _ActionResult = ActionResult.NotExecuted;
        public ActionResult ActionResult
        {
            get { return _ActionResult; }
            set
            {
                if (_ActionResult != value)
                {
                    _ActionResult = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool errorOccured = false;

        // entry point for our custom UI
        protected override void Run()
        {
            Instance = this;

            Error += (sender, e) =>
            {
                errorOccured = true;
                Log(e.ErrorMessage);
            };

            Log("Launching KL² bootstrapper UX");
            BootstrapperDispatcher = Dispatcher.CurrentDispatcher;

            // On peut détecter le mode (Unknown, Help, Layout, Uninstall, Install, Modify, Repair) grace à
            launchAction = Command.Action;

            // On peut détecter si on veut une interface graphique (Unknown, Embedded, None, Passive, Full) avec
            Display gui = Command.Display;

            Log($"LaunchAction : {launchAction}");
            Log($"Gui : {gui}");

            // On peut récupérer les paramètres de la ligne de commande avec
            string[] args = Command.GetCommandLineArgs();
            Log($"CommandLineArgs : {string.Join(" ", args)}");

            LocalizationExt.ProductVersion = Engine.VersionVariables["WixBundleVersion"].ToString();

            // Permet de détecter une réparation
            DetectPackageComplete += (sender, e) =>
            {
                Log($"PackageId & State : {e.PackageId}, {e.State}");
                if (launchAction == LaunchAction.Install && e.PackageId == "KL2VIDEOANALYST_MSI" && e.State == PackageState.Present)
                    launchAction = LaunchAction.Repair;
            };
            // Permet de détecter une mise à jour
            DetectRelatedMsiPackage += (sender, e) =>
            {
                Log($"PackageId & Operation : {e.PackageId}, {e.Operation}");
                if (launchAction == LaunchAction.Install && e.PackageId == "KL2VIDEOANALYST_MSI" && e.Operation == RelatedOperation.MajorUpgrade)
                    launchAction = LaunchAction.UpdateReplace;
            };

            long elevated = Engine.NumericVariables["WixBundleElevated"];
            if (elevated != 1)
            {
                try
                {
                    Engine.Elevate(IntPtr.Zero);
                }
                catch { return; }
            }
            do
            {
                elevated = Engine.NumericVariables["WixBundleElevated"];
            }
            while (elevated != 1 && !errorOccured);
            if (errorOccured)
            {
                Log("Error occured, exit setup");
                Engine.Quit(0);
                return;
            }

            ExecuteMsiMessage += (o, e) => Log(e.Message);
            Progress += (o, e) =>
            {
                PackageProgress = e.ProgressPercentage;
                GlobalProgress = e.OverallPercentage;
                if (CancellationTokenSource.IsCancellationRequested)
                    e.Result = Result.Cancel;
            };
            ExecuteComplete += (o, e) => GlobalProgress = 100;
            ExecutePackageBegin += (o, e) =>
            {
                switch(e.PackageId)
                {
                    case "KL2VIDEOANALYST_MSI":
                        CurrentlyProcessingPackageName = "KL² Video Analyst";
                        break;
                    default:
                        CurrentlyProcessingPackageName = null;
                        break;
                }
            };
            ExecutePackageComplete += (o, e) =>
            {
                CurrentlyProcessingPackageName = null;
            };

            DetectComplete += (sender, e) => SetLaunchAction(gui);

            Engine.Detect();

            Dispatcher.Run();
        }

        private void SetLaunchAction(Display gui)
        {
            if (launchAction == LaunchAction.Install) // Install
            {
                Log($"What have I to do? : {launchAction}");

                PlanPackageBegin += SetInstallPackagePlannedState;

                ApplyComplete += (o, e) =>
                {
                    ActionResult = e.Status >= 0 ? ActionResult.Success : ActionResult.Failure;
                    MainViewModel.Instance.NavigateFromOffset(1);
                };

                try
                {
                    BootstrapperDispatcher.Invoke(() =>
                    {
                        MainView view = new MainView();
                        view.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
                        view.Show();
                    });
                }
                catch (Exception ex)
                {
                    LogExceptionWithInner(ex);
                }
            }
            else if (launchAction == LaunchAction.UpdateReplace) // Update
            {
                Log($"What have I to do? : {launchAction}");

                PlanPackageBegin += SetUpdatePackagePlannedState;

                ApplyComplete += (o, e) =>
                {
                    ActionResult = e.Status >= 0 ? ActionResult.Success : ActionResult.Failure;
                    UninstallViewModel.Instance.NavigateFromOffset(1);
                };

                try
                {
                    BootstrapperDispatcher.Invoke(() =>
                    {
                        UninstallView view = new UninstallView();
                        view.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
                        view.Show();
                    });
                }
                catch (Exception ex)
                {
                    LogExceptionWithInner(ex);
                }
            }
            else if (launchAction == LaunchAction.Repair) // Repair
            {
                Log($"What have I to do? : {launchAction}");

                PlanPackageBegin += SetRepairPackagePlannedState;

                ApplyComplete += (o, e) =>
                {
                    ActionResult = e.Status >= 0 ? ActionResult.Success : ActionResult.Failure;
                    UninstallViewModel.Instance.NavigateFromOffset(1);
                };

                try
                {
                    BootstrapperDispatcher.Invoke(() =>
                    {
                        UninstallView view = new UninstallView();
                        view.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
                        view.Show();
                    });
                }
                catch (Exception ex)
                {
                    LogExceptionWithInner(ex);
                }
            }
            else if (launchAction == LaunchAction.Uninstall) // Uninstall
            {
                Log($"What have I to do? : {launchAction}");

                PlanPackageBegin += SetUninstallPackagePlannedState;

                ApplyComplete += (o, e) =>
                {
                    ActionResult = e.Status >= 0 ? ActionResult.Success : ActionResult.Failure;
                    if (gui == Display.None || gui == Display.Embedded)
                        BootstrapperDispatcher.Invoke(() => Engine.Quit((int)ActionResult));
                    else
                        UninstallViewModel.Instance.NavigateFromOffset(1);
                };

                if (gui == Display.None || gui == Display.Embedded)
                {
                    Engine.Plan(LaunchAction.Uninstall);
                    Engine.Apply(IntPtr.Zero);
                }
                else
                {
                    try
                    {
                        BootstrapperDispatcher.Invoke(() =>
                        {
                            UninstallView view = new UninstallView();
                            view.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
                            view.Show();
                        });
                    }
                    catch (Exception ex)
                    {
                        LogExceptionWithInner(ex);
                    }
                }
            }
        }

        private void SetUpdatePackagePlannedState(object sender, PlanPackageBeginEventArgs e)
        {
            switch (e.PackageId)
            {
                case "NetFx471Redist":
                    e.State = RequestState.None;
                    break;
                case "KL2VIDEOANALYST_MSI":
                    e.State = RequestState.Present;
                    break;
                default:
                    break;
            }
        }

        private void SetInstallPackagePlannedState(object sender, PlanPackageBeginEventArgs e)
        {
            switch(e.PackageId)
            {
                case "NetFx471Redist":
                    e.State = RequestState.Present;
                    break;
                case "KL2VIDEOANALYST_MSI":
                    e.State = RequestState.Present;
                    break;
                default:
                    break;
            }
        }

        private void SetRepairPackagePlannedState(object sender, PlanPackageBeginEventArgs e)
        {
            switch (e.PackageId)
            {
                case "NetFx471Redist":
                    e.State = RequestState.None;
                    break;
                case "KL2VIDEOANALYST_MSI":
                    e.State = RequestState.Repair;
                    break;
                default:
                    break;
            }
        }

        private void SetUninstallPackagePlannedState(object sender, PlanPackageBeginEventArgs e)
        {
            switch (e.PackageId)
            {
                case "NetFx471Redist":
                    e.State = RequestState.None;
                    break;
                case "KL2VIDEOANALYST_MSI":
                    e.State = RequestState.Absent;
                    break;
                default:
                    break;
            }
        }
    }
}
