using Kprocess.KL2.FileTransfer;
using KProcess.Ksmed.Models.Interfaces;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    partial class Procedure : INode, IComparable
    {
        partial void Initialize()
        {
            Projects.CollectionChanged += RefreshNodes;
        }

        void RefreshNodes(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            OnPropertyChanged(nameof(Nodes));

        [DataMember]
        public bool LastScenarioHasLinkedProcess { get; set; }

        #region IComparable

        public int CompareTo(object obj)
        {
            if (string.IsNullOrEmpty(Label))
                return -1;
            return Label.CompareTo(((Procedure)obj).Label);
        }

        #endregion

        #region INode

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
            get { return false; }
            set
            {
                if (_allowDrop != value)
                {
                    _allowDrop = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool SyncVideo =>
            VideoSyncs?.FirstOrDefault()?.SyncValue ?? false;

        public bool IsSyncing
        {
            get
            {
                var fileTransferManager = IoC.Resolve<FileTransferManager>();
                return Videos.Any(_ => fileTransferManager.TransferOperations.ContainsKey(_.Filename)
                                       && !fileTransferManager.TransferOperations[_.Filename].IsFinished)
                       || Videos.Any(_ => _.TranscodingProgress != null);
            }
        }

        public BulkObservableCollection<INode> Nodes
        {
            get
            {
                INode[] projects = Projects.Cast<INode>().ToArray();
                Array.Sort(projects);
                return new BulkObservableCollection<INode>(projects);
            }
        }

        [DataMember]
        public int? NodeProjectId { get; set; }

        [DataMember]
        public int? NodeScenarioId { get; set; }

        #endregion
    }
}
