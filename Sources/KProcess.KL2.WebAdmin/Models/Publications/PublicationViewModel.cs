using KProcess.KL2.WebAdmin.Models.Action;
using KProcess.KL2.WebAdmin.Models.Documentation;
using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class PublicationViewModel
    {
        public string PublicationId { get; set; }
        public string Label { get; set; }
        public ICollection<GenericActionViewModel> Actions { get; set; }
        public int ActionsColumnCount { get; set; }
        public Dictionary<string, string> ActionHeaders { get; set; }
        public Dictionary<string, ActionHeader> ActionHeaderModels { get; set; }
        public int FolderId { get; set; }
        public string Folder { get; set; }
        public string FolderParent { get; set; }
        public string FolderPath { get; set; }
        public int ProcessId { get; set; }
        public int PublishModeEnum { get; set; }
        public string Version { get; set; }
        public bool IsMajor { get; set; }

        public List<ActionValueViewModel> ReferentialsUsed { get; set; }
    }
}