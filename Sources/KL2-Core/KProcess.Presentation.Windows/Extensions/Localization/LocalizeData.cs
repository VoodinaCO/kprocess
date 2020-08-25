using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using KProcess.Globalization;

namespace KProcess.Presentation.Windows.Localization
{
    /// <summary>
    /// Contient les données de localisation.
    /// </summary>
    public class LocalizeData : INotifyPropertyChanged
    {

        /// <summary>
        /// Obtient ou définit le <see cref="ILocalizeExtension"/> parent.
        /// </summary>
        public ILocalizeExtension ParentLocalizeExtension { get; private set; }

        /// <summary>
        /// Obtient ou définit le <see cref="IDependencyLocalizeExtension"/> parent.
        /// </summary>
        public IDependencyLocalizeExtension ParentDependencyLocalizeExtension { get; private set; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="LocalizeData"/>.
        /// </summary>
        /// <param name="parent">Le <see cref="ILocalizeExtension"/> parent.</param>
        /// <param name="targetDependencyObject">L'objet contenant la DP cible</param>
        /// <param name="targetDependencyProperty">La DP cible</param>
        public LocalizeData(ILocalizeExtension parent, DependencyObject targetDependencyObject, DependencyProperty targetDependencyProperty)
        {
            this.ParentLocalizeExtension = parent;
            UpdateValue(targetDependencyObject, targetDependencyProperty);
        }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="LocalizeData"/>.
        /// </summary>
        /// <param name="parent">Le <see cref="IDependencyLocalizeExtension"/> parent.</param>
        /// <param name="targetDependencyObject">L'objet contenant la DP cible</param>
        /// <param name="targetDependencyProperty">La DP cible</param>
        public LocalizeData(IDependencyLocalizeExtension parent, DependencyObject targetDependencyObject, DependencyProperty targetDependencyProperty)
        {
            this.ParentLocalizeExtension = parent;
            this.ParentDependencyLocalizeExtension = parent;
            UpdateValue(targetDependencyObject, targetDependencyProperty);
        }

        /// <summary>
        /// Met à jour la cible
        /// </summary>
        /// <param name="targetDependencyObject">L'objet contenant la DP cible</param>
        /// <param name="targetDependencyProperty">La DP cible</param>
        public void UpdateValue(DependencyObject targetDependencyObject, DependencyProperty targetDependencyProperty)
        {
            if (ParentDependencyLocalizeExtension != null && targetDependencyObject != null && targetDependencyProperty != null)
                this.Value = GetLocalizedValue(true, targetDependencyProperty);
            else
                this.Value = GetLocalizedValue(false, targetDependencyProperty);
        }

        /// <summary>
        /// Récupère la valeur localisée
        /// </summary>
        /// <param name="isDependencyObject"><c>true</c> si la valeur doit être posée sur un <see cref="DependencyObject"/>.</param>
        /// <param name="targetDependencyProperty">La DP cible</param>
        /// <returns>
        /// la valeur localisée
        /// </returns>
        private object GetLocalizedValue(bool isDependencyObject, DependencyProperty targetDependencyProperty)
        {
            // Récupère la valeur localisée
            object value = LocalizationManager.GetValue(ParentLocalizeExtension.Key, ParentLocalizeExtension.ProviderKey, ParentLocalizeExtension.DefaultValue);

            // Tente de convertir automatiquement la valeur
            if (value != null && ParentDependencyLocalizeExtension != null && ParentDependencyLocalizeExtension.TypeConverter != null
              && ParentDependencyLocalizeExtension.TypeConverter.CanConvertFrom(value.GetType()))
                value = ParentDependencyLocalizeExtension.TypeConverter.ConvertFrom(value);

            // En cas d'erreur, on fournit la valeur par défaut
            if (value == null && ParentLocalizeExtension.DefaultValue != null)
                value = ParentLocalizeExtension.DefaultValue;

            // Si aucune valeur n'a été trouvé
            if (value == null)
            {
                if (isDependencyObject)
                    return null;
                else
                    // Return the UnsetValue for all other types of dependency properties
                    return DependencyProperty.UnsetValue;
            }

            // Si un converter a été fourni
            if (ParentDependencyLocalizeExtension != null && ParentDependencyLocalizeExtension.Converter != null)
                value = ParentDependencyLocalizeExtension.Converter.Convert(value, targetDependencyProperty.PropertyType, null, CultureInfo.CurrentCulture);

            // Si une chaîne de formatage a été fournie
            if (!String.IsNullOrEmpty(ParentLocalizeExtension.StringFormat))
            {
                IFormattable formattableValue = value as IFormattable;
                if (formattableValue != null)
                    value = formattableValue.ToString(ParentLocalizeExtension.StringFormat, CultureInfo.CurrentCulture);
            }

            return value;
        }

        /// <summary>
        /// Obtient la valeur localisée
        /// </summary>
        /// <value>La valeur localisée.</value>
        public object Value { get; private set; }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Survient lorsque la valeur d'une propriété a changé.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Appelé lorsque la langue a changé.
        /// </summary>
        private void OnCultureChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
        }

        #endregion
    }
}
