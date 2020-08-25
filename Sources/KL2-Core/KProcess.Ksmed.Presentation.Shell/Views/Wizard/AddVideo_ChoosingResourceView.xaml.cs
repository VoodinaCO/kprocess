using Syncfusion.Windows.Tools.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Shell.Views.Wizard
{
    /// <summary>
    /// Logique d'interaction pour AddVideo_ChoosingResourceView.xaml
    /// </summary>
    public partial class AddVideo_ChoosingResourceView : WizardPage, IGotFocus
    {
        public AddVideo_ChoosingResourceView()
        {
            InitializeComponent();
        }

        async Task IGotFocus.GotFocus()
        {
            var listBoxItem = (ListBoxItem)resourceViewsList.ItemContainerGenerator.ContainerFromItem(resourceViewsList.SelectedItem);
            while (!listBoxItem.IsFocused)
            {
                listBoxItem.Focus();
                await Task.Delay(TimeSpan.FromMilliseconds(10));
            }
        }
    }
}
