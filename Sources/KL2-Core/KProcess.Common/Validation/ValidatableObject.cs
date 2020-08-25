using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace KProcess
{
    /// <summary>
    /// Définit un objet validable.
    /// </summary>
    [HasSelfValidation]
    [DataContract(IsReference = true)]
    [Serializable]
    public abstract class ValidatableObject : NotifiableObject, IDataErrorInfo
    {
        #region Attributs

        bool _isValidating;
        Validator _validator;
        ValidationResults _validationResults;
        bool _enableAutoValidation;

        #endregion

        #region IDataErrorInfo Members

        /// <summary>
        /// Obtient un message d'erreur indiquant ce qui est incorrect dans cet objet.
        /// </summary>
        public string Error =>
            (_validationResults == null || _validationResults.IsValid) ? string.Empty : _validationResults.First().Message;

        /// <summary>
        /// Obtient les erreurs de validation.
        /// </summary>
        /// <value>Une énumération des erreurs de validation.</value>
        [IgnoreDataMember]
        public IEnumerable<ValidationError> Errors =>
            (_validationResults == null || _validationResults.IsValid) ? null : _validationResults.Select(vr => GetValidationError(string.Empty, vr)).ToList();

        /// <summary>
        /// Obtient l'erreur de validation pour la clé et les résultat spécifiés.
        /// </summary>
        /// <param name="parentKey">La clé de l'erreur.</param>
        /// <param name="validationResult">Les résultats de validation.</param>
        /// <returns>L'erreur de validation associée.</returns>
        ValidationError GetValidationError(string parentKey, ValidationResult validationResult)
        {
            string key = string.IsNullOrEmpty(parentKey) ? validationResult.Key : "{parentKey}.{validationResult.Key}";

            ValidationError result = new ValidationError(key, validationResult.Message);

            validationResult.NestedValidationResults.ForEach(vr => result.Add(GetValidationError(result.Key, vr)));

            return result;
        }

        /// <summary>
        /// Obtient une concaténation des messages d'erreur pour le nom du champ spécifié.
        /// </summary>
        public virtual string this[string columnName]
        {
            get
            {
                // L'objet n'a pas encore été validé
                if (_validationResults == null)
                    return string.Empty;

                ValidationResults filteredResults = new ValidationResults();

                filteredResults.AddAllResults(_validationResults.Where(vr => vr.Key == columnName));

                if (!filteredResults.IsValid)
                {
                    //On renvoie la concaténation de toutes les erreurs sur la propriété "columnName"
                    IEnumerable<string> messages = filteredResults
                      .Flatten(vr => vr.NestedValidationResults)
                      .Where(vr => vr.NestedValidationResults == null || !vr.NestedValidationResults.Any())//on prend seulement les feuilles de l'arbre
                      .Select(vr => vr.Message);

                    return string.Join("|", messages);
                }

                return string.Empty;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Déclenche l'événement <see cref="INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Le nom de la propriété.</param>
        public override void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            OnPropertyChanged(propertyName, null);


        /// <summary>
        /// Déclenche l'événement <see cref="INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Le nom de la propriété.</param>
        /// <param name="checkState">
        /// Si définit à <c>true</c>, valide l'objet. 
        /// Si définit à <c>false</c>, ne valide pas l'objet.
        /// Si définit à <c>null</c>, valide l'objet en fonction de <see cref="EnableAutoValidation"/>.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName, bool? checkState)
        {
            base.OnPropertyChanged(propertyName);

            if ((!checkState.HasValue && EnableAutoValidation) || (checkState.HasValue && checkState.Value))
                UpdateValidationState();
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient une valeur indiquant si l'objet est valide.
        /// </summary>
        /// <remarks>
        /// Une valeur <c>null</c> indique que la validation est en cours ou n'a jamais été évaluée.
        /// </remarks>
        public bool? IsValid
        {
            get
            {
                if (_isValidating || _validationResults == null)
                    return null;

                return _validationResults.IsValid;
            }
        }

        /// <summary>
        /// Indique si le model est en cours de validation
        /// </summary>
        protected bool IsValidating
        {
            get { return _isValidating; }
            private set
            {
                if (_isValidating != value)
                {
                    _isValidating = value;
                    OnIsValidChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la validation automatique après un PropertyChanged est activée.
        /// La valeur par défaut est <c>false</c>.
        /// Définir cette valeur à <c>true</c> valide immédiatement l'objet.
        /// </summary>
        [NotMapped]
        public bool EnableAutoValidation
        {
            get { return _enableAutoValidation; }
            set
            {
                if (_enableAutoValidation != value)
                {
                    _enableAutoValidation = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Définit un délégué capable de récupérer les erreurs de la validation externe.
        /// </summary>
        public Func<IList<KeyValuePair<string, string>>> ExternalValidation
        {
            private get;
            set;
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Exécute la validation personnalisée.
        /// </summary>
        /// <param name="results">Les résultats de la validation automatique.</param>
        [SelfValidation]
        internal void CustomValidate(ValidationResults results)
        {
            IEnumerable<ValidationError> internalValidation = OnCustomValidate();

            if (internalValidation != null)
                internalValidation.ForEach(kv => results.AddResult(new ValidationResult(kv.Message, this, kv.Key, null, null)));

            if (ExternalValidation != null)
            {
                var errors = ExternalValidation();
                if (errors != null && errors.Count > 0)
                {
                    foreach (KeyValuePair<string, string> e in errors)
                        results.AddResult(new ValidationResult(e.Value, this, e.Key, null, null));
                }
            }
        }

        #endregion

        #region Constructeur

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ValidatableObject"/>.
        /// </summary>
        protected ValidatableObject()
        {
            _validator = ValidationFactory.CreateValidator(GetType());
        }

        #endregion

        #region Gestion de la déserialisation

        /// <summary>
        /// Appelé lorsque l'objet commence à être désérialisé.
        /// </summary>
        protected override void OnDeserializing()
        {
            base.OnDeserializing();

            if (_validator == null)
                _validator = ValidationFactory.CreateValidator(GetType());
        }

        /// <summary>
        /// Appelé lorsque l'objet est désérialisé.
        /// </summary>
        protected override void OnDeserialized()
        {
            base.OnDeserialized();

            // Si l'objet est déserialisé, il y a de fortes chances pour qu'il soit rempli, il faut alors vérifier son état
            //InternalUpdateValidationState();
        }

        #endregion

        #region Validation

        /// <summary>
        /// Valide l'état du model de façon synchrone
        /// </summary>
        public void Validate() =>
            InternalUpdateValidationState();

        /// <summary>
        /// Remet à zéro l'état de validation.
        /// </summary>
        public void ResetValidationState()
        {
            _validationResults = null;
            IsValidating = false;
        }

        /// <summary>
        /// Valide l'état du model de façon synchrone
        /// </summary>
        protected virtual void UpdateValidationState() =>
            InternalUpdateValidationState();

        /// <summary>
        /// Valide l'état du model de façon synchrone
        /// </summary>
        void InternalUpdateValidationState()
        {
            IsValidating = true;

            try
            {
                bool? wasValid = null;

                // Stocke les propriétés qui étaient en erreur
                IList<string> invalidKeys = null;

                // Récupère les propriétés qui étaient en erreur
                if (_validationResults != null)
                {
                    invalidKeys = _validationResults.Select(vr => vr.Key).ToList();

                    if (!invalidKeys.Any())
                        invalidKeys = null;

                    wasValid = _validationResults.IsValid;
                }

                // Valide l'objet
                _validationResults = _validator.Validate(this);

                if (invalidKeys != null)
                {
                    // Parcourt les propriétés qui étaient en erreur auparavant
                    foreach (string key in invalidKeys)
                    {
                        // S'il y a eu un changement d'état, il faut en prévenir les bindings
                        if (!_validationResults.Any(vr => vr.Key == key))
                            OnPropertyChanged(key, false);
                    }
                }

                if (!_validationResults.IsValid)
                {
                    // Parcourt des propriétés qui sont en erreur
                    foreach (var result in _validationResults.Where(vr => vr.Target == this))
                        OnPropertyChanged(result.Key, false);
                }

                // Si l'objet change d'état de validité, on prévient les bindings
                if (!wasValid.HasValue || wasValid.Value != _validationResults.IsValid)
                    OnIsValidChanged();

                if (!(invalidKeys == null && _validationResults.IsValid))
                {
                    OnPropertyChanged(nameof(Error), false);
                    OnPropertyChanged(nameof(Errors), false);
                }
            }
            finally
            {
                IsValidating = false;
            }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="IsValid"/> a changé.
        /// </summary>
        protected virtual void OnIsValidChanged() =>
            OnPropertyChanged(nameof(IsValid), false);

        /// <summary>
        /// Lorsque surchargé dans une classe fille, cette méthode sert à exécuter une validation personnalisée.
        /// </summary>
        /// <returns>Une énumération des erreurs de validation, ou null s'il n'y en a pas.</returns>
        protected virtual IEnumerable<ValidationError> OnCustomValidate() =>
            Enumerable.Empty<ValidationError>();

        #endregion
    }
}