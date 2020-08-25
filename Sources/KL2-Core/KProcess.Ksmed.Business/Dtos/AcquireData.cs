using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Business.Dtos
{
    /// <summary>
    /// Contient les données pour l'écran de préparation.
    /// </summary>
    public class AcquireData
    {
        /// <summary>
        /// Obtient ou définit les scénarios du projet.
        /// </summary>
        public Scenario[] Scenarios { get; set; }

        /// <summary>
        /// Obtient ou définit les catégories.
        /// </summary>
        public ActionCategory[] Categories { get; set; }

        /// <summary>
        /// Obtient ou définit les compétences.
        /// </summary>
        public Skill[] Skills { get; set; }

        /// <summary>
        /// Obtient ou définit les ressources.
        /// </summary>
        public Resource[] Resources { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 1.
        /// </summary>
        public Ref1[] Ref1s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 2.
        /// </summary>
        public Ref2[] Ref2s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 3.
        /// </summary>
        public Ref3[] Ref3s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 4.
        /// </summary>
        public Ref4[] Ref4s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 5.
        /// </summary>
        public Ref5[] Ref5s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 6.
        /// </summary>
        public Ref6[] Ref6s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 7
        /// </summary>
        public Ref7[] Ref7s { get; set; }

        /// <summary>
        /// Obtient ou définit les vidéos.
        /// </summary>
        public Video[] Videos { get; set; }

        /// <summary>
        /// Obtient ou définit les libellés des champs personnalisés.
        /// </summary>
        public CustomFieldsLabels CustomFieldsLabels { get; set; }

    }
}
