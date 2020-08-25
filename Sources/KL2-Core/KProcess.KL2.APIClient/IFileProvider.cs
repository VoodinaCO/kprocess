using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Interfaces;

namespace KProcess.KL2.APIClient
{
    public interface IFileProvider
    {
        string PublishedFilesDirectory { get; }

        string UploadedFilesDirectory { get; }

        bool SupportsRange { get; }

        Task<bool> Exists(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published);

        Task<bool> IsDirectory(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published);

        Task<Stream> OpenRead(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published);

        Task<Stream> OpenWrite(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published);

        Task<long> GetLengthAsync(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published);

        Task<bool> Delete(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published);

        Task Complete(string oldFileName, string newFileName = null);

        Task Upload(FileInfo fileInfo, string newFileName = null, DirectoryEnum directoryEnum = DirectoryEnum.Published, IProgress<double> progress = null, CancellationToken cancellationToken = default(CancellationToken));

        Task Upload((FileInfo fileInfo, string newFileName)[] files, DirectoryEnum directoryEnum = DirectoryEnum.Published, IProgress<double> progress = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<string>> EnumerateFiles(DirectoryEnum directoryEnum, string searchPattern);

        ITusStore GetTusStore();

        #region Extensions (for ITusStore)

        string ReadAllText(string path);

        void WriteAllText(string path, string contents);

        Stream GetStream(string path, FileMode mode, FileAccess access, FileShare share);

        long GetLength(string path);

        Stream Create(string path);

        #endregion
    }
}
