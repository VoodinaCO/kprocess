using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.Unity;

namespace KProcess.Common.Validation
{
    /// <summary>
    /// Représente une extension permettant de définir une custom ValidationFactory.
    /// </summary>
    class UnityCustomValidationFactoryExtension : UnityContainerExtension
    {
        /// <summary>
        /// Initialise le conteneur.
        /// </summary>
        protected override void Initialize()
        {
            // Création du locator
            var locator = new UnityServiceLocator(base.Container);

            // Association de la factory
            IoC.RegisterType<AttributeValidatorFactory, CustomAttributeValidationFactory>(true);

            // Association du locator et de l'EnterpriseLibraryContainer
            EnterpriseLibraryContainer.Current = locator;
        }
    }
}
