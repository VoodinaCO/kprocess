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
    /// Interaction logic for DependencyLinePresenter.xaml
    /// </summary>
    public partial class DependencyLinePresenter : UserControl
    {
        public DependencyLinePresenter()
        {
            InitializeComponent();
        }

        internal void Delete()
        {
            PredecessorItem dataContext = base.DataContext as PredecessorItem;
            if (dataContext != null)
            {
                GanttChartItem dependentItem = dataContext.DependentItem;
                if (dependentItem != null)
                {
                    dependentItem.Predecessors.Remove(dataContext);
                }
            }
        }

        private void DeleteContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Delete();
        }


    }
}
