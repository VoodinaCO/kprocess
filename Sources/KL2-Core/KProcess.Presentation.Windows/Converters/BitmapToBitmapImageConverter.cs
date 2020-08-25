// -----------------------------------------------------------------------
// <copyright file="BitmapToBitmapImageConverter.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace KProcess.Presentation.Windows.Converters
{
    /// <summary>
    /// Convertit un bitmap (gdi) en BitmapImage compatible avec WPF
    /// </summary>
    public class BitmapToBitmapImageConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un bitmap GDI en BitmapImage WPF.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage result = null;

            if (value is System.Drawing.Image image)
            {
                result = new BitmapImage();
                MemoryStream ms = new MemoryStream();

                image.Save(ms, ImageFormat.Png);

                result.BeginInit();
                result.StreamSource = ms;
                result.EndInit();
            }

            return result;
        }

        /// <summary>
        /// N'est pas supporté.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
