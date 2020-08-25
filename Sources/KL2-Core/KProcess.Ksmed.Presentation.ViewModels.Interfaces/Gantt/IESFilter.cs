using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Globalization;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente un filtre I/E/S.
    /// </summary>
    public class IESFilter
    {

        /// <summary>
        /// Obtient ou définit le libellé du filtre.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Obtient ou définit la valeur.
        /// </summary>
        public IESFilterValue Value { get; set; }

        /// <summary>
        /// Crée les filtres par défaut.
        /// </summary>
        /// <returns>Les filtes par défaut.</returns>
        public static IESFilter[] CreateDefault()
        {
            return new IESFilter[]
            {
                new IESFilter() {Label = LocalizationManager.GetString("View_AnalyzeGridCommon_FilterIES"), Value = IESFilterValue.IES},
                new IESFilter() {Label = LocalizationManager.GetString("View_AnalyzeGridCommon_FilterI"), Value = IESFilterValue.I},
                new IESFilter() {Label = LocalizationManager.GetString("View_AnalyzeGridCommon_FilterIE"), Value = IESFilterValue.IE},
            };
        }

        /// <summary>
        /// Crée les filtres sans celui I/E/S.
        /// </summary>
        /// <returns>Les filtes.</returns>
        public static IESFilter[] CreateWithoutIES()
        {
            return new IESFilter[]
            {
                new IESFilter() {Label = LocalizationManager.GetString("View_AnalyzeGridCommon_FilterI"), Value = IESFilterValue.I},
                new IESFilter() {Label = LocalizationManager.GetString("View_AnalyzeGridCommon_FilterIE"), Value = IESFilterValue.IE},
            };
        }

        /// <summary>
        /// Crée les filtres sans I/E mais en trompant l'affichage de I/E/S en mettant I/E en valeur.
        /// </summary>
        /// <returns>Les filtes.</returns>
        public static IESFilter[] CreateWithoutIESReplacingIEWithIES()
        {
            return new IESFilter[]
            {
                new IESFilter() {Label = LocalizationManager.GetString("View_AnalyzeGridCommon_FilterIE"), Value = IESFilterValue.IES},
                new IESFilter() {Label = LocalizationManager.GetString("View_AnalyzeGridCommon_FilterI"), Value = IESFilterValue.I},
            };
        }

    }

    /// <summary>
    /// Le filtre IES
    /// </summary>
    public enum IESFilterValue
    {
        /// <summary>
        /// I + E + S
        /// </summary>
        IES,

        /// <summary>
        /// I
        /// </summary>
        I,

        /// <summary>
        /// I + E
        /// </summary>
        IE
    }
}
