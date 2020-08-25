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

namespace DlhSoft.Windows.Controls
{
    /// <summary>
    /// Interaction logic for DataGridColumn.xaml
    /// </summary>
    public partial class DataTreeGridColumn : DataGridTemplateColumn
    {
        public DataTreeGridColumn()
        {
            InitializeComponent();
        }

        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            editingElement = (VisualTreeHelper.GetChildrenCount(editingElement) == 1) ? (VisualTreeHelper.GetChild(editingElement, 0) as FrameworkElement) : null;
            TextBox box = editingElement.FindName("TextBox") as TextBox;
            if (box != null)
            {
                box.Undo();
            }
            base.CancelCellEdit(editingElement, uneditedValue);
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            editingElement = (VisualTreeHelper.GetChildrenCount(editingElement) == 1) ? (VisualTreeHelper.GetChild(editingElement, 0) as FrameworkElement) : null;
            TextBox box = editingElement.FindName("TextBox") as TextBox;
            if (box != null)
            {
                box.Focus();
                box.SelectAll();
            }
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

    }
}
