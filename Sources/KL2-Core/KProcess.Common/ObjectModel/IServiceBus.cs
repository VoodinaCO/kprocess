namespace KProcess
{
    /// <summary>
    /// Définit le comportement de base d'un bus de services.
    /// </summary>
    public interface IServiceBus
    {
        /// <summary>
        /// Obtient un service.
        /// </summary>
        /// <typeparam name="TService">Le type de service à obtenir.</typeparam>
        /// <returns>Le service.</returns>
        TService Get<TService>() where TService : IService;

        /// <summary>
        /// Détermine si le service spécifié est disponible.
        /// </summary>
        /// <typeparam name="TService">Le type du service.</typeparam>
        /// <returns>
        /// 	<c>true</c> si le service est disponible; sinon, <c>false</c>.
        /// </returns>
        bool IsAvailable<TService>();

        /// <summary>
        /// Enregistre le service spécifié.
        /// </summary>
        /// <typeparam name="TService">Le type du service.</typeparam>
        /// <param name="service">Le service à enregistrer.</param>
        /// <returns>L'instance du bus de services.</returns>
        IServiceBus Register<TService>(TService service) where TService : IService;

        /// <summary>
        /// Enregistre le service spécifié grâce à son interface et à son type associé.
        /// </summary>
        /// <typeparam name="TIService">Le type de l'interface du service.</typeparam>
        /// <typeparam name="TService">Le type du service.</typeparam>
        /// <returns>
        /// L'instance du bus de services.
        /// </returns>
        IServiceBus RegisterType<TIService, TService>()
            where TIService : IService
            where TService : TIService;

        /// <summary>
        /// Supprime l'enregistrement du service spécifié.
        /// </summary>
        /// <typeparam name="TService">Le type du service.</typeparam>
        /// <returns>L'instance du bus de services.</returns>
        IServiceBus Unregister<TService>() where TService : IService;
    }
}
