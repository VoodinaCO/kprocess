using Kprocess.KL2.TabletClient.ViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace Kprocess.KL2.TabletClient.Dialog
{
    /// <summary>
    /// Interaction logic for AddSignatureOperatorFormationDialog.xaml
    /// </summary>
    public partial class AddSignatureOperatorFormationDialog : CustomDialog
    {
        public AddSignatureOperatorFormationDialog()
        {
            InitializeComponent();
        }
        
        void PasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && DataContext is QualificationViewModel vm)
                vm.ValidateCommand.Execute(null);
        }
    }
}
