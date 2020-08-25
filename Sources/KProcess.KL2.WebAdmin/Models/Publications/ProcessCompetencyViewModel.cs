using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class ProcessCompetencyViewModel
    {
        public string PublicationId { get; set; }
        public string Label { get; set; }
        public Publication Publication { get; set; }
    }
}