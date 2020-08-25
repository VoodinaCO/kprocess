using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Interfaces;
using tusdotnet.Models;

namespace KProcess.KL2.APIClient.Tus
{
    public class TusSFtpFile : ITusFile
    {
        readonly IFileProvider _fileProvider;
        readonly string _metadata;
        readonly string _filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TusSFtpFile"/> class.
        /// </summary>
        ///  <param name="directoryPath">The directory path on disk where the store save it's files</param>
        /// <param name="fileId">The file id</param>
        /// <param name="metadata">The raw Upload-Metadata header</param>
        internal TusSFtpFile(IFileProvider fileProvider, string directoryPath, string fileId, string metadata)
        {
            _fileProvider = fileProvider;
            _metadata = metadata;
            Id = fileId;
            _filePath = GetFilePath(directoryPath, Id);
        }

        /// <summary>
        /// Returns true if the file exists.
        /// </summary>
        /// <returns>True if the file exists</returns>
        internal bool Exist()
        {
            var fileName = Path.GetFileName(_filePath);
            var directoryEnum = _filePath.StartsWith(_fileProvider.UploadedFilesDirectory.TrimEnd('/', '\\')) ? DirectoryEnum.Uploaded : DirectoryEnum.Published;
            return _fileProvider.Exists(fileName, directoryEnum).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public Task<Stream> GetContentAsync(CancellationToken cancellationToken)
        {
            Stream stream = _fileProvider.GetStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult(stream);
        }

        /// <inheritdoc />
        public Task<Dictionary<string, Metadata>> GetMetadataAsync(CancellationToken cancellationToken) =>
            Task.FromResult(Metadata.Parse(_metadata));

        string GetFilePath(string directoryPath, string fileName) =>
            $"{directoryPath.TrimEnd('/', '\\')}{(directoryPath.Contains("/") ? "/" : "\\")}{fileName.TrimStart('/', '\\')}";
    }
}
