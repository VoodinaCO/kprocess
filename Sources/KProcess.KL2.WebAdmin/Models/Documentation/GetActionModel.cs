using KProcess.KL2.WebAdmin.Models.Skill;
using KProcess.Ksmed.Models;
using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class GetActionModel
    {
        public string Verb { get; set; }
        public DocumentationActionDraftWBS DocumentationActionDraftWBS { get; set; }
        public int ProcessId { get; set; }
        public int ProjectId { get; set; }
        public int ScenarioId { get; set; }
        public PublishModeEnum PublishMode { get; set; }
        public IEnumerable<SkillViewModel> Skills { get; set; }
        public List<ReferentialField> Referentials { get; set; }
        public long ProjectTimeScale { get; set; }
    }
}