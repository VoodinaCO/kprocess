using System;
using System.Windows;
using System.Windows.Markup;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Markup permettant de renvoyer la liste des valeurs d'un enum
    /// </summary>
    [MarkupExtensionReturnType(typeof(Array)), Localizability(LocalizationCategory.NeverLocalize)]
    public class EnumValuesExtension : MarkupExtension
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public EnumValuesExtension()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="enumType">Le type de l'énumération.</param>
        public EnumValuesExtension(Type enumType)
        {
            EnumType = enumType;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit le type de l'enum
        /// </summary>
        [ConstructorArgument("EnumType")]
        public Type EnumType { get; set; }

        #endregion

        #region Surcharges

        /// <summary>
        /// Retourne la valeur localisée
        /// </summary>
        /// <param name="serviceProvider">objet fournissant des services au markup</param>
        /// <returns>
        /// la valeur localisée
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }

        #endregion
    }
}