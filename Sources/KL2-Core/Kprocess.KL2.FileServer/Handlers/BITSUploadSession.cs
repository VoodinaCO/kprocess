using KProcess.KL2.APIClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Kprocess.KL2.FileServer
{
    public class BITSUploadSession
    {
        readonly string[] VideoExt = { ".avi", ".flv", ".m2ts", ".mkv", ".mov", ".mp4", ".mpeg", ".mpg", ".mts", ".ts", ".wmv"};

        // holds the file paths that has an active upload session
        public static readonly string VideoBufferDirectory = @"VideosBuffer";
        public readonly bool IsTranscoding;
        readonly IFileProvider _fileProvider;
        readonly long _fragmentSizeLimit;
        readonly List<Fragment> _fragments;
        Stream _stream;
        HttpStatusCode _status_code = HttpStatusCode.OK;
        long _expectedFileLength;

        public string FileName { get; }

        public bool Move { get; set; }

        static BITSUploadSession()
        {
            VideoBufferDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), VideoBufferDirectory);
        }

        BITSUploadSession(IFileProvider fileProvider, string fileName, long fragmentSizeLimit)
        {
            _fileProvider = fileProvider;
            _fragmentSizeLimit = fragmentSizeLimit;
            FileName = fileName;
            _fragments = new List<Fragment>();
            _expectedFileLength = -1;
            IsTranscoding = VideoExt.Contains(Path.GetExtension(FileName).ToLower());

            Directory.CreateDirectory(VideoBufferDirectory);
        }

        public static async Task<BITSUploadSession> Create(IFileProvider fileProvider, string fileName, long fragmentSizeLimit)
        {
            BITSUploadSession session = new BITSUploadSession(fileProvider, fileName, fragmentSizeLimit);

            if (await fileProvider.Exists(fileName)) // case the file already exists
            {
                if (await fileProvider.IsDirectory(fileName)) // case the file is actually a directory
                    session._status_code = HttpStatusCode.Forbidden;
                else if (SimpleBITSRequestHandler.Sessions.Any(_ => _.Value.FileName == fileName)) // case the file is being uploaded in another active session
                    session._status_code = HttpStatusCode.Conflict;
                else // case file exists on server - we overwrite the file with the new upload
                    await session.OpenFile();
            }
            else // case file does not exist but its parent folder does exist - we create the file
                await session.OpenFile();
            /*else // case file does not exist nor its parent folder - we don't create the directory tree
                session._status_code = HttpStatusCode.Forbidden;*/

            return session;
        }

        async Task OpenFile()
        {
            try
            {
                if (IsTranscoding)
                    _stream = File.OpenWrite(Path.Combine(VideoBufferDirectory, Path.GetFileName(FileName)));
                else
                    _stream = await _fileProvider.OpenWrite(FileName, DirectoryEnum.Uploaded);
                _status_code = HttpStatusCode.OK;
            }
            catch (Exception)
            {
                _status_code = HttpStatusCode.Forbidden;
            }
        }

        /// <summary>
        /// Combines all accepted fragments' data into one string
        /// </summary>
        /// <returns></returns>
        byte[] GetFinalDataFromFragments()
        {
            return new byte[0];
        }

        public HttpStatusCode GetLastStatusCode()
            => _status_code;

        /// <summary>
        /// Applies new fragment received from client to the upload session.
        /// Returns a boolean: is the new fragment last in session
        /// </summary>
        /// <param name="file_total_length"></param>
        /// <param name="range_start"></param>
        /// <param name="range_end"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> AddFragment(long file_total_length, long range_start, long range_end, byte[] data)
        {
            // check if fragment size exceeds server limit
            if (range_end - range_start > _fragmentSizeLimit)
                throw new Bits.FragmentTooLarge(range_end - range_start);

            // case new fragment is the first fragment in this session
            if (_expectedFileLength == -1)
                _expectedFileLength = file_total_length;

            if (LastRangeEnd + 1 < range_start) // case new fragment's range is not contiguous with the previous fragment, will cause the server to respond with status code 416
                throw new Bits.InvalidFragment(LastRangeEnd, range_start);
            if (LastRangeEnd + 1 > range_start) // case new fragment partially overlaps last fragment, BITS protocol states that server should treat only the non-overlapping part
                range_start = LastRangeEnd + 1;

            try
            {
                if (_stream.CanSeek)
                    Console.WriteLine("Can seek");
                else
                    Console.WriteLine("Can't seek");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await OpenFile();
            }
            var stream = new PartialFileStream(_stream, range_start, range_end);
            await stream.WriteAsync(data, 0, (int)stream.Length, CancellationToken.None);
            await stream.FlushAsync();

            _fragments.Add(new Fragment
            {
                RangeStart = range_start,
                RangeEnd = range_end
            });

            // case new fragment is the first fragment in this session,
            // we write the final uploaded data to file
            if (range_end + 1 == _expectedFileLength)
            {
                //await _file.WriteAsync(GetFinalDataFromFragments(), 0, 0);
                return true;
            }

            return false;
        }

        public long LastRangeEnd =>
            _fragments.LastOrDefault()?.RangeEnd ?? -1;

        public async Task Close(bool deleteFile = false)
        {
            await _stream.FlushAsync();
            _stream.Close();
            if (deleteFile)
            {
                if (IsTranscoding)
                    File.Delete(Path.Combine(VideoBufferDirectory, Path.GetFileName(FileName)));
                else
                    await _fileProvider.Delete(FileName, DirectoryEnum.Uploaded);
            }
        }
    }

    public class Fragment
    {
        public long RangeStart;
        public long RangeEnd;
    }
}
