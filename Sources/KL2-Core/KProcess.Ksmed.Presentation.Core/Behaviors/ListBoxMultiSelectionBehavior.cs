using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{

    /// <summary>
    /// Permet le data binding avec les SelectedItems d'une ListBox.
    /// </summary>
    [Description("Permet le data binding avec les SelectedItems d'une ListBox.")]
    public class ListBoxMultiSelectionBehavior : MultiSelectionBehaviorBase
    {

        /// <summary>
        /// Obtient la ListBox associée.
        /// </summary>
        private ListBox AssociatedListBox
        {
            get { return (ListBox)base.AssociatedObject; }
        }

        /// <summary>
        /// Appelé après que le Behavior ait été attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (!(base.AssociatedObject is ListBox))
            {
                throw new InvalidOperationException("Ce behavior ne peut être attaché qu'a une ListBox");
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
            return AssociatedListBox.SelectionMode == SelectionMode.Single;
        }

        /// <summary>
        /// Obtient les éléments sélectionnés.
        /// </summary>
        protected override IList AssociatedObjectSelectedItems 
        {
            get { return AssociatedListBox.SelectedItems; }
        }


    }
}
