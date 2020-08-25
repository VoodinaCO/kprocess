using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System;

namespace KProcess.Ksmed.Models
{
    partial class PublishedAction
    {
        [DataMember]
        public KAction Action { get; set; }

        [DataMember]
        public DocumentationActionDraft DocumentationAction { get; set; }

        public PublishedAction(KAction action)
        {
            Action = action;

            WBS = action.WBS;
            Label = action.Label;
            Start = action.Start;
            Finish = action.Finish;
            BuildStart = action.BuildStart;
            BuildFinish = action.BuildFinish;
            IsRandom = action.IsRandom;
            ThumbnailHash = action.ThumbnailHash;
            CustomNumericValue = action.CustomNumericValue;
            CustomNumericValue2 = action.CustomNumericValue2;
            CustomNumericValue3 = action.CustomNumericValue3;
            CustomNumericValue4 = action.CustomNumericValue4;
            CustomTextValue = action.CustomTextValue;
            CustomTextValue2 = action.CustomTextValue2;
            CustomTextValue3 = action.CustomTextValue3;
            CustomTextValue4 = action.CustomTextValue4;
            DifferenceReason = action.DifferenceReason;
            SkillId = action.SkillId;
            IsKeyTask = action.IsKeyTaskManaged;
        }

        public PublishedAction(KAction action, string WBS) : this(action)
        {
            this.WBS = WBS;
        }

        public PublishedAction(DocumentationActionDraft action)
        {
            DocumentationAction = action;

            Label = action.Label;
            IsKeyTask = action.IsKeyTask;
            SkillId = action.SkillId;
            Start = 0;
            Finish = action.Duration;
            BuildStart = 0;
            BuildFinish = 0;
            IsRandom = false;
            ThumbnailHash = action.ThumbnailHash;
            CustomTextValue = action.CustomTextValue;
            CustomTextValue2 = action.CustomTextValue2;
            CustomTextValue3 = action.CustomTextValue3;
            CustomTextValue4 = action.CustomTextValue4;
            CustomNumericValue = action.CustomNumericValue;
            CustomNumericValue2 = action.CustomNumericValue2;
            CustomNumericValue3 = action.CustomNumericValue3;
            CustomNumericValue4 = action.CustomNumericValue4;
            DocumentationRefs1 = action.ReferentialDocumentations.Where(_ => _.RefNumber == 1).Select(_ => new Ref1Action { RefId = _.ReferentialId, Quantity = _.Quantity ?? 1 }).ToList();
            DocumentationRefs2 = action.ReferentialDocumentations.Where(_ => _.RefNumber == 2).Select(_ => new Ref2Action { RefId = _.ReferentialId, Quantity = _.Quantity ?? 1 }).ToList();
            DocumentationRefs3 = action.ReferentialDocumentations.Where(_ => _.RefNumber == 3).Select(_ => new Ref3Action { RefId = _.ReferentialId, Quantity = _.Quantity ?? 1 }).ToList();
            DocumentationRefs4 = action.ReferentialDocumentations.Where(_ => _.RefNumber == 4).Select(_ => new Ref4Action { RefId = _.ReferentialId, Quantity = _.Quantity ?? 1 }).ToList();
            DocumentationRefs5 = action.ReferentialDocumentations.Where(_ => _.RefNumber == 5).Select(_ => new Ref5Action { RefId = _.ReferentialId, Quantity = _.Quantity ?? 1 }).ToList();
            DocumentationRefs6 = action.ReferentialDocumentations.Where(_ => _.RefNumber == 6).Select(_ => new Ref6Action { RefId = _.ReferentialId, Quantity = _.Quantity ?? 1 }).ToList();
            DocumentationRefs7 = action.ReferentialDocumentations.Where(_ => _.RefNumber == 7).Select(_ => new Ref7Action { RefId = _.ReferentialId, Quantity = _.Quantity ?? 1 }).ToList();

        }

        public PublishedAction(DocumentationActionDraft action, string WBS) : this(action)
        {
            this.WBS = WBS;
        }

        /// <summary>
        /// Obtient ou définit la durée.
        /// </summary>
        public long Duration
        {
            get { return Finish - Start; }
            set { Finish = Start + value; }
        }

        TrackableCollection<RefsCollection> _refs;
        public TrackableCollection<RefsCollection> Refs
        {
            get { return _refs; }
            set
            {
                if (_refs != value)
                {
                    _refs = value;
                    OnPropertyChanged();
                }
            }
        }

        TrackableCollection<CustomLabel> _customLabels;
        public TrackableCollection<CustomLabel> CustomLabels
        {
            get { return _customLabels; }
            set
            {
                if (_customLabels != value)
                {
                    _customLabels = value;
                    OnPropertyChanged();
                }
            }
        }

        List<Ref1Action> _documentationRefs1;
        [DataMember]
        public List<Ref1Action> DocumentationRefs1
        {
            get { return _documentationRefs1; }
            set
            {
                if (_documentationRefs1 != value)
                {
                    _documentationRefs1 = value;
                    OnPropertyChanged();
                }
            }
        }

