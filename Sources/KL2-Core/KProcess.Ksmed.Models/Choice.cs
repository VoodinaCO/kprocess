using KProcess.Business;
using KProcess.KL2.Languages;

namespace KProcess.Ksmed.Models
{
    public class Choice : ModelBase
    {
        public ChoiceEnum Id { get; set; }
        public string LabelResourceId { get; set; }

        public string Label =>
            IoC.Resolve<ILocalizationManager>().GetString(LabelResourceId);
    }
}
