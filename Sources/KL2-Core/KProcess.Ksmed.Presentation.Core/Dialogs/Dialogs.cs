using KProcess.Globalization;
using KProcess.Presentation.Windows;
using System;
using System.Linq;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Contient les boîtes de dialogue par défaut.
    /// </summary>
    class Dialogs : IErrorDialog, IMessageDialog, IOpenFileDialog, IOpenFolderDialog, ICommonDialog, IExportDialog, ISaveFileDialog
    {
        #region IErrorDialog

        /// <summary>
        /// Affiche un message d'erreur
        /// </summary>
        /// <param name="message">le message</param>
        /// <param name="caption">la légende du message</param>
        /// <param name="errorMessage">Le message de l'erreur.</param>
        public void Show(string message, string caption = null, string errorMessage = null)
        {
            var errorDialog = new MessageDialog()
            {
                Message = message,
                Caption = caption,
                Image = MessageDialogImage.Error,
                Buttons = MessageDialogButton.OK,
                DefaultButton = MessageDialogResult.OK,
                ReportVisibility = Visibility.Visible,
                Owner = GetCurrentActiveWindow(),
            };
            errorDialog.ShowDialog();
        }

        /// <summary>
        /// Affiche un message d'erreur
        /// </summary>
        /// <param name="message">le message</param>
        /// <param name="caption">la légende du message</param>
        /// <param name="exception">L'exception associée.</param>
        public void Show(string message, string caption, Exception exception, bool forceClose = false)
        {
            var errorDialog = new MessageDialog()
            {
                Message = message,
                Caption = caption,
                InitialException = exception,
                Image = MessageDialogImage.Error,
                Buttons = forceClose ? MessageDialogButton.CloseApp : MessageDialogButton.OK,
                DefaultButton = MessageDialogResult.OK,
                ReportVisibility = Visibility.Visible,
                Owner = GetCurrentActiveWindow(),
            };
            errorDialog.ShowDialog();
        }

        #endregion

        #region IMessageDialog

        /// <summary>
        /// Affiche un message
        /// </summary>
        /// <param name="message">le message</param>
        /// <param name="caption">la légende du message</param>
        /// <param name="buttons">les boutons proposés à l'utilisateur</param>
        /// <param name="image">l'image à afficher</param>
        /// <returns>
        /// le résultat du choix de l'utilisateur
        /// </returns>
        public MessageDialogResult Show(string message, string caption,
            MessageDialogButton buttons = MessageDialogButton.OK, MessageDialogImage image = MessageDialogImage.None)
        {
            var messageDialog = new MessageDialog()
            {
                Message = message,
                Caption = caption,
                Image = image,
                Buttons = buttons,
                Owner = GetCurrentActiveWindow(),
            };

            switch (buttons)
            {
                case MessageDialogButton.OK:
                    messageDialog.DefaultButton = MessageDialogResult.OK;
                    break;
                case MessageDialogButton.OKCancel:
                    messageDialog.DefaultButton = MessageDialogResult.OK;
                    break;
                case MessageDialogButton.YesNoCancel:
                    messageDialog.DefaultButton = MessageDialogResult.Yes;
                    break;
                case MessageDialogButton.YesNo:
                    messageDialog.DefaultButton = MessageDialogResult.Yes;
                    break;
                default:
                    break;
            }

            messageDialog.ShowDialog();
            return messageDialog.MessageDialogResult;
        }

        #endregion

        #region IOpenFileDialog Members

        /// <summary>
        /// Affiche la vue permettant de sélectionner un ou plusieurs fichiers
        /// </summary>
        /// <param name="caption">titre de la vue</param>
        /// <param name="defaultExtension">extension par défaut</param>
        /// <param name="multiSelect">indique si on peut ou non sélectionner plusieurs fichiers</param>
        /// <param name="filter">filtre de sélection</param>
        /// <param name="checkPathExists">indique si le chemin doit exister</param>
        /// <param name="checkFileExists">indique si le fichier doit exister</param>
        /// <param name="initialDirectory">Répertoire par défaut où va commencer la recherche</param>
        /// <returns>
        /// la liste des chemins complets des fichiers sélectionnés, null si aucun
        /// </returns>
        public string[] Show(string caption, string defaultExtension = "", bool multiSelect = false,
            string filter = "All files (*.*)|*.*", bool checkPathExists = true, bool checkFileExists = true, string initialDirectory="")
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Title = caption,
                DefaultExt = defaultExtension,
                Multiselect = multiSelect,
                Filter = filter,
                CheckPathExists = checkPathExists,
                CheckFileExists = checkFileExists,
                RestoreDirectory = true,
                InitialDirectory = initialDirectory
            };

            if (dialog.ShowDialog().GetValueOrDefault())
                return dialog.FileNames;
            else
                return new string[] { };
        }

        #endregion

        #region IOpenFolderDialog Members

        /// <summary>
        /// Affiche la vue permettant de sélectionner un dossier
        /// </summary>
        /// <param name="caption">titre de la vue</param>
        /// <returns>
        /// le dossier sélectionné, ou null si annulé
        /// </returns>
        public string Show(string caption)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = caption,
            })
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return dialog.SelectedPath;
                else
                    return null;
            }
        }

        #endregion

        #region ISaveFileDialog Members

        /// <summary>
        /// Affiche la vue permettant de sauvegarder un fichier
        /// </summary>
        /// <param name="caption">titre de la vue</param>
        /// <param name="defaultExtension">extension par défaut</param>
        /// <param name="filter">filtre de sélection</param>
        /// <param name="checkPathExists">indique si le chemin doit exister</param>
        /// <param name="checkFileExists">indique si le fichier doit exister</param>
        /// <returns>
        /// le chemin complet du fichier sélectionné, null si aucun
        /// </returns>
        public string Show(string caption, string defaultExtension = "", string filter = "All files (*.*)|*.*", bool checkPathExists = true, bool checkFileExists = false)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                Title = caption,
                DefaultExt = defaultExtension,
                Filter = filter,
                CheckPathExists = checkPathExists,
                CheckFileExists = checkFileExists,
                RestoreDirectory = true,
            };

            if (dialog.ShowDialog().GetValueOrDefault())
                return dialog.FileName;
            else
                return null;
        }

        #endregion

        #region IKsmedDialogs Members

        /// <summary>
        /// Affiche une boîte demandant la confirmation de suppression d'un élément.
        /// </summary>
        /// <returns>Le choix de l'utilisateur.</returns>
        public bool ShowSureToDelete()
        {
            return this.Show(
                LocalizationManager.GetString("Common_Message_SureToDelete"),
                LocalizationManager.GetString("Common_Confirm"),
                MessageDialogButton.YesNoCancel,
                MessageDialogImage.Question)
                == MessageDialogResult.Yes;
        }

        #endregion

        #region IExportDialog Members

        /// <summary>
        /// Afficher la boîte de dialogue d'export excel.
        /// </summary>
        /// <param name="format">Le format.</param>
        /// <returns>
        /// le résultat de la boite de dialogue
        /// </returns>
        public ExportResult ShowExportToExcel(ExcelFormat format)
        {
            string extension;
            switch (format)
            {
                case ExcelFormat.Xlsx:
                    extension = ".xlsx";
                    break;
                case ExcelFormat.Xlsm:
                    extension = ".xlsm";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("format");
            }

            var file = IoC.Resolve<IDialogFactory>().GetDialogView<ISaveFileDialog>().Show(string.Empty, filter: string.Format("Excel 2010 (*{0})|*{0}", extension));

            bool accepts = !string.IsNullOrEmpty(file);

            return new ExportResult()
            {
                Accepts = accepts,
                Filename = file,
                OpenWhenCreated = true,
            };
        }

        /// <summary>
        /// Afficher la boîte de dialogue d'export de projet.
        /// </summary>
        /// <returns>Le chemin vers le fichier.</returns>
        public string ExportProject()
        {
            var file = IoC.Resolve<IDialogFactory>().GetDialogView<ISaveFileDialog>().Show(string.Empty, filter: "(*.ksp)|*.ksp");

            if (!string.IsNullOrEmpty(file))
                return file;
            else
                return null;
        }

        /// <summary>
        /// Afficher la boîte de dialogue d'import de projet.
        /// </summary>
        /// <returns>Les paramètres de l'importation.</returns>
        public ImportWithVideoFolderResult ImportProject()
        {
            var dialog = new ExportDialog("(*.ksp)|*.ksp")
            {
                Owner = GetCurrentActiveWindow(),
            };
            dialog.OpenWhenCreatedCB.Visibility = Visibility.Collapsed;
            dialog.UsedToOpen = true;
            dialog.BrowseWhenOpened = true;
            dialog.ShowDialog();
            return new ImportWithVideoFolderResult()
            {
                Accepts = dialog.Accepts,
                Filename = dialog.Filename,
                VideosFolder = dialog.VideoFolder,
            };
        }

        /// <summary>
        /// Afficher la boîte de dialogue d'export de décomposition vidéo.
        /// </summary>
        /// <param name="videos">Les vidéos que l'utilisateur peut sélectionner.</param>
        /// <returns>
        /// Le résultat.
        /// </returns>
        public ExportVideoDecompositionResult ExportVideoDecomposition(Models.Video[] videos)
        {
            var dialog = new ExportDialog("(*.ksv)|*.ksv")
            {
                Owner = GetCurrentActiveWindow(),
            };

            dialog.VideoPickerVisibility = Visibility.Visible;
            dialog.videoPickerCB.ItemsSource = videos;
            dialog.videoPickerCB.DisplayMemberPath = "Name";
            dialog.videoPickerCB.SelectedIndex = 0;
            dialog.BrowseWhenOpened = true;

            dialog.OpenWhenCreatedCB.Visibility = Visibility.Collapsed;
            dialog.VideoFolderVisibility = Visibility.Collapsed;
            dialog.VideoPickerTBlockText = LocalizationManager.GetString("ExportDialog_Video");

            dialog.ShowDialog();

            if (dialog.Accepts)
                return new ExportVideoDecompositionResult()
                {
                    Filename = dialog.Filename,
                    VideoId = ((Models.Video)dialog.videoPickerCB.SelectedItem).VideoId,
                };
            else
                return null;
        }

        /// <summary>
        /// Afficher la boîte de dialogue d'import de décomposition vidéo.
        /// </summary>
        /// <returns>Les paramètres de l'importation.</returns>
        public ImportWithVideoFolderResult ImportVideoDecomposition()
        {
            var dialog = new ExportDialog("(*.ksv)|*.ksv")
            {
                Owner = GetCurrentActiveWindow(),
            };
            dialog.OpenWhenCreatedCB.Visibility = Visibility.Collapsed;
            dialog.UsedToOpen = true;
            dialog.BrowseWhenOpened = true;
            dialog.ShowDialog();
            return new ImportWithVideoFolderResult()
            {
                Accepts = dialog.Accepts,
                Filename = dialog.Filename,
                VideosFolder = dialog.VideoFolder
            };
        }


        #endregion

        /// <summary>
        /// Obtient la fenêtre active de l'application.
        /// </summary>
        /// <returns>La fenêtre active ou null.</returns>
        private Window GetCurrentActiveWindow()
        {
            return Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
        }

    }
}
