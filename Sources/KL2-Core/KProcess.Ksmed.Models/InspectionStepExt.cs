using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KProcess.Ksmed.Models.Validation;

namespace KProcess.Ksmed.Models
{
    partial class InspectionStep
    {
        public List<InspectionStep> Child { get; set; }
        public int Level { get; set; }
        public bool? IsParent { get; set; }
    }
}
