using KProcess.Globalization;
using KProcess.Ksmed.Models.Interfaces;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    [HasSelfValidation]
    partial class ProjectDir : INode, IComparable
    {
        partial void Initialize()
        {
            Childs.CollectionChanged += RefreshNodes;
            Processes.CollectionChanged += RefreshNodes;
        }

        void RefreshNodes(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            OnPropertyChanged(nameof(Nodes));

        #region IComparable

        public int CompareTo(object obj)
        {
            if (obj is ProjectDir secondProjectDir)
            {
                if (Name == null && secondProjectDir.Name == null)
                    return 0;
                if (Name == null)
                    return -1;
                if (secondProjectDir.Name == null)
                    return 1;
                return Name.CompareTo(secondProjectDir.Name);
            }
            throw new ArgumentException($"Arg {nameof(obj)} is not a {nameof(ProjectDir)} entity.");
        }

        #endregion

        #region INode

        public string Label =>
            Name;

        public void StartMonitorNodesChanged()
        {
            Initialize();
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _allowDrop;
        public bool AllowDrop
        {
            get { return true; }
            set
            {
                if (_allowDrop != value)
                {
                    _allowDrop = value;
                    OnPropertyChanged();
                }
            }
        }

        public BulkObservableCollection<INode> Nodes
        {
            get
            {
                INode[] subFolders = Childs.Cast<INode>().ToArray();
                Array.Sort(subFolders);
                INode[] processes = Processes.Cast<INode>().ToArray();
                Array.Sort(processes);
                return new BulkObservableCollection<INode>(subFolders.Union(processes).ToArray());
            }
        }

        [DataMember]
        public int? NodeProjectId { get; set; }

        [DataMember]
        public int? NodeScenarioId { get; set; }

        #endregion

        /// <summary>
        /// Lorsque surchargé dans une classe fille, cette méthode sert à exécuter une validation personnalisée.
        /// </summary>
        /// <returns>
        /// Une énumération des erreurs de validation, ou null s'il n'y en a pas.
        /// </returns>
        [SelfValidation]
        protected override IEnumerable<ValidationError> OnCustomValidate()
        {
            foreach (ValidationError error in base.OnCustomValidate())
                yield return error;

            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationError(nameof(Name), LocalizationManager.GetString("Validation_ProjectDir_Name_Required"));
            }
        }
    }
}
