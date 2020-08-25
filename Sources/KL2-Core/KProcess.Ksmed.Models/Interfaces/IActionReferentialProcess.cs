using System;

namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Définit le comportement d'un référentiel d'action de process.
    /// </summary>
    public interface IActionReferentialProcess : IActionReferential
    {
        /// <summary>
        /// Obtient ou définit le process auquel le référentiel appartient.
        /// </summary>
        Procedure Process { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant du process auquel le référentiel appartient.
        /// </summary>
        int? ProcessId { get; set; }

        /// <summary>
        /// Survient lorsque le process a changé.
        /// </summary>
        event EventHandler<PropertyChangedEventArgs<Procedure>> ProcessChanged;
    }
}
