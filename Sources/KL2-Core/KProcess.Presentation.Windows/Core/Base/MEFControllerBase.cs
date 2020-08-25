using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Threading.Tasks;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'implémentation de base d'un contrôleur gérant MEF.
    /// </summary>
    public abstract class MEFControllerBase : ApplicationControllerBase, IPartImportsSatisfiedNotification
    {
        #region Attributs

        private TransientCompositionContainer _container;

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient le conteneur MEF du contrôleur.
        /// </summary>
        /// <value>Le conteneur MEF du contrôleur.</value>
        protected CompositionContainer Container
        {
            get { return _container; }
        }

        #endregion

        #region Méthodes protégées

        /// <summary>
        /// Récupère le catalogue à charger
        /// </summary>
        /// <returns>le catalogue à charger</returns>
        protected virtual ComposablePartCatalog GetCatalog() { return null; }

        #endregion

        #region Surcharges

        /// <summary>
        /// Démarre le contrôleur
        /// </summary>
        protected override Task OnStart()
        {
            // On créé le conteneur MEF supportant les instances PerCall
            _container = new TransientCompositionContainer(GetCatalog());

            // On compose le contrôleur
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(this);
            batch.AddExportedValue<CompositionContainer>(_container);
            IoC.RegisterInstance<CompositionContainer>(_container);
            _container.Compose(batch);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Arrête le contrôleur
        /// </summary>
        protected override Task OnStop()
        {
            // Nettoie le conteneur MEF
            _container.Dispose();
            return Task.CompletedTask;
        }

        #endregion

        #region IPartImportsSatisfiedNotification Members

        /// <summary>
        /// Méthode invoquée lorsque les imports ont été effectués
        /// </summary>
        public async void OnImportsSatisfied()
        {
            await OnLoaded();
        }

        #endregion
    }
}