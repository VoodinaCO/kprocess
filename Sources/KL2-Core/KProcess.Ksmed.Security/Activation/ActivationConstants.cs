namespace KProcess.Ksmed.Security.Activation
{
    /// <summary>
    /// Contient des constantes.
    /// </summary>
    public static class ActivationConstants
    {
        /// <summary>
        /// Le nom du produit.
        /// </summary>
        public const string WebProductName = "KL² Suite";

        /// <summary>
        /// Le nom du produit.
        /// </summary>
        public const string WebUsersPoolName = "KL2UsersPool";

        /// <summary>
        /// Le nom du produit.
        /// </summary>
        public const string ProductName = "KL²";

        /// <summary>
        /// L'identifiant du produit.
        /// </summary>
        public const short ProductId = 2;

        /// <summary>
        /// L'identifiant du produit.
        /// </summary>
        public const short WebProductId = 4;

        /// <summary>
        /// L'extension par défaut de la clé d'activation. 
        /// Sans le point "."
        /// </summary>
        public const string DefaultKeyExtension = "ksk";

        /// <summary>
        /// L'extension par défaut de la clé d'activation. 
        /// Sans le point "."
        /// </summary>
        public const string DefaultWebKeyExtension = "kok";

        /// <summary>
        /// Le filtre par défaut pour les dialogs du fichier contenant la clé d'activation. 
        /// </summary>
        public const string DefaultKeyFilter = "Ksmed Key (*.ksk)|*.ksk";

        /// <summary>
        /// Le filtre par défaut pour les dialogs du fichier contenant la clé d'activation. 
        /// </summary>
        public const string DefaultWebKeyFilter = "KL² Web Key (*.kok)|*.kok";

        /// <summary>
        /// L'adresse email à laquelle envoyer les demandes d'activation.
        /// </summary>
        public const string KProcessEmail = "KL2Suite@k-process.com";

    }
}
