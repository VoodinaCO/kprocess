namespace KProcess.Ksmed.Models
{
    partial class PublishedReferential
    {
        public PublishedReferential(IMultipleActionReferential referential)
        {
            Label = referential.Label;
            Description = referential.Description;
            FileHash = referential.Hash;
            //File = referential.CloudFile == null ? null : new PublishedFile(referential.CloudFile);
        }

        byte[] _embeddedFile;
        public byte[] EmbeddedFile
        {
            get { return _embeddedFile; }
            set
            {
                if (_embeddedFile != value)
                {
                    _embeddedFile = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
