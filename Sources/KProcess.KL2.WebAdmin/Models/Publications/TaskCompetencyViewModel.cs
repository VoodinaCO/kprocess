using KProcess.Ksmed.Models;
using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class TaskCompetencyViewModel
    {
        public int SkillId { get; set; }
        public string Label { get; set; }
        public IEnumerable<Publication> Publications { get; set; }
    }
}