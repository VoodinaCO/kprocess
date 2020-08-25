// -----------------------------------------------------------------------
// <copyright file="DragDropBehavior.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace KProcess.Presentation.Windows
{
#if !SILVERLIGHT
    /// <summary>
    /// Behavior permettant de découpler un drag'n drop
    /// </summary>
    [System.ComponentModel.Description("Behavior permettant de découpler un drag'n drop")]
    public class DragDropBehavior : Behavior<UIElement>
    {
        #region Définition des DPs

        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Receiver"/>.
        /// </summary>
        public static readonly DependencyProperty ReceiverProperty =
            DependencyProperty.Register("Receiver", typeof(UIElement), typeof(DragDropBehavior), new PropertyMetadata(OnReceiverPropertyChanged));

        /// <summary>
        /// Identifie la propriété de dépendance <see cref="OnDroppedCommand"/>.
        /// </summary>
        public static readonly DependencyProperty OnDroppedCommandProperty =
            DependencyProperty.Register("OnDroppedCommand", typeof(ICommand), typeof(DragDropBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Identifie la propriété de dépendance <see cref="DraggingDataSource"/>.
        /// </summary>
        public static readonly DependencyProperty DraggingDataSourceProperty =
            DependencyProperty.Register("DraggingDataSource", typeof(IList), typeof(DragDropBehavior), new PropertyMetadata(null));

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit le contrôle devant recevoir l'élément draggé
        /// </summary>
        public UIElement Receiver
        {
            get { return (UIElement)GetValue(ReceiverProperty); }
            set { SetValue(ReceiverProperty, value); }
        }

        /// <summary>
        /// Obtient ou définit le type d'élément pouvant être drag'n droppé
        /// </summary>
        public Type ItemType { get; set; }

        /// <summary>
        /// Obtient ou définit la commande exécutée lors du drop
        /// </summary>
        public ICommand OnDroppedCommand
        {
            get { return (ICommand)GetValue(OnDroppedCommandProperty); }
            set { SetValue(OnDroppedCommandProperty, value); }
        }

        /// <summary>
        /// Obtient ou définit la source de données extérieure du drag
        /// </summary>
        public IList DraggingDataSource
        {
            get { return (IList)GetValue(DraggingDataSourceProperty); }
            set { SetValue(DraggingDataSourceProperty, value); }
        }

        #endregion

        #region Surcharges

        /// <summary>
        /// Appelé après que le Behavior ait été attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            // Abonnement à l'événement détectant un drag'n drop
#if SILVERLIGHT
            this.AssociatedObject.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_PreviewMouseDown);
#else
            this.AssociatedObject.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_PreviewMouseDown);
#endif
        }

        /// <summary>
        /// Appelé lorsque le Behavior est en train d'être détaché de l'AssociatedObject.
        /// </summary>
        protected override void OnDetaching()
        {
            // Désabonnements
#if SILVERLIGHT
            Receiver.Drop -= new DragEventHandler(element_PreviewDrop);
            this.AssociatedObject.MouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_PreviewMouseDown);
#else
            Receiver.PreviewDrop -= new DragEventHandler(element_PreviewDrop);
            this.AssociatedObject.PreviewMouseDown -= new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_PreviewMouseDown);
#endif

            base.OnDetaching();
        }

        #endregion

        #region Méthodes statiques

        /// <summary>
        /// Appelé lorsque la valeur de la propriété de dépendance <see cref="ReceiverProperty"/> a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="o">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        static void OnReceiverPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs o)
        {
            var oldValue = o.OldValue as UIElement;
            var newValue = o.NewValue as UIElement;

            // Gère les abonnements
            ((DragDropBehavior)d).Setup(oldValue, newValue);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Met en place le drag &amp; drop.
        /// </summary>
        /// <param name="oldValue">L'ancien élément attaché.</param>
        /// <param name="newValue">Le nouvel élément attaché.</param>
        private void Setup(UIElement oldValue, UIElement newValue)
        {
            // Désabonnement de l'élément
            if (oldValue != null)
#if SILVERLIGHT
                oldValue.Drop -= new DragEventHandler(element_PreviewDrop);
#else
                oldValue.PreviewDrop -= new DragEventHandler(element_PreviewDrop);
#endif

            if (newValue != null)
            {
                // Abonnement à l'événement de drop
                newValue.AllowDrop = true;
#if SILVERLIGHT
                newValue.Drop += new DragEventHandler(element_PreviewDrop);
#else
                newValue.PreviewDrop += new DragEventHandler(element_PreviewDrop);
#endif
            }
        }

        /// <summary>
        /// Traite l'évènement PreviewDrop de l'élément associé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.DragEventArgs"/> contenant les données de l'évènement.</param>
        private void element_PreviewDrop(object sender, DragEventArgs e)
        {
            // Si aucune source extérieure n'est fourni, on se base sur l'élément de configuration ItemType
            var type = (DraggingDataSource != null) ? DraggingDataSource.GetType() : ItemType;

            // On exécute la commande lors du drop
            if (e.Data.GetDataPresent(type))
                OnDroppedCommand.Execute(e.Data.GetData(type));
        }

        /// <summary>
        /// Traite l'évènement PreviewMouseDown de l'élément associé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.Input.MouseButtonEventArgs"/> contenant les données de l'évènement.</param>
        private void AssociatedObject_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            object dc = ((FrameworkElement)sender).DataContext;
            
            // S'assure qu'on essaie de dragdropper le bon type
            if (dc.GetType() == ItemType)
                DragDrop.DoDragDrop(sender as DependencyObject, DraggingDataSource, DragDropEffects.Copy);
        }

        #endregion
    }
#endif
}
