//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : MatchEverythingButProperties.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Matching rule permettant d'intercepter toutes méthodes d'une classe concrète et n'étant pas une propriété.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------
using Microsoft.Practices.Unity.InterceptionExtension;

namespace KProcess
{
    /// <summary>
    /// Matching rule permettant d'intercepter toutes méthodes d'une classe concrète et n'étant pas une propriété.
    /// </summary>
    public class MatchEverythingButProperties : IMatchingRule
    {
        #region IMatchingRule members

        /// <summary>
        /// Tests to see if this rule applies to the given member.
        /// </summary>
        /// <param name="member">Member to test.</param>
        /// <returns>
        /// true if the rule applies, false if it doesn't.
        /// </returns>
        public bool Matches(System.Reflection.MethodBase member)
        {
            // Ecarte les interfaces et supprime les propriétés
            return (!member.DeclaringType.IsInterface && !member.IsSpecialName);
        }

        #endregion
    }
}