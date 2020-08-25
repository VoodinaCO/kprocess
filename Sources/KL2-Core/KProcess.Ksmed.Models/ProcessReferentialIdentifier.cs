namespace KProcess.Ksmed.Models
{

    /// <summary>
    /// Identifie les référentiels utilisés dans un process.
    /// </summary>
    public enum ProcessReferentialIdentifier : byte
    {
        /// <summary>
        /// Opérateur
        /// </summary>
        Operator = 1,

        /// <summary>
        /// Equipement
        /// </summary>
        Equipment = 2,

        /// <summary>
        /// Catégorie
        /// </summary>
        Category = 3,

        /// <summary>
        /// Outil
        /// </summary>
        Ref1 = 4,

        /// <summary>
        /// Consommable
        /// </summary>
        Ref2 = 5,

        /// <summary>
        /// Lieu
        /// </summary>
        Ref3 = 6,

        /// <summary>
        /// Document
        /// </summary>
        Ref4 = 7,

        /// <summary>
        /// Supplémentaire 1
        /// </summary>
        Ref5 = 8,

        /// <summary>
        /// Supplémentaire 2
        /// </summary>
        Ref6 = 9,

        /// <summary>
        /// Supplémentaire 3
        /// </summary>
        Ref7 = 10,

        /// <summary>
        /// CustomTextLabel
        /// </summary>
        CustomTextLabel = 11,

        /// <summary>
        /// CustomTextLabel2
        /// </summary>
        CustomTextLabel2 = 12,

        /// <summary>
        /// CustomTextLabel3
        /// </summary>
        CustomTextLabel3 = 13,

        /// <summary>
        /// CustomTextLabel4
        /// </summary>
        CustomTextLabel4 = 14,

        /// <summary>
        /// CustomNumericLabel
        /// </summary>
        CustomNumericLabel = 21,

        /// <summary>
        /// CustomNumericLabel2
        /// </summary>
        CustomNumericLabel2 = 22,

        /// <summary>
        /// CustomNumericLabel3
        /// </summary>
        CustomNumericLabel3 = 23,

        /// <summary>
        /// CustomNumericLabel4
        /// </summary>
        CustomNumericLabel4 = 24,

        /// <summary>
        /// Compétence
        /// </summary>
        Skill = 100
    }

    /// <summary>
    /// Extension method used to retrieve type of ref identifier are we working with
    /// </summary>
    public static class ProcessReferentialIdentifierExt
    {
        public static bool IsRefs(this ProcessReferentialIdentifier identifier) =>
            GetReferentialCategory(identifier) == ReferentialCategory.Referential;

        public static bool IsCustomLabel(this ProcessReferentialIdentifier identifier) =>
            IsCustomNumericLabel(identifier) || IsCustomTextLabel(identifier);

        public static bool IsCustomNumericLabel(this ProcessReferentialIdentifier identifier) =>
            GetReferentialCategory(identifier) == ReferentialCategory.CustomNumericField;

        public static bool IsCustomTextLabel(this ProcessReferentialIdentifier identifier) =>
            GetReferentialCategory(identifier) == ReferentialCategory.CustomTextField;

        public static bool IsOthers(this ProcessReferentialIdentifier identifier) =>
            GetReferentialCategory(identifier) == ReferentialCategory.Other;

        public static ReferentialCategory GetReferentialCategory(this ProcessReferentialIdentifier identifier)
        {
            switch (identifier)
            {
                case ProcessReferentialIdentifier.Operator:
                case ProcessReferentialIdentifier.Equipment:
                case ProcessReferentialIdentifier.Category:
                    return ReferentialCategory.Other;
                case ProcessReferentialIdentifier.Ref1:
                case ProcessReferentialIdentifier.Ref2:
                case ProcessReferentialIdentifier.Ref3:
                case ProcessReferentialIdentifier.Ref4:
                case ProcessReferentialIdentifier.Ref5:
                case ProcessReferentialIdentifier.Ref6:
                case ProcessReferentialIdentifier.Ref7:
                    return ReferentialCategory.Referential;
                case ProcessReferentialIdentifier.CustomTextLabel:
                case ProcessReferentialIdentifier.CustomTextLabel2:
                case ProcessReferentialIdentifier.CustomTextLabel3:
                case ProcessReferentialIdentifier.CustomTextLabel4:
                    return ReferentialCategory.CustomTextField;
                case ProcessReferentialIdentifier.CustomNumericLabel:
                case ProcessReferentialIdentifier.CustomNumericLabel2:
                case ProcessReferentialIdentifier.CustomNumericLabel3:
                case ProcessReferentialIdentifier.CustomNumericLabel4:
                    return ReferentialCategory.CustomNumericField;
                case ProcessReferentialIdentifier.Skill:
                default:
                    return ReferentialCategory.Other;
            }
        }
    }

}
