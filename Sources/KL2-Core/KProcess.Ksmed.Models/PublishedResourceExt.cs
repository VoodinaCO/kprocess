namespace KProcess.Ksmed.Models
{
    partial class PublishedResource
    {
        public PublishedResource(Resource res)
        {
            PaceRating = res.PaceRating;
            Label = res.Label;
            Description = res.Description;
            FileHash = res.Hash;
        }
    }
}
