using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows;
using System.Collections.Generic;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Décrit le comportement d'un service de récupération d'informations sur l'utilisation des référentiels dans un projet.
    /// </summary>
    public interface IReferentialsUseService : IPresentationService
    {
        /// <summary>
        /// Met à jour les propriétés des référentiels dans le contexte projet.
        /// </summary>
        /// <param name="referentials">Les référentiels</param>
        void UpdateProjectReferentials(IEnumerable<ProjectReferential> referentials);

        /// <summary>
        /// Obtient les propriétés d'utilisation des référentiels.
        /// </summary>
        Dictionary<ProcessReferentialIdentifier, ProjectReferentialInfo> Referentials { get; }

        /// <summary>
        /// Obtient l'utilisation des référentiels.
        /// </summary>
        IDictionary<ProcessReferentialIdentifier, bool> ReferentialsEnabled { get; }

        /// <summary>
        /// Obtient une valeur indiquant si le référentiel spécifié est activé.
        /// </summary>
        /// <param name="refe">Le référentiel.</param>
        /// <returns><c>true</c> si le référentiel est activé.</returns>
        bool IsReferentialEnabled(ProcessReferentialIdentifier refe);

        /// <summary>
        /// Obtient le libellé pour un référentiel.
        /// </summary>
        /// <param name="refe"></param>
        /// <returns></returns>
        string GetLabel(ProcessReferentialIdentifier refe);

        /// <summary>
        /// Obtient le libellé pour un référentiel, au pluriel.
        /// </summary>
        /// <param name="refe">Le référentiel.</param>
        /// <returns>Le libellé.</returns>
        string GetLabelPlural(ProcessReferentialIdentifier refe);
    }

    /// <summary>
    /// Décrit un référentiel d'un projet.
    /// </summary>
    public class ProjectReferentialInfo
    {
        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="referential">Le modèle d'origine.</param>
        public ProjectReferentialInfo(ProjectReferential referential)
        {
            this.Referential = (ProcessReferentialIdentifier)referential.ReferentialId;
            this.IsEnabled = referential.IsEnabled;
            this.HasMultipleSelection = referential.HasMultipleSelection;
            this.KeepsSelection = referential.KeepsSelection;
            this.HasQuantity = referential.HasQuantity;
        }

        /// <summary>
        /// Obtient ou définit l'identifiant du référentiel.
        /// </summary>
        public ProcessReferentialIdentifier Referential { get; private set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le référentiel est activé.
        /// </summary>
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la selection multiple est activée.
        /// </summary>
        public bool HasMultipleSelection { get; private set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la sélection doit être conservée.
        /// </summary>
        public bool KeepsSelection { get; private set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les référentiels doivent avoir des quantités.
        /// </summary>
        public bool HasQuantity { get; private set; }
    }

    /// <summary>
    /// Permet d'obtenir les libellés des référentiels.
    /// </summary>
    public static class ReferentialsUse
    {
        public static string Operator
        {
            get { return GetLabel(ProcessReferentialIdentifier.Operator); }
        }

        public static string AllOperators
        {
            get { return GetLabelPlural(ProcessReferentialIdentifier.Operator); }
        }

        public static string Equipment
        {
            get { return GetLabel(ProcessReferentialIdentifier.Equipment); }
        }

        public static string AllEquipments
        {
            get { return GetLabelPlural(ProcessReferentialIdentifier.Equipment); }
        }

        public static string Category
        {
            get { return GetLabel(ProcessReferentialIdentifier.Category); }
        }

        public static string Skill
        {
            get { return GetLabel(ProcessReferentialIdentifier.Skill); }
        }

        public static string Ref1
        {
            get { return GetLabel(ProcessReferentialIdentifier.Ref1); }
        }

        public static string Ref2
        {
            get { return GetLabel(ProcessReferentialIdentifier.Ref2); }
        }

        public static string Ref3
        {
            get { return GetLabel(ProcessReferentialIdentifier.Ref3); }
        }

        public static string Ref4
        {
            get { return GetLabel(ProcessReferentialIdentifier.Ref4); }
        }

        public static string Ref5
        {
            get { return GetLabel(ProcessReferentialIdentifier.Ref5); }
        }

        public static string Ref6
        {
            get { return GetLabel(ProcessReferentialIdentifier.Ref6); }
        }

        public static string Ref7
        {
            get { return GetLabel(ProcessReferentialIdentifier.Ref7); }
        }

        /// <summary>
        /// Obtient le libellé d'un référentiel.
        /// </summary>
        /// <param name="id">L'identifiant du référentiel.</param>
        /// <returns>Le libellé.</returns>
        public static string GetLabel(ProcessReferentialIdentifier id)
        {
            if (DesignMode.IsInDesignMode)
                return id.ToString();
            else
            {
                return IoC.Resolve<IReferentialsUseService>().GetLabel(id);
            }
        }

        /// <summary>
        /// Obtient le libellé d'un référentiel.
        /// </summary>
        /// <param name="id">L'identifiant du référentiel.</param>
        /// <returns>Le libellé.</returns>
        public static string GetLabelPlural(ProcessReferentialIdentifier id)
        {
            if (DesignMode.IsInDesignMode)
                return id.ToString() + " All";
            else
            {
                return IoC.Resolve<IReferentialsUseService>().GetLabelPlural(id);
            }
        }

        public static Visibility OperatorVisibility
        {
            get { return GetVisiblity(ProcessReferentialIdentifier.Operator); }
        }

        public static Visibility EquipmentVisibility
        {
            get { return GetVisiblity(ProcessReferentialIdentifier.Equipment); }
        }

        public static Visibility CategoryVisibility
        {
            get { return GetVisiblity(ProcessReferentialIdentifier.Category); }
        }

        public static Visibility Ref1Visibility
        {
            get { return GetVisiblity(ProcessReferentialIdentifier.Ref1); }
        }

        public static Visibility Ref2Visibility
        {
            get { return GetVisiblity(ProcessReferentialIdentifier.Ref2); }
        }

        public static Visibility Ref3Visibility
        {
            get { return GetVisiblity(ProcessReferentialIdentifier.Ref3); }
        }

        public static Visibility Ref4Visibility
        {
            get { return GetVisiblity(ProcessReferentialIdentifier.Ref4); }
        }

        public static Visibility Ref5Visibility
        {
            get { return GetVisiblity(ProcessReferentialIdentifier.Ref5); }
        }

        public static Visibility Ref6Visibility
        {
            get { return GetVisiblity(ProcessReferentialIdentifier.Ref6); }
        }

        public static Visibility Ref7Visibility
        {
            get { return GetVisiblity(ProcessReferentialIdentifier.Ref7); }
        }

        /// <summary>
        /// Obtient la visibilité liées à l'activation d'un référentiel.
        /// </summary>
        /// <param name="id">L'identifiant du référentiel.</param>
        /// <returns>la visibilité</returns>
        private static Visibility GetVisiblity(ProcessReferentialIdentifier id)
        {
            bool isEnabled;
            if (DesignMode.IsInDesignMode)
                isEnabled = true;
            else
            {
                var referentials = IoC.Resolve<IReferentialsUseService>().Referentials;
                if (referentials != null)
                    isEnabled = referentials[id].IsEnabled;
                else
                    isEnabled = true;
            }
            return isEnabled ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
