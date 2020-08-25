using System;

namespace KProcess.Ksmed.Models
{
    [Serializable]
    partial class Resource : IAuditableUserRequired, IActionReferential, IResource
    {
        partial void Initialize()
        {
            // Valeur par défaut
            _paceRating = 1;
        }

        /// <summary>
        /// Obtient la description ou, si ce dernier n'est pas défini, le libellé.
        /// </summary>
        public string DescriptionOrLabel =>
            string.IsNullOrEmpty(Description) ? Label : Description;

        /// <summary>
        /// Obtient ou définit l'identifiant du référentiel.
        /// </summary>
        public int Id
        {
            get { return ResourceId; }
            set { ResourceId = value; }
        }

        /// <summary>
        /// Obtient l'identifiant associé au référentiel lorsqu'il désigne le cadre d'utilisation du référentiel dans un projet.
        /// </summary>
        public abstract ProcessReferentialIdentifier ProcessReferentialId { get; }

        public bool IsDeleted { get; set; }
    }


    /// <summary>
    /// Représente un type de ressource.
    /// </summary>
    public class ResourceType : NotifiableObject
    {
        Action<ResourceType> _onChecked;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ResourceType"/>.
        /// </summary>
        public ResourceType()
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ResourceType"/>.
        /// </summary>
        /// <param name="onChecked">Un délégué appelé quand le type est coché.</param>
        public ResourceType(Action<ResourceType> onChecked)
        {
            _onChecked = onChecked;
        }

        /// <summary>
        /// Obtient ou définit le libellé.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Obtient ou définit le type de la classe de base abstraite.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Obtient ou définit le type de la classe concrète représentant une ressource liée à un projet.
        /// </summary>
        public Type ProjectType { get; set; }

        /// <summary>
        /// Crée une nouvelle instance de la ressource du type spécifié.
        /// </summary>
        /// <returns>L'instance</returns>
        public Resource CreateNew()
        {
            Resource instance = (Resource)Activator.CreateInstance(ProjectType);
            instance.Color = ColorsHelper.GetRandomColor(ColorsHelper.StandardColors).ToString();
            return instance;
        }

        /// <summary>
        /// Détermine si la ressource spécifiée est une instance du type associé.
        /// </summary>
        /// <param name="resource">La ressource.</param>
        /// <returns>
        ///   <c>true</c> si la ressource spécifiée est une instance du type associé; sinon, <c>false</c>.
        /// </returns>
        public bool IsInstanceOf(Resource resource) =>
            Type.IsAssignableFrom(resource.GetType());

        bool _isChecked;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le type est coché.
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    if (_isChecked && _onChecked != null)
                        _onChecked(this);
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Définit la valeur de la propriété IsChecked sans avertir le parent.
        /// </summary>
        /// <param name="isChecked">la valeur d'isChecked.</param>
        public void SetIsCheckedQuiet(bool isChecked)
        {
            _isChecked = isChecked;
            OnPropertyChanged(nameof(IsChecked));
        }
    }
}