using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit un sous-menu.
    /// </summary>
    [InheritedExport]
    public interface ISubMenuDefinition
    {

        /// <summary>
        /// Initialise le menu.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Obtient le code identifiant le menu parent.
        /// </summary>
        string ParentCode { get; }

        /// <summary>
        /// Obtient le code identifiant le menu.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Obtient le titre du menu.
        /// </summary>
        string TitleResourceKey { get; }

        /// <summary>
        /// Obtient le délégué appelé lorsque le menu est cliqué.
        /// </summary>
        Func<IServiceBus, Task<bool>> Action { get; }

        /// <summary>
        /// Obtient le type du ViewModel.
        /// </summary>
        Type ViewModelType { get; }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        Func<bool> IsEnabledDelegate { get; }

        /// <summary>
        /// Survient lorsque le titre a changé.
        /// </summary>
        event EventHandler TitleChanged;

        /// <summary>
        /// Survient lorsque la propriété IsEnabled doit être rafraîchie.
        /// </summary>
        event EventHandler IsEnabledInvalidated;

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        bool IsSecurityProjectContext { get; }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        string[] RolesCanRead { get; }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        string[] RolesCanWrite { get; }

        /// <summary>
        /// Obtient la liste des fonctionnalités permettant d'accéder à cet écran.
        /// </summary>
        short[] FeaturesCanRead { get; }

        /// <summary>
        /// Obtient la liste des fonctionnalités permettant d'écrire sur l'écran.
        /// </summary>
        short[] FeaturesCanWrite { get; }

        /// <summary>
        /// Obtient un délégué permettant de déterminer si l'utilisateur en cours peut lire.
        /// </summary>
        Func<string, bool> CustomCanRead { get; }

        /// <summary>
        /// Obtient un délégué permettant de déterminer si l'utilisateur en cours peut écrire.
        /// </summary>
        Func<string, bool> CustomCanWrite { get; }

    }
}
