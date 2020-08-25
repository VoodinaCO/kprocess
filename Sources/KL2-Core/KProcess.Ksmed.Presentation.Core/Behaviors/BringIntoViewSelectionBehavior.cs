using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using DlhSoft.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Lors d'un changement de sélection, affiché l'élément sélectionné.
    /// </summary>
    public class BringIntoViewSelectionBehavior : Behavior<Selector>
    {
        private TargetControl _controlType;

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (base.AssociatedObject is ListBox)
                _controlType = TargetControl.ListBox;
            else if (base.AssociatedObject is GanttChartDataGrid)
                _controlType = TargetControl.GanttChartDataGrid;
            else if (base.AssociatedObject is DataGrid)
                _controlType = TargetControl.DataGrid;
            else
                throw new InvalidOperationException("Le type n'est pas supporté");

            base.AssociatedObject.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(AssociatedObject_SelectionChanged);
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            base.AssociatedObject.SelectionChanged -= new System.Windows.Controls.SelectionChangedEventHandler(AssociatedObject_SelectionChanged);
        }

        /// <summary>
        /// Gère l'évènement SelectionChanged.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> contenant les données de l'évènement.</param>
        private void AssociatedObject_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    switch (_controlType)
                    {
                        case TargetControl.ListBox:
                            ((ListBox)base.AssociatedObject).ScrollIntoView(e.AddedItems[0]);
                            break;
                        case TargetControl.DataGrid:
                            ((DataGrid)base.AssociatedObject).ScrollIntoView(e.AddedItems[0]);
                            break;
                        case TargetControl.GanttChartDataGrid:
                            ((GanttChartDataGrid)base.AssociatedObject).ScrollTo((GanttChartItem)e.AddedItems[0]);
                            break;
                        default:
                            break;
                    }
                }));
            }
        }

        private enum TargetControl
        {
            ListBox,
            DataGrid,
            GanttChartDataGrid
        }
    }
}
