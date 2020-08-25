// -----------------------------------------------------------------------
// <copyright file="ValueConverterRelay.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Représente un relai contenant un <see cref="IValueConverter"/>
    /// </summary>
    public class ValueConverterRelay : IValueConverter
    {
        #region Attributs

        private IValueConverter _value;

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit le convertisseur.
        /// </summary>
        /// <value>Le convertisseur.</value>
        public IValueConverter Converter
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        #region IValueConverter Members

        /// <summary>
        /// Convertit une valeur.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>   
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Converter == null) ? null : Converter.Convert(value, targetType, parameter, culture);
        }

        /// <summary>
        /// Convertit une valeur de retour.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Converter == null) ? null : Converter.ConvertBack(value, targetType, parameter, culture);
        }

        #endregion
    }
}
