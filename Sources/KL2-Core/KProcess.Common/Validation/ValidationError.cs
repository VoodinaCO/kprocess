//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : ValidationError.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Définit une erreur de validation.
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
    /// Définit une erreur de validation.
    /// </summary>
    public class ValidationError
    {
        #region Attributs

        private IList<ValidationError> _nestedErrors;

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="key">clé de la propriété en erreur</param>
        /// <param name="message">message de l'erreur</param>
        public ValidationError(string key, string message)
        {
            Key = key;
            Message = message;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="key">clé de la propriété en erreur</param>
        /// <param name="message">message de l'erreur</param>
        /// <param name="nestedErrors">erreurs imbriquées</param>
        public ValidationError(string key, string message, IEnumerable<ValidationError> nestedErrors)
            : this(key, message)
        {
            _nestedErrors = new List<ValidationError>(nestedErrors);
        }

        /// <summary>
        /// Crée une instance de <see cref="ValidationError"/> avec un accesseur à la propriété.
        /// </summary>
        /// <typeparam name="TValidable">Le type de l'objet validable.</typeparam>
        /// <typeparam name="TValue">Le type de la valeur.</typeparam>
        /// <param name="property">L'accesseur à la propriété.</param>
        /// <param name="message">message de l'erreur</param>
        /// <param name="nestedErrors">erreurs imbriquées</param>
        /// <returns>l'instance de <see cref="ValidationError"/></returns>
        public static ValidationError Create<TValidable, TValue>(System.Linq.Expressions.Expression<Func<TValidable, TValue>> property,
            string message, IEnumerable<ValidationError> nestedErrors = null)
        {
            if (nestedErrors != null)
                return new ValidationError(ReflectionHelper.GetExpressionPropertyName(property), message, nestedErrors);
            else
                return new ValidationError(ReflectionHelper.GetExpressionPropertyName(property), message);
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit la clé de la propriété en erreur
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Obtient ou définit le message d'erreur
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Obtient les erreurs imbriquées
        /// </summary>
        public IEnumerable<ValidationError> NestedErrors
        {
            get { return _nestedErrors; }
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Ajoute une erreur imbriquée
        /// </summary>
        /// <param name="nestedError">erreur à ajouter</param>
        public void Add(ValidationError nestedError)
        {
            if (_nestedErrors == null)
                _nestedErrors = new List<ValidationError>(1);

            _nestedErrors.Add(nestedError);
        }

        /// <summary>
        /// Ajoute plusieurs erreurs imbriquées
        /// </summary>
        /// <param name="nestedErrors">erreurs à ajouter</param>
        public void AddRange(IEnumerable<ValidationError> nestedErrors)
        {
            if (_nestedErrors == null)
                _nestedErrors = new List<ValidationError>(nestedErrors);
            else
                _nestedErrors.AddRange(nestedErrors);
        }

        #endregion
    }
}
