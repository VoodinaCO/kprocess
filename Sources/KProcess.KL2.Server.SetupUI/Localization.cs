using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.KL2.Server.SetupUI
{
    public static class LocalizationExt
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged([CallerMemberName] string propertyName = null) =>
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));

        public static string GetLocalizedValue(this Languages lang, string key) =>
            Dictionary[CurrentLanguage.ToCultureInfoString()]?[key];

        private static Languages _CurrentLanguage = Languages.Français;
        public static Languages CurrentLanguage
        {
            get { return _CurrentLanguage; }
            set
            {
                if (_CurrentLanguage != value)
                {
                    _CurrentLanguage = value;
                    RaiseStaticPropertyChanged();
                }
            }
        }

        // Settable string
        private static string _ProductName = "KL²® Suite";
        public static string ProductName
        {
            get { return _ProductName; }
            set
            {
                if (_ProductName != value)
                {
                    _ProductName = value;
                    RaiseStaticPropertyChanged();
                }
            }
        }

        private static string _ProductVersion = "0.0.0.0";
        public static string ProductVersion
        {
            get { return $"v{ToCompactVersionString(Version.Parse(_ProductVersion))}"; }
            set
            {
                if (_ProductVersion != value)
                {
                    _ProductVersion = value;
                    RaiseStaticPropertyChanged();
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

        public static BindingList<Languages> AvailableLanguages { get; private set; } = new BindingList<Languages>(new Languages[2] { Languages.Français, Languages.English });

        public static Dictionary<string, Dictionary<string, string>> Dictionary = new Dictionary<string, Dictionary<string, string>>()
        {
            { Languages.Français.ToCultureInfoString(), new Dictionary<string, string>(){
                { "Yes", "Oui" },
                { "No", "Non" },
                { "Ok", "Ok"},
                { "CancelAskMessage", "Êtes-vous sûr de vouloir annuler l'installation de {0} {1} ?" },
                { "IsRunningMessage", "Pour continuer l'installation, veuillez fermer l'application KL² Server."},
                { "Retry", "REESSAYER"},
                { "Title", "Installation du serveur de {0}" },
                { "UninstallTitle", "Désinstallation de {0}" },
                { "UpdateTitle", "Mise à jour de {0}" },
                { "RepairTitle", "Réparation de {0}" },
                { "Welcome", "Bienvenue"},
                { "License", "Licence"},
                { "Connection", "Connexion aux composants"},
                { "InstallPath", "Paramétrage des services"},
                { "Summary", "Résumé"},
                { "InstallationProgress", "Progression de l'installation"},
                { "Finish", "Installation terminée" },
                { "BackButton", "PRECEDENT"},
                { "NextButton", "SUIVANT"},
                { "CancelButton", "ANNULER"},
                { "InstallButton", "INSTALLER"},
                { "UpdateButton", "METTRE A JOUR"},
                { "RepairButton", "REPARER"},
                { "UninstallButton", "DESINSTALLER"},
                { "ExitButton", "QUITTER"},
                { "SaveLogButton", "Enregistrer les fichiers de log" },
                { "LogDialogUsername", "Nom" },
                { "LogDialogCompany", "Société" },
                { "LogDialogEmail", "Courriel" },
                { "LogProgressDialogTitle", "Envoi du rapport" },
                { "LogProgressDialogInProgress", "En cours..." },
                { "LogProgressDialogSuccess", "Rapport envoyé avec succès." },
                { "LogProgressDialogFailure", "Une erreur s'est produite lors de l'envoi du rapport." },
                { "WelcomeTitle", "Bienvenue dans l'assistant d'installation du serveur de {0}" },
                { "WelcomeText", "L'assistant d'installation va installer le serveur de {0} sur l'ordinateur. Cliquer sur Suivant pour continuer, ou sur Annuler pour quitter l'assistant d'installation." },
                { "WelcomeLanguage", "Langue :" },
                { "LicenseAgreement_Title", "Avertissement à l'Installateur" },
                { "LicenseAgreement_Description", "Lire attentivement les informations suivantes :" },
                { "LicenseAgreement_LicenseAcceptedCheckBox", "J'accepte les termes de cet avertissement." },
                { "LicenseAgreement_SendReportCheckBox", "Envoyer automatiquement les rapports d'erreur détaillés au support K-process." },
                { "LicenseTooltip", "Veuillez accepter les termes pour continuer" },
                { "DatabaseConnectionTitle", "Connexion à la base de données :" },
                { "FileConnectionTitle", "Connexion au serveur de fichiers :" },
                { "CancelMessageBoxButton", "Annuler"},
                { "TestConnectionButton", "Tester les connexions" },
                { "TestConnectionTooltip", "Veuillez tester les connexions pour continuer"},
                { "Required", "Requis"},
                { "Optional", "Optionnel"},
                { "ServerLabel", "Adresse IP :" },
                { "PortLabel", "Port :" },
                { "UserLabel", "Nom d'utilisateur :" },
                { "PasswordLabel", "Mot de passe :" },
                { "PublishedFilesLabel", "Emplacement des fichiers publiés :" },
                { "UploadedFilesLabel", "Emplacement des fichiers uploadés :" },
                { "ServerWatermark", "ex : 10.0.0.4" },
                { "PortWatermark", "ex : 22" },
                { "UserWatermark", "Veuillez entrer le nom d'utilisateur" },
                { "PasswordWatermark", "Veuillez entrer le mot de passe" },
                { "SFtp_PublishedFilesWatermark", "ex : /PublishedFiles" },
                { "SFtp_UploadedFilesWatermark", "ex : /UploadedFiles" },
                { "Local_PublishedFilesWatermark", @"ex : C:\K-process\KL² Suite\LocalStorage\PublishedFiles" },
                { "Local_UploadedFilesWatermark", @"ex : C:\K-process\KL² Suite\LocalStorage\UploadedFiles" },
                { "DataSourceLabel", "Source de données :" },
                { "DataSourceWatermark", @"ex : .\kl2test" },
                { "FileServerTypeLabel", "Type de stockage :" },
                { "SFtpStorageName", "SFTP" },
                { "LocalStorageName", "Local" },
                { "FileConnectionWatermark", "ex : https://www.kl2.com:8082" },
                { "ConnectionsTestTitle", "Test de connexion"},
                { "DatabaseConnectionTestMessage", "Tentative de connexion à la base de données..."},
                { "FileConnectionTestMessage", "Tentative de connexion au serveur de fichier..."},
                { "ConnectionsErrorTitle", "Erreur de connexion"},
                { "CantConnectToDatabaseMessage", "Impossible de se connecter à la base de données."},
                { "CantGetDatabaseVersionMessage", "Impossible de récupérer la version de la base de données."},
                { "InvalidDatabaseVersionMessage", "La version de la base de données est invalide."},
                { "CantConnectToSFTPMessage", "Impossible de se connecter au serveur SFTP."},
                { "SFTP_PublishedFilesDoesntExistMessage", "Le chemin d'accès aux fichiers publiés est invalide."},
                { "SFTP_UploadedFilesDoesntExistMessage", "Le chemin d'accès aux fichiers uploadés est invalide."},
                { "UnableConnectToMessage", "Impossible de se connecter à l'adresse '{0}'."},
                { "WebParametersTitle", "Services web" },
                { "WebUrlLabel", "URL d'accès au serveur {0} :" },
                { "WebUrlWatermark", "https://www.kl2.com" },
                { "NotificationParametersTitle", "Service de notification" },
                { "IntervalNotificationLabel", "Intervalle de vérification (en minutes) :" },
                { "IntervalNotificationWatermark", "Veuillez entrer un nombre" },
                { "KL2LocationHeader", $"Emplacement de {_ProductName}" },
                { "SelectFolder", "Sélectionner le dossier" },
                { "ShortcutsHeader", "Raccourcis" },
                { "EditButton", "Modifier" },
                { "Shortcut_DesktopLabel", "Placer un raccourci sur le bureau" },
                { "Shortcut_StartMenuLabel", "Créer un raccourci dans le menu démarré" },
                { "Summary_KL2Components", $"Composants de {_ProductName}" },
                { "Summary_KL2InstallPath", _ProductName + " sera installé dans \"{0}\"." },
                { "Summary_KL2API", "KL² API sera installé sur le port 8081." },
                { "Summary_KL2FileServer", "KL² File Server sera installé sur le port 8082." },
                { "Summary_KL2FileServer_SFTP_Provider", "KL² File Server utilisera le stockage sur SFTP." },
                { "Summary_KL2FileServer_Local_Provider", "KL² File Server utilisera le stockage local." },
                { "Summary_KL2Notification", "KL² Notification sera installé avec une vérification toutes les {0} minutes." },
                { "Summary_KL2WebAdmin", "KL² Web Services sera accessible sur le port 8080." },
                { "Summary_KL2APILocation", "KL² API sera accessible à l'adresse '{0}'." },
                { "Summary_KL2FileServerLocation", "KL² File Server sera accessible à l'adresse '{0}'." },
                { "Summary_KL2DesktopShortcut", "Un raccouci sera placé sur le bureau." },
                { "Summary_KL2StartMenuShortcut", "Un raccourci sera créé dans le menu démarré." },
                { "Finish_Success", "L'installation s'est correctement terminée." },
                { "Finish_Canceled", "L'installation a été annulée." },
                { "Finish_Failed", "L'installation a échoué.\nVeuillez contacter votre administrateur et/ou envoyer un rapport à nos équipes en cliquant sur \"Envoyer les fichiers de log\"." },
                { "Finish_Thanks", $"Merci d'avoir installé {_ProductName}." },
                { "Finish_LaunchKL2OnExit", $"Ouvrir KL² Web Services à la sortie" },
                { "UpdateText", "L'assistant d'installation va mettre à jour {0} {1} sur l'ordinateur. Cliquer sur Mettre à jour pour continuer, ou sur Annuler pour quitter l'assistant d'installation." },
                { "RepairText", "L'assistant d'installation va réparer {0} {1} sur l'ordinateur. Cliquer sur Réparer pour continuer, ou sur Annuler pour quitter l'assistant d'installation." },
                { "UninstallText", "L'assistant d'installation va supprimer {0} {1} de l'ordinateur. Cliquer sur Désinstaller pour continuer, ou sur Annuler pour quitter l'assistant d'installation." },
                { "UpdateFinish_Success", "La mise à jour s'est correctement terminée." },
                { "UpdateFinish_Failed", "La mise à jour a échoué.\nVeuillez contacter votre administrateur et/ou envoyer un rapport à nos équipes en cliquant sur \"Envoyer les fichiers de log\"." },
                { "RepairFinish_Success", "La réparation s'est correctement terminée." },
                { "RepairFinish_Failed", "La réparation a échoué.\nVeuillez contacter votre administrateur et/ou envoyer un rapport à nos équipes en cliquant sur \"Envoyer les fichiers de log\"." },
                { "UninstallFinish_Success", "La suppression s'est correctement terminée." },
                { "UninstallFinish_Failed", "La suppression a échoué.\nVeuillez contacter votre administrateur et/ou envoyer un rapport à nos équipes en cliquant sur \"Envoyer les fichiers de log\"." },
                { "UninstallFinish_Thanks", $"Merci d'avoir utilisé {_ProductName}." } } },
            { Languages.English.ToCultureInfoString(), new Dictionary<string, string>(){
                { "Yes", "Yes" },
                { "No", "No" },
                { "Ok", "Ok"},
                { "CancelAskMessage", "Are you sure you want to cancel {0} {1} installation ?" },
                { "IsRunningMessage", "To continue the installation, please close opened KL² Server application."},
                { "Retry", "RETRY"},
                { "Title", "{0} server setup" },
                { "UninstallTitle", "{0} Uninstall" },
                { "UpdateTitle", "{0} Update" },
                { "RepairTitle", "{0} Repair" },
                { "Welcome", "Welcome"},
                { "License", "License"},
                { "Connection", "Connection to the components"},
                { "InstallPath", "Services settings"},
                { "Summary", "Summary"},
                { "InstallationProgress", "Installation progress"},
                { "Finish", "Installation finished" },
                { "BackButton", "BACK"},
                { "NextButton", "NEXT"},
                { "CancelButton", "CANCEL"},
                { "InstallButton", "INSTALL"},
                { "UpdateButton", "UPDATE"},
                { "RepairButton", "REPAIR"},
                { "UninstallButton", "UNINSTALL"},
                { "ExitButton", "EXIT"},
                { "SaveLogButton", "Save log files" },
                { "LogDialogUsername", "Username" },
                { "LogDialogCompany", "Company" },
                { "LogDialogEmail", "Email" },
                { "LogProgressDialogTitle", "Log sending" },
                { "LogProgressDialogInProgress", "In progress..." },
                { "LogProgressDialogSuccess", "Logs sent successfully." },
                { "LogProgressDialogFailure", "An error occured while sending logs." },
                { "WelcomeTitle", "Welcome to the {0} server setup wizard" },
                { "WelcomeText", "The setup wizard will install {0} server on your computer. Click Next to continue or Cancel to exit the setup wizard." },
                { "WelcomeLanguage", "Language :" },
                { "LicenseAgreement_Title", "Warning for the Installer" },
                { "LicenseAgreement_Description", "Please read the following warning carefully :"},
                { "LicenseAgreement_LicenseAcceptedCheckBox", "I accept the terms in this warning." },
                { "LicenseAgreement_SendReportCheckBox", "Automatically send detailed error reports to K-process support." },
                { "LicenseTooltip", "Please accept terms to continue" },
                { "DatabaseConnectionTitle", "Connection to database :" },
                { "FileConnectionTitle", "Connection to files server :" },
                { "CancelMessageBoxButton", "Cancel"},
                { "TestConnectionButton", "Test connections" },
                { "TestConnectionTooltip", "Please test connections to continue"},
                { "Required", "Required"},
                { "Optional", "Optional"},
                { "ServerLabel", "IP address :" },
                { "PortLabel", "Port :" },
                { "UserLabel", "Username :" },
                { "PasswordLabel", "Password :" },
                { "PublishedFilesLabel", "Published files directory :" },
                { "UploadedFilesLabel", "Uploaded files directory :" },
                { "ServerWatermark", "ex : 10.0.0.4" },
                { "PortWatermark", "ex : 22" },
                { "UserWatermark", "Please type the username" },
                { "PasswordWatermark", "Please type the password" },
                { "SFtp_PublishedFilesWatermark", "ex : /PublishedFiles" },
                { "SFtp_UploadedFilesWatermark", "ex : /UploadedFiles" },
                { "Local_PublishedFilesWatermark", @"ex : C:\K-process\KL² Suite\LocalStorage\PublishedFiles" },
                { "Local_UploadedFilesWatermark", @"ex : C:\K-process\KL² Suite\LocalStorage\UploadedFiles" },
                { "DataSourceLabel", "Data source :" },
                { "DataSourceWatermark", @"ex : .\kl2test" },
                { "FileServerTypeLabel", "Storage type :" },
                { "SFtpStorageName", "SFTP" },
                { "LocalStorageName", "Local" },
                { "FileConnectionWatermark", "ex : https://www.kl2.com:8082" },
                { "ConnectionsTestTitle", "Connection test"},
                { "DatabaseConnectionTestMessage", "Attempting to connect to database..."},
                { "FileConnectionTestMessage", "Attempting to connect to file server..."},
                { "ConnectionsErrorTitle", "Connection error"},
                { "CantConnectToDatabaseMessage", "Can't connect to database."},
                { "CantGetDatabaseVersionMessage", "Can't get database version."},
                { "InvalidDatabaseVersionMessage", "Database version is invalid."},
                { "CantConnectToSFTPMessage", "Can't connect to SFTP."},
                { "SFTP_PublishedFilesDoesntExistMessage", "Published files directory is invalid."},
                { "SFTP_UploadedFilesDoesntExistMessage", "Uploaded files directory is invalid."},
                { "UnableConnectToMessage", "Unable connect to the address '{0}'."},
                { "WebParametersTitle", "Web services" },
                { "WebUrlLabel", "{0} server URL :" },
                { "WebUrlWatermark", "https://www.kl2.com" },
                { "NotificationParametersTitle", "Notification service" },
                { "IntervalNotificationLabel", "Check interval (in minutes) :" },
                { "IntervalNotificationWatermark", "Please type a number" },
                { "KL2LocationHeader", $"{_ProductName} installation path" },
                { "SelectFolder", "Select folder" },
                { "ShortcutsHeader", "Shortcuts" },
                { "EditButton", "Edit" },
                { "Shortcut_DesktopLabel", "Place a shortcut on the desktop" },
                { "Shortcut_StartMenuLabel", "Create a shortcut in the start menu" },
                { "Summary_KL2Components", $"{_ProductName} components" },
                { "Summary_KL2InstallPath", _ProductName + " will be installed on \"{0}\"." },
                { "Summary_KL2API", "KL² API will be installed on 8081 port." },
                { "Summary_KL2FileServer", "KL² File Server will be installed on 8082 port." },
                { "Summary_KL2FileServer_SFTP_Provider", "KL² File Server will use a SFTP provider." },
                { "Summary_KL2FileServer_Local_Provider", "KL² File Server will use a local provider." },
                { "Summary_KL2Notification", "KL² Notification will be installed with a check interval of {0} minutes." },
                { "Summary_KL2WebAdmin", "KL² Web Services will be available on 8080 port." },
                { "Summary_KL2APILocation", "KL² API will be available at '{0}'." },
                { "Summary_KL2FileServerLocation", "KL² File Server will be available at '{0}'." },
                { "Summary_KL2DesktopShortcut", "A shortcut will be placed on the desktop." },
                { "Summary_KL2StartMenuShortcut", "A shortcut will be created in the start menu." },
                { "Finish_Success", "Installation completed successfully." },
                { "Finish_Canceled", "Installation was canceled." },
                { "Finish_Failed", "Installation failed.\nPlease contact your administrator and/or send a report to our teams by clicking \"Send log files\"." },
                { "Finish_Thanks", $"Thank you for installing {_ProductName}." },
                { "Finish_LaunchKL2OnExit", $"Launch KL² Web Services on exit" },
                { "UpdateText", "The setup wizard will update {0} {1} on your computer. Click Update to continue or Cancel to exit the setup wizard." },
                { "RepairText", "The setup wizard will repair {0} {1} on your computer. Click Repair to continue or Cancel to exit the setup wizard." },
                { "UninstallText", "The setup wizard will remove {0} {1} from your computer. Click Uninstall to continue or Cancel to exit the setup wizard." },
                { "UpdateFinish_Success", "Update completed successfully." },
                { "UpdateFinish_Failed", "Update failed.\nPlease contact your administrator and/or send a report to our teams by clicking \"Send log files\"." },
                { "RepairFinish_Success", "Repair completed successfully." },
                { "RepairFinish_Failed", "Repair failed.\nPlease contact your administrator and/or send a report to our teams by clicking \"Send log files\"." },
                { "UninstallFinish_Success", "Uninstallation completed successfully." },
                { "UninstallFinish_Failed", "Uninstallation failed.\nPlease contact your administrator and/or send a report to our teams by clicking \"Send log files\"." },
                { "UninstallFinish_Thanks", $"Thank you for using {_ProductName}." } } }
        };
    }

    [MarkupExtensionReturnType(typeof(object))]
    public class Localization : MarkupExtension
    {
        [ConstructorArgument("key")]
        public string Key { get; set; }

        public Localization() { }
        public Localization(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (pvt == null)
                return null;

            var targetObject = pvt.TargetObject as DependencyObject;
            if (targetObject == null)
                return null;

            var targetProperty = pvt.TargetProperty as DependencyProperty;
            if (targetProperty == null)
                return null;

            var binding = new MultiBinding
            {
                Converter = new LocalizationConverter(),
                ConverterCulture = CultureInfo.GetCultureInfo(LocalizationExt.CurrentLanguage.ToCultureInfoString()),
                ConverterParameter = Key
            };
            binding.Bindings.Add(new Binding { Path = new PropertyPath("(0)", typeof(LocalizationExt).GetProperty("CurrentLanguage")) });

            var expression = BindingOperations.SetBinding(targetObject, targetProperty, binding);

            return binding.ProvideValue(serviceProvider);
        }
    }
}
