using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KProcess.Globalization;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Interaction logic for ExportDialog.xaml
    /// </summary>
    public partial class ExportDialog : Window
    {
        private string _fileFilter;
        public ExportDialog(string fileFilter)
        {
            InitializeComponent();
            this.Loaded += ExportDialog_Loaded;

            _fileFilter = fileFilter;
            this.VideoPickerVisibility = System.Windows.Visibility.Collapsed;
        }

        public bool Accepts { get; set; }
        public string Filename { get; set; }
        public string VideoFolder { get; set; }
        public bool OpenWhenCreated { get; set; }
        public bool UsedToOpen { get; set; }
        public bool BrowseWhenOpened { get; set; }

        public Visibility VideoFolderVisibility
        {
            get { return this.VideoFolderTB.Visibility; }
            set
            {
                this.VideoFolderTB.Visibility = value;
                this.VideoFolderB.Visibility = value;
                this.VideoFolderTBlock.Visibility = value;
            }
        }

        public Visibility VideoPickerVisibility
        {
            get { return this.VideoPickerTBlock.Visibility; }
            set
            {
                this.VideoPickerTBlock.Visibility = value;
                this.videoPickerCB.Visibility = value;
            }
        }

        public string VideoPickerTBlockText
        {
            get { return VideoPickerTBlock.Text; }
            set { VideoPickerTBlock.Text = value; }
        }

        private void ExportDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (BrowseWhenOpened)
            {
                if (!Browse())
                {
                    this.Accepts = false;
                    this.Close();
                }
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.FilePathTB.Text))
            {
                this.Accepts = true;
                this.Filename = this.FilePathTB.Text;
                this.OpenWhenCreated = this.OpenWhenCreatedCB.IsChecked.GetValueOrDefault();
                this.VideoFolder = this.VideoFolderTB.Text;
                this.Close();
            }
            else
            {
                var dialogFactory = IoC.Resolve<IDialogFactory>();
                dialogFactory.GetDialogView<IMessageDialog>().Show(
                    LocalizationManager.GetString("ExportDialog_PleaseSetAFile"),
                    LocalizationManager.GetString("Common_Error"));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Accepts = false;
            this.Close();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            Browse();
        }

        private void BrowseVideoFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialogFactory = IoC.Resolve<IDialogFactory>();

            var folder = dialogFactory.GetDialogView<IOpenFolderDialog>().Show(string.Empty);
            if (!string.IsNullOrEmpty(folder))
                this.VideoFolderTB.Text = folder;
        }

        private bool Browse()
        {
            var dialogFactory = IoC.Resolve<IDialogFactory>();

            string file;
            if (UsedToOpen)
                file = dialogFactory.GetDialogView<IOpenFileDialog>().Show(string.Empty, filter: _fileFilter).FirstOrDefault();
            else
                file = dialogFactory.GetDialogView<ISaveFileDialog>().Show(string.Empty, filter: _fileFilter);

            if (!string.IsNullOrEmpty(file))
            {
                this.FilePathTB.Text = file;
                if (string.IsNullOrEmpty(this.VideoFolderTB.Text))
#if DEBUG
                    this.VideoFolderTB.Text = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos), "Sample Videos");
#else
                    this.VideoFolderTB.Text = System.IO.Path.GetDirectoryName(file);
#endif
                return true;
            }
            else
                return false;
        }

    }
}
