using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using KProcess.Globalization;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente un menu de l'application.
    /// </summary>
    public class ApplicationMenuItem : ApplicationMenuItemBase
    {

        private List<ApplicationSubMenuItem> _subItems;
        /// <summary>
        /// Obtient ou définit les éléments du sous menu.
        /// </summary>
        public List<ApplicationSubMenuItem> SubItems
        {
            get { return _subItems; }
            set
            {
                if (_subItems != value)
                {
                    _subItems = value;
                    OnPropertyChanged("SubItems");
                }
            }
        }

        private ApplicationSubMenuItem _currentSubMenuItem;
        /// <summary>
        /// Obtient ou définit le sous menu courant.
        /// </summary>
        public ApplicationSubMenuItem CurrentSubMenuItem
        {
            get { return _currentSubMenuItem; }
            set
            {
                if (_currentSubMenuItem != value)
                {
                    _currentSubMenuItem = value;
                    OnPropertyChanged("CurrentSubMenuItem");
                }
            }
        }


        private Func<bool> _isEnabledDelegate;
        /// <summary>
        /// Obtient ou définit un délégué permettant de définir dynamiquement si l'élément est activé.
        /// </summary>
        public Func<bool> IsEnabledDelegate
        {
            get { return _isEnabledDelegate; }
            set
            {
                if (_isEnabledDelegate != value)
                {
                    _isEnabledDelegate = value;
                    this.InvalidateIsEnabled();
                }
            }
        }

        /// <summary>
        /// Invalide la propriété IsEnabled.
        /// </summary>
        public void InvalidateIsEnabled()
        {
            if (IsEnabledDelegate != null)
            {
                this.IsEnabled = IsEnabledDelegate();
            }
        }

        private MenuStrip _strip;
        /// <summary>
        /// Obtient ou définit la barre de menu à laquelle appartient ce menu;
        /// </summary>
        public MenuStrip Strip
        {
            get { return _strip; }
            set
            {
                if (_strip != value)
                {
                    _strip = value;
                    OnPropertyChanged("Strip");
                }
            }
        }

    }
}
