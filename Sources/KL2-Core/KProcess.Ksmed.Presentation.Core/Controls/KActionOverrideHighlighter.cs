using KProcess.Business;
using KProcess.Ksmed.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Met en évidence le fait qu'une <see cref="KAction"/> ait été modifiée vis à vis de son parent
    /// </summary>
    public class KActionOverrideHighlighter: ContentControl
    {
        #region Initialization
        static KActionOverrideHighlighter()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(KActionOverrideHighlighter), new FrameworkPropertyMetadata(typeof(KActionOverrideHighlighter)));
        }
        #endregion

        #region Private dp
        private static readonly DependencyProperty _currentValueProperty = DependencyProperty.Register("_currentValue", typeof(object), typeof(KActionOverrideHighlighter), new PropertyMetadata(null, OnCurrentActionValuePropertyChanged));
        private static readonly DependencyProperty _parentValueProperty = DependencyProperty.Register("_parentValue", typeof(object), typeof(KActionOverrideHighlighter), new PropertyMetadata(OnParentValuePropertyChanged));
        private static readonly DependencyProperty _parentDisplayValueProperty = DependencyProperty.Register("_parentDisplayValue", typeof(object), typeof(KActionOverrideHighlighter), new PropertyMetadata(OnParentDisplayValuePropertyChanged));

        private static void OnParentValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = (KActionOverrideHighlighter)d;
            sender.ParentValue = e.NewValue;
        }

        private static void OnParentDisplayValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KActionOverrideHighlighter)d).ParentDisplayValue = e.NewValue;
        }
        #endregion

        #region Properties


        /// <summary>
        /// Obtient la valeur parente à afficher 
        /// </summary>
        public object ParentDisplayValue
        {
            get { return (object)GetValue(ParentDisplayValueProperty); }
            protected set { SetValue(ParentDisplayValuePropertyKey, value); }
        }

        public static readonly DependencyPropertyKey ParentDisplayValuePropertyKey =
            DependencyProperty.RegisterReadOnly("ParentDisplayValue", typeof(object), typeof(KActionOverrideHighlighter), new PropertyMetadata());
        public static readonly DependencyProperty ParentDisplayValueProperty = ParentDisplayValuePropertyKey.DependencyProperty;


        /// <summary>
        /// Obtient la valeur originale de la propriété de l'action
        /// </summary>
        public object ParentValue
        {
            get { return (object)GetValue(ParentValueProperty); }
            protected set { SetValue(ParentValuePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ParentValuePropertyKey = DependencyProperty.RegisterReadOnly("ParentValue", typeof(object), typeof(KActionOverrideHighlighter), new PropertyMetadata());
        public static readonly DependencyProperty ParentValueProperty = ParentValuePropertyKey.DependencyProperty;

        

        private static void OnCurrentActionValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KActionOverrideHighlighter)d).RefreshOverrideStatus();
        }

        /// <summary>
        /// L'action de référence
        /// </summary>
        public KAction KAction
        {
            get { return (KAction)GetValue(KActionProperty); }
            set { SetValue(KActionProperty, value); }
        }

        public static readonly DependencyProperty KActionProperty = DependencyProperty.Register("KAction", typeof(KAction), typeof(KActionOverrideHighlighter), new PropertyMetadata( OnActionPropertyChanged));


        /// <summary>
        /// La description du chemin depuis l'action vers la propriété à analyser
        /// </summary>
        public string KActionPropertyPath
        {
            get { return (string)GetValue(KActionPropertyPathProperty); }
            set { SetValue(KActionPropertyPathProperty, value); }
        }

        public static readonly DependencyProperty KActionPropertyPathProperty =
            DependencyProperty.Register("KActionPropertyPath", typeof(string), typeof(KActionOverrideHighlighter), new PropertyMetadata(null, OnKActionPropertyPathPropertyChanged));


        /// <summary>
        /// Obtient ou définit le chemin vers la propriété permettant d'afficher une valeur
        /// </summary>
        public string KActionPropertyPathDisplay
        {
            get { return (string)GetValue(KActionPropertyPathDisplayProperty); }
            set { SetValue(KActionPropertyPathDisplayProperty, value); }
        }

        public static readonly DependencyProperty KActionPropertyPathDisplayProperty = DependencyProperty.Register("KActionPropertyPathDisplay", typeof(string), typeof(KActionOverrideHighlighter), new PropertyMetadata(OnKActionPropertyPathDisplayPropertyChanged));

        /// <summary>
        /// Obtient ou définit le convertisseur à utiliser pour l'affichage de la propriété décrite par "KActionPropertyPathDisplay"
        /// </summary>
        public IValueConverter KActionDisplayConverter
        {
            get { return (IValueConverter)GetValue(KActionDisplayConverterProperty); }
            set { SetValue(KActionDisplayConverterProperty, value); }
        }

        public static readonly DependencyProperty KActionDisplayConverterProperty = DependencyProperty.Register("KActionDisplayConverter", typeof(IValueConverter), typeof(KActionOverrideHighlighter), new PropertyMetadata(OnKActionDisplayConverterPropertyPropertyChanged));

        private static void OnKActionPropertyPathDisplayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KActionOverrideHighlighter)d).HandleInputChange();
        }

        private static void OnKActionDisplayConverterPropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KActionOverrideHighlighter)d).HandleInputChange();
        }

        private static void OnActionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KActionOverrideHighlighter)d).HandleInputChange();
        }

        private static void OnKActionPropertyPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KActionOverrideHighlighter)d).HandleInputChange();
        }

        private void RefreshOverrideStatus()
        {
            if (this.KAction != null && this.KAction.Original != null)
            {
                object currentValue = GetValue(_currentValueProperty);  // Attention au boxing
                object parentValue = GetValue(_parentValueProperty);    // Attention au boxing

                if (IsConsideredAsCollection(currentValue))
                {
                    this.IsOverriden = CheckCollectionOverride(currentValue, parentValue);
                }
                else 
                {
                    this.IsOverriden = CheckVectorObjectsOverride(currentValue, parentValue);
                }
            }
            else
            {
                this.IsOverriden = false;
            }
        }

        private static bool CheckVectorObjectsOverride(object currentValue, object parentValue)
        {
            if (currentValue == null)
            {
                return parentValue != null;
            }
            else if(ActionRefComparer.CanHandle(currentValue))
            {
                return !ActionRefComparer.AreEquivalents(currentValue, parentValue);
            }

            return !currentValue.Equals(parentValue);
        }

        private static bool IsConsideredAsCollection(object currentValue)
        {
            return !(currentValue is string) && currentValue is IEnumerable;
        }

        private static bool CheckCollectionOverride(object currentValue, object parentValue)
        {
            if (parentValue == null)
            {
                return true;
            }
            else
            {
                var current = ((IEnumerable)currentValue).OfType<object>();
                var parent = ((IEnumerable)parentValue).OfType<object>();

                if (current.Count() != parent.Count())
                {
                    return true;
                }
                else
                {
                    foreach (var currentItem in current)
                    {
                        if (ActionRefComparer.CanHandle(currentItem)) // Il faudrait rendre la behavior modulaire pour gérer ce type de comportement spécifique
                        {
                            if (!parent.OfType<IReferentialActionLink>().Any(parentItem => ActionRefComparer.AreEquivalents(currentItem, parentItem)))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
            }
        }

        private void HandleInputChange()
        {
            if (this.KAction != null && this.KAction.Original != null && !string.IsNullOrWhiteSpace(this.KActionPropertyPath))
            {
                var currentAction = this.KAction;
                var parentAction = currentAction.Original;

                BindingOperations.ClearBinding(this, _currentValueProperty);
                BindingOperations.SetBinding(this, _currentValueProperty, new Binding(this.KActionPropertyPath) { Source = currentAction });
                
                BindingOperations.ClearBinding(this, _parentValueProperty);
                BindingOperations.SetBinding(this, _parentValueProperty, new Binding(this.KActionPropertyPath) { Source = parentAction });
                
                BindingOperations.ClearBinding(this, _parentDisplayValueProperty);
                BindingOperations.SetBinding(this, _parentDisplayValueProperty, 
                    this.KActionPropertyPathDisplay != null
                    ? new Binding(this.KActionPropertyPathDisplay) { Source = parentAction, Converter = this.KActionDisplayConverter }
                    : new Binding(this.KActionPropertyPath) { Source = parentAction, Converter = this.KActionDisplayConverter });
                
                RefreshOverrideStatus();
            }
        }

        /// <summary>
        /// Détermine si la propriété de la <see cref="KAction"/> a été surchargée
        /// </summary>
        public bool IsOverriden
        {
            get { return (bool)GetValue(IsOverridenProperty); }
            protected set { SetValue(IsOverridenPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsOverridenPropertyKey = DependencyProperty.RegisterReadOnly("IsOverriden", typeof(bool), typeof(KActionOverrideHighlighter), new PropertyMetadata(false));
        public static readonly DependencyProperty IsOverridenProperty = IsOverridenPropertyKey.DependencyProperty;


        #endregion

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        #endregion

        private static class ActionRefComparer
        {
            public static bool CanHandle(object itemSample) 
            {
                return itemSample is IReferentialActionLink;
            }

            public static bool AreEquivalents(object item1, object item2)
            {
                if(item1 == null)
                {
                    return item2 == null;
                }

                else
                {
                    return ((IReferentialActionLink)item1).ReferentialId == ((IReferentialActionLink)item2).ReferentialId;
                }
            }
        }
    }
}
