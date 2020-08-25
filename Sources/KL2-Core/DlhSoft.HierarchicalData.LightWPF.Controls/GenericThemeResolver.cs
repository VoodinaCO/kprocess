using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace DlhSoft
{
    /// <summary>
    /// Ajout Tekigo : permet d'ajouter Generic.xaml en tant que ressources (nécessaire alors que ça ne devrait pas)
    /// sans faire planter les designer (où une perte de l'intellisense était observée)
    /// </summary>
    public static class GenericThemeResolver
    {

        private static bool? _isInDesignMode;
        /// <summary>
        /// Indique si le contexte courant est en mode design.
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                    _isInDesignMode = DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
                return _isInDesignMode.Value;
            }
        }

        public static void Resolve(FrameworkElement control)
        {
            if (!IsInDesignMode)
            {
                string fullName = control.GetType().Assembly.FullName;
                int index = fullName.IndexOf(',');
                if (index >= 0)
                {
                    fullName = fullName.Substring(0, index);
                }
                Uri uri = new Uri(string.Format("pack://application:,,,/{0};component/Themes/Generic.xaml", fullName));
                control.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = uri });
            }
        }
    }
}
