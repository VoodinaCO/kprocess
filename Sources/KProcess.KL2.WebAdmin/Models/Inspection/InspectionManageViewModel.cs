using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Inspection
{
    public class InspectionManageViewModel
    {
        public InspectionManageViewModel()
        {
            Inspections = new List<InspectionViewModel>();
        }
        public IEnumerable<InspectionViewModel> Inspections { get; set; }
        public int selectedIndexTeam { get; set; }
        public int selectedIndexOperator { get; set; }
        public int selectedIndexPublication { get; set; }

        /// <summary>
        /// Inspection id associated to the current audit (if any)
        /// </summary>
        public int CurrentAuditInspectionId { get; set; }

        /// <summary>
        /// Information to know whether the current audit (if any) has been started or not
        /// To know that information, we check if any auditItem available
        /// </summary>
        public bool IsCurrentAuditStarted { get; set; }

        public string Question { get; set; }
    }
}