using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Security.Activation;
using KProcess.Presentation.Windows;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran d'activation.
    /// </summary>
    class ActivationViewModel : ViewModelBase, IActivationViewModel
    {

        #region Champs privés

        private string _name;
        private string _email;
        private string _machineHash;
        private string _company;

        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            _name = string.Empty;
            _company = string.Empty;
            _email = string.Empty;
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override Task OnLoading()
        {
            UpdateLicenseStatus();

            MachineHash = ProductLicenseManager.Current.GetMachineHash();

            var userInfoProvider = new Security.Activation.Providers.UserInformationProvider();
            Name = userInfoProvider.Username;
            Company = userInfoProvider.Company;
            Email = userInfoProvider.Email;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            Status = "Licensed expired";
            LicenseStatus = LicenseStatus.Expired;
            return Task.CompletedTask;
        }

        #endregion

        #region Propriétés

        private string _status;
        /// <summary>
        /// Obtient le statut de l'activation.
        /// </summary>
        public string Status
        {
            get { return _status; }
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        private LicenseStatus _licenseStatus;
        /// <summary>
        /// Obtient le statut de l'activation sour forme d'énumération.
        /// </summary>
        public LicenseStatus LicenseStatus
        {
            get { return _licenseStatus; }
            private set
            {
                if (_licenseStatus != value)
                {
                    _licenseStatus = value;
                    OnPropertyChanged("LicenseStatus");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le nom.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                    SaveUserInfo();
                    UpdateLicenseStatus();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la société.
        /// </summary>
        public string Company
        {
            get { return _company; }
            set
            {
                if (_company != value)
                {
                    _company = value;
                    OnPropertyChanged("Company");
                    SaveUserInfo();
                    UpdateLicenseStatus();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit l'email.
        /// </summary>
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged("Email");
                    SaveUserInfo();
                    UpdateLicenseStatus();
                }
            }
        }

        /// <summary>
        /// Obtient le hash de la machine.
        /// </summary>
        public string MachineHash
        {
            get { return _machineHash; }
            private set
            {
                if (_machineHash != value)
                {
                    _machineHash = value;
                    OnPropertyChanged("MachineHash");
                }
            }
        }

        #endregion

        #region Commandes

        private Command _askKeyEmailCommand;
        /// <summary>
        /// Obtient la commande permettant de demander une clé par email.
        /// </summary>
        public ICommand AskKeyEmailCommand
        {
            get
            {
                if (_askKeyEmailCommand == null)
                    _askKeyEmailCommand = new Command(() =>
                    {
                        SaveUserInfo();

                        string body = BuildKeyRequestContent();

                        string mailto = new Maito
                        {
                            To = new string[] { ActivationConstants.KProcessEmail },
                            Subject = LocalizationManager.GetString("View_ActivationView_KeyEmailSubject"),
                            Body = body,
                        }.ToString();

                        try
                        {
                            System.Diagnostics.Process.Start(mailto);
                        }
                        catch (Exception e)
                        {
                            this.TraceInfo(e.Message, e);
                            try
                            {
                                // Sauvegarder dans un fichier temporaire et l'ouvrir
                                string filename = System.IO.Path.GetTempFileName() + ".txt";
                                System.IO.File.WriteAllText(filename, body);
                                System.Diagnostics.Process.Start(filename);
                            }
                            catch (Exception ex)
                            {
                                this.TraceInfo(ex.Message, ex);
                                SendContentToClipboard(body);
                            }

                        }
                    });
                return _askKeyEmailCommand;
            }
        }

        private Command _askKeyClipboardCommand;
        /// <summary>
        /// Obtient la commande permettant de demander une clé en passant pas le presse papier.
        /// </summary>
        public ICommand AskKeyClipboardCommand
        {
            get
            {
                if (_askKeyClipboardCommand == null)
                    _askKeyClipboardCommand = new Command(() =>
                    {
                        SaveUserInfo();

                        string body = BuildKeyRequestContent();
                        SendContentToClipboard(body);

                    });
                return _askKeyClipboardCommand;
            }
        }

        private Command _importKeyCommand;
        /// <summary>
        /// Obtient la commande permettant d'importer une clé.
        /// </summary>
        public ICommand ImportKeyCommand
        {
            get
            {
                if (_importKeyCommand == null)
                    _importKeyCommand = new Command(() =>
                    {
                        SaveUserInfo();

                        string[] filePathes = DialogFactory.GetDialogView<IOpenFileDialog>()
                            .Show(string.Empty,
                            ActivationConstants.DefaultKeyExtension,
                            filter: ActivationConstants.DefaultKeyFilter);

                        if (filePathes != null && filePathes.Any())
                        {
                            string filePath = filePathes.First();

                            ProductLicenseInfo licenseInfo = null;
                            try
                            {
                                using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
                                {
                                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ProductLicenseInfo));
                                    licenseInfo = (ProductLicenseInfo)serializer.Deserialize(reader);
                                }
                            }
                            catch (Exception e)
                            {
                                this.TraceError(e, e.Message);
                                DialogFactory.GetDialogView<IErrorDialog>().Show(
                                    LocalizationManager.GetString("View_ActivationView_Message_ErrorWhileOpeningKeyFile"),
                                    LocalizationManager.GetString("Common_Error"), e);
                                return;
                            }

                            try
                            {
                                ProductLicense license = ProductLicenseManager.Current.ActivateProduct(licenseInfo);
                                ProductLicenseManager.Current.SaveLicense(ActivationConstants.ProductName, license);
                                UpdateLicenseStatus();
                            }
                            catch (Exception e)
                            {
                                this.TraceError(e, e.Message);
                                DialogFactory.GetDialogView<IErrorDialog>().Show(
                                    LocalizationManager.GetString("View_ActivationView_Message_ErrorWhileActivating"),
                                    LocalizationManager.GetString("Common_Error"), e);
                                return;
                            }
                        }
                    });
                return _importKeyCommand;
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Met à jour le statut de la license
        /// </summary>
        private void UpdateLicenseStatus()
        {
            var status = GetLicenseStatus();
            this.Status = status.Status;
            this.LicenseStatus = status.LicenseStatus;
        }

        /// <summary>
        /// Obtient les informations formattées de la licence.
        /// </summary>
        internal static (string Status, LicenseStatus LicenseStatus) GetLicenseStatus()
        {
            var licenseInfo = ProductLicenseManager.Current.LoadWebLicense(ActivationConstants.WebProductName);
            Security.SecurityContext.CurrentProductLicense = licenseInfo;

            string status;

            switch ((LicenseStatus)licenseInfo.Status)
            {
                case LicenseStatus.Licensed:
                    status = LocalizationManager.GetString("View_ActivationView_Status_Licenced");
                    break;
                case LicenseStatus.TrialVersion:
                    status = string.Format(LocalizationManager.GetString("View_ActivationView_Status_Trial"), licenseInfo.TrialDaysLeft);
                    break;
                case LicenseStatus.Expired:
                    status = LocalizationManager.GetString("View_ActivationView_Status_Expired");
                    break;
                case LicenseStatus.MachineHashMismatch:
                    status = LocalizationManager.GetString("View_ActivationView_Status_Invalid");
                    break;
                case LicenseStatus.NotFound:
                    status = LocalizationManager.GetString("View_ActivationView_Status_NoLicence");
                    break;
                case LicenseStatus.Invalid:
                    status = LocalizationManager.GetString("View_ActivationView_Status_Invalid");
                    break;
                case LicenseStatus.InternalError:
                    status = LocalizationManager.GetString("View_ActivationView_Status_Invalid");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("licenseInfo.Status");
            }

            TraceManager.TraceDebug(licenseInfo.Status.ToString());
            TraceManager.TraceDebug(licenseInfo.StatusReason);

            return (status, (LicenseStatus)licenseInfo.Status);
        }

        /// <summary>
        /// Enregistrer les infos utilisateurs
        /// </summary>
        private void SaveUserInfo()
        {
            var userProvider = new KProcess.Ksmed.Security.Activation.Providers.UserInformationProvider();
            if (userProvider.Username != this.Name ||
                userProvider.Company != this.Company ||
                userProvider.Email != this.Email)
            {
                userProvider.SetUserInformation(this.Name, this.Company, this.Email);
            }
        }

        /// <summary>
        /// Envoie le contenu spécifié dans le presse papiers;
        /// </summary>
        /// <param name="content">Le contenu</param>
        private void SendContentToClipboard(string content)
        {
            System.Windows.Clipboard.SetText(content);
            base.DialogFactory.GetDialogView<IMessageDialog>().Show(
                LocalizationManager.GetString("View_ActivationView_Message_BodySentToClipboard"),
                string.Empty,
                image: MessageDialogImage.Information);
        }

        /// <summary>
        /// Crée le contenu pour la demande de clé.
        /// </summary>
        /// <returns>Le contenu pour la demande de clé.</returns>
        private string BuildKeyRequestContent()
        {
            var sys = base.ServiceBus.Get<ISystemInformationService>().GetBasicInformation();

            var systemInfoStr = string.Format(@"{0}: {1},
{2}: {3},
{4}: {5},
{6}: {7},
{8}: {9},
{10}: {11},
{12}: {13},
{14}: {15}",
           "Machine name", sys.MachineName,
           "Operating System", string.Format("{0} {1} {2}", sys.OperatingSystem, sys.OperatingSystemArchitecture, sys.OperatingSystemVersion),
           "Language", sys.OperatingSystemLanguage,
           "System Manufacturer", sys.Manufacturer,
           "System Model", sys.Model,
           "Processor(s)", sys.Processors != null ? string.Join("|", sys.Processors) : null,
           "Memory", string.Format("{0} MB", (int)(sys.Memory / 1048576d)),
           "Available OS Memory", string.Format("{0} MB", (int)(sys.OSVisibleMemory / 1024d))
           );

            if (sys.VideoControllers != null)
            {
                foreach (var vc in sys.VideoControllers)
                    if (!string.IsNullOrEmpty(vc.Resolution))
                    {
                        systemInfoStr += string.Format("\r\n{0}: {1}, {2}", "Video controller", vc.Name, vc.Resolution);
                    }
            }

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var productLicense = Security.SecurityContext.CurrentProductLicense;

            var content = string.Format(LocalizationManager.GetString("View_ActivationView_KeyEmailBody"),
                           this.Name,
                           this.Company,
                           this.Email,
                           this.MachineHash,
                           //On n'affiche plus le statut TrialVersion, on Licensed à la place.
                           string.Format("{0} - {1}", (int)productLicense.Status == (int)LicenseStatus.TrialVersion ? LicenseStatus.Licensed.ToString() : productLicense.Status.ToString(), productLicense.StatusReason),
                           version,
                           systemInfoStr
                           );

            return content;
        }

        #endregion

    }
}