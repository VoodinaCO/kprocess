using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DlhSoft.Windows.Controls
{
    partial class GanttChartDataGrid
    {
        /// <summary>
        /// Obtient ou définit une valeur permettant de désactiver la coercion du Timing des parents.
        /// </summary>
        public bool DisableParentTimingCoercion
        {
            get { return (bool)GetValue(DisableParentTimingCoercionProperty); }
            set { SetValue(DisableParentTimingCoercionProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="DisableParentTimingCoercion"/>.
        /// </summary>
        public static readonly DependencyProperty DisableParentTimingCoercionProperty =
            DependencyProperty.Register("DisableParentTimingCoercion", typeof(bool), typeof(GanttChartDataGrid), new UIPropertyMetadata(true));

        #region Gestion de l'affichage des liens

        public static Visibility GetDependencyLinesVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(DependencyLinesVisibilityProperty);
        }

        public static void SetDependencyLinesVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(DependencyLinesVisibilityProperty, value);
        }

        public static readonly DependencyProperty DependencyLinesVisibilityProperty =
            DependencyProperty.RegisterAttached("DependencyLinesVisibility", typeof(Visibility), typeof(GanttChartDataGrid),
            new FrameworkPropertyMetadata(Visibility.Collapsed, FrameworkPropertyMetadataOptions.Inherits));

        #endregion
    }
}
