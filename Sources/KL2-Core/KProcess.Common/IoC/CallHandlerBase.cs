//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : CallHandlerBase.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Classe de base des intercepteurs de méthode Unity.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------

using Microsoft.Practices.Unity.InterceptionExtension;
using System;

namespace KProcess
{
    /// <summary>
    /// Classe de base des intercepteurs de méthode Unity.
    /// </summary>
    public abstract class CallHandlerBase : ICallHandler
    {
        #region Protected Methods
        /// <summary>
        /// Méthode de traitement avant appel
        /// </summary>
        /// <param name="pInput">Description de l'appel intercepté</param>
        /// <returns>null pour poursuivre le traitement (défaut), tout autre valeur intercepte l'appel</returns>
        protected virtual IMethodReturn PreProcessHandler(IMethodInvocation pInput)
        {
            return null;
        }
        /// <summary>
        /// Méthode de traitement après appel
        /// </summary>
        /// <param name="pInput">Description de l'appel intercepté</param>
        /// <param name="pReturn">Description du résultat de l'appel</param>
        /// <returns>Résultat final de l'appel</returns>
        protected virtual IMethodReturn PostProcessHandler(IMethodInvocation pInput, IMethodReturn pReturn)
        {
            return pReturn;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Méthode de traitement avant appel
        /// </summary>
        /// <param name="pInput">Description de l'appel intercepté</param>
        /// <returns>null pour poursuivre le traitement (défaut), tout autre valeur intercepte l'appel</returns>
        private IMethodReturn PreProcessHandlerInternal(IMethodInvocation pInput)
        {
            try
            {
                return PreProcessHandler(pInput);
            }
            catch (Exception ex)
            {
                // En cas d'exception non gérée, la définir comme message de retour
                return pInput.CreateExceptionMethodReturn(ex);
            }
        }
        /// <summary>
        /// Méthode de traitement après appel
        /// </summary>
        /// <param name="pInput">Description de l'appel intercepté</param>
        /// <param name="pReturn">Description du résultat de l'appel</param>
        /// <returns>Résultat final de l'appel</returns>
        private IMethodReturn PostProcessHandlerInternal(IMethodInvocation pInput, IMethodReturn pReturn)
        {
            try
            {
                return PostProcessHandler(pInput, pReturn);
            }
            catch (Exception ex)
            {
                // En cas d'exception non gérée, la définir comme message de retour
                return pInput.CreateExceptionMethodReturn(ex);
            }
        }
        #endregion

        #region ICallHandler Membres

        /// <summary>
        /// Méthode déclenchée lors de l'appel à la méthode interceptée
        /// </summary>
        /// <param name="input">Paramètre de description de l'appel de méthode intercepté</param>
        /// <param name="getNext">Délégué permettant d'obtenir l'handler suivant pour poursuivre la chaîne d'interception</param>
        /// <returns>Description du résultat de l'appel</returns>
        IMethodReturn ICallHandler.Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            // Traitement avant appel
            IMethodReturn mReturn = PreProcessHandlerInternal(input);
            // Si le retour est non null => appel intercepté ne pas faire l'appel ni de post traitement
            if (mReturn == null)
            {
                // Appel non intercepté, poursuivre la chaîne d'interception
                mReturn = getNext()(input, getNext);
                // Après l'appel, effectuer le post traitement. Le retour du handler sera le retour du post traitement si défini,
                // sinon ce sera le retour de l'appel
                mReturn = PostProcessHandlerInternal(input, mReturn) ?? mReturn;
            }

            return mReturn;
        }

        /// <summary>
        /// CallHandler order
        /// </summary>
        int ICallHandler.Order
        {
            get;
            set;
        }

        #endregion
    }
}
