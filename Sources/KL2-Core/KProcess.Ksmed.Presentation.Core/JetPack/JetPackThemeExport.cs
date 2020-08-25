using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.JetPack
{
    class JetPackThemeExport : IThemeDescription
    {
        internal const string ThemeGuidStr = "{36B21B63-BB68-40CC-8078-31F20C38B70C}";
        private Guid _id = new Guid(ThemeGuidStr);

        /// <summary>
        /// Obtient l'id du thème.
        /// </summary>
        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le thème est celui par défaut.
        /// </summary>
        public bool IsDefault
        {
            get { return true; }
        }

        /// <summary>
        /// Obtient le nom du thème.
        /// </summary>
        public string Name
        {
            get { return "JetPack"; }
        }

        /// <summary>
        /// Obtient les dictionnaires de ressources spécifique à ce thème.
        /// </summary>
        /// <returns>
        /// Les dictionnaires de ressources spécifique à ce thème.
        /// </returns>
        public IEnumerable<System.Windows.ResourceDictionary> GetResourceDictionaries()
        {
            var rds = new string[]
            {
                "Brushes.xaml",
                "Fonts.xaml",
                "CoreStyles.xaml",
                "Styles.xaml",
                "SdkStyles.xaml",
                // Ne fonctionne pas encore
                //"DataGridStyles.xaml", 
                "ToolkitStyles.xaml",
                "CustomStyles.xaml",
                "ChromelessWindowStyle.xaml"
            };

            foreach (var r in rds)
            {
                yield return new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/KProcess.Ksmed.Presentation.Core;component/JetPack/" + r),
                };
            }
        }
    }
}
