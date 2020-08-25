using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using System.Windows.Threading;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Permet le data binding avec les SelectedItems d'une ListBox.
    /// </summary>
    [Description("Permet le data binding avec les SelectedItems d'une ListBox.")]
    public abstract class MultiSelectionBehaviorBase : Behavior<Selector>
    {
        private bool _isSyncing = false;

        /// <summary>
        /// Obtient ou définit les SelectedItems.
        /// </summary>
        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="SelectedItems"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelectionBehaviorBase),
            new UIPropertyMetadata(null, OnSelectedItemsChanged));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance <see cref="SelectedItems"/> a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (MultiSelectionBehaviorBase)d;

            var oldCollection = e.OldValue as INotifyCollectionChanged;
            if (oldCollection != null)
                oldCollection.CollectionChanged -= source.OnSelectedItemsChanged;

            var collection = e.NewValue as INotifyCollectionChanged;
            if (collection != null)
                collection.CollectionChanged += source.OnSelectedItemsChanged;

            var list = (IList)e.NewValue;
            source.Sync(true);
        }

        /// <summary>
        /// Appelé après que le Behavior ait été attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.SelectionChanged += new SelectionChangedEventHandler(OnAssociatedObjectSelectionChanged);

            Sync(true);
        }

        /// <summary>
        /// Appelé lorsque le Behavior est en train d'être détaché de l'AssociatedObject.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.SelectionChanged -= new SelectionChangedEventHandler(OnAssociatedObjectSelectionChanged);

            var collection = this.SelectedItems as INotifyCollectionChanged;
            if (collection != null)
                collection.CollectionChanged -= OnSelectedItemsChanged;
        }

        /// <summary>
        /// Appelé lorsque les SelectedItems ont changés sur cette instance.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Sync(true);
        }

        /// <summary>
        /// Appelé lorsque les SelectedItems ont changés sur l'objet associé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnAssociatedObjectSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Sync(false);
        }

        private DispatcherOperation _syncOp;

        /// <summary>
        /// Synchronise les sélections.
        /// </summary>
        /// <param name="sourceIsThis"><c>true</c> si la synchro provient des <see cref="SelectedItems"/> de cette instance.</param>
        private void Sync(bool sourceIsThis)
        {

            if (!DesignMode.IsInDesignMode)
            {
                if (_isSyncing)
                {
                    if (_syncOp != null && !sourceIsThis)
                        _syncOp.Abort();
                    else
                        return;
                }

                _isSyncing = true;

                if (sourceIsThis)
                {
                    _syncOp = Dispatcher.BeginInvoke((Action)(() =>
                    {
                        if (IsSelectionModeSingle())
                        {
                            if (this.SelectedItems != null)
                                this.AssociatedObject.SelectedItem = this.SelectedItems.Cast<object>().FirstOrDefault();
                            else
                                this.AssociatedObject.SelectedItem = null;
                        }
                        else
                            Sync(this.SelectedItems, AssociatedObjectSelectedItems);

                        _isSyncing = false;
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                }
                else
                {
                    Sync(AssociatedObjectSelectedItems, this.SelectedItems);
                    _isSyncing = false;
                }

            }
        }

        /// <summary>
        /// Détermine si le mode de sélection est unitaire.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si le mode de sélection est unitaire; sinon, <c>false</c>.
        /// </returns>
        protected abstract bool IsSelectionModeSingle();

        /// <summary>
        /// Obtient les éléments sélectionnés.
        /// </summary>
        protected abstract IList AssociatedObjectSelectedItems { get; }

        /// <summary>
        /// Synchronise la source et la cible spécifiés.
        /// </summary>
        /// <param name="source">La source.</param>
        /// <param name="target">La cible.</param>
        private void Sync(IList source, IList target)
        {
            if (target != null)
            {
                target.Clear();
                if (source != null)
                    foreach (var item in source)
                    {
                        if (target.IndexOf(item) == -1)
                            target.Add(item);
                    }
            }
        }

    }
}
