using KProcess.KL2.WebAdmin.Models.Action;
using KProcess.Ksmed.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class FormatViewModel
    {
        public int ProjectId { get; set; }
        public int ProcessId { get; set; }
        public int ScenarioId { get; set; }
        public List<GenericActionViewModel> Actions { get; set; }
        public List<DocumentationActionDraftWBS> PreviousTrainingDocumentationActions { get; set; }
        public List<DocumentationActionDraftWBS> PreviousEvaluationDocumentationActions { get; set; }
        public List<DocumentationActionDraftWBS> PreviousInspectionDocumentationActions { get; set; }
        public int ActionsColumnCount { get; set; }
        public Dictionary<string, ActionHeader> FormationActionHeaders { get; set; }
        public Dictionary<string, ActionHeader> InspectionActionHeaders { get; set; }
        public Dictionary<string, ActionHeader> EvaluationActionHeaders { get; set; }

        public List<string> PublishForList { get; set; }
        public bool PublishForTraining { get; set; }
        public bool PublishForInspection { get; set; }
        public bool PublishForEvaluation { get; set; }
        public string TrainingFormatAsJson { get; set; }
        public string InspectionFormatAsJson { get; set; }
        public string EvaluationFormatAsJson { get; set; }
        public bool IsDraftForPreviousScenario { get; set; }
    }

    public class ActionHeader
    {
        public string Label { get; set; }
        public bool IsVisible { get; set; }
        public string Width { get; set; }
    }

    public static class FormatViewModelExt
    {
        public static readonly List<string> ManagedColumns = new List<string>
        {
            nameof(PublishedAction.WBS),
            nameof(PublishedAction.Label),
            nameof(PublishedAction.Thumbnail),
            nameof(PublishedAction.PublishedResource),
            nameof(PublishedAction.PublishedActionCategory),
            nameof(PublishedAction.Duration),
            nameof(PublishedAction.Skill),
            nameof(PublishedAction.Refs1),
            nameof(PublishedAction.Refs2),
            nameof(PublishedAction.Refs3),
            nameof(PublishedAction.Refs4),
            nameof(PublishedAction.Refs5),
            nameof(PublishedAction.Refs6),
            nameof(PublishedAction.Refs7),
            nameof(PublishedAction.CustomTextValue),
            nameof(PublishedAction.CustomTextValue2),
            nameof(PublishedAction.CustomTextValue3),
            nameof(PublishedAction.CustomTextValue4),
            nameof(PublishedAction.CustomNumericValue),
            nameof(PublishedAction.CustomNumericValue2),
            nameof(PublishedAction.CustomNumericValue3),
            nameof(PublishedAction.CustomNumericValue4)
        };

        public static readonly Dictionary<PublishModeEnum, DocumentationPreference> DocumentationColumnsPreferences =
            new Dictionary<PublishModeEnum, DocumentationPreference>
            {
                [PublishModeEnum.Formation] = new DocumentationPreference
                {
                    ColumnsPreferences = new Dictionary<string, ColumnsPreference>
                    {
                        [nameof(GenericActionViewModel.ActionId)] = new ColumnsPreference
                        {
                            Width = "0%",
                            ShouldBeVisible = false,
                            ShouldBeShownInColumnChooser = false
                        },
                        [nameof(GenericActionViewModel.IsDocumentation)] = new ColumnsPreference
                        {
                            Width = "0%",
                            ShouldBeVisible = false,
                            ShouldBeShownInColumnChooser = false
                        },
                        [nameof(GenericActionViewModel.IsGroup)] = new ColumnsPreference
                        {
                            Width = "0%",
                            ShouldBeVisible = false,
                            ShouldBeShownInColumnChooser = false
                        },
                        [nameof(PublishedAction.WBS)] = new ColumnsPreference
                        {
                            Width = "10%",
                            ShouldBeVisible = true,
                            ShouldBeShownInColumnChooser = false,
                            TemplateName = "#NoTemplate",
                            AlternativeName = " "
                        },
                        [nameof(PublishedAction.Label)] = new ColumnsPreference
                        {
                            ShouldBeVisible = true,
                            ShouldBeShownInColumnChooser = false,
                            TextAlign = "Left",
                            TemplateName = "#labelTemplate"
                        },
                        [nameof(PublishedAction.Thumbnail)] = new ColumnsPreference
                        {
                            ShouldBeVisible = true,
                            ShouldBeShownInColumnChooser = true,
                            TemplateName = "#NoTemplate",
                            AlternativeName = " "
                        }
                    },
                    DefaultColumnPreference = new ColumnsPreference
                    {
                        ShouldBeVisible = false,
                        ShouldBeShownInColumnChooser = true,
                        TemplateName = "#NoTemplate"
                    }
                },
                [PublishModeEnum.Evaluation] = new DocumentationPreference
                {
                    ColumnsPreferences = new Dictionary<string, ColumnsPreference>
                    {
                        [nameof(GenericActionViewModel.ActionId)] = new ColumnsPreference
                        {
                            Width = "0%",
                            ShouldBeVisible = false,
                            ShouldBeShownInColumnChooser = false
                        },
                        [nameof(GenericActionViewModel.IsDocumentation)] = new ColumnsPreference
                        {
                            Width = "0%",
                            ShouldBeVisible = false,
                            ShouldBeShownInColumnChooser = false
                        },
                        [nameof(GenericActionViewModel.IsGroup)] = new ColumnsPreference
                        {
                            Width = "0%",
                            ShouldBeVisible = false,
                            ShouldBeShownInColumnChooser = false
                        },
                        [nameof(PublishedAction.WBS)] = new ColumnsPreference
                        {
                            Width = "10%",
                            ShouldBeVisible = true,
                            ShouldBeShownInColumnChooser = false,
                            TemplateName = "#NoTemplate",
                            AlternativeName = " "
                        },
                        [nameof(PublishedAction.Label)] = new ColumnsPreference
                        {
                            Width = "35%",
                            ShouldBeVisible = true,
                            ShouldBeShownInColumnChooser = false,
                            TextAlign = "Left",
                            TemplateName = "#labelTemplate"
                        },
                        [nameof(PublishedAction.Thumbnail)] = new ColumnsPreference
                        {
                            ShouldBeVisible = false,
                            ShouldBeShownInColumnChooser = true,
                            TemplateName = "#NoTemplate",
                            AlternativeName = " "
                        },
                        [HeadersHelper.IsQualified] = new ColumnsPreference
                        {
                            Width = "15%",
                            ShouldBeVisible = true,
                            ShouldBeShownInColumnChooser = false,
                            TemplateName = "#NoTemplate"
                        },
                        [HeadersHelper.IsNotQualified] = new ColumnsPreference
                        {
                            Width = "15%",
                            ShouldBeVisible = true,
                            ShouldBeShownInColumnChooser = false,
                            TemplateName = "#NoTemplate"
                        },
                        [HeadersHelper.QualificationStep_Comment] = new ColumnsPreference
                        {
                            Width = "25%",
                            ShouldBeVisible = true,
                            ShouldBeShownInColumnChooser = false,
                            TemplateName = "#NoTemplate"
                        }
                    },
                    DefaultColumnPreference = new ColumnsPreference
                    {
                        ShouldBeVisible = false,
                        ShouldBeShownInColumnChooser = true,
                        TemplateName = "#NoTemplate"
                    }
                },
                [PublishModeEnum.Inspection] = new DocumentationPreference
                {
                    ColumnsPreferences = new Dictionary<string, ColumnsPreference>
                    {
                        [nameof(GenericActionViewModel.ActionId)] = new ColumnsPreference
                        {
                            Width = "0%",
                            ShouldBeVisible = false,
                            ShouldBeShownInColumnChooser = false
                        },
                        [nameof(GenericActionViewModel.IsDocumentation)] = new ColumnsPreference
                        {
                            Width = "0%",
                            ShouldBeVisible = false,
                            ShouldBeShownInColumnChooser = false
                        },
                        [nameof(GenericActionViewModel.IsGroup)] = new ColumnsPreference
                        {
                            Width = "0%",
                            ShouldBeVisible = false,
                            ShouldBeShownInColumnChooser = false
                        },
                        [nameof(PublishedAction.WBS)] = new ColumnsPreference
                        {
                            Width = "10%",
                            ShouldBeVisible = true,
                            ShouldBeShownInColumnChooser = false,
                            TemplateName = "#NoTemplate",
                            AlternativeName = " "
                        },
                        [nameof(PublishedAction.Label)] = new ColumnsPreference
                        {
                            ShouldBeVisible = true,
                            ShouldBeShownInColumnChooser = false,
                            TextAlign = "Left",
                            TemplateName = "#labelTemplate"
                        },
                        [nameof(PublishedAction.Thumbnail)] = new ColumnsPreference
                        {
                            ShouldBeVisible = true,
                            ShouldBeShownInColumnChooser = true,
                            TemplateName = "#NoTemplate",
                            AlternativeName = " "
                        }
                    },
                    DefaultColumnPreference = new ColumnsPreference
                    {
                        ShouldBeVisible = false,
                        ShouldBeShownInColumnChooser = true,
                        TextAlign = "Center",
                        TemplateName = "#NoTemplate"
                    }
                }
            };

        public static string Width(this PublishModeEnum publishMode, string field) =>
            DocumentationColumnsPreferences[publishMode].ColumnsPreferences.ContainsKey(field)
            ? DocumentationColumnsPreferences[publishMode].ColumnsPreferences[field].Width
            : DocumentationColumnsPreferences[publishMode].DefaultColumnPreference.Width;

        public static string TextAlign(this PublishModeEnum publishMode, string field) =>
            DocumentationColumnsPreferences[publishMode].ColumnsPreferences.ContainsKey(field)
            ? DocumentationColumnsPreferences[publishMode].ColumnsPreferences[field].TextAlign
            : DocumentationColumnsPreferences[publishMode].DefaultColumnPreference.TextAlign;

        public static bool ShouldBeVisible(this PublishModeEnum publishMode, string field) =>
            DocumentationColumnsPreferences[publishMode].ColumnsPreferences.ContainsKey(field)
            ? DocumentationColumnsPreferences[publishMode].ColumnsPreferences[field].ShouldBeVisible
            : DocumentationColumnsPreferences[publishMode].DefaultColumnPreference.ShouldBeVisible;

        public static bool ShouldBeShownInColumnChooser(this PublishModeEnum publishMode, string field) =>
            DocumentationColumnsPreferences[publishMode].ColumnsPreferences.ContainsKey(field)
            ? DocumentationColumnsPreferences[publishMode].ColumnsPreferences[field].ShouldBeShownInColumnChooser
            : DocumentationColumnsPreferences[publishMode].DefaultColumnPreference.ShouldBeShownInColumnChooser;

        public static string TemplateName(this PublishModeEnum publishMode, string field) =>
            DocumentationColumnsPreferences[publishMode].ColumnsPreferences.ContainsKey(field)
            ? DocumentationColumnsPreferences[publishMode].ColumnsPreferences[field].TemplateName
            : DocumentationColumnsPreferences[publishMode].DefaultColumnPreference.TemplateName;

        public static bool HasAlternativeName(this PublishModeEnum publishMode, string field) =>
            DocumentationColumnsPreferences[publishMode].ColumnsPreferences.ContainsKey(field)
            ? !string.IsNullOrEmpty(DocumentationColumnsPreferences[publishMode].ColumnsPreferences[field].AlternativeName)
            : !string.IsNullOrEmpty(DocumentationColumnsPreferences[publishMode].DefaultColumnPreference.AlternativeName);

        public static string AlternativeName(this PublishModeEnum publishMode, string field) =>
            DocumentationColumnsPreferences[publishMode].ColumnsPreferences.ContainsKey(field)
            ? DocumentationColumnsPreferences[publishMode].ColumnsPreferences[field].AlternativeName
            : DocumentationColumnsPreferences[publishMode].DefaultColumnPreference.AlternativeName;

        public static string AllAlternativeNames(this PublishModeEnum publishMode) =>
            JsonConvert.SerializeObject(DocumentationColumnsPreferences[publishMode].ColumnsPreferences
                .Where(cp => !string.IsNullOrEmpty(cp.Value.AlternativeName))
                .ToDictionary(cp => cp.Key, cp => cp.Value.AlternativeName));
    }
}