using KProcess.Presentation.Windows;
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

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Interaction logic for BackupRestoreView.xaml
    /// </summary>
    [ViewExport(typeof(KProcess.Ksmed.Presentation.ViewModels.Interfaces.IBackupRestoreViewModel))]
    public partial class BackupRestoreView : UserControl, IView
    {
        public BackupRestoreView()
        {
            InitializeComponent();
        }
    }
}
