using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Presentation.Core.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Adapte une colonne permettant d'afficher un libellé personnalisé.
    /// </summary>
    /// <remarks>
    /// Copié tel quel de <see cref="KDataGridCustomLabelColumn"/>. Pour créer une composition plus adaptée qu'un héritage.
    /// </remarks>
    public class KDataGridCustomLabelColumnBehavior: Behavior<DataGridTemplateColumn>
    {
        private const string HeaderTemplateFormat = @"
<DataTemplate>
    <TextBlock Text=""{{k:UCBinding DataContext.{0}}}"" />
</DataTemplate>";

        /// <summary>
        /// Obtient ou définit l'index.
        /// </summary>
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Index"/>.
        /// </summary>
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(KDataGridCustomLabelColumnBehavior),
            new UIPropertyMetadata(0, OnIndexChanged));

        /// <summary>
        /// Obtient ou définit le type de champ.
        /// </summary>
        public CustomFieldType? FieldType
        {
            get { return (CustomFieldType?)GetValue(FieldTypeProperty); }
            set { SetValue(FieldTypeProperty, value); }
        }

        /// <summary>
        /// Identifie la propriété de dépendance <see cref="FieldType"/>.
        /// </summary>
        public static readonly DependencyProperty FieldTypeProperty =
            DependencyProperty.Register("FieldType", typeof(CustomFieldType?), typeof(KDataGridCustomLabelColumnBehavior),
            new UIPropertyMetadata((CustomFieldType?)null, OnFieldTypeChanged));

        /// <summary>
        /// Obtient ou définit le chemin vers les libellés dans le DataContext.
        /// </summary>
        public string LabelsPath
        {
            get { return (string)GetValue(LabelsPathProperty); }
            set { SetValue(LabelsPathProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="LabelsPath"/>.
        /// </summary>
        public static readonly DependencyProperty LabelsPathProperty =
            DependencyProperty.Register("LabelsPath", typeof(string), typeof(KDataGridCustomLabelColumnBehavior),
            new UIPropertyMetadata("CustomFieldsLabels"));

        protected override void OnAttached()
        {
            this.UpdateColumnDefinition();
        }

        /// <summary>
        /// Appelé lorsque la valeur de la propriété de dépendance <see cref="MyProperty"/> a changé.
        /// </summary>
        /// <param name="d">L'instance où la propriété a changé.</param>
        /// <param name="e">Les arguments de l'évènement <see cref="System.Windows.DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KDataGridCustomLabelColumnBehavior)d).UpdateColumnDefinition();
        }

        /// <summary>
        /// Appelé lorsque la valeur de la propriété de dépendance <see cref="FieldType"/> a changé.
        /// </summary>
        /// <param name="d">L'instance où la propriété a changé.</param>
        /// <param name="e">Les arguments de l'évènement <see cref="System.Windows.DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnFieldTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KDataGridCustomLabelColumnBehavior)d).UpdateColumnDefinition();
        }

        /// <summary>
        /// Met à jour la définition de la colonne.
        /// </summary>
        private void UpdateColumnDefinition()
        {
            if (this.AssociatedObject != null && this.Index > 0)
            {
                string typeText = this.FieldType.Value.ToString();

                string typePlusIndex = typeText + this.Index;
                string id = "Custom" + typePlusIndex;

                // KSmedDataGridBehavior.Id
                KSmedDataGridBehavior.SetId(this.AssociatedObject, id);

                // Binding
                //string valuePath = string.Format("Action.Custom{0}Value{1}", typeText, this.Index > 1 ? this.Index.ToString() : "");
                //KDataGridFeatures.SetBinding(this.AssociatedObject, new KDataGridBindingDescription { Value = new Binding(valuePath) });

                // HeaderTemplate
                var labelBindingPath = string.Format("{0}.{1}.LabelOrFallbackDescription", this.LabelsPath, typePlusIndex);
                this.AssociatedObject.HeaderTemplate = CreateHeaderTemplate(labelBindingPath);

                //// ExportFormatBehavior
                //var behaviors = Interaction.GetBehaviors(this.AssociatedObject);
                //if (!behaviors.IsFrozen && !behaviors.OfType<ExportFormatBehavior>().Any() && !isAttaching)
                //{
                //    Interaction.GetBehaviors(this.AssociatedObject).Add(new ExportFormatBehavior()
                //    {
                //        Binding = new Binding(valuePath),
                //    });
                //}
            }
        }

        /// <summary>
        /// Crée le DataTemplate permettant d'afficher l'entête de la colonne.
        /// </summary>
        /// <param name="labelBindingPath">le chemin vers le libellé.</param>
        /// <returns>Le DataTemplate.</returns>
        private static DataTemplate CreateHeaderTemplate(string labelBindingPath)
        {
            var context = new ParserContext();

            context.XamlTypeMapper = new XamlTypeMapper(new string[0]);

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("k", "http://schemas.kprocess.com/xaml/framework");

            var templateString = string.Format(HeaderTemplateFormat, labelBindingPath);

            var template = (DataTemplate)XamlReader.Parse(templateString, context);
            return template;
        }
    }

    /// <summary>
    /// Le type du champ.
    /// </summary>
    public enum CustomFieldType
    {
        /// <summary>
        /// Texte.
        /// </summary>
        Text,

        /// <summary>
        /// Numérique.
        /// </summary>
        Numeric,
    }
}