        List<Ref2Action> _documentationRefs2;
        [DataMember]
        public List<Ref2Action> DocumentationRefs2
        {
            get { return _documentationRefs2; }
            set
            {
                if (_documentationRefs2 != value)
                {
                    _documentationRefs2 = value;
                    OnPropertyChanged();
                }
            }
        }

        List<Ref3Action> _documentationRefs3;
        [DataMember]
        public List<Ref3Action> DocumentationRefs3
        {
            get { return _documentationRefs3; }
            set
            {
                if (_documentationRefs3 != value)
                {
                    _documentationRefs3 = value;
                    OnPropertyChanged();
                }
            }
        }

        List<Ref4Action> _documentationRefs4;
        [DataMember]
        public List<Ref4Action> DocumentationRefs4
        {
            get { return _documentationRefs4; }
            set
            {
                if (_documentationRefs4 != value)
                {
                    _documentationRefs4 = value;
                    OnPropertyChanged();
                }
            }
        }

        List<Ref5Action> _documentationRefs5;
        [DataMember]
        public List<Ref5Action> DocumentationRefs5
        {
            get { return _documentationRefs5; }
            set
            {
                if (_documentationRefs5 != value)
                {
                    _documentationRefs5 = value;
                    OnPropertyChanged();
                }
            }
        }

        List<Ref6Action> _documentationRefs6;
        [DataMember]
        public List<Ref6Action> DocumentationRefs6
        {
            get { return _documentationRefs6; }
            set
            {
                if (_documentationRefs6 != value)
                {
                    _documentationRefs6 = value;
                    OnPropertyChanged();
                }
            }
        }

