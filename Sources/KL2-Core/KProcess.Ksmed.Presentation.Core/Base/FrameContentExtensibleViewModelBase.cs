using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente un VM de base qui est une frame de l'application gérant en plus la barre d'actions extensible.
    /// </summary>
    /// <typeparam name="TViewModel">Le type du ViewModel.</typeparam>
    /// <typeparam name="TIViewModel">Le type de l'interface du ViewModel.</typeparam>
    public abstract class FrameContentExtensibleViewModelBase<TViewModel, TIViewModel> : FrameContentViewModelBase
        where TViewModel : FrameContentExtensibleViewModelBase<TViewModel, TIViewModel>, TIViewModel
        where TIViewModel : IFrameContentViewModel
    {

        private IExtBarAction<TIViewModel>[] _extBarActions;
        /// <summary>
        /// Obtient ou définit les actions de la barre d'extensibilités.
        /// </summary>
        [ImportMany(AllowRecomposition = false)]
        public IExtBarAction<TIViewModel>[] ExtBarActions
        {
            get { return _extBarActions; }
            set
            {
                // On prend le service bus depuis l'IoC et pas base.ServiceBus car la propriété est aussi alimentée par MEF.
                // On ne peut être sûrs que ServiceBus soit définit avant ExtBarActions.
                var service = IoC.Resolve<IServiceBus>().Get<IExtensionsService>();

                _extBarActions = value.Where(e => service.IsExtensionEnabledAndVersionValid(e.ExtensionId)).ToArray();
                if (value != null)
                {
                    foreach (var ext in value)
                        ext.ViewModel = (TViewModel)this;
                }
            }
        }

    }
}
