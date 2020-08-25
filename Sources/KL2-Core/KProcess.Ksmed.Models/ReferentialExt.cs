namespace KProcess.Ksmed.Models
{
    partial class Referential
    {
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
    }
}
