using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Microsoft.Expression.Interactivity;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Bascule entre deux états en fonction d’une instruction conditionnelle.
    /// </summary>
    /// <remarks>Contrairement à <see cref="Microsoft.Expression.Interactivity.Core.DataStateBehavior"/>, l'état est directement changé sur l'objet associé.</remarks>
    public class DataStateBehavior : Behavior<FrameworkElement>
    {

        #region Propriétés de dépendance

        /// <summary>
        /// Obtient ou définit la liaison qui produit la valeur de propriété de l’objet de données. Il s’agit d’une propriété de dépendance.
        /// </summary>
        public object Binding
        {
            get { return base.GetValue(BindingProperty); }
            set { base.SetValue(BindingProperty, value); }
        }

        /// <summary>
        /// Obtient ou définit la liaison qui produit la valeur de propriété de l’objet de données. Il s’agit d’une propriété de dépendance.
        /// </summary>
        public string FalseState
        {
            get { return (string)base.GetValue(FalseStateProperty); }
            set { base.SetValue(FalseStateProperty, value); }
        }

        /// <summary>
        /// Obtient ou définit le nom de l’état visuel vers lequel effectuer la transition lorsque la condition est remplie. Il s’agit d’une propriété de dépendance.
        /// </summary>
        public string TrueState
        {
            get { return (string)base.GetValue(TrueStateProperty); }
            set { base.SetValue(TrueStateProperty, value); }
        }

        /// <summary>
        /// Obtient ou définit la valeur à comparer avec la valeur de propriété de l’objet de données. Il s’agit d’une propriété de dépendance.
        /// </summary>
        public object Value
        {
            get { return base.GetValue(ValueProperty); }
            set { base.SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Binding"/>.
        /// </summary>
        public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(object), typeof(DataStateBehavior), new PropertyMetadata(new PropertyChangedCallback(DataStateBehavior.OnBindingChanged)));
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="FalseState"/>.
        /// </summary>
        public static readonly DependencyProperty FalseStateProperty = DependencyProperty.Register("FalseState", typeof(string), typeof(DataStateBehavior), new PropertyMetadata(new PropertyChangedCallback(DataStateBehavior.OnFalseStateChanged)));
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="TrueState"/>.
        /// </summary>
        public static readonly DependencyProperty TrueStateProperty = DependencyProperty.Register("TrueState", typeof(string), typeof(DataStateBehavior), new PropertyMetadata(new PropertyChangedCallback(DataStateBehavior.OnTrueStateChanged)));
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Value"/>.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DataStateBehavior), new PropertyMetadata(new PropertyChangedCallback(DataStateBehavior.OnValueChanged)));

        #endregion

        #region Surcharges

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.ValidateStateNamesDeferred();
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Evalue le binding et change d'état si nécessaire.
        /// </summary>
        private void Evaluate()
        {
            if (this.TargetObject != null)
            {
                string stateName = null;
                if (AreEqual(this.Binding, this.Value))
                {
                    stateName = this.TrueState;
                }
                else
                {
                    stateName = this.FalseState;
                }
                GoToState(this.TargetObject, stateName, true);
            }
        }

        /// <summary>
        /// Détermine l'égalité entre deux éléments.
        /// </summary>
        /// <param name="left">Opérande de gauche.</param>
        /// <param name="right">Opérande de droite.</param>
        /// <returns><c>true</c> si les éléments sont considérés comme égaux.</returns>
        private bool AreEqual(object left, object right)
        {
            IComparable comparableLeft = left as IComparable;
            IComparable comparableRight = right as IComparable;
            if (comparableLeft != null && comparableRight != null)
            {
                object rightConverted = null;
                try
                {
                    rightConverted = Convert.ChangeType(comparableRight, comparableLeft.GetType(), CultureInfo.CurrentCulture);
                }
                catch (FormatException) { }
                catch (InvalidCastException) { }

                if (rightConverted == null)
                    return false;

                return comparableLeft.CompareTo((IComparable)rightConverted) == 0;
            }

            return object.Equals(left, right);
        }

        /// <summary>
        /// Change d'état sur l'élément spécifié.
        /// </summary>
        /// <param name="element">L'element.</param>
        /// <param name="stateName">Le nom de l'état.</param>
        /// <param name="useTransitions"><c>true</c> pour utiliser les transitions.</param>
        /// <returns><c>true</c> si l'état a été changé.</returns>
        private bool GoToState(FrameworkElement element, string stateName, bool useTransitions)
        {
            bool flag = false;
            if (string.IsNullOrEmpty(stateName))
            {
                return flag;
            }
            Control control = element as Control;
            if (control != null)
            {
                control.ApplyTemplate();
                return VisualStateManager.GoToState(control, stateName, useTransitions);
            }
            return VisualStateManager.GoToElementState(element, stateName, useTransitions);
        }

        /// <summary>
        /// Détermine si l'élément spécifié est chargé.
        /// </summary>
        /// <param name="element">L'élément.</param>
        /// <returns>
        ///   <c>true</c> si l'élément spécifié est chargé; sinon, <c>false</c>.
        /// </returns>
        internal static bool IsElementLoaded(FrameworkElement element)
        {
            return element.IsLoaded;
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance <see cref="Binding"/> a changé.
        /// </summary>
        /// <param name="obj">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="args">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnBindingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((DataStateBehavior)obj).Evaluate();
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance <see cref="FalseState"/> a changé.
        /// </summary>
        /// <param name="obj">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="args">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnFalseStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataStateBehavior behavior = (DataStateBehavior)obj;
            behavior.ValidateStateName(behavior.FalseState);
            behavior.Evaluate();
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance <see cref="TrueState"/> a changé.
        /// </summary>
        /// <param name="obj">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="args">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnTrueStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataStateBehavior behavior = (DataStateBehavior)obj;
            behavior.ValidateStateName(behavior.TrueState);
            behavior.Evaluate();
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance <see cref="Value"/> a changé.
        /// </summary>
        /// <param name="obj">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="args">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((DataStateBehavior)obj).Evaluate();
        }

        /// <summary>
        /// Valide le nom de l'état spécifié.
        /// </summary>
        /// <param name="stateName">Le nom de l'état.</param>
        private void ValidateStateName(string stateName)
        {
            if ((base.AssociatedObject != null) && !string.IsNullOrEmpty(stateName))
            {
                foreach (VisualState state in this.TargetedVisualStates)
                {
                    if (stateName == state.Name)
                        return;
                }
                throw new ArgumentException(string.Format("Le nom de l'état {0} est introuvable sur le stype {1}",
                    new object[] { stateName, (this.TargetObject != null) ? this.TargetObject.GetType().Name : "null" }));
            }
        }

        /// <summary>
        /// Valide les noms des états.
        /// </summary>
        private void ValidateStateNames()
        {
            this.ValidateStateName(this.TrueState);
            this.ValidateStateName(this.FalseState);
        }

        /// <summary>
        /// Valide les noms des états, maintenant ou plus tard.
        /// </summary>
        private void ValidateStateNamesDeferred()
        {
            RoutedEventHandler handler = null;
            FrameworkElement parent = base.AssociatedObject.Parent as FrameworkElement;
            if ((parent != null) && IsElementLoaded(parent))
            {
                this.ValidateStateNames();
            }
            else
            {
                if (handler == null)
                {
                    handler = delegate(object o, RoutedEventArgs e)
                    {
                        this.ValidateStateNames();
                    };
                }
                base.AssociatedObject.Loaded += handler;
            }
        }

        #endregion

        /// <summary>
        /// Obtient les états de la cible.
        /// </summary>
        private IEnumerable<VisualState> TargetedVisualStates
        {
            get
            {
                List<VisualState> list = new List<VisualState>();
                if (this.TargetObject != null)
                {
                    foreach (VisualStateGroup group in VisualStateUtilities.GetVisualStateGroups(this.TargetObject))
                        foreach (VisualState state in group.States)
                            list.Add(state);
                }
                return list;
            }
        }

        /// <summary>
        /// Obtient la cible.
        /// </summary>
        private FrameworkElement TargetObject
        {
            get { return base.AssociatedObject; }
        }

    }
}
