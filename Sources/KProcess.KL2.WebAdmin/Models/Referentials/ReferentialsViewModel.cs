using KProcess.Ksmed.Models;
using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.Referentials
{
    public class ReferentialsViewModel
    {
        public List<ReferentialViewModel> Referentials { get; set; }
        public List<RefResourceViewModel> Refs { get; set; }
        public List<int> ProjectIds { get; set; }
        public List<string> Projects { get; set; }

        public ProcessReferentialIdentifier RefIdentifier { get; set; }
        public string RefLabel { get; set; }
        public int IntRefIdentifier => (int)RefIdentifier;
    }
}