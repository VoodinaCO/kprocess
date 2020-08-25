using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Teams
{
    public class TeamsViewModel
    {
        public IEnumerable<TeamViewModel> TeamViewModel { get; set; }
        public IEnumerable<string> AllUsers { get; set; }
    }
    
}