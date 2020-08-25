// -----------------------------------------------------------------------
// <copyright file="MultipleSelectionTrigger.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Trigger chargé d'externaliser la gestion de la sélection multiple (évitant de rajoutant une propriété IsSelected à un model)
    /// </summary>
    [System.ComponentModel.Description("Trigger chargé d'externaliser la gestion de la sélection multiple (évitant de rajoutant une propriété IsSelected à un model)")]
    public class MultipleSelectionTrigger : TriggerBase<ToggleButton>
    {
        #region Déclaration des DPs

        /// <summary>
        /// Identifie la propriété de dépendance <see cref="SelectedItemsSource"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsSourceProperty = DependencyProperty.Register("SelectedItemsSource", typeof(INotifyCollectionChanged), typeof(MultipleSelectionTrigger), new PropertyMetadata(OnSelectedItemsSourcePropertyChanged));

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit la source de données des éléments sélectionnés
        /// </summary>
        public INotifyCollectionChanged SelectedItemsSource
        {
            get { return (INotifyCollectionChanged)GetValue(SelectedItemsSourceProperty); }
            set { SetValue(SelectedItemsSourceProperty, value); }
        }

        #endregion

        #region Surcharges

        /// <summary>
        /// Appelé après que le Behavior ait été attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            // Abonnements
            AssociatedObject.Checked += new RoutedEventHandler(toggleButton_Checked);
            AssociatedObject.Unchecked += new RoutedEventHandler(toggleButton_Unchecked);
        }

        /// <summary>
        /// Appelé lorsque le Behavior est en train d'être détaché de l'AssociatedObject.
        /// </summary>
        protected override void OnDetaching()
        {
            // Désabonnements
            AssociatedObject.Checked -= new RoutedEventHandler(toggleButton_Checked);
            AssociatedObject.Unchecked -= new RoutedEventHandler(toggleButton_Unchecked);

            base.OnDetaching();
        }

        #endregion

        #region Méthodes statiques

        /// <summary>
        /// Appelé lorsque la valeur de la propriété de dépendance <see cref="SelectedItemsSource"/> a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="o">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnSelectedItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs o)
        {
            var handler = d as MultipleSelectionTrigger;

            if (handler != null)
                // Abonnements
                ((MultipleSelectionTrigger)d).Setup((INotifyCollectionChanged)o.OldValue, (INotifyCollectionChanged)o.NewValue);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Traite l'évènement CollectionChanged de l'élément associé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> contenant les données de l'évènement.</param>
        private void SelectedItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // L'élément est checké selon que son dataContext est contenu ou non dans la source de données lorsqu'un élément de la source est ajoute ou retiré
            AssociatedObject.IsChecked = ((System.Collections.IList)SelectedItemsSource).Contains(AssociatedObject.DataContext);
        }

        /// <summary>
        /// Met en place le behavior.
        /// </summary>
        /// <param name="oldValue">L'ancienne collection associée.</param>
        /// <param name="newValue">La nouvelle collection associée.</param>
        private void Setup(INotifyCollectionChanged oldValue, INotifyCollectionChanged newValue)
        {
            // Désabonnement
            if (oldValue != null)
                oldValue.CollectionChanged -= new NotifyCollectionChangedEventHandler(SelectedItemsSource_CollectionChanged);

            // Abonnement
            if (newValue != null)
                newValue.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedItemsSource_CollectionChanged);
        }

        /// <summary>
        /// Traite l'évènement Unchecked de l'élément associé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void toggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var tb = sender as ToggleButton;

            if (tb != null)
            {
                // Désabonnement temporaire pour éviter les effets de bords
                SelectedItemsSource.CollectionChanged -= new NotifyCollectionChangedEventHandler(SelectedItemsSource_CollectionChanged);
                // On supprime l'item lors du décoche
                ((System.Collections.IList)SelectedItemsSource).Remove(tb.DataContext);
                // Réabonnement
                SelectedItemsSource.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedItemsSource_CollectionChanged);
            }
        }

        /// <summary>
        /// Traite l'évènement Checked de l'élément associé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void toggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var tb = sender as ToggleButton;

            if (tb != null)
            {
                // Désabonnement temporaire pour éviter les effets de bords
                SelectedItemsSource.CollectionChanged -= new NotifyCollectionChangedEventHandler(SelectedItemsSource_CollectionChanged);
                // On ajoute l'item lors de la coche
                ((System.Collections.IList)SelectedItemsSource).Add(tb.DataContext);
                // Réabonnement
                SelectedItemsSource.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedItemsSource_CollectionChanged);
            }
        }

        #endregion
    }
}
