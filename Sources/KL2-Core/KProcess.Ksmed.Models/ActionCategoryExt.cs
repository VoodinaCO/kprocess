using KProcess.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;

namespace KProcess.Ksmed.Models
{
    [HasSelfValidation]
    [Serializable]
    partial class ActionCategory : IAuditableUserRequired, IActionCategory
    {

        /// <summary>
        /// Obtient la description ou, si ce dernier n'est pas défini, le libellé.
        /// </summary>
        public string DescriptionOrLabel =>
            string.IsNullOrEmpty(Description) ? Label : Description;

        /// <summary>
        /// Lorsque surchargé dans une classe fille, cette méthode sert à exécuter une validation personnalisée.
        /// </summary>
        /// <returns>
        /// Une énumération des erreurs de validation, ou null s'il n'y en a pas.
        /// </returns>
        [SelfValidation]
        protected override IEnumerable<ValidationError> OnCustomValidate()
        {
            foreach (ValidationError error in base.OnCustomValidate())
                yield return error;

            if (ActionValueCode == null || Value == null)
            {
                yield return new ValidationError(nameof(ActionValueCode), LocalizationManager.GetString("Validation_ActionCategory_Value_Required"));
                yield return new ValidationError(nameof(Value), LocalizationManager.GetString("Validation_ActionCategory_Value_Required"));
            }
        }

        /// <summary>
        /// Obtient ou définit l'identifiant du référentiel.
        /// </summary>
        public int Id
        {
            get { return ActionCategoryId; }
            set { ActionCategoryId = value; }
        }

        /// <summary>
        /// Obtient l'identifiant associé au référentiel lorsqu'il désigne le cadre d'utilisation du référentiel dans un process.
        /// </summary>
        public ProcessReferentialIdentifier ProcessReferentialId { get { return ProcessReferentialIdentifier.Category; } }

    }
}
