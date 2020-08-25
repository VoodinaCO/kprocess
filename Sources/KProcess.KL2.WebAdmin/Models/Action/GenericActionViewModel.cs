using KProcess.KL2.WebAdmin.Models.Documentation;
using KProcess.KL2.WebAdmin.Models.Skill;
using KProcess.KL2.WebAdmin.Utils;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Serialization;

namespace KProcess.KL2.WebAdmin.Models.Action
{
    public class GenericActionViewModel : IIdentable
    {
        public int ActionId { get; set; }
        public string WBS { get; set; }
        public string Label { get; set; }
        public ActionValueViewModel PublishedResource { get; set; }
        public Dictionary<string, string> ActionHeaders { get; set; }
        public string VideoHash { get; set; }
        public string VideoExt { get; set; }
        public string VideoExtension { get; set; }
        public Dictionary<string, ActionColumnViewModel> ColumnValues { get; set; }

        public int ProcessId { get; set; }
        public int ScenarioId { get; set; }
        public string ProcessLabel { get; set; }
        public string PublicationVersion { get; set; }
        public bool PublicationVersionIsMajor { get; set; }
        public int PreviousActionId { get; set; }
        public int NextActionId { get; set; }
        public int PublishModeFilter { get; set; }
        public bool IsDocumentation { get; set; }
        //[DisplayName("Key task")]
        public bool IsKeyTask { get; set; }
        public bool IsGroup { get; set; }

        public List<GenericActionViewModel> Children { get; set; }
        public int? ParentId { get; set; }
        public string TaskType { get; set; }

        public List<string> DetailActionDispositions { get; set; }

        #region Documentation
        public long TimeScale { get; set; }
        public int ProjectId { get; set; }
        public PublishModeEnum PublishMode { get; set; }
        long _duration;
        public long Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                _durationString = GetTimeScaleMaskValue(TimeScale, _duration);
            }
        }
        string _durationString;
        public string DurationString
        {
            get => _durationString;
            set
            {
                _durationString = value;
                _duration = string.IsNullOrEmpty(_durationString) ? 0 : GetTicksValue(TimeScale, _durationString.Replace(":", "").Replace(".", ""));
            }
        }
        public string Formation_Duration { get; set; }
        public string Evaluation_Duration { get; set; }
        public string Inspection_Duration { get; set; }
        [DisplayName("Picture")]
        public string ImageUri { get; set; }
        public string ImageHash { get; set; }
        public string Extension { get; set; }
        [DisplayName("Skill")]
        public int? SkillId { get; set; }
        public IEnumerable<SkillViewModel> Skills { get; set; }
        public string TusId { get; set; }
        public List<ReferentialField> ReferentialsFields { get; set; }
        public List<ReferentialFieldValues> ReferentialFieldValues { get; set; }
        public List<ReferentialField> CustomTextFields { get; set; }
        public List<ReferentialField> CustomNumericFields { get; set; }
        [ScriptIgnore]
        public Dictionary<ProcessReferentialIdentifier, string> CustomTextValues { get; set; }
        [ScriptIgnore]
        public Dictionary<ProcessReferentialIdentifier, double?> CustomNumericValues { get; set; }
        public static string GetTimeScaleMaskValue(long projectTimeScale, long ticks)
        {
            var timeSpan = TimeSpan.FromTicks(ticks);
            var timeSpanHours = Math.Floor(timeSpan.TotalHours);
            var timeSpanMinutes = timeSpan.Minutes;
            var timeSpanSeconds = timeSpan.Seconds;
            var timeSpanMilliseconds = timeSpan.Milliseconds;

            if (projectTimeScale == TimeSpan.FromSeconds(1).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}";
            else if (projectTimeScale == TimeSpan.FromSeconds(.1).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}{timeSpanMilliseconds.ToString().PadLeft(3, '0').Substring(0, 1)}";
            else if (projectTimeScale == TimeSpan.FromSeconds(.01).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}{timeSpanMilliseconds.ToString().PadLeft(3, '0').Substring(0, 2)}";
            else if (projectTimeScale == TimeSpan.FromSeconds(.001).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}{timeSpanMilliseconds.ToString().PadLeft(3, '0')}";
            return null;
        }
        public static long GetTicksValue(long projectTimeScale, string timeScaleMaskValue)
        {
            int timeSpanHours = 0;
            int timeSpanMinutes = 0;
            int timeSpanSeconds = 0;
            int timeSpanMilliseconds = 0;

            if (projectTimeScale == TimeSpan.FromSeconds(1).Ticks)
            {
                var stringValue = $"{timeScaleMaskValue}".PadLeft(6, '0');
                timeSpanHours = int.Parse(stringValue.Substring(0, 2));
                timeSpanMinutes = int.Parse(stringValue.Substring(2, 2));
                timeSpanSeconds = int.Parse(stringValue.Substring(4, 2));
                timeSpanMilliseconds = 0;
            }
            else if (projectTimeScale == TimeSpan.FromSeconds(.1).Ticks)
            {
                var stringValue = $"{timeScaleMaskValue}".PadLeft(7, '0');
                timeSpanHours = int.Parse(stringValue.Substring(0, 2));
                timeSpanMinutes = int.Parse(stringValue.Substring(2, 2));
                timeSpanSeconds = int.Parse(stringValue.Substring(4, 2));
                timeSpanMilliseconds = int.Parse(stringValue.Substring(6, 1));
            }
            else if (projectTimeScale == TimeSpan.FromSeconds(.01).Ticks)
            {
                var stringValue = $"{timeScaleMaskValue}".PadLeft(8, '0');
                timeSpanHours = int.Parse(stringValue.Substring(0, 2));
                timeSpanMinutes = int.Parse(stringValue.Substring(2, 2));
                timeSpanSeconds = int.Parse(stringValue.Substring(4, 2));
                timeSpanMilliseconds = int.Parse(stringValue.Substring(6, 2));
            }
            else if (projectTimeScale == TimeSpan.FromSeconds(.001).Ticks)
            {
                var stringValue = $"{timeScaleMaskValue}".PadLeft(9, '0');
                timeSpanHours = int.Parse(stringValue.Substring(0, 2));
                timeSpanMinutes = int.Parse(stringValue.Substring(2, 2));
                timeSpanSeconds = int.Parse(stringValue.Substring(4, 2));
                timeSpanMilliseconds = int.Parse(stringValue.Substring(6, 3));
            }

            return new TimeSpan(0, timeSpanHours, timeSpanMinutes, timeSpanSeconds, timeSpanMilliseconds).Ticks;
        }
        #endregion

        #region Update
        public string Verb { get; set; }
        public string PreviousWBS { get; set; }
        #endregion

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

    public class ActionColumnViewModel
    {
        public IList<ActionValueViewModel> Values { get; set; }
    }

    public class ActionValueViewModel
    {
        public string Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Value { get; set; }
        public string FileHash { get; set; }
        public string FileExt { get; set; }
        public int? Quantity { get; set; }

        public int ReferentialId { get; set; }
        public string MappingName { get; set; }
        public string LocalizeName { get; set; }
    }
}