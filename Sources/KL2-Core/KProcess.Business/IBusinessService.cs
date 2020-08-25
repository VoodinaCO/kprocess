namespace KProcess.Business
{
    /// <summary>
    /// Définit le comportement d'un service de la couche domaine ou métier.
    /// </summary>
    public interface IBusinessService : IService, IInterceptable
    {
    }
}
