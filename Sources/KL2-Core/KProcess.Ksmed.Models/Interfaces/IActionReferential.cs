using System;

namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Définit le comportement d'un référentiel d'action.
    /// </summary>
    public interface IActionReferential : IObjectWithChangeTracker, IIsDeleted
    {
        /// <summary>
        /// Obtient ou définit l'identifiant du référentiel.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'instance est liée à des actions.
        /// </summary>
        bool HasRelatedActions { get; set; }

        /// <summary>
        /// Obtient ou définit le libellé.
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Obtient ou définit la description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Obtient la description ou, si ce dernier n'est pas défini, le libellé.
        /// </summary>
        string DescriptionOrLabel { get; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le référentiel est modifiable.
        /// </summary>
        bool IsEditable { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le référentiel est sélectionné.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Obtient ou définit la couleur de ce référentiel.
        /// </summary>
        string Color { get; set; }

        /// <summary>
        /// Obtient ou définit le hash du document associé au référentiel.
        /// </summary>
        string Hash { get; set; }

        /// <summary>
        /// Obtient ou définit le CloudFile du document associé au référentiel.
        /// </summary>
        CloudFile CloudFile { get; set; }

        /// <summary>
        /// Obtient ou définit la quantité de référentiels.
        /// </summary>
        int Quantity { get; set; }

        /// <summary>
        /// Survient lorsque la valeur de le propriété <see cref="IsSelected"/> a changé.
        /// </summary>
        event EventHandler IsSelectedChanged;

        /// <summary>
        /// Survient lorsque la valeur de le propriété <see cref="Quantity"/> a changé.
        /// </summary>
        event EventHandler QuantityChanged;

        /// <summary>
        /// Survient lorsque la valeur de le propriété <see cref="IsEditable"/> a changé.
        /// </summary>
        event EventHandler IsEditableChanged;

        /// <summary>
        /// Obtient l'identifiant associé au référentiel lorsqu'il désigne le cadre d'utilisation du référentiel dans un process.
        /// </summary>
        ProcessReferentialIdentifier ProcessReferentialId { get; }

    }
}
