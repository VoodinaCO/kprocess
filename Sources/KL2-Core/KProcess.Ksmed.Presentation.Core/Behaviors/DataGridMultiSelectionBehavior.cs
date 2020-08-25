using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Permet le data binding avec les SelectedItems d'un DataGrid.
    /// </summary>
    [Description("Permet le data binding avec les SelectedItems d'une ListBox.")]
    public class DataGridMultiSelectionBehavior : MultiSelectionBehaviorBase
    {

        /// <summary>
        /// Obtient le DataGrid associée.
        /// </summary>
        private DataGrid AssociatedDataGrid
        {
            get { return (DataGrid)base.AssociatedObject; }
        }

        /// <summary>
        /// Appelé après que le Behavior ait été attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (!(base.AssociatedObject is DataGrid))
            {
                throw new InvalidOperationException("Ce behavior ne peut être attaché qu'a un DataGrid");
            }
        }

        /// <summary>
        /// Détermine si le mode de sélection est unitaire.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si le mode de sélection est unitaire; sinon, <c>false</c>.
        /// </returns>
        protected override bool IsSelectionModeSingle()
        {
            return false;
        }

        /// <summary>
        /// Obtient les éléments sélectionnés.
        /// </summary>
        protected override IList AssociatedObjectSelectedItems
        {
            get { return AssociatedDataGrid.SelectedItems; }
        }


    }
}
