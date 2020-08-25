//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : WeakDelegate.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Permet de stocker un délégué associé à une référence faible, donc collectable par le GC.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*
//------------------------------------------------------------------------------

using System;
using System.Reflection;

namespace KProcess
{
    /// <summary>
    /// Permet de stocker un délégué associé à une référence faible, donc collectable par le GC.
    /// </summary>
    public class WeakDelegate
    {
        #region Attributs

        private readonly Type _delegateType;
        private readonly MethodInfo _method;
        private readonly WeakReference _targetRef;

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="item">Délégué pour lequel la référence est créé.</param>
        public WeakDelegate(Delegate item)
        {
            Assertion.NotNull(item, "item");
            
            _targetRef = new WeakReference(item.Target);
            _method = item.Method;
            _delegateType = item.GetType();
        }

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="item">Délégué pour lequel la référence est créé.</param>
        /// <param name="target">Cible du délégué.</param>
        public WeakDelegate(Delegate item, object target)
        {
            Assertion.NotNull(item, "item");

            _targetRef = new WeakReference(target);
            _method = item.Method;
            _delegateType = item.GetType();
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient l'instance de la classe sur lequel le délégué est invoqué.
        /// </summary>
        /// <remarks>Peut être null, alors soit la référence n'existe plus soit il s'agissait d'une méthode statique.</remarks>
        public object Target
        {
            get
            {
                if (_targetRef == null || !_targetRef.IsAlive)
                    return null;

                return _targetRef.Target;
            }
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Retourne le délégué à invoquer.
        /// </summary>
        /// <returns>Le délégué à invoquer ou null si la référence faible n'a plus lieue d'être.</returns>
        public Delegate GetDelegate()
        {
            // Si la référence existe encore, on crée un délégué lui étant associé
            if (Target != null)
                return Delegate.CreateDelegate(_delegateType, _targetRef.Target, _method);
            // S'il s'agit d'une méthode statique, on crée le délégué sans référence particulière
            else if (_method.IsStatic)
                return Delegate.CreateDelegate(_delegateType, null, _method);

            // En cas de référence null, on ne renvoie rien
            return null;
        }

        #endregion
    }
}
