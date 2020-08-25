using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Est publié lorsue le model en cours d'édition (VM typiquement) est en passe d'être validé.
    /// La souscription à cet événement permet de synchroniser les données de la vue avec le model en question.
    /// Fonctionne de pair avec l'évenement <see cref="UIUpdatedNotSynchronizedEvent"/>
    /// </summary>
    public class UIBindingSynchronizationRequested: EventBase
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        public UIBindingSynchronizationRequested(object sender)
            : base(sender)
        {

        }
    }
}
