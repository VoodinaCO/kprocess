using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    public class StackedItemsGridBehavior : StackedItemsBehaviorBase<DataGrid>
    {
        private int? _firstColumnsCount;

        /// <summary>
        /// Obtient ou définit la clé de la ressource permettant de récupérer le template des cellule.
        /// </summary>
        public string CellTemplateResourceKey { get; set; }

        /// <summary>
        /// Obtient ou définit le template des entêtes.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="HeaderTemplate"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(StackedItemsGridBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les lignes et les colonnes doivent être inversées.
        /// </summary>
        public bool Reverse
        {
            get { return (bool)GetValue(ReverseProperty); }
            set { SetValue(ReverseProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Reverse"/>.
        /// </summary>
        public static readonly DependencyProperty ReverseProperty =
            DependencyProperty.Register("Reverse", typeof(bool), typeof(StackedItemsGridBehavior), new UIPropertyMetadata(false));

        /// <summary>
        /// Obtient ou définit les instances qui identifient la ligne de total.
        /// </summary>
        public object[] TotalItems
        {
            get { return (object[])GetValue(TotalItemsProperty); }
            set { SetValue(TotalItemsProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="TotalItems"/>.
        /// </summary>
        public static readonly DependencyProperty TotalItemsProperty =
            DependencyProperty.Register("TotalItems", typeof(object), typeof(StackedItemsGridBehavior), new UIPropertyMetadata(OnTotalItemPropertyChanged));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="TotalItemsProperty"/> a changé.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnTotalItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (StackedItemsGridBehavior)d;
            source.OnLoaded();
        }

        /// <summary>
        /// Appelé afin de créer les éléments.
        /// </summary>
        /// <param name="count">Le nombre d'enfants.</param>
        /// <param name="firstItem">Le première élément.</param>
        protected override void CreateItems(int count, object firstItem)
        {
            if (_firstColumnsCount == null)
                _firstColumnsCount = base.AssociatedObject.Columns.Count;

            base.AssociatedObject.ItemsSource = null;

            var currentCount = base.AssociatedObject.Columns.Count;
            for (int i = _firstColumnsCount.Value; i < currentCount; i++)
            {
                base.AssociatedObject.Columns.RemoveAt(_firstColumnsCount.Value);
            }

            if (Reverse)
                GenerateReverse(count, firstItem);
            else
                Generate(count);
        }

        /// <summary>
        /// Applique les données.
        /// </summary>
        /// <param name="count">Le nombre d'enfants.</param>
        private void Generate(int count)
        {
            if (count == 0)
                return;

            bool hasTotalRow = TotalItems != null;

            // Inverser la structure des données
            var rows = new object[count + (hasTotalRow ? 1 : 0)][];

            var helper = new BindingHelper();
            int ii = 0;
            var itemsSourceCount = this.ItemsSource.OfType<object>().Count();
            foreach (var scenario in this.ItemsSource)
            {
                var items = (IEnumerable)helper.EvaluateBinding(ItemsBinding, scenario);

                int jj = hasTotalRow ? 1 : 0;
                foreach (var item in items)
                {
                    if (rows[jj] == null)
                        rows[jj] = new object[itemsSourceCount];

                    rows[jj][ii] = item;
                    jj++;
                }
                ii++;
            }

            ii = 0;
            if (hasTotalRow && itemsSourceCount != 0 && rows[1].Length > 0)
            {
                rows[0] = new object[itemsSourceCount];

                foreach (var scenario in this.TotalItems)
                {
                    var item = ((IEnumerable)helper.EvaluateBinding(ItemsBinding, scenario))
                        .OfType<object>()
                        .First();

                    rows[0][ii] = item;
                    ii++;
                }
            }

            int index = 0;
            foreach (var item in this.ItemsSource)
            {
                string templateXaml =
                    @"<DataTemplate 
                                        xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                                        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                        <ContentControl x:Name='LayoutRoot' Content='{Binding %%BINDING%%}' ContentTemplate='{DynamicResource %%CellTemplateResourceKey%%}' />
                                    </DataTemplate>";

                templateXaml = templateXaml.Replace("%%BINDING%%", string.Format("[{0}]", index));
                templateXaml = templateXaml.Replace("%%CellTemplateResourceKey%%", this.CellTemplateResourceKey);

                DataTemplate template;
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(templateXaml)))
                {
                    template = (DataTemplate)XamlReader.Load(stream);
                }

                if (IndependentValueBinding != null)
                {
                    var column = new DataGridTemplateColumn()
                    {
                        Header = new BindingHelper().EvaluateBinding(base.IndependentValueBinding, item),
                        HeaderTemplate = this.HeaderTemplate,
                        CellTemplate = template,
                        Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    };

                    Interaction.GetBehaviors(column).Add(new ExportFormatBehavior() 
                    {
                        Binding = new Binding(string.Format("[{0}].DurationAndPercentageFormatted", index)),
                    });

                    base.AssociatedObject.Columns.Add(column);
                }

                index++;
            }

            base.AssociatedObject.ItemsSource = rows;
        }

        /// <summary>
        /// Applique les données.
        /// </summary>
        /// <param name="count">Le nombre d'enfants.</param>
        /// <param name="firstItem">Le premier élément.</param>
        private void GenerateReverse(int count, object firstItem)
        {
            for (int i = 0; i < count; i++)
            {
                // Hard coded template is never meant to be changed and avoids the 
                // need for generic.xaml.
                string templateXaml =
                    @"<DataTemplate 
                        xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                        <ContentControl x:Name='LayoutRoot' Content='{Binding %%BINDING%%}' ContentTemplate='{DynamicResource %%CellTemplateResourceKey%%}' />
                    </DataTemplate>";

                templateXaml = templateXaml.Replace("%%BINDING%%", base.CreateItemBindingPath(i));
                templateXaml = templateXaml.Replace("%%CellTemplateResourceKey%%", this.CellTemplateResourceKey);

                DataTemplate template;
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(templateXaml)))
                {
                    template = (DataTemplate)XamlReader.Load(stream);
                }

                var column = new DataGridTemplateColumn()
                {
                    Header = new BindingHelper().EvaluateBinding(base.CreateItemBinding(i), firstItem),
                    HeaderTemplate = this.HeaderTemplate,
                    CellTemplate = template,
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                };

                Interaction.GetBehaviors(column).Add(new ExportFormatBehavior() { Binding = base.CreateItemBinding(i) });

                base.AssociatedObject.Columns.Add(column);

                base.AssociatedObject.ItemsSource = this.ItemsSource;
            }
        }
    }
}
