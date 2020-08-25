//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : Disposer.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Fournit une implémentation de disposer.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace KProcess
{
    /// <summary>
    /// Fournit une implémentation de disposer.
    /// </summary>
    public class Disposer : IDisposable, IDisposer
    {
        #region Attributs

        private bool _isDisposed;
        private IList<WeakReference> _items;
        private IList<Action> _actions;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        public Disposer()
        {
            _items = new List<WeakReference>();
            _actions = new List<Action>();
        }

        #endregion

        #region IDisposer members

        /// <summary>
        /// Enregistre un disposable auprès du disposer
        /// </summary>
        /// <param name="disposable">disposable à enregistrer</param>
        /// <returns>l'instance du disposer</returns>
        public IDisposer Register(IDisposable disposable)
        {
            _items.Add(new WeakReference(disposable));
            return this;
        }

        /// <summary>
        /// S'enregistre auprès du disposer
        /// </summary>
        /// <param name="removeAction">action à exécuter lors du Dispose</param>
        /// <returns>l'instance du disposer</returns>
        public IDisposer Register(Action removeAction)
        {
            _actions.Add(removeAction);
            return this;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Nettoie les resources
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                // Parcourt toutes les actions de nettoyage à exécuter
                while (_actions.Count > 0)
                {
                    var action = _actions[0];
                    action();
                    _actions.RemoveAt(0);
                }

                // Parcourt toutes les instances de référence faible pour les disposer
                while (_items.Count > 0)
                {
                    WeakReference wr = _items[0];
                    if (wr.IsAlive)
                    {
                        ((IDisposable)wr.Target).Dispose();
                        wr.Target = null;
                    }

                    _items.RemoveAt(0);
                }
            }
        }

        #endregion
    }
}
