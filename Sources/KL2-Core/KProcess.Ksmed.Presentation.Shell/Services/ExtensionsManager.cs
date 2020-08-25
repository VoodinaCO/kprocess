using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Presentation.Core;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Définit le gestionnaire d'extensions.
    /// </summary>
    internal class ExtensionsManager : IExtensionsManager
    {
        private ExtensionDescription[] _extensions;

        /// <summary>
        /// Détermine si l'extension spécifiée est activée et si la version requise est valide.
        /// </summary>
        /// <param name="extensionId">L'identifiant de l'extension.</param>
        /// <returns>
        ///   <c>true</c> si l'extension est activée; sinon, <c>false</c>.
        /// </returns>
        public bool IsExtensionEnabledAndVersionValid(Guid extensionId)
        {
            if (!((IExtensionsManager)this).IsExtensionEnabled(extensionId))
                return false;
            else
                return _extensions.First(e => e.Extension.ExtensionId == extensionId).IsVersionValid;
        }

        /// <summary>
        /// Détermine si l'extension spécifiée est activée.
        /// </summary>
        /// <param name="extensionId">L'identifiant de l'extension.</param>
        /// <returns>
        ///   <c>true</c> si l'extension est activée; sinon, <c>false</c>.
        /// </returns>
        bool IExtensionsManager.IsExtensionEnabled(Guid extensionId)
        {
            if (LocalSettings.Instance.DisabledExtensions != null)
                return !LocalSettings.Instance.DisabledExtensions.Contains(extensionId);
            else
                return true;
        }

        /// <summary>
        /// Active ou désactive une extension.
        /// </summary>
        /// <param name="extensionId">L'identifiant de l'extension.</param>
        /// <param name="isEnabled"><c>true</c> si l'extension est à activer.</param>
        void IExtensionsManager.SetExtensionEnabled(Guid extensionId, bool isEnabled)
        {
            if (LocalSettings.Instance.DisabledExtensions == null)
                LocalSettings.Instance.DisabledExtensions = new List<Guid>();

            if (isEnabled)
                LocalSettings.Instance.DisabledExtensions.Remove(extensionId);
            else
                LocalSettings.Instance.DisabledExtensions.AddNew(extensionId);

            RefreshExtensionsIsEnabled();
        }

        /// <summary>
        /// Définit la validité et l'état activé des extensions.
        /// </summary>
        /// <param name="extensions">Les extensions.</param>
        void IExtensionsManager.SetExtensions(IEnumerable<ExtensionDescription> extensions)
        {
            _extensions = extensions.ToArray();
        }

        /// <summary>
        /// Obtient les extensions de l'application.
        /// </summary>
        /// <returns>Les extensions.</returns>
        IEnumerable<ExtensionDescription> IExtensionsManager.GetExtensions()
        {
            return _extensions;
        }

        /// <summary>
        /// Rafraichit l'état IsEnabled des extensions.
        /// </summary>
        private void RefreshExtensionsIsEnabled()
        {
            _extensions = _extensions.Select(e =>
                new ExtensionDescription(e.Extension, e.IsVersionValid,
                    ((IExtensionsManager)this).IsExtensionEnabled(e.Extension.ExtensionId)))
                .ToArray();
        }


        /// <summary>
        /// Sauvegarde l'état désactivé des extensions.
        /// </summary>
        void IExtensionsManager.SaveEnabledStates()
        {
            LocalSettings.Instance.Save();
        }
    }
}
