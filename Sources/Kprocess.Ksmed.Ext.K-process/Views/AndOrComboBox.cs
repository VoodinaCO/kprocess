using System.Collections.Generic;
using System.Windows.Controls;

namespace KProcess.Ksmed.Ext.Kprocess.Views
{
    public class AndOrComboBox : ComboBox
    {
        private Dictionary<bool, string> BindItems = new Dictionary<bool, string>
        {
            [false] = "AND",
            [true] = "OR"
        };

        private void InitializeItems() =>
            ItemsSource = BindItems;

        public AndOrComboBox()
        {
            InitializeItems();
        }
    }
}
