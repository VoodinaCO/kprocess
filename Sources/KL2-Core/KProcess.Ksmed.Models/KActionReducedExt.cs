using System;

namespace KProcess.Ksmed.Models
{
    [Serializable]
    partial class KActionReduced
    {

        Resource _resource;
        /// <summary>
        /// La valeur de la resource en fonction de l'approbation de la solution
        /// </summary>
        public Resource Resource
        {
            get { return _resource; }
            set
            {
                if (_resource != value)
                {
                    ChangeTracker.RecordValue(nameof(Resource), _resource, value);
                    Resource oldValue = _resource;
                    _resource = value;
                    OnEntityPropertyChanged(nameof(Resource));
                }
            }
        }
        public event EventHandler<PropertyChangedEventArgs<bool>> ResourceChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Resource"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
        protected virtual void OnResourceChanged(bool oldValue, bool newValue) =>
            ResourceChanged?.Invoke(this, new PropertyChangedEventArgs<bool>(oldValue, newValue));

        bool _approved = true;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si la solution a été approuvée.
        /// </summary>
        public bool Approved
        {
            get { return _approved; }
            set
            {
                if (_approved != value)
                {
                    ChangeTracker.RecordValue(nameof(Approved), _approved, value);
                    bool oldValue = _approved;
                    _approved = value;
                    OnEntityPropertyChanged(nameof(Approved));
                    OnApprovedChanged(oldValue, value);
                }
            }
        }
        public event EventHandler<PropertyChangedEventArgs<bool>> ApprovedChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Approved"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
        protected virtual void OnApprovedChanged(bool oldValue, bool newValue) =>
            ApprovedChanged?.Invoke(this, new PropertyChangedEventArgs<bool>(oldValue, newValue));

        /// <summary>
        /// Obtient ou définit le gain constaté.
        /// </summary>
        public long Saving { get; set; }


        #region Taille maximum des champs

        /// <summary>
        /// Taille maximum du champ Solution.
        /// </summary>
        public int GetSolutionMaxLength { get { return SolutionMaxLength; } }

        #endregion

    }
}
