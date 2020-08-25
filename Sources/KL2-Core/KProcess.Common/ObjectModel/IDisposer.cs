//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : IDisposer.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Définit l'interface d'un disposer.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------

using System;

namespace KProcess
{
    /// <summary>
    /// Définit l'interface d'un disposer.
    /// </summary>
    public interface IDisposer : IDisposable
    {
        /// <summary>
        /// S'enregistre auprès du disposer
        /// </summary>
        /// <param name="removeAction">action à exécuter lors du Dispose</param>
        /// <returns>l'instance du disposer</returns>
        IDisposer Register(Action removeAction);

        /// <summary>
        /// Enregistre un disposable auprès du disposer
        /// </summary>
        /// <param name="disposable">disposable à enregistrer</param>
        /// <returns>l'instance du disposer</returns>
        IDisposer Register(IDisposable disposable);
    }
}
