using KProcess.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace KProcess.Ksmed.Models
{
    [HasSelfValidation]
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{Label} {WBS}")]
    partial class KAction : IAuditableUserRequired, ICloneable
    {

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'audit ne doit pas être géré automatiquement.
        /// </summary>
        public bool CustomAudit =>
            false;

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

            if (Start < 0 || Duration <= 0 || Finish < 0)
            {
                string message = LocalizationManager.GetString("Validation_KAction_StartFinishDuration_Required");
                yield return new ValidationError(nameof(Start), message);
                yield return new ValidationError(nameof(Duration), message);
                yield return new ValidationError(nameof(Finish), message);
            }
            else if (Start + Duration != Finish)
            {
                string message = LocalizationManager.GetString("Validation_KAction_StartFinishDuration_Incoherent");
                yield return new ValidationError(nameof(Start), message);
                yield return new ValidationError(nameof(Duration), message);
                yield return new ValidationError(nameof(Finish), message);
            }

            if (IsLinkToProcess && LinkedProcessId == null)
            {
                string message = LocalizationManager.GetString("Validation_KAction_LinkedProcess_Incoherent");
                yield return new ValidationError(nameof(LinkedProcess), message);
            }
        }

        /// <summary>
        /// Obtient ou définit l'équipement réalisant l'action.
        /// </summary>
        public Equipment Equipment
        {
            get { return Resource as Equipment; }
            set { Resource = value; }
        }

        bool _isLinkToProcess;
        /// <summary>
        /// Obtient ou définit si l'action est lié à un mode opératoire.
        /// </summary>
        public bool IsLinkToProcess
        {
            get { return _isLinkToProcess; }
            set
            {
                if (_isLinkToProcess != value)
                {
                    _isLinkToProcess = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit l'opérateur réalisant l'action.
        /// </summary>
        public Operator Operator
        {
            get { return Resource as Operator; }
            set { Resource = value; }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Resource"/> a changé.
        /// </summary>
        partial void OnResourceChangedPartial(Resource oldValue, Resource newValue)
        {
            OnPropertyChanged(nameof(Equipment));
            OnPropertyChanged(nameof(Operator));
        }

        /// <summary>
        /// Obtient ou définit la durée.
        /// </summary>
        public long Duration
        {
            get { return Finish - Start; }
            set { Finish = Start + value; }
        }

        public string DurationString { get; set; }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="StartChanged"/> a changé.
        /// </summary>
        partial void OnStartChangedPartial(long oldValue, long newValue) =>
            OnPropertyChanged(nameof(Duration));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="FinishChanged"/> a changé.
        /// </summary>
        partial void OnFinishChangedPartial(long oldValue, long newValue) =>
            OnPropertyChanged(nameof(Duration));

        /// <summary>
        /// Obtient ou définit la durée de la partie "Construire".
        /// </summary>
        public long BuildDuration
        {
            get { return BuildFinish - BuildStart; }
            set { BuildFinish = BuildStart + value; }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="BuildStartChanged"/> a changé.
        /// </summary>
        partial void OnBuildStartChangedPartial(long oldValue, long newValue) =>
            OnPropertyChanged(nameof(BuildDuration));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="BuildFinishChanged"/> a changé.
        /// </summary>
        partial void OnBuildFinishChangedPartial(long oldValue, long newValue) =>
            OnPropertyChanged(nameof(BuildDuration));

        string _customNumericValueText;
        public string CustomNumericValueText
        {
            get => string.IsNullOrEmpty(_customNumericValueText) ? string.Format("{0:G29}", CustomNumericValue) : _customNumericValueText;
            set
            {
                if (_customNumericValueText == value)
                    return;
                _customNumericValueText = value;
                OnPropertyChanged();
                if (ValidateValueHelper.TryParse(value, CultureInfo.InvariantCulture, out double? numValue))
                    CustomNumericValue = numValue;
            }
        }

        string _customNumericValue2Text;
        public string CustomNumericValue2Text
        {
            get => string.IsNullOrEmpty(_customNumericValue2Text) ? string.Format("{0:G29}", CustomNumericValue2) : _customNumericValue2Text;
            set
            {
                if (_customNumericValue2Text == value)
                    return;
                _customNumericValue2Text = value;
                OnPropertyChanged();
                if (ValidateValueHelper.TryParse(value, CultureInfo.InvariantCulture, out double? numValue))
                    CustomNumericValue2 = numValue;
            }
        }

        string _customNumericValue3Text;
        public string CustomNumericValue3Text
        {
            get => string.IsNullOrEmpty(_customNumericValue3Text) ? string.Format("{0:G29}", CustomNumericValue3) : _customNumericValue3Text;
            set
            {
                if (_customNumericValue3Text == value)
                    return;
                _customNumericValue3Text = value;
                OnPropertyChanged();
                if (ValidateValueHelper.TryParse(value, CultureInfo.InvariantCulture, out double? numValue))
                    CustomNumericValue3 = numValue;
            }
        }

        string _customNumericValue4Text;
        public string CustomNumericValue4Text
        {
            get => string.IsNullOrEmpty(_customNumericValue4Text) ? string.Format("{0:G29}", CustomNumericValue4) : _customNumericValue4Text;
            set
            {
                if (_customNumericValue4Text == value)
                    return;
                _customNumericValue4Text = value;
                OnPropertyChanged();
                if (ValidateValueHelper.TryParse(value, CultureInfo.InvariantCulture, out double? numValue))
                    CustomNumericValue4 = numValue;
            }
        }

        bool _isGroup;
        [DataMember]
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'action est un groupe.
        /// </summary>
        public bool IsGroup
        {
            get { return _isGroup; }
            set
            {
                if (_isGroup != value)
                {
                    _isGroup = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'action est une tâche importante.
        /// </summary>
        public bool IsKeyTaskManaged
        {
            get { return IsGroup ? false : IsKeyTask; }
            set
            {
                if (IsKeyTask != value)
                {
                    IsKeyTask = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les parties du WBS.
        /// </summary>
        public int[] WBSParts { get; private set; }

        partial void OnWBSChangedPartial(string oldValue, string newValue)
        {
            if (WBS != null)
                WBSParts = WBS.Split('.').Select(str => int.Parse(str)).ToArray();
            else
                WBSParts = null;
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'action est réduite.
        /// </summary>
        public bool IsReduced =>
            Reduced != null;

        partial void OnReducedChangedPartial(KActionReduced oldValue, KActionReduced newValue) =>
            OnPropertyChanged(nameof(IsReduced));

        public ICollection<ActionPredecessorSuccessor> ActionPredecessorSuccessors { get; set; }

        List<KAction> _predecessorsManaged;
        /// <summary>
        /// Obtient ou définit la collection de prédécesseurs en une version gérée indépendamment.
        /// </summary>
        public List<KAction> PredecessorsManaged
        {
            get
            {
                if (_predecessorsManaged == null)
                    _predecessorsManaged = new List<KAction>();
                return _predecessorsManaged;
            }
            set
            {
                if (!ReferenceEquals(_predecessorsManaged, value))
                {
                    _predecessorsManaged = value;
                    OnNavigationPropertyChanged(nameof(Predecessors));
                }
            }
        }

        List<KAction> _successorsManaged;
        /// <summary>
        /// Obtient ou définit la collection de successeurs en une version gérée indépendamment.
        /// </summary>
        public List<KAction> SuccessorsManaged
        {
            get
            {
                if (_successorsManaged == null)
                    _successorsManaged = new List<KAction>();
                return _successorsManaged;
            }
            set
            {
                if (!ReferenceEquals(_successorsManaged, value))
                {
                    _successorsManaged = value;
                    OnNavigationPropertyChanged(nameof(Successors));
                }
            }
        }

        /// <summary>
        /// Survient lorsque la valeur de le propriété <see cref="DifferenceReasonManaged"/> a changé.
        /// </summary>
        public event EventHandler DifferenceReasonManagedChanged;

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="DifferenceReasonManaged"/> a changé.
        /// </summary>
        protected virtual void OnDifferenceReasonManagedChanged() =>
            DifferenceReasonManagedChanged?.Invoke(this, EventArgs.Empty);

        string _differenceReasonManaged;
        /// <summary>
        /// Obtient ou définit la raison des écarts d'un des descendants.
        /// </summary>
        public string DifferenceReasonManaged
        {
            get { return _differenceReasonManaged; }
            set
            {
                if (_differenceReasonManaged != value)
                {
                    _differenceReasonManaged = value;
                    OnPropertyChanged();
                    OnDifferenceReasonManagedChanged();
                }
            }
        }

        string _ameliorationDescription;
        /// <summary>
        /// Obtient ou définit la description de l'amélioration.
        /// </summary>
        public string AmeliorationDescription
        {
            get { return _ameliorationDescription; }
            set
            {
                if (_ameliorationDescription != value)
                {
                    _ameliorationDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        public TrackableCollection<IReferentialActionLink> Refs(ProcessReferentialIdentifier refId)
        {
            switch (refId)
            {
                case ProcessReferentialIdentifier.Ref1:
                    return new TrackableCollection<IReferentialActionLink>(Ref1);
                case ProcessReferentialIdentifier.Ref2:
                    return new TrackableCollection<IReferentialActionLink>(Ref2);
                case ProcessReferentialIdentifier.Ref3:
                    return new TrackableCollection<IReferentialActionLink>(Ref3);
                case ProcessReferentialIdentifier.Ref4:
                    return new TrackableCollection<IReferentialActionLink>(Ref4);
                case ProcessReferentialIdentifier.Ref5:
                    return new TrackableCollection<IReferentialActionLink>(Ref5);
                case ProcessReferentialIdentifier.Ref6:
                    return new TrackableCollection<IReferentialActionLink>(Ref6);
                case ProcessReferentialIdentifier.Ref7:
                    return new TrackableCollection<IReferentialActionLink>(Ref7);
                default:
                    return null;
            }
        }
      
        public object Clone() =>
            MemberwiseClone();

        /// <summary>
        /// Permet de savoir si on peut modifier le curseur de fin
        /// </summary>
        public bool CanModifyFinish { get; set; }

        public string PredecessorsString =>
            Predecessors.Count == 0 ? null : string.Join(",", Predecessors.Select(_ => _.WBS).OrderBy(_ => _).ToArray());

        /// <summary>
        /// Taille maximum du champ CustomTextValue.
        /// </summary>
        /// <remarks>
        /// CHE - La MaxLength étant utilisé dans l'écran BuildView dans une CustomLabelUC avec une MaxLEngth récupéré depuis [KProcess.Ksmed.Models.Project.CustomTextLabelMaxLength] (seulement pour les strings), la modification de la maxlength risquerait de causer trop de regressions.
        /// C'est pourquoi il a été choisi de créer une valeur CustomTextValueUIGlobalMaxLength permettant de mettre en commun la maxLEngth pour les TextBox comme ceux utilisés dans la vue BuildView.
        /// Cet ajout a été effectué dans le contexte suivant: "User Story 4209:Augmenter le nb de caractères mini des "Free text fields" et de "Task""
        /// 
        /// Attention, vérifier que cette valeur soit inférieure à celle spécifiées au dessus (CustomTextValueMaxLength, CustomTextValue2MaxLength ect...)
        /// </remarks>
        public const int CustomTextValueUIGlobalMaxLength = 200;

    }
}
