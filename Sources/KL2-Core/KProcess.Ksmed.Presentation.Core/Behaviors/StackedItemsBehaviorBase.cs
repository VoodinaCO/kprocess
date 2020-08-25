using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Représente un beheavior de base permettant de gérer les bindings en Stack.
    /// </summary>
    /// <typeparam name="T">Le type du contrôle associé.</typeparam>
    public abstract class StackedItemsBehaviorBase<T> : LoadedBehavior<T>
        where T : FrameworkElement
    {

        /// <summary>
        /// Obtient ou définit la source.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ItemsSource"/>.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(StackedItemsBehaviorBase<T>),
            new UIPropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ItemsSource"/> a changé.
        /// </summary>
        /// <param name="d">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (StackedItemsBehaviorBase<T>)d;
            source.OnLoaded();
        }

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnLoaded()
        {
            if (base.AssociatedObject.IsLoaded)
                Bind();
        }

        /// <summary>
        /// Obtient ou définit le binding permettant d'attendre la collection d'éléments.
        /// </summary>
        public Binding ItemsBinding { get; set; }

        /// <summary>
        /// Obtient ou définit le binding permettant d'atteindre le discriminateur.
        /// </summary>
        public Binding IndependentValueBinding { get; set; }

        /// <summary>
        /// Crée un binding permettant d'accéder à la donnée finale.
        /// </summary>
        /// <param name="index">L'index de la données.</param>
        /// <returns>Le binding</returns>
        protected Binding CreateItemBinding(int index)
        {
            return new Binding(CreateItemBindingPath(index));
        }

        /// <summary>
        /// Crée le chemin du binding permettant d'accéder à la donnée finale.
        /// </summary>
        /// <param name="index">L'index de la données.</param>
        /// <returns>Le chemin.</returns>
        protected virtual string CreateItemBindingPath(int index)
        {
            return ItemsBinding.Path.Path + string.Format("[{0}]", index);
        }

        /// <summary>
        /// Se bind aux données.
        /// </summary>
        private void Bind()
        {
            if (ItemsBinding == null || ItemsSource == null)
                return;

            var helper = new BindingHelper();
            int? count = null;
            object firstItem = null;

            foreach (var item in this.ItemsSource)
            {
                firstItem = item;
                if (!count.HasValue)
                {
                    var items = (IEnumerable)helper.EvaluateBinding(ItemsBinding, item);
                    count = items.OfType<Object>().Count();
                }
                break;
            }

            if (!count.HasValue)
                return;

            CreateItems(count.Value, firstItem);

        }

        /// <summary>
        /// Appelé afin de créer les éléments.
        /// </summary>
        /// <param name="count">Le nombre d'enfants.</param>
        /// <param name="firstItem">Le première élément.</param>
        protected abstract void CreateItems(int count, object firstItem);

        /// <summary>
        /// Provides an easy way to evaluate a Binding against a source instance.
        /// </summary>
        protected class BindingHelper : FrameworkElement
        {
            /// <summary>
            /// Initializes a new instance of the BindingHelper class.
            /// </summary>
            public BindingHelper()
            {
            }

            /// <summary>
            /// Identifies the Result dependency property.
            /// </summary>
            private static readonly DependencyProperty ResultProperty =
                DependencyProperty.Register("Result", typeof(object), typeof(BindingHelper), null);

            /// <summary>
            /// Evaluates a Binding against a source instance.
            /// </summary>
            /// <param name="binding">Binding to evaluate.</param>
            /// <param name="instance">Source instance.</param>
            /// <returns>Result of Binding on source instance.</returns>
            public object EvaluateBinding(Binding binding, object instance)
            {
                DataContext = instance;
                SetBinding(ResultProperty, binding);
                object result = GetValue(ResultProperty);
                ClearValue(ResultProperty);
                DataContext = null;
                return result;
            }
        }

    }
}
