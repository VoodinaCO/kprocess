using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Teams
{
    public class TeamViewModel
    {
        public int? TeamId { get; set; }
        [Required]
        [Display(Name = "Équipe")]
        public string TeamName { get; set; }
        public List<int> UserId { get; set; }
        [Display(Name = "Utilisateurs")]
        public List<string> Username { get; set; }
        public List<string> Fullname { get; set; }

        public List<User> Users { get; set; }

    }
}