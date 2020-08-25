namespace KProcess.Ksmed.Models
{
    partial class AppResourceValue : IAuditableUserRequired
    {
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'audit ne doit pas être géré automatiquement.
        /// </summary>
        public bool CustomAudit =>
            false;
    }
}
