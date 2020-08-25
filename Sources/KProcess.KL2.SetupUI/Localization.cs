using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.KL2.SetupUI
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
        private static string _ProductName = "KL²® Video Analyst";
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
                { "IsRunningMessage", "Pour continuer l'installation, veuillez fermer l'application KL²."},
                { "Retry", "REESSAYER"},
                { "Title", "Installation de {0}" },
                { "UninstallTitle", "Désinstallation de {0}" },
                { "UpdateTitle", "Mise à jour de {0}" },
                { "RepairTitle", "Réparation de {0}" },
                { "Welcome", "Bienvenue"},
                { "License", "Licence"},
                { "Connection", "Connexion au serveur"},
                { "InstallPath", "Répertoire d'installation"},
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
                { "SendLogButton", "Envoyer les fichiers de log" },
                { "LogDialogUsername", "Nom" },
                { "LogDialogCompany", "Société" },
                { "LogDialogEmail", "Courriel" },
                { "LogProgressDialogTitle", "Envoi du rapport" },
                { "LogProgressDialogInProgress", "En cours..." },
                { "LogProgressDialogSuccess", "Rapport envoyé avec succès." },
                { "LogProgressDialogFailure", "Une erreur s'est produite lors de l'envoi du rapport." },
                { "WelcomeTitle", "Bienvenue dans l'assistant d'installation de {0}" },
                { "WelcomeText", "L'assistant d'installation va installer {0} sur l'ordinateur. Cliquer sur Suivant pour continuer, ou sur Annuler pour quitter l'assistant d'installation." },
                { "WelcomeLanguage", "Langue :" },
                { "LicenseAgreement_Title", "Avertissement à l'Installateur" },
                { "LicenseAgreement_Description", "Lire attentivement les informations suivantes :" },
                { "LicenseAgreement_LicenseAcceptedCheckBox", "J'accepte les termes de cet avertissement." },
                { "LicenseAgreement_SendReportCheckBox", "Envoyer automatiquement les rapports d'erreur détaillés au support K-process." },
                { "LicenseTooltip", "Veuillez accepter les termes pour continuer" },
                { "APIConnectionTitle", "Connexion à l'API :" },
                { "FileConnectionTitle", "Connexion au serveur de fichiers :" },
                { "CancelMessageBoxButton", "Annuler"},
                { "TestConnectionButton", "Tester les connexions" },
                { "TestConnectionTooltip", "Veuillez tester les connexions pour continuer"},
                { "Required", "Requis"},
                { "Optional", "Optionnel"},
                { "AddressLabel", "Adresse :" },
                { "APIConnectionWatermark", "ex : https://www.kl2.com:8081" },
                { "FileConnectionWatermark", "ex : https://www.kl2.com:8082" },
                { "ConnectionsTestTitle", "Test de connexion"},
                { "APIConnectionTestMessage", "Tentative de connexion à l'API..."},
                { "FileConnectionTestMessage", "Tentative de connexion au serveur de fichier..."},
                { "ConnectionsErrorTitle", "Erreur de connexion"},
                { "NotCorrectAPILocationMessage", "'{0}' n'est pas un service API valide."},
                { "NotCorrectFileLocationMessage", "'{0}' n'est pas un service de serveur de fichier valide."},
                { "UnableConnectToMessage", "Impossible de se connecter à l'adresse '{0}'."},
                { "KL2LocationHeader", $"Emplacement de {_ProductName}" },
                { "KL2SyncLocationHeader", $"Emplacement des fichiers de synchronisation" },
                { "SelectFolder", "Sélectionner le dossier" },
                { "UsersSelectionHeader", "Type d'installation" },
                { "UsersSelectionAllUsers", "Installer pour tous les utilisateurs (droits administrateur requis)" },
                { "UsersSelectionOnlyMe", "Installer seulement pour moi" },
                { "ShortcutsHeader", "Raccourcis" },
                { "EditButton", "Modifier" },
                { "WarningUsersRights", "Les utilisateurs doivent avoir le droit de lecture/écriture sur ce dossier" },
                { "Shortcut_DesktopLabel", "Placer un raccourci sur le bureau" },
                { "Shortcut_StartMenuLabel", "Créer un raccourci dans le menu Démarrer" },
                { "Summary_KL2Components", $"Composants de {_ProductName}" },
                { "Summary_KL2InstallPath", _ProductName + " sera installé dans \"{0}\"." },
                { "Summary_KL2SyncPath", "Les fichiers seront synchronisés dans \"{0}\"." },
                { "Summary_KL2DesktopShortcut", "Un raccouci sera placé sur le bureau." },
                { "Summary_KL2StartMenuShortcut", "Un raccourci sera créé dans le menu Démarrer." },
                { "Finish_Success", "L'installation s'est correctement terminée." },
                { "Finish_Canceled", "L'installation a été annulée." },
                { "Finish_Failed", "L'installation a échoué.\nVeuillez contacter votre administrateur et/ou envoyer un rapport à nos équipes en cliquant sur \"Envoyer les fichiers de log\"." },
                { "Finish_Thanks", $"Merci d'avoir installé {_ProductName}." },
                { "Finish_LaunchKL2OnExit", $"Ouvrir {_ProductName} à la sortie" },
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
                { "IsRunningMessage", "To continue the installation, please close opened KL² application."},
                { "Retry", "RETRY"},
                { "Title", "{0} Setup" },
                { "UninstallTitle", "{0} Uninstall" },
                { "UpdateTitle", "{0} Update" },
                { "RepairTitle", "{0} Repair" },
                { "Welcome", "Welcome"},
                { "License", "License"},
                { "Connection", "Connection to the server"},
                { "InstallPath", "Installation path"},
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
                { "SendLogButton", "Send log files" },
                { "LogDialogUsername", "Username" },
                { "LogDialogCompany", "Company" },
                { "LogDialogEmail", "Email" },
                { "LogProgressDialogTitle", "Log sending" },
                { "LogProgressDialogInProgress", "In progress..." },
                { "LogProgressDialogSuccess", "Logs sent successfully." },
                { "LogProgressDialogFailure", "An error occured while sending logs." },
                { "WelcomeTitle", "Welcome to the {0} setup wizard" },
                { "WelcomeText", "The setup wizard will install {0} on your computer. Click Next to continue or Cancel to exit the setup wizard." },
                { "WelcomeLanguage", "Language :" },
                { "LicenseAgreement_Title", "Warning for the Installer" },
                { "LicenseAgreement_Description", "Please read the following warning carefully :"},
                { "LicenseAgreement_LicenseAcceptedCheckBox", "I accept the terms in this warning." },
                { "LicenseAgreement_SendReportCheckBox", "Automatically send detailed error reports to K-process support." },
                { "LicenseTooltip", "Please accept terms to continue" },
                { "APIConnectionTitle", "Connection to API :" },
                { "FileConnectionTitle", "Connection to files server :" },
                { "CancelMessageBoxButton", "Cancel"},
                { "TestConnectionButton", "Test connections" },
                { "TestConnectionTooltip", "Please test connections to continue"},
                { "Required", "Required"},
                { "Optional", "Optional"},
                { "AddressLabel", "Location :" },
                { "APIConnectionWatermark", "ex : https://www.kl2.com:8081" },
                { "FileConnectionWatermark", "ex : https://www.kl2.com:8082" },
                { "ConnectionsTestTitle", "Connection test"},
                { "APIConnectionTestMessage", "Attempting to connect to API..."},
                { "FileConnectionTestMessage", "Attempting to connect to file server..."},
                { "ConnectionsErrorTitle", "Connection error"},
                { "NotCorrectAPILocationMessage", "'{0}' isn't a valid API service."},
                { "NotCorrectFileLocationMessage", "'{0}' isn't a valid File service."},
                { "UnableConnectToMessage", "Unable connect to the address '{0}'."},
                { "KL2LocationHeader", $"{_ProductName} installation path" },
                { "KL2SyncLocationHeader", $"Synchronization files path" },
                { "SelectFolder", "Select folder" },
                { "UsersSelectionHeader", "Installation type" },
                { "UsersSelectionAllUsers", "Install for all users (requires admin privileges)" },
                { "UsersSelectionOnlyMe", "Install for just me" },
                { "ShortcutsHeader", "Shortcuts" },
                { "EditButton", "Edit" },
                { "WarningUsersRights", "The users must have reading/writing rigths on this folder" },
                { "Shortcut_DesktopLabel", "Place a shortcut on the desktop" },
                { "Shortcut_StartMenuLabel", "Create a shortcut in the Start menu" },
                { "Summary_KL2Components", $"{_ProductName} components" },
                { "Summary_KL2InstallPath", _ProductName + " will be installed on \"{0}\"." },
                { "Summary_KL2SyncPath", "The files will be synchronized to \"{0}\"." },
                { "Summary_KL2DesktopShortcut", "A shortcut will be placed on the desktop." },
                { "Summary_KL2StartMenuShortcut", "A shortcut will be created in the Start menu." },
                { "Finish_Success", "Installation completed successfully." },
                { "Finish_Canceled", "Installation was canceled." },
                { "Finish_Failed", "Installation failed.\nPlease contact your administrator and/or send a report to our teams by clicking \"Send log files\"." },
                { "Finish_Thanks", $"Thank you for installing {_ProductName}." },
                { "Finish_LaunchKL2OnExit", $"Launch {_ProductName} on exit" },
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
