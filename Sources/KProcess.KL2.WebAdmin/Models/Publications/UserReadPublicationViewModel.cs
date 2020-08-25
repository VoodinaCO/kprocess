using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class UserReadPublicationViewModel
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string TenuredDisplay { get; set; }
        public List<string> Teams { get; set; }
        public string TeamsString { get; set; }
        public List<bool> HasRead { get; set; }
        public List<bool> HasReadPreviousVersion { get; set; }
        public List<bool> HasReadPreviousMajorVersion { get; set; }
        public List<string> ReadDate { get; set; }
    }
}