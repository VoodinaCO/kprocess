//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : ObjectExtensions.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Regroupe les méthodes d'extension de la classe Object.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------
namespace KProcess
{
    /// <summary>
    /// Regroupe les méthodes d'extension de la classe Object.
    /// </summary>
    public static class ObjectExtensions
    {
        #region Méthodes publiques

        /// <summary>
        /// Retourne un représentation sûre de l'élément, même si l'instance est null.
        /// </summary>
        /// <param name="item">Elément à vérifier.</param>
        /// <returns>La représentation de l'élément ou {null}</returns>
        public static string SafeToString(this object item)
        {
            return (item != null) ? item.ToString() : "{null}";
        }

        #endregion
    }
}
