using System.Collections.Generic;

namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    class Trackability
    {
        public List<TrackabilityItem> ModificationList { get; set; }

        public List<TrackabilityVideoItem> VideoList { get; set; }

        public Trackability()
        {
            ModificationList = new List<TrackabilityItem>();
            VideoList = new List<TrackabilityVideoItem>();
        }
    }

    class TrackabilityItem
    {
        public string Modification { get; set; }

        public string Date { get; set; }

        public string Visa { get; set; }

        public int Indice { get; set; }
    }
    class TrackabilityVideoItem
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public string Applicable { get; set; }
    }
}
