using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using KProcess;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Représente un bus de services.
    /// </summary>
    [Export(typeof(IServiceBus))]
    public class ServiceBus : IServiceBus
    {
        #region Attributs

        private readonly string uniqueId = Guid.NewGuid().ToString();

        #endregion

        #region Constructeur

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ServiceBus"/>.
        /// </summary>
        public ServiceBus()
        {
            IoC.RegisterInstance<IServiceBus>(this);
        }

        #endregion

        #region IServiceBus Members

        /// <summary>
        /// Enregistre le service spécifié.
        /// </summary>
        /// <typeparam name="TService">Le type du service.</typeparam>
        /// <param name="service">Le service à enregistrer.</param>
        /// <returns>L'instance du bus de services.</returns>
        public IServiceBus Register<TService>(TService service)
            where TService : IService
        {
            IoC.RegisterInstance<TService>(uniqueId, service);

            return this;
        }


        /// <summary>
        /// Enregistre le service spécifié grâce à son interface et à son type associé.
        /// </summary>
        /// <typeparam name="TIService">Le type de l'interface du service.</typeparam>
        /// <typeparam name="TService">Le type du service.</typeparam>
        /// <returns>
        /// L'instance du bus de services.
        /// </returns>
        public IServiceBus RegisterType<TIService, TService>()
            where TIService : IService
            where TService : TIService
        {
            IoC.RegisterType<TIService, TService>(uniqueId, true);

            return this;
        }

        /// <summary>
        /// Supprime l'enregistrement du service spécifié.
        /// </summary>
        /// <typeparam name="TService">Le type du service.</typeparam>
        /// <returns>L'instance du bus de services.</returns>
        public IServiceBus Unregister<TService>()
            where TService : IService
        {
            IoC.CleanUp(IoC.Resolve<TService>(uniqueId));

            return this;
        }

        /// <summary>
        /// Détermine si le service spécifié est disponible.
        /// </summary>
        /// <typeparam name="TService">Le type du service.</typeparam>
        /// <returns>
        /// 	<c>true</c> si le service est disponible; sinon, <c>false</c>.
        /// </returns>
        public bool IsAvailable<TService>()
        {
            return IoC.IsRegistered<TService>(uniqueId);
        }

        /// <summary>
        /// Obtient un service.
        /// </summary>
        /// <typeparam name="TService">Le type de service à obtenir.</typeparam>
        /// <returns>Le service.</returns>
        public TService Get<TService>()
            where TService : IService
        {
            return IoC.Resolve<TService>(uniqueId);
        }

        #endregion

    }
}
