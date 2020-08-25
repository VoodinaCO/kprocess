using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Concatenation;

namespace KProcess.KL2.APIClient.Tus
{
    public class TusSFtpStore : ITusStore,
        ITusCreationStore,
        ITusReadableStore,
        ITusTerminationStore,
        ITusChecksumStore,
        ITusConcatenationStore,
        ITusExpirationStore,
        ITusCreationDeferLengthStore
    {
        readonly string _directoryPath;
        readonly bool _deletePartialFilesOnConcat;
        readonly InternalSftpFileRep.SftpFileRepFactory _fileRepFactory;
        readonly IFileProvider _fileProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TusSFtpStore"/> class.
        /// Using this overload will not delete partial files if a final concatenation is performed.
        /// </summary>
        /// <param name="directoryPath">The path on disk where to save files</param>
        public TusSFtpStore(IFileProvider fileProvider, string directoryPath) : this(fileProvider, directoryPath, false)
        {
            // Left blank.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TusSFtpStore"/> class.
        /// </summary>
        /// <param name="directoryPath">The path on disk where to save files</param>
        /// <param name="deletePartialFilesOnConcat">True to delete partial files if a final concatenation is performed</param>
        public TusSFtpStore(IFileProvider fileProvider, string directoryPath, bool deletePartialFilesOnConcat)
        {
            _fileProvider = fileProvider;
            _directoryPath = directoryPath;
            _deletePartialFilesOnConcat = deletePartialFilesOnConcat;
            _fileRepFactory = new InternalSftpFileRep.SftpFileRepFactory(_fileProvider, _directoryPath);
        }

        public async Task<long> AppendDataAsync(string fileId, Stream stream, CancellationToken cancellationToken)
        {
            var internalFileId = new InternalFileId(fileId);
            long bytesWritten = 0;
            var uploadLength = await GetUploadLengthAsync(fileId, cancellationToken);
            using (var file = _fileRepFactory.Data(internalFileId).GetStream(FileMode.Append, FileAccess.Write, FileShare.None))
            {
                var fileLength = file.Length;
                if (uploadLength == fileLength)
                    return 0;

                var chunkStart = _fileRepFactory.ChunkStartPosition(internalFileId);
                var chunkComplete = _fileRepFactory.ChunkComplete(internalFileId);

                if (chunkComplete.Exist())
                {
                    chunkStart.Delete();
                    chunkComplete.Delete();
                }

                if (!chunkStart.Exist())
                    chunkStart.Write(fileLength.ToString());

                int bytesRead;
                do
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var buffer = new byte[StreamExtensions.BufferSize];
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                    fileLength += bytesRead;

                    if (fileLength > uploadLength)
                        throw new TusStoreException($"Stream contains more data than the file's upload length. Stream data: {fileLength}, upload length: {uploadLength}.");

                    await file.WriteAsync(buffer, 0, bytesRead);
                    bytesWritten += bytesRead;

                } while (bytesRead != 0);

                // Chunk is complete. Mark it as complete.
                chunkComplete.Write("1");

                return bytesWritten;
            }
        }

        public Task<bool> FileExistAsync(string fileId, CancellationToken cancellationToken) =>
            Task.FromResult(_fileRepFactory.Data(new InternalFileId(fileId)).Exist());

        public Task<long?> GetUploadLengthAsync(string fileId, CancellationToken cancellationToken)
        {
            var firstLine = _fileRepFactory.UploadLength(new InternalFileId(fileId)).ReadFirstLine(true);
            return firstLine == null
                ? Task.FromResult<long?>(null)
                : Task.FromResult(new long?(long.Parse(firstLine)));
        }

        public Task<long> GetUploadOffsetAsync(string fileId, CancellationToken cancellationToken) =>
            Task.FromResult(_fileRepFactory.Data(new InternalFileId(fileId)).GetLength());

        #region ITusCreationStore

        public async Task<string> CreateFileAsync(long uploadLength, string metadata, CancellationToken cancellationToken)
        {
            var fileId = new InternalFileId();
            _fileProvider.Create(_fileRepFactory.Data(fileId).Path).Dispose();
            if (uploadLength != -1)
                await SetUploadLengthAsync(fileId.FileId, uploadLength, cancellationToken);
            _fileRepFactory.Metadata(fileId).Write(metadata);
            return fileId.FileId;
        }

        public Task<string> GetUploadMetadataAsync(string fileId, CancellationToken cancellationToken)
        {
            var firstLine = _fileRepFactory.Metadata(new InternalFileId(fileId)).ReadFirstLine(true);
            return string.IsNullOrEmpty(firstLine) ? Task.FromResult<string>(null) : Task.FromResult(firstLine);
        }

        #endregion

        #region ITusReadableStore

        public async Task<ITusFile> GetFileAsync(string fileId, CancellationToken cancellationToken)
        {
            var metadata = await GetUploadMetadataAsync(fileId, cancellationToken);
            var file = new TusSFtpFile(_fileProvider, _directoryPath, fileId, metadata);
            return file.Exist() ? file : null;
        }

        #endregion

        #region ITusTerminationStore

        public Task DeleteFileAsync(string fileId, CancellationToken cancellationToken)
        {
            var internalFileId = new InternalFileId(fileId);
            return Task.Run(() =>
            {
                _fileRepFactory.Data(internalFileId).Delete();
                _fileRepFactory.UploadLength(internalFileId).Delete();
                _fileRepFactory.Metadata(internalFileId).Delete();
                _fileRepFactory.UploadConcat(internalFileId).Delete();
                _fileRepFactory.Expiration(internalFileId).Delete();
                _fileRepFactory.ChunkStartPosition(internalFileId).Delete();
                _fileRepFactory.ChunkComplete(internalFileId).Delete();
            }, cancellationToken);
        }

        #endregion

        #region ITusChecksumStore

        public Task<IEnumerable<string>> GetSupportedAlgorithmsAsync(CancellationToken cancellationToken) =>
            Task.FromResult(new[] { "sha1" } as IEnumerable<string>);

        public Task<bool> VerifyChecksumAsync(string fileId, string algorithm, byte[] checksum, CancellationToken cancellationToken)
        {
            bool valid;
            var internalFileId = new InternalFileId(fileId);
            using (var dataStream = _fileRepFactory.Data(internalFileId).GetStream(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var chunkPositionFile = _fileRepFactory.ChunkStartPosition(internalFileId);
                var chunkStartPosition = chunkPositionFile.ReadFirstLineAsLong(true, 0);

                var calculateSha1 = CalculateSha1(dataStream, chunkStartPosition);
                valid = checksum.SequenceEqual(calculateSha1);

                if (!valid)
                {
                    dataStream.Seek(0, SeekOrigin.Begin);
                    dataStream.SetLength(chunkStartPosition);
                    chunkPositionFile.Delete();
                    _fileRepFactory.ChunkComplete(internalFileId).Delete();
                }
            }

            return Task.FromResult(valid);
        }

        public byte[] CalculateSha1(Stream stream, long chunkStartPosition)
        {
            byte[] fileHash;
            using (var sha1 = SHA1.Create())
            {
                var originalPos = stream.Position;
                stream.Seek(chunkStartPosition, SeekOrigin.Begin);
                fileHash = sha1.ComputeHash(stream);
                stream.Seek(originalPos, SeekOrigin.Begin);
            }

            return fileHash;
        }

        #endregion

        #region ITusConcatenationStore

        public Task<FileConcat> GetUploadConcatAsync(string fileId, CancellationToken cancellationToken)
        {
            var firstLine = _fileRepFactory.UploadConcat(new InternalFileId(fileId)).ReadFirstLine(true);
            return string.IsNullOrWhiteSpace(firstLine)
                ? Task.FromResult<FileConcat>(null)
                : Task.FromResult(new UploadConcat(firstLine).Type);
        }

        public async Task<string> CreatePartialFileAsync(long uploadLength, string metadata, CancellationToken cancellationToken)
        {
            var fileId = await CreateFileAsync(uploadLength, metadata, cancellationToken);
            _fileRepFactory.UploadConcat(new InternalFileId(fileId)).Write(new FileConcatPartial().GetHeader());
            return fileId;
        }

        public async Task<string> CreateFinalFileAsync(string[] partialFiles, string metadata, CancellationToken cancellationToken)
        {
            var partialInternalFileReps = partialFiles.Select(f =>
            {
                var partialData = _fileRepFactory.Data(new InternalFileId(f));

                if (!partialData.Exist())
                    throw new TusStoreException($"File {f} does not exist");

                return partialData;
            }).ToArray();

            var length = partialInternalFileReps.Sum(f => f.GetLength());

            var fileId = await CreateFileAsync(length, metadata, cancellationToken);

            var internalFileId = new InternalFileId(fileId);

            _fileRepFactory.UploadConcat(internalFileId).Write(new FileConcatFinal(partialFiles).GetHeader());

            using (var finalFile = _fileRepFactory.Data(internalFileId).GetStream(FileMode.Open, FileAccess.Write, FileShare.None))
            {
                foreach (var partialFile in partialInternalFileReps)
                {
                    using (var partialStream = partialFile.GetStream(FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await partialStream.CopyToAsync(finalFile, StreamExtensions.BufferSize);
                    }
                }
            }

            // ReSharper disable once InvertIf
            if (_deletePartialFilesOnConcat)
                await Task.WhenAll(partialInternalFileReps.Select(f => DeleteFileAsync(f.FileId, cancellationToken)));

            return fileId;
        }

        #endregion

        #region ITusExpirationStore

        public Task SetExpirationAsync(string fileId, DateTimeOffset expires, CancellationToken cancellationToken) =>
            Task.Run(() => _fileRepFactory.Expiration(new InternalFileId(fileId)).Write(expires.ToString("O")), cancellationToken);

        public Task<DateTimeOffset?> GetExpirationAsync(string fileId, CancellationToken cancellationToken)
        {
            var expiration = _fileRepFactory.Expiration(new InternalFileId(fileId)).ReadFirstLine(true);

            return Task.FromResult(expiration == null
                ? (DateTimeOffset?)null
                : DateTimeOffset.ParseExact(expiration, "O", null));
        }

        public async Task<IEnumerable<string>> GetExpiredFilesAsync(CancellationToken cancellationToken)
        {
            var directoryEnum = _directoryPath.StartsWith(_fileProvider.UploadedFilesDirectory.TrimEnd('/', '\\')) ? DirectoryEnum.Uploaded : DirectoryEnum.Published;
            var expiredFiles = (await _fileProvider.EnumerateFiles(directoryEnum, "*.expiration"))
                .Select(Path.GetFileNameWithoutExtension)
                .Select(f => new InternalFileId(f))
                .Where(f => FileHasExpired(f) && FileIsIncomplete(f))
                .Select(f => f.FileId)
                .ToList();

            return expiredFiles;

            bool FileHasExpired(InternalFileId fileId)
            {
                var firstLine = _fileRepFactory.Expiration(fileId).ReadFirstLine();
                return !string.IsNullOrWhiteSpace(firstLine)
                       && HasPassed(DateTimeOffset.ParseExact(firstLine, "O", null));
            }

            bool FileIsIncomplete(InternalFileId fileId) =>
                _fileRepFactory.UploadLength(fileId).ReadFirstLineAsLong() != _fileRepFactory.Data(fileId).GetLength();
        }

        public async Task<int> RemoveExpiredFilesAsync(CancellationToken cancellationToken)
        {
            return await Cleanup(await GetExpiredFilesAsync(cancellationToken));

            async Task<int> Cleanup(IEnumerable<string> files)
            {
                var tasks = files.Select(file => DeleteFileAsync(file, cancellationToken)).ToList();
                await Task.WhenAll(tasks);
                return tasks.Count;
            }
        }

        bool HasPassed(DateTimeOffset dateTime) =>
            dateTime.ToUniversalTime().CompareTo(DateTimeOffset.UtcNow) == -1;

        #endregion

        public Task SetUploadLengthAsync(string fileId, long uploadLength, CancellationToken cancellationToken) =>
            Task.Run(() => _fileRepFactory.UploadLength(new InternalFileId(fileId)).Write(uploadLength.ToString()), cancellationToken);
    }
}
