using KProcess.Business;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Représente un référentiel de base.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ReferentialBase : ModelBase
    {
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'audit ne doit pas être géré automatiquement.
        /// </summary>
        public bool CustomAudit =>
            false;

        /// <summary>
        /// Obtient une valeur indiquant si l'utilisateur a été validé et s'il est valide.
        /// </summary>
        public bool IsValidatedAndIsNotValid =>
            IsValid.HasValue && !IsValid.Value;

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ValidatableObject.IsValid"/> a changé.
        /// </summary>
        protected override void OnIsValidChanged()
        {
            base.OnIsValidChanged();
            base.OnPropertyChanged(nameof(IsValidatedAndIsNotValid), false);
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'instance est liée à des actions.
        /// </summary>
        [NotMapped]
        public bool HasRelatedActions { get; set; }

        bool _isEditable;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le référentiel est modifiable.
        /// </summary>
        [NotMapped]
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                if (_isEditable != value)
                {
                    _isEditable = value;
                    OnIsEditableChanged();
                    OnPropertyChanged();
                }
            }
        }

        bool _isSelected;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'élément est sélectionné.
        /// </summary>
        [NotMapped]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnIsSelectedChanged();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Survient lorsque la valeur de le propriété <see cref="IsSelected"/> a changé.
        /// </summary>
        public event EventHandler IsSelectedChanged;

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="IsSelected"/> a changé.
        /// </summary>
        protected virtual void OnIsSelectedChanged() =>
            IsSelectedChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Survient lorsque la valeur de le propriété <see cref="IsEditable"/> a changé.
        /// </summary>
        public event EventHandler IsEditableChanged;

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="IsEditable"/> a changé.
        /// </summary>
        protected virtual void OnIsEditableChanged() =>
            IsEditableChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Obtient ou définit la quantité de référentiels.
        /// </summary>
        int _quantity;
        [NotMapped]
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnQuantityChanged();
                }
            }
        }

        /// <summary>
        /// Survient lorsque la valeur de le propriété <see cref="Quantity"/> a changé.
        /// </summary>
        public event EventHandler QuantityChanged;

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Quantity"/> a changé.
        /// </summary>
        protected virtual void OnQuantityChanged() =>
            QuantityChanged?.Invoke(this, EventArgs.Empty);
    }
}
