using Syncfusion.Windows.Tools.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Shell.Views.Wizard
{
    /// <summary>
    /// Logique d'interaction pour AddVideo_ChoosingSync.xaml
    /// </summary>
    public partial class AddVideo_ChoosingSync : WizardPage, IGotFocus
    {
        public AddVideo_ChoosingSync()
        {
            InitializeComponent();
        }

        async Task IGotFocus.GotFocus()
        {
            var listBoxItem = (ListBoxItem)choicesList.ItemContainerGenerator.ContainerFromItem(choicesList.SelectedItem);
            while (!listBoxItem.IsFocused)
            {
                listBoxItem.Focus();
                await Task.Delay(TimeSpan.FromMilliseconds(10));
            }
        }
    }
}
