using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class PublicationQualificationsViewModel
    {
        public IEnumerable<PublicationQualificationViewModel> PublicationQualificationViewModel { get; set; }
        public int selectedIndexTeam { get; set; }
        public int selectedIndexPosition { get; set; }
    }
}