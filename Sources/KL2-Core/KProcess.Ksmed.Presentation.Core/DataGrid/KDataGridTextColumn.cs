using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using KProcess.Ksmed.Presentation.Core.Behaviors;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente une <see cref="DataGridTextColumn"/> stylée KP.
    /// </summary>
    public class KDataGridTextColumn : DataGridTextColumn
    {
        /// <summary>
        /// Initialise la classe <see cref="KDataGridCheckBoxColumn"/>.
        /// </summary>
        static KDataGridTextColumn()
        {
            DataGridBoundColumn.ElementStyleProperty.OverrideMetadata(
                typeof(KDataGridTextColumn), new FrameworkPropertyMetadata(Application.Current.FindResource("DataGridTextColumnDefaultElementStyle")));
            DataGridBoundColumn.EditingElementStyleProperty.OverrideMetadata(
                typeof(KDataGridTextColumn), new FrameworkPropertyMetadata(Application.Current.FindResource("DataGridTextColumnDefaultEditingElementStyle")));
        }

        /// <inheritdoc />
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var tb = (TextBlock)base.GenerateElement(cell, dataItem);

            TextBoxAutoTooltip.Attach(tb, cell);

            return tb;
        }


    }
}