        List<Ref7Action> _documentationRefs7;
        [DataMember]
        public List<Ref7Action> DocumentationRefs7
        {
            get { return _documentationRefs7; }
            set
            {
                if (_documentationRefs7 != value)
                {
                    _documentationRefs7 = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _isGroup;
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

        #region Inspection

        bool? _isOk;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'inspection de l'action est ok.
        /// </summary>
        public bool? IsOk
        {
            get { return _isOk; }
            set
            {
                if (_isOk != value)
                {
                    _isOk = value;
                    OnPropertyChanged();
                }
            }
        }

        InspectionStep _inspectionStep;
        /// <summary>
        /// 
        /// </summary>
        public InspectionStep InspectionStep
        {
            get => _inspectionStep;
            set
            {
                if (_inspectionStep != value)
                {
                    _inspectionStep = value;
                    OnPropertyChanged();
                }
            }
        }

        string _inspectionDate;
        /// <summary>
        /// Obtient ou définit la date de validation de l'inspection
        /// </summary>
        public string InspectionDate
        {
            get => _inspectionDate;
            set
            {
                if (_inspectionDate != value)
                {
                    _inspectionDate = value;
                    OnPropertyChanged();
                }
            }
        }

        string _inspectBy;
        /// <summary>
        /// Obtient ou définit l'inspecteur
        /// </summary>
        public string InspectBy
        {
            get => _inspectBy;
            set
            {
                if (_inspectBy != value)
                {
                    _inspectBy = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Formation

        bool _canValidateTrainingStep;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'action peut faire objet d'une formation.
        /// </summary>
        public bool CanValidateTrainingStep
        {
            get { return _canValidateTrainingStep; }
            set
            {
                if (_canValidateTrainingStep != value)
                {
                    _canValidateTrainingStep = value;
                    OnPropertyChanged();
                }
            }
        }

        string _formationDate;
        /// <summary>
        /// Obtient ou définit la date de validation de la formation
        /// </summary>
        public string FormationDate
        {
            get => _formationDate;
            set
            {
                if (_formationDate != value)
                {
                    _formationDate = value;
                    OnPropertyChanged();
                }
            }
        }

        string _trainedBy;
        /// <summary>
        /// Obtient ou définit le formateur
        /// </summary>
        public string TrainedBy
        {
            get => _trainedBy;
            set
            {
                if (_trainedBy != value)
                {
                    _trainedBy = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Qualification

        bool? _isQualified;
        /// <summary>
        /// 
        /// </summary>
        public bool? IsQualified
        {
            get => _isQualified;
            set
            {
                if (_isQualified != value)
                {
                    _isQualified = value;
                    OnPropertyChanged();
                }
            }
        }

        QualificationStep _qualificationStep;
        /// <summary>
        /// 
        /// </summary>
        public QualificationStep QualificationStep
        {
            get => _qualificationStep;
            set
            {
                if (_qualificationStep != value)
                {
                    _qualificationStep = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _canValidateQualificationStep;
        /// <summary>
        /// 
        /// </summary>
        public bool CanValidateQualificationStep
        {
            get => _canValidateQualificationStep;
            set
            {
                _canValidateQualificationStep = value;
                OnPropertyChanged();
            }
        }

        string _qualifier;
        /// <summary>
        /// 
        /// </summary>
        public string Qualifier
        {
            get => _qualifier;
            set
            {
                if (_qualifier != value)
                {
                    _qualifier = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public IEnumerable<PublishedReferentialAction> Refs1
        {
            get
            {
                var result = PublishedReferentialActions.Where(_ => _.RefNumber == 1);
                return result;
            }
        }

        public IEnumerable<PublishedReferentialAction> Refs2
        {
            get
            {
                var result = PublishedReferentialActions.Where(_ => _.RefNumber == 2);
                return result;
            }
        }

        public IEnumerable<PublishedReferentialAction> Refs3
        {
            get
            {
                var result = PublishedReferentialActions.Where(_ => _.RefNumber == 3);
                return result;
            }
        }

        public IEnumerable<PublishedReferentialAction> Refs4
        {
            get
            {
                var result = PublishedReferentialActions.Where(_ => _.RefNumber == 4);
                return result;
            }
        }

        public IEnumerable<PublishedReferentialAction> Refs5
        {
            get
            {
                var result = PublishedReferentialActions.Where(_ => _.RefNumber == 5);
                return result;
            }
        }

        public IEnumerable<PublishedReferentialAction> Refs6
        {
            get
            {
                var result = PublishedReferentialActions.Where(_ => _.RefNumber == 6);
                return result;
            }
        }

        public IEnumerable<PublishedReferentialAction> Refs7
        {
            get
            {
                var result = PublishedReferentialActions.Where(_ => _.RefNumber == 7);
                return result;
            }
        }

        /// <summary>
        /// Obtient les parties du WBS.
        /// </summary>
        public int[] WBSParts
        {
            get
            {
                if (WBS != null)
                    return WBS.Split('.').Select(str => int.Parse(str)).ToArray();
                else
                    return null;
            }
        }

    }


    /// <summary>
    /// Compare des WBS.
    /// </summary>
    public class WBSPartsComparer : IComparer<int[]>
    {
        /// <summary>
        /// Compare deux WBS.
        /// </summary>
        /// <param name="x">Le premier.</param>
        /// <param name="y">Le second.</param>
        /// <returns>Le résultat de la comparaison.</returns>
        public int Compare(int[] x, int[] y)
        {
            return CompareInternal(x, y);
        }

        /// <summary>
        /// Compare deux WBS.
        /// </summary>
        /// <param name="x">Le premier.</param>
        /// <param name="y">Le second.</param>
        /// <returns>Le résultat de la comparaison.</returns>
        internal static int CompareInternal(int[] x, int[] y)
        {
            if (x == null && y != null)
                return -1;
            else if (x != null && y == null)
                return 1;
            else if (x == y)
                return 0;
            else
            {
                var minLength = Math.Min(x.Length, y.Length);

                for (int i = 0; i < minLength; i++)
                {
                    if (x[i] > y[i])
                        return 1;
                    else if (x[i] < y[i])
                        return -1;
                }

                // toute la première partie est égale
                if (x.Length != y.Length)
                    return
                        x.Length > y.Length ? 1 : -1;

                return 0;
            }
        }
    }


    public class WBSComparer : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            string xWBS;
            string yWBS;

            if (x is PublishedAction xPublishedAction && y is PublishedAction yPublishedAction)
            {
                xWBS = xPublishedAction.WBS;
                yWBS = yPublishedAction.WBS;
            }
            else if (x is DocumentationActionDraftWBS xDocumentationActionDraftWBS && y is DocumentationActionDraftWBS yDocumentationActionDraftWBS)
            {
                xWBS = xDocumentationActionDraftWBS.WBS;
                yWBS = yDocumentationActionDraftWBS.WBS;
            }
            else
                return 0;

            int[] x_SplittedWBS = string.IsNullOrEmpty(xWBS) ? new int[] { } : xWBS.Split('.').Select(str => int.Parse(str)).ToArray();
            int[] y_SplittedWBS = string.IsNullOrEmpty(yWBS) ? new int[] { } : yWBS.Split('.').Select(str => int.Parse(str)).ToArray();

            if (x_SplittedWBS.Length == 0 && y_SplittedWBS.Length == 0)
                return 0;

            if (x_SplittedWBS.Length == 0)
                return -1;

            if (y_SplittedWBS.Length == 0)
                return 1;

            if (x_SplittedWBS.Length > y_SplittedWBS.Length)
            {
                for (int i = 0; i < y_SplittedWBS.Length; i++)
                {
                    if (x_SplittedWBS[i] != y_SplittedWBS[i])
                        return x_SplittedWBS[i] < y_SplittedWBS[i] ? -1 : 1;
                }
                return 1;
            }

            if (x_SplittedWBS.Length < y_SplittedWBS.Length)
            {
                for (int i = 0; i < x_SplittedWBS.Length; i++)
                {
                    if (x_SplittedWBS[i] != y_SplittedWBS[i])
                        return (x_SplittedWBS[i] < y_SplittedWBS[i] ? -1 : 1);
                }
                return -1;
            }

            for (int i = 0; i < y_SplittedWBS.Length; i++)
            {
                if (x_SplittedWBS[i] != y_SplittedWBS[i])
                    return (x_SplittedWBS[i] < y_SplittedWBS[i] ? -1 : 1);
            }
            return 0;
        }
    }
}
