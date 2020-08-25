namespace MigrateOldFiles
{
    public class ResourceFile
    {
        public const string HashPropertyName = "Hash";
        public const string UriPropertyName = "Uri";

        public string TableName { get; set; }

        public string IdPropertyName { get; set; }

        public int Id { get; set; }

        public string Uri { get; set; }

        public bool LocalFileExists { get; set; }

        public string Hash { get; set; }

        public bool Copied { get; set; }

        public bool AlreadyInBase { get; set; }
    }
}
