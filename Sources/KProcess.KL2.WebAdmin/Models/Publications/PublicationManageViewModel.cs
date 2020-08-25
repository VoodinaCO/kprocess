using KProcess.KL2.WebAdmin.Models.Procedure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class PublicationManageViewModel
    {
        public ICollection<ProcedureNodeViewModel> TreeNode { get; set; }
        public PublicationViewModel Publication { get; set; }
        public string PublicationType { get; set; }
    }
}