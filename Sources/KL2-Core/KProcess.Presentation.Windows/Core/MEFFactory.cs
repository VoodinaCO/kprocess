using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Représente la fabrique UX et Dialog utilisant MEF.
    /// </summary>
    public class MEFFactory : IUXFactory, IDialogFactory
    {
        #region Attributs

        [Import]
        private CompositionContainer _container = null;

        [Import]
        private IServiceBus _serviceBus = null;

        #endregion

        #region Constructeur

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="MEFFactory"/>.
        /// </summary>
        public MEFFactory()
        {
            IoC.RegisterInstance<IUXFactory>(this);
            IoC.RegisterInstance<IDialogFactory>(this);
        }

        #endregion

        #region IUXFactory Members

        /// <summary>
        /// Retourne l'instance du viewModel demandée
        /// </summary>
        /// <typeparam name="TViewModel">type de viewModel demandé</typeparam>
        /// <returns>l'instance du viewModel demandée</returns>
        public TViewModel GetViewModel<TViewModel>()
            where TViewModel : IViewModel
        {
            // Récupère l'export depuis MEF
            var export = _container.GetExport<TViewModel, IExport>();

            // Récupère l'instance du view model
            var result = export.Value;

            // Retourne une instance du type de viewModel demandé
            return result;
        }

        /// <summary>
        /// Obtient le service gérant les thèmes.
        /// </summary>
        /// <returns>Le service gérant les thèmes ou null si le service est introuvable.</returns>
        private IThemeManagerService GetThemeManager()
        {
            return
                _serviceBus != null && _serviceBus.IsAvailable<IThemeManagerService>() ?
                    _serviceBus.Get<IThemeManagerService>() :
                    null;
        }

        /// <summary>
        /// Retourne l'instance de la vue correspondant au viewModel
        /// </summary>
        /// <param name="viewModel">instance du viewModel demandée</param>
        /// <returns>l'instance de la vue correspondant au viewModel</returns>
        public IView GetView(IViewModel viewModel)
        {
            // Récupère les vues associées au viewModel passé en paramètre
            var views = _container.GetExports<IView, IViewMetadata>().Where(vm =>
                vm.Metadata.ViewModelType.IsAssignableFrom(viewModel.GetType()));

            IView view;

            var themeManager = GetThemeManager();
            if (themeManager != null && themeManager.CurrentTheme != null)
            {
                // Récupére la première isntance de la vue dont le theme correspond
                var matchingExport = views.FirstOrDefault(vm => !string.IsNullOrEmpty(vm.Metadata.ThemeID)
                    && new Guid(vm.Metadata.ThemeID).Equals(themeManager.CurrentTheme.Id));

                // Récupére la première isntance de la vue qui n'a pas de thème défini (par défaut)
                if (matchingExport == null)
                    matchingExport = views.FirstOrDefault(vm => !string.IsNullOrEmpty(vm.Metadata.ThemeID));

                // Récupére la première isntance de la vue
                if (matchingExport == null)
                    matchingExport = views.First();

                view = matchingExport.Value;
            }
            else
            {
                // Récupére la première isntance de la vue qui n'a pas de thème défini (par défaut)
                var matchingExport = views.FirstOrDefault(vm => !string.IsNullOrEmpty(vm.Metadata.ThemeID));

                // Récupére la première isntance de la vue
                if (matchingExport == null)
                    matchingExport = views.First();

                view = matchingExport.Value;
            }


            // Assigne le dataContext
            view.DataContext = viewModel;

            // Lie la durée de vie de la vue au viewModel
            RoutedEventHandler onUnloaded = null;
            onUnloaded = new RoutedEventHandler((s, e) =>
            {
                viewModel.Shutdown();
                view.Unloaded -= onUnloaded;
            });
            view.Unloaded += onUnloaded;

            // Retourne la vue bindée
            return view;
        }


        /// <summary>
        /// Retourne l'instance de la vue correspondant au TViewModel
        /// </summary>
        /// <typeparam name="TViewModel">type de viewModel demandé</typeparam>
        /// <returns>l'instance de la vue correspondant au TViewModel</returns>
        public IView GetView<TViewModel>()
            where TViewModel : IViewModel
        {
            TViewModel viewModel;
            return GetView<TViewModel>(out viewModel);
        }


        /// <summary>
        /// Retourne l'instance de la vue correspondant au TViewModel
        /// </summary>
        /// <typeparam name="TViewModel">type de viewModel demandé</typeparam>
        /// <param name="onCreated">délégué à exécuter lors de la création du viewModel</param>
        /// <returns>l'instance de la vue correspondant au TViewModel</returns>
        public IView GetView<TViewModel>(Action<TViewModel> onCreated)
            where TViewModel : IViewModel
        {
            Assertion.NotNull(onCreated, "onCreated");


            TViewModel viewModel;
            var view = GetView<TViewModel>(out viewModel);
            onCreated(viewModel);
            return view;
        }


        /// <summary>
        /// Retourne l'instance de la vue correspondant au TViewModel
        /// </summary>
        /// <typeparam name="TViewModel">type de viewModel demandé</typeparam>
        /// <param name="viewModel">instance du viewModel créée</param>
        /// <returns>l'instance de la vue correspondant au TViewModel</returns>
        public IView GetView<TViewModel>(out TViewModel viewModel)
            where TViewModel : IViewModel
        {
            // Récupère une instance du viewModel
            var newViewModel = GetViewModel<TViewModel>();

            // Récupère l'instance de la vue correspondante
            IView view = GetView(newViewModel);

            // Assigne la sortie
            viewModel = newViewModel;

            // Retourne la vue bindée
            return view;
        }

        #endregion

        #region IDialogFactory Members

        /// <summary>
        /// Obtient une instance de la vue de dialogue demandée
        /// </summary>
        /// <typeparam name="TDialogView">type de vue de dialogue demandée</typeparam>
        /// <returns>
        /// l'instance de la vue de dialogue demandée
        /// </returns>
        public TDialogView GetDialogView<TDialogView>()
             where TDialogView : IDialogView
        {
            return _container.GetExport<TDialogView>().Value;
        }

        #endregion

    }
}

