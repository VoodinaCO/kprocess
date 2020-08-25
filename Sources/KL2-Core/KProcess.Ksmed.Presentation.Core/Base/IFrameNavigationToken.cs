
namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente un jeton de navigation permettant d'executer une opération de navigation de façon asynchrone
    /// </summary>
    public interface IFrameNavigationToken
    {
        /// <summary>
        /// Execute l'opération de navigation
        /// </summary>
        void Navigate();

        /// <summary>
        /// Détermine si le jeton est valide.
        /// </summary>
        bool IsValid { get; }
    }
}
