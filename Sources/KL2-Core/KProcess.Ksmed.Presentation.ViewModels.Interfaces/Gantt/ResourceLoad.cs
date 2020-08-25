using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente la charge d'une ressource.
    /// </summary>
    public class ReferentialLoad : NotifiableObject
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ReferentialLoad"/>.
        /// </summary>
        /// <param name="resource">La ressource.</param>
        public ReferentialLoad(Resource resource)
        {
            this.Resource = resource;
        }

        /// <summary>
        /// Obtient la ressource.
        /// </summary>
        public Resource Resource { get; private set; }

        private double _load;
        /// <summary>
        /// Obtient ou définit la charge.
        /// </summary>
        public double Load
        {
            get { return _load; }
            set
            {
                if (_load != value)
                {
                    _load = value;
                    OnPropertyChanged("Load");
                }
            }
        }

        private double _overload;
        /// <summary>
        /// Obtient ou définit la surcharge.
        /// </summary>
        public double Overload
        {
            get { return _overload; }
            set
            {
                if (_overload != value)
                {
                    _overload = value;
                    OnPropertyChanged("Overload");
                }
            }
        }

        private string _description;
        /// <summary>
        /// Obtient ou définit la description.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
    }
}
