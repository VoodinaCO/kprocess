//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : IDisposableExtensions.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Regroupe les méthodes d'extension de la classe IDisposable.
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
    /// Regroupe les méthodes d'extension de la classe IDisposable.Regroupe les méthodes d'extension de la classe IDisposable.
    /// </summary>
    public static class IDisposableExtensions
    {
        #region Méthodes publiques

        /// <summary>
        /// Permet de disposer un IDisposable avec un disposer
        /// </summary>
        /// <typeparam name="TDisposable">type de disposable</typeparam>
        /// <param name="disposable">disposable à associer</param>
        /// <param name="disposer">disposer à associer</param>
        /// <returns>l'instance du disposable</returns>
        public static TDisposable DisposeWith<TDisposable>(this TDisposable disposable, IDisposer disposer)
            where TDisposable : IDisposable
        {
            disposer.Register(disposable);

            return disposable;
        }

        #endregion
    }
}
