// -----------------------------------------------------------------------
// <copyright file="ViewModelStateEnum.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit les états possibles d'un ViewModel
    /// </summary>
    public enum ViewModelStateEnum
    {
        /// <summary>
        /// Le ViewModel est inactif
        /// </summary>
        Inactive,

        /// <summary>
        /// Le ViewModel est en erreur
        /// </summary>
        Failed,

        /// <summary>
        /// Le ViewModel est en cours de chargement
        /// </summary>
        Loading,

        /// <summary>
        /// Le ViewModel est en cours de réactualisation d'une ou plusieurs propriétés
        /// </summary>
        Refreshing,

        /// <summary>
        /// Le ViewModel attend une action de l'utilisateur
        /// </summary>
        Waiting,

        /// <summary>
        /// Le ViewModel est prêt à l'emploi 
        /// </summary>
        Ready,

        /// <summary>
        /// Le ViewModel est en train de se fermer.
        /// </summary>
        ShuttingDown
    };
}
