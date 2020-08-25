using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Spécifie les boutons affichés dans la boîte de message.
    /// </summary>
    public enum MessageDialogButton
    {
        /// <summary>
        /// La boîte de message affiche un bouton OK
        /// </summary>
        OK,

        /// <summary>
        /// La boîte de message affiche les boutons OK et Annuler
        /// </summary>
        OKCancel,

        /// <summary>
        /// La boîte de message affiche les boutons Oui, Non et Annuler
        /// </summary>
        YesNoCancel,

        /// <summary>
        /// La boîte de message affiche les boutons Oui et Non
        /// </summary>
        YesNo,

        /// <summary>
        /// Exit application
        /// </summary>
        CloseApp,
    }

    /// <summary>
    /// Spécifie l'icône à afficher dans la boite de message.
    /// </summary>
    public enum MessageDialogImage
    {
        /// <summary>
        /// Aucune icône n'est affichée
        /// </summary>
        None,

        /// <summary>
        /// La boîte de message affiche une îcone d'erreur
        /// </summary>
        Error,

        /// <summary>
        /// La boîte de message affiche une îcone en forme de main
        /// </summary>
        Hand,

        /// <summary>
        /// La boîte de message affiche une îcone d'arrêt
        /// </summary>
        Stop,

        /// <summary>
        /// La boîte de message affiche une îcone en forme de point d'interrogation
        /// </summary>
        Question,

        /// <summary>
        /// La boîte de message affiche une îcone en forme de point d'exclamation
        /// </summary>
        Exclamation,

        /// <summary>
        /// La boîte de message affiche une îcone d'avertissement
        /// </summary>
        Warning,

        /// <summary>
        /// La boîte de message affiche une îcone d'information
        /// </summary>
        Information,

        /// <summary>
        /// La boîte de message affiche une îcone en forme d'astérisque
        /// </summary>
        Asterisk,
    }

    /// <summary>
    /// Spécifie le bouton de la boîte de message sur lequel l'utilisateur a cliqué
    /// </summary>
    public enum MessageDialogResult
    {
        /// <summary>
        /// La boîte de message ne retourne aucun résultat
        /// </summary>
        None,

        /// <summary>
        /// La valeur de retour de la boîte de message est OK
        /// </summary>
        OK,

        /// <summary>
        /// La valeur de retour de la boîte de message est Annuler
        /// </summary>
        Cancel,

        /// <summary>
        /// La valeur de retour de la boîte de message est Valider
        /// </summary>
        Yes,

        /// <summary>
        /// La valeur de retour de la boîte de message est Non
        /// </summary>
        No,
    }
}
