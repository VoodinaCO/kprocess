using System;

namespace KProcess.Ksmed.Models
{
    [Serializable]
    partial class Scenario : IAuditableUserRequired
    {
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'audit ne doit pas être géré automatiquement.
        /// </summary>
        public bool CustomAudit =>
            false;

        bool _isLocked;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le scénario est figé.
        /// </summary>
        public bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit les vidéos utilisées dans ce scénario.
        /// </summary>
        public Video[] UsedVideos { get; set; }

    }
}
