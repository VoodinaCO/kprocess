using KProcess.KL2.Languages;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class VideoViewModel
    {
        public bool cbVideoExport { get; set; }

        public bool cbSlowMotion { get; set; }

        public int tbSlowDuration { get; set; }

        public bool cbWatermarking { get; set; }

        public string tbWatermarkText { get; set; }
        public int verticalAlign { get; set; } = (int)EVerticalAlign.Top;
        public int horizontalAlign { get; set; } = (int)EHorizontalAlign.Center;

        public List<AlignmentValue> VerticalAlignments { get; private set; }

        public List<AlignmentValue> HorizontalAlignments { get; private set; }

        public VideoViewModel()
        {
            var LocalizedStrings = DependencyResolver.Current.GetService<ILocalizationManager>();

            VerticalAlignments = new List<AlignmentValue>
            {
                new AlignmentValue
                {
                    Value = (int)EVerticalAlign.Top,
                    Text = LocalizedStrings.GetString("View_Documentation_VerticalAlignementTop"),
                    Icon = "verticalTop"
                },
                new AlignmentValue
                {
                    Value = (int)EVerticalAlign.Center,
                    Text = LocalizedStrings.GetString("View_Documentation_VerticalAlignementCenter"),
                    Icon = "verticalCenter"
                },
                new AlignmentValue
                {
                    Value = (int)EVerticalAlign.Bottom,
                    Text = LocalizedStrings.GetString("View_Documentation_VerticalAlignementBottom"),
                    Icon = "verticalBottom"
                }
            };

            HorizontalAlignments = new List<AlignmentValue>
            {
                new AlignmentValue
                {
                    Value = (int)EHorizontalAlign.Left,
                    Text = LocalizedStrings.GetString("View_Documentation_HorizontalAlignementLeft"),
                    Icon = "horizontalLeft"
                },
                new AlignmentValue
                {
                    Value = (int)EHorizontalAlign.Center,
                    Text = LocalizedStrings.GetString("View_Documentation_HorizontalAlignementCenter"),
                    Icon = "horizontalCenter"
                },
                new AlignmentValue
                {
                    Value = (int)EHorizontalAlign.Right,
                    Text = LocalizedStrings.GetString("View_Documentation_HorizontalAlignementRight"),
                    Icon = "horizontalRight"
                }
            };
        }
    }

    public class AlignmentValue
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
    }

}