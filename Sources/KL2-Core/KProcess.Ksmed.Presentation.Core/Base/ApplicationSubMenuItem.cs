using System;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente un élément de sous menu.
    /// </summary>
    public class ApplicationSubMenuItem : ApplicationMenuItemBase
    {

        /// <summary>
        /// Obtient ou définit l'action à exécuter lorsque le MenuItem est cliqué.
        /// </summary>
        public Func<IServiceBus, Task<bool>> Action { get; set; }

        private Func<bool> _isEnabledDelegate;
        /// <summary>
        /// Obtient ou définit un délégué permettant de définir dynamiquement si l'élément est activé.
        /// </summary>
        public Func<bool> IsEnabledDelegate
        {
            get { return _isEnabledDelegate; }
            set
            {
                if (_isEnabledDelegate != value)
                {
                    _isEnabledDelegate = value;
                    this.InvalidateIsEnabled();
                }
            }
        }

        /// <summary>
        /// Invalide la propriété IsEnabled.
        /// </summary>
        public void InvalidateIsEnabled()
        {
            if (IsEnabledDelegate != null)
            {
                this.IsEnabled = IsEnabledDelegate();
            }
        }

    }
}
