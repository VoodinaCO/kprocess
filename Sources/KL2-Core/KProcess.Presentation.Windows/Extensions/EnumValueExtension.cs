using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Représente une extension permettant de fournir une valeur d'énumération.
    /// </summary>
    public class EnumValueExtension : MarkupExtension
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public EnumValueExtension()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="enumType">Le type de l'énumération.</param>
        /// <param name="enumValue">la valeur de l'énumération.</param>
        public EnumValueExtension(Type enumType, string enumValue)
            : this()
        {
            this.EnumType = enumType;
            this.EnumValue = enumValue;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit le type de l'énumération
        /// </summary>
        [ConstructorArgument("EnumType")]
        public Type EnumType { get; set; }

        /// <summary>
        /// Obtient ou définit la valeur de l'énumération
        /// </summary>
        [ConstructorArgument("EnumValue")]
        public string EnumValue { get; set; }

        #endregion

        /// <summary>
        /// Retourne la valeur.
        /// </summary>
        /// <param name="serviceProvider">objet fournissant des services au markup</param>
        /// <returns>
        /// La valeur.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType != null)
            {
                var typeConverter = new EnumConverter(EnumType);
                return typeConverter.ConvertFrom(this.EnumValue);
            }
            else
                return null;

        }
    }
}
