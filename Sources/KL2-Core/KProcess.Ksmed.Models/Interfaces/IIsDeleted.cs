namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Définit le comportement d'une entity qui peut être marquée comme effacée.
    /// </summary>
    public interface IIsDeleted
    {
        bool IsDeleted { get; set; }
    }
}
