using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Interactivity;
using KProcess.Ksmed.Presentation.Core.Charting;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Permet de binder une source et faire automatiquement des Stacked Items.
    /// </summary>
    public class StackedItemsChartBehavior : StackedItemsBehaviorBase<Chart>
    {

        /// <summary>
        /// Obtient ou définit le style des DataPoints.
        /// </summary>
        public Style DataPointStyle
        {
            get { return (Style)GetValue(DataPointStyleProperty); }
            set { SetValue(DataPointStyleProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="DataPointStyle"/>.
        /// </summary>
        public static readonly DependencyProperty DataPointStyleProperty =
            DependencyProperty.Register("DataPointStyle", typeof(Style), typeof(StackedItemsChartBehavior), new UIPropertyMetadata(null));

        #region Axes

        /// <summary>
        /// Obtient ou définit le mode d'affichage des valeurs.
        /// </summary>
        public RestitutionValueMode ValueMode
        {
            get { return (RestitutionValueMode)GetValue(ValueModeProperty); }
            set { SetValue(ValueModeProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ValueMode"/>.
        /// </summary>
        public static readonly DependencyProperty ValueModeProperty =
            DependencyProperty.Register("ValueMode", typeof(RestitutionValueMode), typeof(StackedItemsChartBehavior), new UIPropertyMetadata(RestitutionValueMode.Absolute));


        #region Absolute

        /// <summary>
        /// Obtient ou définit l'axis des valeurs dépendantes dans un affichage absolu.
        /// </summary>
        public IAxis AbsoluteDependentAxis
        {
            get { return (IAxis)GetValue(AbsoluteDependentAxisProperty); }
            set { SetValue(AbsoluteDependentAxisProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="AbsoluteDependentAxis"/>.
        /// </summary>
        public static readonly DependencyProperty AbsoluteDependentAxisProperty =
            DependencyProperty.Register("AbsoluteDependentAxis", typeof(IAxis), typeof(StackedItemsChartBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit l'axis des valeurs dépendantes dans un affichage absolu.
        /// </summary>
        public IAxis AbsoluteIndependentAxis
        {
            get { return (IAxis)GetValue(AbsoluteIndependentAxisProperty); }
            set { SetValue(AbsoluteIndependentAxisProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="AbsoluteIndependentAxis"/>.
        /// </summary>
        public static readonly DependencyProperty AbsoluteIndependentAxisProperty =
            DependencyProperty.Register("AbsoluteIndependentAxis", typeof(IAxis), typeof(StackedItemsChartBehavior), new UIPropertyMetadata(null));

        #endregion

        #region Relative

        /// <summary>
        /// Obtient ou définit l'axis des valeurs dépendantes dans un affichage absolu.
        /// </summary>
        public IAxis RelativeIndependentAxis
        {
            get { return (IAxis)GetValue(RelativeIndependentAxisProperty); }
            set { SetValue(RelativeIndependentAxisProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="RelativeIndependentAxis"/>.
        /// </summary>
        public static readonly DependencyProperty RelativeIndependentAxisProperty =
            DependencyProperty.Register("RelativeIndependentAxis", typeof(IAxis), typeof(StackedItemsChartBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit l'axis des valeurs dépendantes dans un affichage absolu.
        /// </summary>
        public IAxis RelativeDependentAxis
        {
            get { return (IAxis)GetValue(RelativeDependentAxisProperty); }
            set { SetValue(RelativeDependentAxisProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="RelativeDependentAxis"/>.
        /// </summary>
        public static readonly DependencyProperty RelativeDependentAxisProperty =
            DependencyProperty.Register("RelativeDependentAxis", typeof(IAxis), typeof(StackedItemsChartBehavior), new UIPropertyMetadata(null));

        #endregion

        #region Occurence

        /// <summary>
        /// Obtient ou définit l'axis des valeurs dépendantes dans un affichage absolu.
        /// </summary>
        public IAxis OccurenceIndependentAxis
        {
            get { return (IAxis)GetValue(OccurenceIndependentAxisProperty); }
            set { SetValue(OccurenceIndependentAxisProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="OccurenceIndependentAxis"/>.
        /// </summary>
        public static readonly DependencyProperty OccurenceIndependentAxisProperty =
            DependencyProperty.Register("OccurenceIndependentAxis", typeof(IAxis), typeof(StackedItemsChartBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit l'axis des valeurs dépendantes dans un affichage absolu.
        /// </summary>
        public IAxis OccurenceDependentAxis
        {
            get { return (IAxis)GetValue(OccurenceDependentAxisProperty); }
            set { SetValue(OccurenceDependentAxisProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="OccurenceDependentAxis"/>.
        /// </summary>
        public static readonly DependencyProperty OccurenceDependentAxisProperty =
            DependencyProperty.Register("OccurenceDependentAxis", typeof(IAxis), typeof(StackedItemsChartBehavior), new UIPropertyMetadata(null));

        #endregion


        #endregion


        /// <summary>
        /// Obtient ou définit le binding pour la valeur dépendante.
        /// </summary>
        public Binding DependentValueBinding { get; set; }

        /// <summary>
        /// Crée le chemin du binding permettant d'accéder à la donnée finale.
        /// </summary>
        /// <param name="index">L'index de la données.</param>
        /// <returns>Le chemin.</returns>
        protected override string CreateItemBindingPath(int index)
        {
            var path = ItemsBinding.Path.Path + string.Format("[{0}]", index);
            if (this.DependentValueBinding != null)
                path += "." + DependentValueBinding.Path.Path;

            return path;
        }

        /// <summary>
        /// Appelé afin de créer les éléments.
        /// </summary>
        /// <param name="count">Le nombre d'enfants.</param>
        /// <param name="firstItem">Le première élément.</param>
        protected override void CreateItems(int count, object firstItem)
        {

            //      <charting:SeriesDefinition
            //ItemsSource="{Binding Occupations}"
            //IndependentValuePath="Scenario"
            //DependentValueBinding="{Binding Items[0]}"
            //DataPointStyle="{StaticResource ColumnsStyle}" />

            this.AssociatedObject.Series.Clear();

            var stackedSeries = new KStackedColumnSeries();
            switch (ValueMode)
            {
                case RestitutionValueMode.Absolute:
                    stackedSeries.DependentAxis = this.AbsoluteDependentAxis;
                    stackedSeries.IndependentAxis = this.AbsoluteIndependentAxis;
                    break;
                case RestitutionValueMode.Relative:
                    stackedSeries.DependentAxis = this.RelativeDependentAxis;
                    stackedSeries.IndependentAxis = this.RelativeIndependentAxis;
                    break;
                case RestitutionValueMode.Occurences:
                    stackedSeries.DependentAxis = this.OccurenceDependentAxis;
                    stackedSeries.IndependentAxis = this.OccurenceIndependentAxis;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("ValueMode");
            }

            for (int i = 0; i < count; i++)
            {
                var sd = new SeriesDefinition()
                {
                    ItemsSource = this.ItemsSource,
                    IndependentValueBinding = this.IndependentValueBinding,
                    DependentValueBinding = CreateItemBinding(i),
                };

                if (base.GetValue(DataPointStyleProperty) != DependencyProperty.UnsetValue)
                {
                    sd.DataPointStyle = this.DataPointStyle;
                }
                stackedSeries.SeriesDefinitions.Add(sd);
            }

            this.AssociatedObject.Series.Add(stackedSeries);
        }

    }

    /// <summary>
    /// Représente le mdoe de valeur pour un affichage dans les écrans de restition.
    /// </summary>
    public enum RestitutionValueMode
    {
        /// <summary>
        /// Values absolues.
        /// </summary>
        Absolute = 0,

        /// <summary>
        /// Valeurs relatives.
        /// </summary>
        Relative = 1,

        /// <summary>
        /// Valeurs par occurences.
        /// </summary>
        Occurences = 2
    }

}
