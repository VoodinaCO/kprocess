using System;

namespace KProcess.Ksmed.Models
{
    [Serializable]
    partial class Skill : IAuditableUserRequired, IActionReferential
    {

        /// <summary>
        /// Obtient la description ou, si ce dernier n'est pas défini, le libellé.
        /// </summary>
        public string DescriptionOrLabel =>
            string.IsNullOrEmpty(Description) ? Label : Description;

        /// <summary>
        /// Obtient ou définit l'identifiant du référentiel.
        /// </summary>
        public int Id
        {
            get { return SkillId; }
            set { SkillId = value; }
        }

        /// <summary>
        /// Obtient l'identifiant associé au référentiel lorsqu'il désigne le cadre d'utilisation du référentiel dans un process.
        /// </summary>
        public ProcessReferentialIdentifier ProcessReferentialId { get { return ProcessReferentialIdentifier.Skill; } }
    }
}
