using KProcess.Ksmed.Models;
using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.Procedure
{
    public class ProcedureNodeViewModel
    {
        public string Id{ get; set; }
        public string Name { get; set; }
        public bool IsExpanded  { get; set; }
        public bool HasChild { get; set; }
        public string ParentId { get; set; }
        public string LinkAttribute { get; set; }
        public string LinkAttributeFunction { get; set; }
        public string Sprite { get; set; }
        public PublicationStatus? Status { get; set; }
        
        public string StateAsLabel { get; set; }
        public string StateAsString { get; set; }

        public Dictionary<string, object> NodeProperty { get; set; }
    }
}