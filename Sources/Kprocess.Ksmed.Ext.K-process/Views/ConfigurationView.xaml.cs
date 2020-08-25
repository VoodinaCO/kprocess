using KProcess.Presentation.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KProcess.Ksmed.Ext.Kprocess.Views
{
    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    [ViewExport(typeof(ViewModels.Interfaces.IConfigurationViewModel))]
    public partial class ConfigurationView : UserControl, IView
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int result = 0;
            e.Handled = (!int.TryParse(e.Text, out result));
        }
    }
}
