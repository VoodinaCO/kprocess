using Kprocess.PackIconKprocess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Users
{
    public class LanguageViewModel
    {
        public string LanguageCode { get; set; }
        public string Label { get; set; }
        public string FlagName { get; set; }
        public PackIconCountriesFlagsKind Flag { get; set; }
        public string FlagPath { get; set; }
    }
}