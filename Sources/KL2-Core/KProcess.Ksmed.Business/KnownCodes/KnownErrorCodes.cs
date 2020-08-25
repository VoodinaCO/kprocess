using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Contient les codes d'erreurs que la couche business peut lever.
    /// </summary>
    public static class KnownErrorCodes
    {

        /// <summary>
        /// Impossible de valider plus d'un scénario de même nature dans un projet.
        /// </summary>
        public const int CannotValidateMoreThanOneScenarioOfSameNature = 1;

        /// <summary>
        /// Impossible de valider plus d'un scénario de même nature dans un projet.
        /// </summary>
        public const string CannotValidateMoreThanOneScenarioOfSameNature_ScenarioNameKey = "ScenarioName";

        /// <summary>
        /// Impossible de supprimer la vidéo car elle a des actions associées
        /// </summary>
        public const int CannotDeleteVideoWithRelatedActions = 2;

        /// <summary>
        /// Impossible d'enregistrer un utilisateur car il y a déjà un avec ce nom.
        /// </summary>
        public const int CannotUseSameUserName = 2;

        /// <summary>
        /// Impossible d'enregistrer un utilisateur car il y a déjà un avec ce nom.
        /// </summary>
        public const string CannotUseSameUserName_UsersKey = "Users";
        
        /// <summary>
        /// Impossible d'invalider un scénario cible s'il existe un scénario de validation figé
        /// </summary>
        public const int CannotInvalidateAScenarioWhenHavingRealizedScenario = 3;


        /// <summary>
        /// Impossible d'afficher un scénario dans la synthèse quand le nombre max est déjà atteint.
        /// </summary>
        public const int CannotShowScenarioInSummaryWhenMaxReached = 4;

        /// <summary>
        /// La clé pour récupérer le nom des scénarios déjà affichés.
        /// </summary>
        public const string CannotShowScenarioInSummaryWhenMaxReached_ScenarioNamesKey = "ScenarioNames";

        /// <summary>
        /// Le nom du référentiel est déjà utilisé.
        /// </summary>
        public const int ReferentialNameAlreadyUsed = 3;

        /// <summary>
        /// Erreur code correspondant a une exception retourne par EF (UpdateException)
        /// </summary>
        public const int UpdateException = 5;

    }
}
