using KProcess.Presentation.Windows;
using System.Windows;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Se produit lorsque l'UI a été modifié sans que le ViewModel ait été encore synchronisé (TextBox qui n'a pas perdu le focus par exemple).
    /// Nécessite typiquement l'affichage des boutons de sauvegardes qui gèrent eux même la synchronisation avant la sauvegarde
    /// </summary>
    public class UIUpdatedNotSynchronizedEvent: EventBase
    {
        private DependencyObject _focusedElement;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        /// <param name="focusedElement">L'element qui porte le focus au moment de l'évenement</param>
        public UIUpdatedNotSynchronizedEvent(object sender, DependencyObject focusedElement)
            : base(sender)
        {
            _focusedElement = focusedElement;
        }

        /// <summary>
        /// Détermine si l'objet UI concerné porte toujours le focus
        /// </summary>
        /// <returns></returns>
        public bool StillHasFocus()
        {
            try
            {
                var focusScope = FocusManager.GetFocusScope(_focusedElement);
                return FocusManager.GetFocusedElement(focusScope) == _focusedElement;
            }
            catch
            {
                return false;
            }
        }
    }
}
