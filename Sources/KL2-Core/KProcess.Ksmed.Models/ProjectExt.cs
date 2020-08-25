using KProcess.Globalization;
using KProcess.Ksmed.Models.Interfaces;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    [HasSelfValidation]
    partial class Project : IAuditableUserRequired, INode, IComparable
    {
        /// <summary>
        /// Store infor save to database for Other_Disposition
        /// </summary>
        public PublicationPreferences PublicationPreferences { get; set; }

        partial void Initialize()
        {
            ProjectChilds.CollectionChanged += RefreshNodes;
        }

        void RefreshNodes(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            OnPropertyChanged(nameof(Nodes));

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'audit ne doit pas être géré automatiquement.
        /// </summary>
        public bool CustomAudit =>
            false;

        [DataMember]
        public ScenarioDescription[] ScenariosDescriptions { get; set; }

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

            if (Objective == null && string.IsNullOrEmpty(OtherObjectiveLabel))
            {
                yield return new ValidationError(nameof(Objective), LocalizationManager.GetString("Validation_Project_Objective_Required"));
                yield return new ValidationError(nameof(OtherObjectiveLabel), LocalizationManager.GetString("Validation_Project_Objective_Required"));
            }
        }

        public bool IsReadOnly => RealEndDate != null && Process != null && Process.Projects.Count > 1;

        /// <summary>
        /// Obtient ou définit le les informations de chemin critique des scénarios du projet.
        /// </summary>
        public ScenarioCriticalPath[] ScenariosCriticalPath { get; set; }

        #region IComparable

        public int CompareTo(object obj) =>
            Label.CompareTo(((Project)obj).Label);

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

        public BulkObservableCollection<INode> Nodes
        {
            get
            {
                INode[] projectChilds = ProjectChilds.Cast<INode>().ToArray();
                Array.Sort(projectChilds);
                return new BulkObservableCollection<INode>(projectChilds);
            }
        }

        [DataMember]
        public int? NodeProjectId { get; set; }

        [DataMember]
        public int? NodeScenarioId { get; set; }

        #endregion

        /// <summary>
        /// Serializable and Deserializable for Other_Disposition
        /// </summary>
        #region Other_Disposition

        public void SerializableOtherDisposition()
        {
            var prefs = JsonConvert.SerializeObject(PublicationPreferences);
            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms))
            {
                writer.Write(prefs);
                writer.Flush();
                Other_Disposition = ms.ToArray();
            }
        }

        public void DeserializableOtherDisposition()
        {
            if (PublicationPreferences != null)
                return;
            if (Other_Disposition == null || Other_Disposition.Length == 0)
            {
                PublicationPreferences = null;
                return;
            }

            try
            {
                using (var ms = new MemoryStream(Other_Disposition))
                using (var reader = new StreamReader(ms))
                {
                    var prefs = reader.ReadToEnd();
                    PublicationPreferences = JsonConvert.DeserializeObject<PublicationPreferences>(prefs);
                }
            }
            catch
            {
                PublicationPreferences = null;
            }
        }

        #endregion

    }

    /// <summary>
    /// Représente la description d'un scénario.
    /// </summary>
    public class ScenarioDescription : NotifiableObject
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ScenarioDescription"/>.
        /// </summary>
        public ScenarioDescription()
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ScenarioDescription"/>.
        /// </summary>
        /// <param name="scenario">le scénario correspondant.</param>
        public ScenarioDescription(Scenario scenario)
            : this(scenario.ScenarioId, scenario.Label, scenario.NatureCode, scenario.StateCode)
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ScenarioDescription"/>.
        /// </summary>
        /// <param name="id">L'identifiant.</param>
        /// <param name="label">Le libellé.</param>
        /// <param name="natureCode">Le code de la nature</param>
        /// <param name="stateCode">Le code de l'état</param>
        public ScenarioDescription(int id, string label, string natureCode, string stateCode)
        {
            Id = id;
            _label = label;
            NatureCode = natureCode;
            StateCode = stateCode;
        }

        /// <summary>
        /// Obtient l'identifiant.
        /// </summary>
        [DataMember]
        public int Id { get; private set; }

        string _label;
        /// <summary>
        /// Obtient ou définit le libellé.
        /// </summary>
        [DataMember]
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient le code de la nature.
        /// </summary>
        [DataMember]
        public string NatureCode { get; private set; }

        /// <summary>
        /// Obtient le code de l'état.
        /// </summary>
        [DataMember]
        public string StateCode { get; set; }

        bool _isEnabled;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le scénario est activé.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isLocked;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le scénario est figé.
        /// </summary>
        [DataMember]
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
    }

    /// <summary>
    /// Représente le chemin critique d'un scénario.
    /// </summary>
    public class ScenarioCriticalPath
    {
        /// <summary>
        /// Identifie la valorisation "nulle".
        /// </summary>
        public const string ValueNoneKey = "";

        /// <summary>
        /// Obtient ou définit l'id du scénario
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtient ou définit le nom du scénario.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Obtient ou définit le nom du scénario original.
        /// </summary>
        public string OriginalLabel { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si is le scénario est figé.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Obtient ou définit la durée du chemin critique en I.
        /// </summary>
        public long CriticalPathDuration { get; set; }

        /// <summary>
        /// Obtient ou définit le gain en pourcentage en I, de 0 à 1.
        /// </summary>
        public double? EarningPercent { get; set; }

        /// <summary>
        /// Obtient ou définit la durée du chemin critique en I+E.
        /// </summary>
        public long? CriticalPathDurationIE { get; set; }

        /// <summary>
        /// Obtient ou définit le gain en pourcentage en I+E, de 0 à 1.
        /// </summary>
        public double? EarningPercentIE { get; set; }

        /// <summary>
        /// Obtient ou définit les pourcentages des valorisations
        /// </summary>
        public IDictionary<string, double> Values { get; set; }

        public ResourceCriticalPath OperatorsTotal { get; set; }
        public ResourceCriticalPath[] Operators { get; set; }

        public ResourceCriticalPath EquipmentsTotal { get; set; }
        public ResourceCriticalPath[] Equipments { get; set; }
    }

    public class ResourceCriticalPath
    {

        public string Label { get; set; }
        public long Duration { get; set; }
        public double? EarningPercent { get; set; }
        public double LoadPercent { get; set; }
        public double OverloadPercent { get; set; }

        /// <summary>
        /// Obtient ou définit les pourcentages des valorisations
        /// </summary>
        public IDictionary<string, double> Values { get; set; }

    }

    public class PublicationPreferences
    {
        [DataMember]
        public PublishModeEnum PublishMode { get; set; } = PublishModeEnum.Formation;

        [DataMember]
        public bool VideoExportIsEnabled { get; set; } = true;

        [DataMember]
        public bool SlowMotionIsEnabled { get; set; } = false;

        [DataMember]
        public double DurationMini { get; set; } = 5.0;

        [DataMember]
        public string OverlayTextVideo { get; set; } = null;

        [DataMember]
        public EHorizontalAlign HorizontalAlignement { get; set; } = EHorizontalAlign.Center;

        [DataMember]
        public EVerticalAlign VerticalAlignement { get; set; } = EVerticalAlign.Top;

        [DataMember]
        public bool VideoMarkingIsEnabled { get; set; } = false;
    }
}
