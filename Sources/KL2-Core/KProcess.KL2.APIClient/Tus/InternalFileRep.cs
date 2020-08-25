using System.IO;

namespace KProcess.KL2.APIClient.Tus
{
    sealed class InternalSftpFileRep
    {
        readonly IFileProvider _fileProvider;

        public string Path { get; }

        public string FileId { get; set; }

        InternalSftpFileRep(IFileProvider fileProvider, string fileId, string path)
        {
            _fileProvider = fileProvider;
            FileId = fileId;
            Path = path;
        }

        public void Delete()
        {
            var fileName = System.IO.Path.GetFileName(Path);
            var directoryEnum = Path.StartsWith(_fileProvider.UploadedFilesDirectory) ? DirectoryEnum.Uploaded : DirectoryEnum.Published;
            _fileProvider.Delete(fileName, directoryEnum).GetAwaiter().GetResult();
        }

        public bool Exist()
        {
            var fileName = System.IO.Path.GetFileName(Path);
            var directoryEnum = Path.StartsWith(_fileProvider.UploadedFilesDirectory) ? DirectoryEnum.Uploaded : DirectoryEnum.Published;
            return _fileProvider.Exists(fileName, directoryEnum).GetAwaiter().GetResult();
        }

        public void Write(string text) =>
            _fileProvider.WriteAllText(Path, text);

        public long ReadFirstLineAsLong(bool fileIsOptional = false, long defaultValue = -1)
        {
            var content = ReadFirstLine(fileIsOptional);
            if (long.TryParse(content, out var value))
                return value;

            return defaultValue;
        }

        public string ReadFirstLine(bool fileIsOptional = false)
        {
            if (fileIsOptional && !Exist())
                return null;

            using (var stream = GetStream(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadLine();
            }
        }

        public Stream GetStream(FileMode mode, FileAccess access, FileShare share) =>
            _fileProvider.GetStream(Path, mode, access, share);

        public long GetLength() =>
            _fileProvider.GetLength(Path);

        internal sealed class SftpFileRepFactory
        {
            readonly IFileProvider _fileProvider;
            readonly string _directoryPath;

            public SftpFileRepFactory(IFileProvider fileProvider, string directoryPath)
            {
                _fileProvider = fileProvider;
                _directoryPath = directoryPath;
            }

            public InternalSftpFileRep Data(InternalFileId fileId) => Create(fileId, "");

            public InternalSftpFileRep UploadLength(InternalFileId fileId) => Create(fileId, "uploadlength");

            public InternalSftpFileRep UploadConcat(InternalFileId fileId) => Create(fileId, "uploadconcat");

            public InternalSftpFileRep Metadata(InternalFileId fileId) => Create(fileId, "metadata");

            public InternalSftpFileRep Expiration(InternalFileId fileId) => Create(fileId, "expiration");

            public InternalSftpFileRep ChunkStartPosition(InternalFileId fileId) => Create(fileId, "chunkstart");

            public InternalSftpFileRep ChunkComplete(InternalFileId fileId) => Create(fileId, "chunkcomplete");

            InternalSftpFileRep Create(InternalFileId fileId, string extension)
            {
                var fileName = $"{fileId.FileId}{(string.IsNullOrEmpty(extension) ? string.Empty : $".{extension}")}";
                return new InternalSftpFileRep(_fileProvider, fileId.FileId, GetFilePath(_directoryPath, fileName));
            }

            string GetFilePath(string directoryPath, string fileName) =>
                $"{directoryPath.TrimEnd('/', '\\')}{(directoryPath.Contains("/") ? "/" : "\\")}{fileName.TrimStart('/', '\\')}";
        }
    }
}
