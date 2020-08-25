using KProcess.Ksmed.Models;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;

namespace Kprocess.KL2.TabletClient.Extensions
{
    public static class InspectionExtensions
    {
        public static TrackableCollection<Anomaly> GetLinkedAnomalies(this Inspection inspection)
        {
            if (inspection == null)
                return new TrackableCollection<Anomaly>();
            IEnumerable<Anomaly> rootAnomalies = inspection.InspectionSteps.Where(_ => _.Anomaly != null)
                                                                           .Select(_ => UpdateAnomalyType(inspection, _.Anomaly));
            IEnumerable<Anomaly> subAnomalies = inspection.InspectionSteps.Where(_ => _.LinkedInspection != null)
                                                                          .SelectMany(_ => _.LinkedInspection.InspectionSteps)
                                                                          .Where(_ => _.Anomaly != null)
                                                                          .Select(_ => UpdateAnomalyType(inspection, _.Anomaly));
            return new TrackableCollection<Anomaly>(rootAnomalies.Concat(subAnomalies));
        }
        
        public static TrackableCollection<Anomaly> GetOutsidedAnomalies(this Inspection inspection)
        {
            if (inspection == null)
                return new TrackableCollection<Anomaly>();
            return new TrackableCollection<Anomaly>(inspection.Anomalies.Where(_ => !_.InspectionSteps.Any())
                                                                        .Select(x => UpdateAnomalyType(inspection, x, true)));
        }

        public static TrackableCollection<Anomaly> GetAllAnomalies(this Inspection inspection)
        {
            var linkedAnomalies = inspection.GetLinkedAnomalies();
            var outsidedAnomalies = inspection.GetOutsidedAnomalies();

            return new TrackableCollection<Anomaly>(linkedAnomalies.Concat(outsidedAnomalies));
        }

        static Anomaly UpdateAnomalyType(Inspection inspection, Anomaly anomaly, bool isHors = false)
        {
            anomaly.IsHorsVisite = isHors;
            anomaly.AnomalyTypeAndName = anomaly.Type.AnomalyTypeToString() + ": " + anomaly.Label;
            if (!isHors)
            {
                anomaly.ActionLabel = inspection.Publication.PublishedActions.SingleOrDefault(pa =>
                    pa.PublishedActionId == inspection.InspectionSteps.SingleOrDefault(y =>
                        y.PublishedActionId == anomaly.InspectionSteps.First().PublishedActionId).PublishedActionId)?.Label ?? string.Empty;
            }
            return anomaly;
        }
    }
}
