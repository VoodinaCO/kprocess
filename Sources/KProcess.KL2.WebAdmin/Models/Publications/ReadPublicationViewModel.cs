using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class ReadPublicationViewModel
    {
        public IEnumerable<UserReadPublicationViewModel> UserReadPublicationViewModel { get; set; }
        public IEnumerable<PublicationViewModel> PublicationViewModel { get; set; }
        public int PublicationCount { get; set; }
        public IEnumerable<string> DirectoryList { get; set; }
        public IEnumerable<string> DirectoryListPath { get; set; }
    }
}