using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class PublicationQualificationViewModel
    {
        public string ProcessId { get; set; }
        public string Folder { get; set; }
        public string FolderPath { get; set; }
        public string Label { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public int Total { get; set; }
        public double PercentageRate { get; set; }
    }
}