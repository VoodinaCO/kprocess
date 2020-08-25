using KProcess.Ksmed.Models;

namespace KProcess.KL2.WebAdmin.Models.Referentials
{
    public class ReferentialViewModel
    {
        public ProcessReferentialIdentifier refId { get; set; }
        public string Label { get; set; }
        public bool isResource { get; set; }
    }
}