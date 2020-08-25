using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class DocumentationPreference
    {
        public Dictionary<string, ColumnsPreference> ColumnsPreferences { get; set; }
        public ColumnsPreference DefaultColumnPreference { get; set; }
    }

    public class ColumnsPreference
    {
        public string Width { get; set; } = "45%";
        public string TextAlign { get; set; } = "Center";
        public bool ShouldBeVisible { get; set; }
        public bool ShouldBeShownInColumnChooser { get; set; }
        public string TemplateName { get; set; }
        public string AlternativeName { get; set; }
    }
}