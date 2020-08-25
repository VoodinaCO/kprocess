using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Active l'édition automatique d'une cellule.
    /// </summary>
    /// <remarks>
    /// Pour appliquer cette action sur un style, utiliser la propriété attachée.
    /// </remarks>
    [Description("Active l'édition automatique d'une cellule. Pour appliquer cette action sur un style, utiliser la propriété attachée.")]
    public class DataGridCellSingleClickEditAction : TargetedTriggerAction<DataGridCell>
    {

        /// <summary>
        /// Invoque l'action.
        /// </summary>
        /// <param name="parameter">Paramètre de l'action. Si l'action ne nécessite pas de paramètre, le paramètre peut être défini sur une référence null.</param>
        protected override void Invoke(object parameter)
        {
            DataGridCell cell = base.Target;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }

                DataGrid dataGrid = VisualTreeHelperExtensions.TryFindParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = VisualTreeHelperExtensions.TryFindParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                }

                var firstFocusable = cell.GetFirstFocusableChild();
                if (firstFocusable != null)
                    firstFocusable.Focus();
            }
        }

        #region Attached property

        /// <summary>
        /// Obtient une valeur indiquant si l'action doit être attachée au chargement de la cellule.
        /// </summary>
        /// <param name="obj">La cellule.</param>
        /// <returns>Lne valeur indiquant si l'action doit être attachée au chargement de la cellule.</returns>
        public static bool GetAttach(DataGridCell obj)
        {
            return (bool)obj.GetValue(AttachProperty);
        }
        /// <summary>
        /// Définit une valeur indiquant si l'action doit être attachée au chargement de la cellule.
        /// </summary>
        /// <param name="obj">La cellule.</param>
        /// <param name="value"><c>true</c> pour attacher automatiquement.</param>
        public static void SetAttach(DataGridCell obj, bool value)
        {
            obj.SetValue(AttachProperty, value);
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Attach"/>.
        /// </summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(DataGridCellSingleClickEditAction),
            new UIPropertyMetadata(false, OnAttachChanged));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance <see cref="Attach"/> a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cell = (DataGridCell)d;

            if (GetAttach(cell))
            {
                var triggers = Interaction.GetTriggers(cell);

                // Attacher
                var trigger = new System.Windows.Interactivity.EventTrigger("PreviewMouseLeftButtonDown");
                var action = new DataGridCellSingleClickEditAction();

                trigger.Actions.Add(action);
                triggers.Add(trigger);
            }
            else
            {
                // Détacher
                var triggers = Interaction.GetTriggers(cell);
                var eventTriggers = triggers.OfType<System.Windows.Interactivity.EventTrigger>()
                    .Where(et => et.Actions.OfType<DataGridCellSingleClickEditAction>().Any()).ToArray();

                if (eventTriggers.Any())
                {
                    foreach (var et in eventTriggers)
                    {
                        et.Detach();
                        triggers.Remove(et);
                    }
                }
            }

        }

        #endregion
    }
}
