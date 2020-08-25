namespace KProcess.Ksmed.Models
{
    public class DefaultResource
    {
        public string ReferentialLabel { get; private set; }

        public Resource Resource { get; private set; }

        public DefaultResource(string referentialLabel, Resource resource)
        {
            ReferentialLabel = referentialLabel;
            Resource = resource;
        }
    }
}
