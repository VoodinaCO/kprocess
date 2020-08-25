using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Active la validation (commit) de la ligne lors de la sortie d'une celulle en mode édition
    /// </summary>
    public class DataGridCellCommitBehavior : Behavior<DataGrid>
    {

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(AssociatedObject_CellEditEnding);
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.CellEditEnding -= new EventHandler<DataGridCellEditEndingEventArgs>(AssociatedObject_CellEditEnding);
        }

        private bool _isManualEditCommit = false;
        void AssociatedObject_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!_isManualEditCommit)
            {
                _isManualEditCommit = true;
                this.AssociatedObject.CommitEdit(DataGridEditingUnit.Row, true);
                _isManualEditCommit = false;
            }
        }

    }
}
