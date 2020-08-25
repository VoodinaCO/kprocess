using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente une <see cref="DataGridCheckBoxColumn"/> stylée KP.
    /// </summary>
    public class KDataGridCheckBoxColumn : DataGridCheckBoxColumn
    {

        /// <summary>
        /// Initialise la classe <see cref="KDataGridCheckBoxColumn"/>.
        /// </summary>
        static KDataGridCheckBoxColumn()
        {
            DataGridBoundColumn.ElementStyleProperty.OverrideMetadata(
                typeof(KDataGridCheckBoxColumn), new FrameworkPropertyMetadata(Application.Current.FindResource("DataGridCheckBoxColumnDefaultElementStyle")));
            DataGridBoundColumn.EditingElementStyleProperty.OverrideMetadata(
                typeof(KDataGridCheckBoxColumn), new FrameworkPropertyMetadata(Application.Current.FindResource("DataGridCheckBoxColumnDefaultEditingElementStyle")));
        }


        /// <summary>
        /// Obtient ou définit une valeur indiquant si la validation de la cellule doit avoir lieu juste après la modification de sa valeur.
        /// </summary>
        public bool ImmediateCommit
        {
            get { return (bool)GetValue(ImmediateCommitProperty); }
            set { SetValue(ImmediateCommitProperty, value); }
        }

        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ImmediateCommit"/>.
        /// </summary>
        public static readonly DependencyProperty ImmediateCommitProperty =
            DependencyProperty.Register("ImmediateCommit", typeof(bool), typeof(KDataGridCheckBoxColumn), new PropertyMetadata(false));



        /// <inheritdoc />
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            if (ImmediateCommit)
            {
                var cb = editingElement as CheckBox;
                if (cb != null)
                    cb.Checked += cb_Checked;
            }

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        /// <inheritdoc />
        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            if (ImmediateCommit)
            {
                var cb = editingElement as CheckBox;
                if (cb != null)
                    cb.Checked -= cb_Checked;
            }

            base.CancelCellEdit(editingElement, uneditedValue);
        }

        /// <inheritdoc />
        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            if (ImmediateCommit)
            {
                var cb = editingElement as CheckBox;
                if (cb != null)
                    cb.Checked -= cb_Checked;
            }

            return base.CommitCellEdit(editingElement);
        }

        /// <summary>
        /// Appelé lorsque la CheckBox est cochée ou décochée.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les données de l'évènement.</param>
        private void cb_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;

            var dataGrid = VisualTreeHelperExtensions.TryFindParent<DataGrid>(cb);
            if (dataGrid != null)
                Dispatcher.BeginInvoke((Action<DataGrid>)(dg => dg.CommitEdit(DataGridEditingUnit.Row, true)), dataGrid);
        }

    }
}
