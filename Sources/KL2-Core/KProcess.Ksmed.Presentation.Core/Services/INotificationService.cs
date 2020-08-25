using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service de notifications.
    /// </summary>
    public interface INotificationService : IPresentationService
    {

        /// <summary>
        /// Affiche la notification spécifiée.
        /// </summary>
        /// <param name="notification">La notification.</param>
        void Notify(Notification notification);

        /// <summary>
        /// Supprime la notification spécifiée.
        /// </summary>
        /// <param name="notification">La notification.</param>
        void DeleteNotification(Notification notification);

    }

    /// <summary>
    /// Représente une notification.
    /// </summary>
    public class Notification : NotifiableObject
    {
        private bool _isHighlighted;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si la notification est en surbrillance.
        /// </summary>
        public bool IsHightlighted
        {
            get { return _isHighlighted; }
            set
            {
                if (_isHighlighted != value)
                {
                    _isHighlighted = value;
                    OnPropertyChanged("IsHightlighted");
                }
            }
        }

        private bool _isSelected;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si la notification est sélectionnée.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");

                    if (IsSelected) 
                        FireUpdateRequested();
                }
            }
        }

        private object _title;
        /// <summary>
        /// Obtient ou définit le titre.
        /// </summary>
        public object Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        private object _content;
        /// <summary>
        /// Obtient ou définit le contenu.
        /// </summary>
        public object Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged("Content");
                }
            }
        }

        /// <summary>
        /// Survient lorsqu'une mise à jour de la notification est demandée.
        /// </summary>
        public event EventHandler UpdateRequested;

        /// <summary>
        /// Lève l'évènement UpdateRequested.
        /// </summary>
        public void FireUpdateRequested()
        {
            if (UpdateRequested != null)
                UpdateRequested(this, EventArgs.Empty);
        }
    }
}
