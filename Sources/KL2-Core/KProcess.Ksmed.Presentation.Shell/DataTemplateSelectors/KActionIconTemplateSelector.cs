using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Presentation.Core.Controls;
using KProcess.Ksmed.Presentation.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    public class KActionIconTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DependencyObject depObj = container;
            while (depObj.GetType() != typeof(DataTreeGrid))
            {
                depObj = VisualTreeHelper.GetParent(depObj);
                if (depObj == null)
                    return null;
            }
            if (depObj is FrameworkElement element)
            {
                if (item is ActionGridItem actionGridItem)
                {
                    if (actionGridItem.Action.LinkedProcessId != null)
                        return element.FindResource("KActionWithLinkedProcessIconTemplate") as DataTemplate;
                    return element.FindResource("KActionWithoutLinkedProcessIconTemplate") as DataTemplate;
                }
            }
            return Application.Current.FindResource("NoTemplate") as DataTemplate;
        }
    }

    public class ActionGanttItemIconTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DependencyObject depObj = container;
            while (depObj.GetType() != typeof(KGanttChartDataGrid))
            {
                depObj = VisualTreeHelper.GetParent(depObj);
                if (depObj == null)
                    return null;
            }
            if (depObj is FrameworkElement element)
            {
                if (item is ActionGanttItem actionGanttItem)
                {
                    if (actionGanttItem.Action.LinkedProcessId != null)
                        return element.FindResource("ActionGanttItemWithLinkedProcessIconTemplate") as DataTemplate;
                    return element.FindResource("ActionGanttItemWithoutLinkedProcessIconTemplate") as DataTemplate;
                }
            }
            return Application.Current.FindResource("NoTemplate") as DataTemplate;
        }
    }
}
