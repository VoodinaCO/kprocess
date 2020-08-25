using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Shell
{
    public partial class App : IThemeManagerService
    {
        [Import]
        private IEventBus _eventBus = null;

        private IThemeDescription[] _availableThemes;
        private IThemeDescription _currentTheme;
        private ResourceDictionary[] _currentResourceDictionaries;

        /// <summary>
        /// Obtient les thèmes disponibles.
        /// </summary>
        [ImportMany]
        public IThemeDescription[] AvailableThemes
        {
            get { return _availableThemes; }
            private set
            {
                _availableThemes = value;
                InitializeThemeSelection();
            }
        }

        /// <summary>
        /// Initialise la selection du thème.
        /// </summary>
        private void InitializeThemeSelection()
        {
            IThemeDescription currentThemeToSet;

            // If there a default theme
            if (_availableThemes != null && CurrentTheme == null)
                // Use it
                currentThemeToSet = _availableThemes.FirstOrDefault(t => t.IsDefault);
            else
                // remove it
                currentThemeToSet = null;

            this.CurrentTheme = currentThemeToSet;
        }

        /// <summary>
        /// Obtient ou définit le thème actuel.
        /// </summary>
        public IThemeDescription CurrentTheme
        {
            get { return _currentTheme; }
            set
            {
                if (_currentTheme != value)
                {
                    var ancienTheme = _currentTheme;
                    _currentTheme = value;

                    ApplyNewTheme(ancienTheme, _currentTheme);

                    _eventBus.Publish<ThemeChangedEvent>(new ThemeChangedEvent(this, ancienTheme, _currentTheme));
                }
            }
        }

        /// <summary>
        /// Applique le changement de thème.
        /// </summary>
        /// <param name="oldTheme">l'ancien thème.</param>
        /// <param name="newTheme">le nouveau thème.</param>
        private void ApplyNewTheme(IThemeDescription oldTheme, IThemeDescription newTheme)
        {
            // Supprimer les dictionnaires du thème actuel
            if (_currentResourceDictionaries != null && _currentResourceDictionaries.Any())
            {
                foreach (var dic in _currentResourceDictionaries)
                    this.Resources.MergedDictionaries.Remove(dic);
            }

            _currentResourceDictionaries = newTheme.GetResourceDictionaries().ToArray();

            // Ajouter les dictionnaires du nouveau thème
            foreach (var dic in _currentResourceDictionaries)
                if (!this.Resources.MergedDictionaries.Any(rd => rd.Source == dic.Source))
                    this.Resources.MergedDictionaries.Add(dic);

        }
    }
}
