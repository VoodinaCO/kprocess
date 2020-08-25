using KProcess.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente les libellés des champs libres.
    /// </summary>
    public class CustomFieldsLabels : NotifiableObject
    {

        /// <summary>
        /// Constructeur par défaut.
        /// </summary>
        public CustomFieldsLabels()
        {
        }

        /// <summary>
        /// Crée une instance à partir des données d'un DTO.
        /// </summary>
        /// <param name="labels">Le Dto.</param>
        public CustomFieldsLabels(KProcess.Ksmed.Business.Dtos.CustomFieldsLabels labels)
        {
            this.Text1 = new CustomFieldLabel(false, 1, labels.Text1);
            this.Text2 = new CustomFieldLabel(false, 2, labels.Text2);
            this.Text3 = new CustomFieldLabel(false, 3, labels.Text3);
            this.Text4 = new CustomFieldLabel(false, 4, labels.Text4);

            this.Numeric1 = new CustomFieldLabel(true, 1, labels.Numeric1);
            this.Numeric2 = new CustomFieldLabel(true, 2, labels.Numeric2);
            this.Numeric3 = new CustomFieldLabel(true, 3, labels.Numeric3);
            this.Numeric4 = new CustomFieldLabel(true, 4, labels.Numeric4);
        }

        /// <summary>
        /// Obtient ou définit le libellé du champ libre texte n° 1.
        /// </summary>
        public CustomFieldLabel Text1 { get; set; }

        /// <summary>
        /// Obtient ou définit le libellé du champ libre texte n° 2.
        /// </summary>
        public CustomFieldLabel Text2 { get; set; }

        /// <summary>
        /// Obtient ou définit le libellé du champ libre texte n° 3.
        /// </summary>
        public CustomFieldLabel Text3 { get; set; }

        /// <summary>
        /// Obtient ou définit le libellé du champ libre texte n° 4.
        /// </summary>
        public CustomFieldLabel Text4 { get; set; }

        /// <summary>
        /// Obtient ou définit le libellé du champ libre numérique n° 1.
        /// </summary>
        public CustomFieldLabel Numeric1 { get; set; }

        /// <summary>
        /// Obtient ou définit le libellé du champ libre numérique n° 2.
        /// </summary>
        public CustomFieldLabel Numeric2 { get; set; }

        /// <summary>
        /// Obtient ou définit le libellé du champ libre numérique n° 3.
        /// </summary>
        public CustomFieldLabel Numeric3 { get; set; }

        /// <summary>
        /// Obtient ou définit le libellé du champ libre numérique n° 4.
        /// </summary>
        public CustomFieldLabel Numeric4 { get; set; }

    }
}
