using KProcess.KL2.WebAdmin.Models.Procedure;
using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class DocumentationManageViewModel
    {
        public ICollection<ProcedureNodeViewModel> TreeNode { get; set; }
        public int? ProcessId { get; set; }
        public string ProcessLabel { get; set; }
        public string ProjectName { get; set; }
    }
}