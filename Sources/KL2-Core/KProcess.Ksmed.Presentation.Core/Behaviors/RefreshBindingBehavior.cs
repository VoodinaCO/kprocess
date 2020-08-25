using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Permet de déclarer un binding qui se met automatiquement à jour après que la valeur ait été définie dans la source.
    /// Cela permet notamment de modifier la valeur dans le setter de la propriété et de voir le changement sur l'UI.
    /// </summary>
    public class RefreshBindingBehavior : Behavior<DependencyObject>
    {

        private bool _isBound;
        private IValueBag _bag;
        private bool _refreshingBinding;
        private DependencyProperty _targetDp;

        /// <summary>
        /// Obtient ou définit le nom de la propriété de dépendance sur l'élément attaché.
        /// Ne pas ajouter le suffixe "Property".
        /// </summary>
        public string DependencyPropertyName
        {
            get { return (string)GetValue(DependencyPropertyNameProperty); }
            set { SetValue(DependencyPropertyNameProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="DependencyPropertyName"/>.
        /// </summary>
        public static readonly DependencyProperty DependencyPropertyNameProperty =
            DependencyProperty.Register("DependencyPropertyName", typeof(string), typeof(RefreshBindingBehavior),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit la valeur.
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
            DependencyProperty.Register("Value", typeof(object), typeof(RefreshBindingBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        /// <summary>
        /// Appelé lorsque la valeur de la propriété de dépendance <see cref="Value"/> a changé.
        /// </summary>
        /// <param name="d">L'instance où la propriété a changé.</param>
        /// <param name="e">Les arguments de l'évènement <see cref="System.Windows.DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (RefreshBindingBehavior)d;
            source.RefreshBinding();
        }

        /// <summary>
        /// Obtient ou définit le type de la valeur.
        /// </summary>
        public Type ValueType
        {
            get { return (Type)GetValue(ValueTypeProperty); }
            set { SetValue(ValueTypeProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ValueType"/>.
        /// </summary>
        public static readonly DependencyProperty ValueTypeProperty =
            DependencyProperty.Register("ValueType", typeof(Type), typeof(RefreshBindingBehavior),
            new UIPropertyMetadata(null));

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();

            _targetDp = GetTargetDp();
            RefreshBinding();
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_bag != null)
                _bag.PropertyChanged -= _bag_PropertyChanged;

            BindingOperations.ClearBinding(base.AssociatedObject, _targetDp);
        }

        /// <summary>
        /// Rafraichit les bindings.
        /// </summary>
        private void RefreshBinding()
        {
            if (base.AssociatedObject == null)
                return;

            if (!_isBound)
            {
                var thisBinding = BindingOperations.GetBinding(this, ValueProperty);
                if (thisBinding != null)
                {
                    if (ValueType == null)
                        throw new InvalidOperationException("Définir le ValueType");

                    _bag = CreateBag(ValueType);
                    var binding = new Binding("Value")
                    {
                        Mode = BindingMode.TwoWay,
                        Source = _bag,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    };

                    BindingOperations.SetBinding(base.AssociatedObject, _targetDp, binding);

                    _bag.PropertyChanged += _bag_PropertyChanged;

                    _isBound = true;
                }
            }

            var hasValueChanged = (_bag.InternalValue == null ^ this.Value == null) || (_bag.InternalValue != null && this.Value != null && !_bag.InternalValue.Equals(this.Value));

            if (_isBound && !_refreshingBinding && hasValueChanged)
            {
                _refreshingBinding = true;

                _bag.InternalValue = null;
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    _refreshingBinding = true;
                    _bag.InternalValue = this.Value;
                    _refreshingBinding = false;
                }), System.Windows.Threading.DispatcherPriority.Loaded);

                _refreshingBinding = false;
            }
        }

        void _bag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_refreshingBinding)
                this.Value = _bag.InternalValue;
        }

        private IValueBag CreateBag(Type valueType)
        {
            var vbType = typeof(ValueBag<>);
            var generic = vbType.MakeGenericType(valueType);
            return (IValueBag)Activator.CreateInstance(generic);
        }

        /// <summary>
        /// Obtient la propriété de dépendance sur l'AssociatedObject par réflexion.
        /// </summary>
        /// <returns>La propriété de dépendance.</returns>
        private DependencyProperty GetTargetDp()
        {
            var type = base.AssociatedObject.GetType();

            var dp = type.GetField(this.DependencyPropertyName + "Property", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            return (DependencyProperty)dp.GetValue(null);
        }

        private interface IValueBag : INotifyPropertyChanged
        {
            object InternalValue { get; set; }
        }

        private class ValueBag<T> : IValueBag
        {
            private T _value;
            public T Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    OnPropertyChanged("Value");
                }
            }

            private void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public object InternalValue
            {
                get { return _value; }
                set { this.Value = value == null ? default(T) : (T)value; }
            }
        }

    }
}
