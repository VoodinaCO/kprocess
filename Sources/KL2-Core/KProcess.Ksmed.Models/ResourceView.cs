using KProcess.Business;
using KProcess.KL2.Languages;

namespace KProcess.Ksmed.Models
{
    public class ResourceView : ModelBase
    {
        public ResourceViewEnum Id { get; set; }
        public string LabelResourceId { get; set; }

        public string Label =>
            IoC.Resolve<ILocalizationManager>().GetString(LabelResourceId);
    }
}
