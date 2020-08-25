using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    partial class Role
    {
        [DataMember]
        public ICollection<UserRole> UserRoles { get; set; }

        #region Propriétés de présentation

        bool _isChecked;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le rôle est coché.
        /// </summary>
        [DataMember]
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged();
                }
            }
        }
        
        #endregion
    }
}
