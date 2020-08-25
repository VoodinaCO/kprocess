using KProcess.Presentation.Windows;
using System;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Definit le comportement d'un service d'affichage de spinner.
    /// </summary>
    public interface ISpinnerService : IPresentationService
    {
        /// <summary>
        /// Initialise le service.
        /// </summary>
        /// <param name="container">Le conteneur à partir duquel sera affiché le spinner.</param>
        /// <param name="spinnerTemplate">Le DataTemplate utilisé pour afficher le spinner.</param>
        /// <param name="defaultMessage">Le message par défaut à afficher.</param>
        void Initialize(UIElement container, DataTemplate spinnerTemplate, string defaultMessage);

        /// <summary>
        /// Obtient ou définit l'objet suivant les progressions.
        /// </summary>
        Progress<double> Progress { get; set; }

        /// <summary>
        /// Obtient ou définit si le spinner est visible.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Affiche le spinner.
        /// Incrémente le compteur.
        /// </summary>
        /// <param name="message">Le message à afficher.</param>
        void ShowIncrement(string message = null);

        /// <summary>
        /// Change le message du spinner.
        /// </summary>
        /// <param name="message">Le message à afficher.</param>
        void SetMessage(string message = null);

        /// <summary>
        /// Décrément le compteur.
        /// Cache le spinner lorsque le compteur retourne à sa valeur d'origine.
        /// </summary>
        void HideDecrement();

        /// <summary>
        /// Remet à zéro le compteur, cachant le spinner s'il y en a.
        /// </summary>
        void Reset();
    }
}
