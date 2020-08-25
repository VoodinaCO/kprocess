namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Représente le référentiel additionel 1.
    /// </summary>
    partial class Ref5 : IAuditableUserRequired, IActionReferentialProcess, IMultipleActionReferential
    {
        /// <summary>
        /// Obtient la description ou, si ce dernier n'est pas défini, le libellé.
        /// </summary>
        public string DescriptionOrLabel
        {
            get { return string.IsNullOrEmpty(this.Description) ? this.Label : this.Description; }
        }

        /// <summary>
        /// Obtient ou définit l'identifiant du référentiel.
        /// </summary>
        public int Id
        {
            get { return RefId; }
            set { RefId = value; }
        }

        /// <summary>
        /// Obtient l'identifiant associé au référentiel lorsqu'il désigne le cadre d'utilisation du référentiel dans un projet.
        /// </summary>
        public ProcessReferentialIdentifier ProcessReferentialId { get { return ProcessReferentialIdentifier.Ref5; } }
    }

    partial class Ref5Action : IReferentialActionLink
    {
        partial void Initialize()
        {
            this.Quantity = 1;
        }

        /// <summary>
        /// Obtient ou définit l'identifiant du référentiel.
        /// </summary>
        public int ReferentialId
        {
            get { return this.RefId; }
            set { this.RefId = value; }
        }

        /// <summary>
        /// Obtient ou définit le référentiel associé.
        /// </summary>
        public IMultipleActionReferential Referential
        {
            get { return this.Ref5; }
            set { this.Ref5 = (Ref5)value; }
        }

    }
}
