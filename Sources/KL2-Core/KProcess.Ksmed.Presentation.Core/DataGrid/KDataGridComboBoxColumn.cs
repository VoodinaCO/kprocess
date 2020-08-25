using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente une <see cref="DataGridComboBoxColumn"/> stylée KP.
    /// </summary>
    public class KDataGridComboBoxColumn : DataGridComboBoxColumn
    {
        /// <summary>
        /// Initialise la classe <see cref="KDataGridComboBoxColumn"/>.
        /// </summary>
        static KDataGridComboBoxColumn()
        {
            DataGridComboBoxColumn.EditingElementStyleProperty.OverrideMetadata(
                typeof(KDataGridComboBoxColumn), new FrameworkPropertyMetadata(Application.Current.FindResource("DataGridComboBoxColumnDefaultEditingElementStyle")));
        }
    }
}
