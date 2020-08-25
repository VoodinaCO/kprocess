using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Behavior permettant d'avoir accès au mot de passe d'une PasswordBox.
    /// </summary>
    [System.ComponentModel.Description("Behavior permettant d'avoir accès au mot de passe d'une PasswordBox.")]
    public class PasswordBoxBindingBehavior : Behavior<PasswordBox>
    {
        /// <summary>
        /// Obtient ou définit le mot de passe.
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Password"/>.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordBoxBindingBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordChanged));

        /// <summary>
        /// Appellé lorsque la valeur de la propriété de dépendance <see cref="Password"/> a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance sur lequel la propriété a changé.</param>
        /// <param name="e">L'instance de <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données d'évènement.</param>
        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (PasswordBoxBindingBehavior)d;
            string newValue = (e.NewValue as string) ?? string.Empty;

            if (source.AssociatedObject.Password != newValue)
                source.AssociatedObject.Password = newValue;
        }

        /// <summary>
        /// Obtient une valeur indiquant si le mot de passe est valide.
        /// </summary>
        /// <param name="obj">L'objet attaché..</param>
        /// <returns><c>true</c> si le mot de passe est valide.</returns>
        /// <remarks>Ne devrait pas être utilisé explicitement en dehors du behavior.</remarks>
        public static bool GetIsValid(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsValidProperty);
        }

        /// <summary>
        /// Définit une valeur indiquant si le mot de passe est valide.
        /// </summary>
        /// <param name="obj">L'objet attaché..</param>
        /// <param name="value"><c>true</c> si le mot de passe est valide.</param>
        /// <remarks>Ne devrait pas être utilisé explicitement en dehors du behavior.</remarks>
        public static void SetIsValid(DependencyObject obj, bool value)
        {
            obj.SetValue(IsValidProperty, value);
        }

        /// <summary>
        /// Identifie la propriété de dépendance IsValid.
        /// </summary>
        /// <remarks>Ne devrait pas être utilisé explicitement en dehors du behavior.</remarks>
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.RegisterAttached("IsValid", typeof(bool), typeof(PasswordBoxBindingBehavior),
            new UIPropertyMetadata(true, OnIsvalidChanged));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance Isvalid a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnIsvalidChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (PasswordBox)d;
            var exp = BindingOperations.GetBindingExpression(source, IsValidProperty);
            if (exp != null)
            {
                exp.UpdateTarget();
            }

        }

        /// <summary>
        /// Appelé après que le Behavior ait été attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            // Créer un binding entre (Validation.HasError) de cet objet et (PasswordBoxBindingBehavior.IsValid) de l'associated object
            // IsValid est utile ici car en lui injectant les erreurs de validation, la notification des erreurs sera automatique
            // La notification des erreurs ne fonctionnera automatique que sur une propriété attachée à l'objet et pas un behavior
            var b = new Binding("(Validation.HasError)")
            {
                Source = this,
            };
            b.ValidationRules.Add(new PasswordValidationRule(this));
            BindingOperations.SetBinding(base.AssociatedObject, IsValidProperty, b);

            base.AssociatedObject.PasswordChanged += new RoutedEventHandler(AssociatedObject_PasswordChanged);
            if (this.Password != null)
                base.AssociatedObject.Password = this.Password;
        }

        /// <summary>
        /// Appelé lorsque le Behavior est en train d'être détaché de l'AssociatedObject.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            BindingOperations.ClearBinding(base.AssociatedObject, IsValidProperty);
            base.AssociatedObject.PasswordChanged -= new RoutedEventHandler(AssociatedObject_PasswordChanged);
        }

        /// <summary>
        /// Traite l'évènement PasswordChanged de l'élément associé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.Password = base.AssociatedObject.Password;
        }

        /// <summary>
        /// Représente une règle de validation qui qui renvoie simplement les erreurs du behavior.
        /// </summary>
        private class PasswordValidationRule : ValidationRule
        {
            private PasswordBoxBindingBehavior _behavior;

            /// <summary>
            /// Initialise une nouvelle instance de la classe <see cref="PasswordValidationRule"/>.
            /// </summary>
            /// <param name="b">Le behavior associé.</param>
            public PasswordValidationRule(PasswordBoxBindingBehavior b)
            {
                _behavior = b;
                this.ValidatesOnTargetUpdated = true;
            }

            /// <summary>
            /// Valide l'objet.
            /// </summary>
            /// <param name="value">La valeur du binding.</param>
            /// <param name="cultureInfo">la culture utilisée.</param>
            /// <returns>
            /// Un <see cref="T:System.Windows.Controls.ValidationResult"/>.
            /// </returns>
            public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
            {
                if (Validation.GetHasError(_behavior))
                {
                    var validationMessage = Validation.GetErrors(_behavior)[0].ErrorContent;
                    return new ValidationResult(false, validationMessage);
                }
                else
                    return ValidationResult.ValidResult;
            }
        }

    }

}
