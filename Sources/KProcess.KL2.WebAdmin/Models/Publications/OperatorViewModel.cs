using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class OperatorViewModel
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string TenuredDisplay { get; set; }
        public List<string> Teams { get; set; }
        public string TeamsString { get; set; }
        public List<bool> Uptodate { get; set; }
        public List<bool> UptodatePreviousVersion { get; set; }
        public List<bool> UptodatePreviousMajorVersion { get; set; }
        public List<string> UptodateDate { get; set; }
    }
}