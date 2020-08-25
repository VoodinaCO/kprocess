using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class PublishedActionViewModel
    {
        public int ActionId { get; set; }
        public string Label { get; set; }
    }
}