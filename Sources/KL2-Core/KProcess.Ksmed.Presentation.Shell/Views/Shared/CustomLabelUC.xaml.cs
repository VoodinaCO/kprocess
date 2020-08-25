using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Interaction logic for AcquireCustomLabelUC.xaml
    /// </summary>
    public partial class CustomLabelUC : UserControl
    {
        public const string LabelSharedSizeScope = "Label";

        public CustomLabelUC()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Obtient ou définit le libellé du maxLength.
        /// </summary>
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="MaxLength"/>.
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(CustomLabelUC), new PropertyMetadata(default(int)));

        /// <summary>
        /// Obtient ou définit le libellé du champ.
        /// </summary>
        public CustomFieldLabel Label
        {
            get { return (CustomFieldLabel)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Label"/>.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(CustomFieldLabel), typeof(CustomLabelUC),
            new UIPropertyMetadata(null, OnLabelChanged));

        /// <summary>
        /// Appelé lorsque la valeur de la propriété de dépendance <see cref="Label"/> a changé.
        /// </summary>
        /// <param name="d">L'instance où la propriété a changé.</param>
        /// <param name="e">Les arguments de l'évènement <see cref="System.Windows.DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var userControl = (CustomLabelUC)d;
            userControl.UpdateTextBoxMaxLength();
            userControl.UpdateColumnDefinitions();
        }


        /// <summary>
        /// Obtient ou définit la valeur du champ.
        /// </summary>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Value"/>.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(CustomLabelUC),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Obtient ou définit une valeur indiquant la colonne contenant la valeur devrait être étendue.
        /// </summary>
        public bool ExpandedValueColumn
        {
            get { return (bool)GetValue(ExpandedValueColumnProperty); }
            set { SetValue(ExpandedValueColumnProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ExpandedValueColumn"/>.
        /// </summary>
        public static readonly DependencyProperty ExpandedValueColumnProperty =
            DependencyProperty.Register("ExpandedValueColumn", typeof(bool), typeof(CustomLabelUC),
            new UIPropertyMetadata(false));

        /// <summary>
        /// Obtient ou définit si la valeur accepte le retour à la ligne.
        /// </summary>
        public bool AcceptsReturn
        {
            get { return (bool)GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="AcceptsReturn"/>.
        /// </summary>
        public static readonly DependencyProperty AcceptsReturnProperty =
            DependencyProperty.Register(nameof(AcceptsReturn), typeof(bool), typeof(CustomLabelUC),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Met à jour la taille max du champ.
        /// </summary>
        private void UpdateTextBoxMaxLength()
        {
            if (this.Label.IsNumeric)
                this.ValueControl.MaxLength = MaxLength;
            else
                this.ValueControl.MaxLength = MaxLength > 0 ? this.MaxLength : KProcess.Ksmed.Models.Project.CustomTextLabelMaxLength; // Part du principe que tous les libellés ont la même taille max
        }

        /// <summary>
        /// Met à jour les définitions des colonnes.
        /// </summary>
        private void UpdateColumnDefinitions()
        {
            if (this.Label.IsNumeric && !this.ExpandedValueColumn)
            {
                this.LabelColumnDefinition.SharedSizeGroup = null;
                this.ValueColumnDefinition.Width = new GridLength(0, GridUnitType.Auto);
            }
            else
            {
                this.LabelColumnDefinition.SharedSizeGroup = LabelSharedSizeScope;
                this.ValueColumnDefinition.Width = new GridLength(1, GridUnitType.Star);
            }
        }
    }
}
