using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace KProcess.Presentation.Windows
{

    /// <summary>
    /// Fournit une propriété attachée afin de pouvoir ajouter des Behaviors.
    /// </summary>
    /// <remarks>
    /// Une fois la propriété de dépendance définie dans un style, il est recommandé de ne plus la modifier. On peut cependant utiliser des triggers comme dans l'exemple ci-dessous.
    /// Pour chaque behavior ajouté dans la collection behaviors, un clone sera créé et appliqué sur le contrôle réel. Si certaines propriétés autres que des propriétés de dépendances
    /// doivent être clonées, il faudra utiliser les surcharges de clone de la classe Freezable.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// <Style x:Key="monStyle" TargetType="{x:Type ToggleButton}">
    /// 
    ///     <Setter Property="local:AttachedBehaviors.Behaviors">
    ///      <Setter.Value>
    ///        <local:AttachedBehaviorsCollection>
    ///          <local:BehaviorTruc />
    ///        </local:AttachedBehaviorsCollection>
    ///      </Setter.Value>
    ///    </Setter>
    /// 
    ///    <Style.Triggers>
    ///      <Trigger Property="IsChecked" Value="true">
    ///        <Setter Property="local:AttachedBehaviors.Behaviors">
    ///          <Setter.Value>
    ///            <local:AttachedBehaviorsCollection>
    ///              <local:BehaviorTruc />
    ///            </local:AttachedBehaviorsCollection>
    ///          </Setter.Value>
    ///        </Setter>
    ///      </Trigger>
    /// 
    ///    </Style.Triggers>
    /// </Style>
    /// 
    /// ]]>
    /// </example>
    public static class AttachedBehaviors
    {
        /// <summary>
        /// Obtient la collection de behaviors attachés.
        /// </summary>
        /// <param name="d">L'objet cible.</param>
        /// <returns>La collection de behaviors attachés.</returns>
        public static AttachedBehaviorsCollection GetBehaviors(DependencyObject d)
        {
            return (AttachedBehaviorsCollection)d.GetValue(BehaviorsProperty);
        }

        /// <summary>
        /// Définit la collection de behaviors attachés.
        /// </summary>
        /// <param name="d">L'objet cible.</param>
        /// <param name="value">La collection.</param>
        public static void SetBehaviors(DependencyObject d, AttachedBehaviorsCollection value)
        {
            d.SetValue(BehaviorsProperty, value);
        }

        /// <summary>
        /// Identifie la propriété de dépendance attachée Behaviors.
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached(
            "Behaviors", typeof(AttachedBehaviorsCollection), typeof(AttachedBehaviors),
            new FrameworkPropertyMetadata(null, OnBehaviorsChanged));

        /// <summary>
        /// Appelé lorsque la valeur de ka propriété de dépendance attachée Behaviors a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la valeur a changé.</param>
        /// <param name="e">Les données de l'évènement.</param>
        private static void OnBehaviorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (AttachedBehaviorsCollection)e.OldValue;
            var newValue = (AttachedBehaviorsCollection)e.NewValue;

            if (oldValue != newValue)
            {
                if (oldValue != null && oldValue.AssociatedObject != null)
                    oldValue.Detach();

                if ((newValue != null) && (d != null))
                {
                    if (newValue.AssociatedObject != null)
                        throw new InvalidOperationException("Impossible d'utiliser la collection plusieurs fois");
                    newValue.Attach(d);
                }
            }
        }
    }
}