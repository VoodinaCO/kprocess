// -----------------------------------------------------------------------
// <copyright file="ValueConverterOfT.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Value converter générique
    /// </summary>
    /// <typeparam name="TIn">type d'objet en entrée</typeparam>
    /// <typeparam name="TOut">type d'objet en sortie</typeparam>
    public class ValueConverter<TIn, TOut> : IValueConverter
    {
        #region Constructeurs


        /// <summary>
        /// Initialise une nouvelle instance de <see cref="ValueConverter&lt;TIn, TOut&gt;"/>.
        /// </summary>
        /// <param name="convert">Un délégué permettant de convertir.</param>
        public ValueConverter(Func<TIn, TOut> convert)
            : this((value, parameter) => convert(value))
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="ValueConverter&lt;TIn, TOut&gt;"/>.
        /// </summary>
        /// <param name="convert">Un délégué permettant de convertir.</param>
        /// <param name="convertBack">Un délégue permettant de convertir la valeur de retour.</param>
        public ValueConverter(Func<TIn, TOut> convert, Func<TOut, TIn> convertBack)
            : this((value, parameter) => convert(value), (value, parameter) => convertBack(value))
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="ValueConverter&lt;TIn, TOut&gt;"/>.
        /// </summary>
        /// <param name="convert">Un délégué permettant de convertir et prenant en argument un paramètre.</param>
        public ValueConverter(Func<TIn, Object, TOut> convert)
        {
            Convertor = convert;
        }
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="ValueConverter&lt;TIn, TOut&gt;"/>.
        /// </summary>
        /// <param name="convert">Un délégué permettant de convertir et prenant en argument un paramètre.</param>
        /// <param name="convertBack">Un délégué permettant de convertir la valeur de retour et prenant en argument un paramètre.</param>
        public ValueConverter(Func<TIn, Object, TOut> convert, Func<TOut, Object, TIn> convertBack)
        {
            Convertor = convert;
            BackConvertor = convertBack;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit le convertisseur.
        /// </summary>
        /// <value>Le convertisseur.</value>
        public Func<TIn, Object, TOut> Convertor { get; set; }

        /// <summary>
        /// Obtient ou définit le convertisseur de la valeur de retour.
        /// </summary>
        /// <value>Le convertisseur de la valeur de retour.</value>
        public Func<TOut, Object, TIn> BackConvertor { get; set; }

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
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            if (Convertor == null)
                throw new NotSupportedException("Convertor must be defined.");

            try
            {
                return Convertor(ReferenceEquals(value, DependencyProperty.UnsetValue) ? default(TIn) : (TIn)value, parameter);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convertit une valeur de retour.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (BackConvertor == null)
                throw new NotSupportedException("BackConvertor must be defined.");

            try
            {
                return BackConvertor(ReferenceEquals(value, DependencyProperty.UnsetValue) ? default(TOut) : (TOut)value, parameter);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
