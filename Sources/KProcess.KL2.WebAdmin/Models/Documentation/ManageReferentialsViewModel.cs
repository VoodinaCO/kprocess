using KProcess.Ksmed.Models;
using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class ManageReferentialsViewModel
    {
        public int ProcessId { get; set; }
        public List<DocumentationReferential> DocumentationReferentials { get; set; }
    }
}