using FluentFTP;
using System.IO;
using System.Threading;

namespace KProcess.KL2.APIClient
{
    public class SeekableFtpDataStream : Stream
    {
        readonly FtpClient _ftpClient;
        Stream _stream;
        readonly string _filePath;
        readonly FileAccess _fileAccess;

        public SeekableFtpDataStream(FtpClient ftpClient, FileAccess fileAccess, string filePath)
        {
            _ftpClient = ftpClient;
            _fileAccess = fileAccess;
            _filePath = filePath;
            _stream = _fileAccess == FileAccess.Read ? _ftpClient.OpenRead(_filePath) : _ftpClient.OpenWrite(_filePath);
        }

        public override bool CanRead =>
            _stream.CanRead;

        public override bool CanSeek =>
            true;

        public override bool CanWrite =>
            _stream.CanWrite;

        public override long Length =>
            _stream.Length;

        long _position;
        public override long Position
        {
            get
            {
                _position = _stream.Position;
                return _position;
            }
            set => _position = value;
        }

        public override void Flush() =>
            _stream.Flush();

        public override int Read(byte[] buffer, int offset, int count) =>
            _stream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPosition = _stream.Position;
            _stream?.Close();
            switch (origin)
            {
                case SeekOrigin.Current:
                    newPosition += offset;
                    break;
                case SeekOrigin.End:
                    newPosition = _stream.Length - offset;
                    break;
                default:
                case SeekOrigin.Begin:
                    newPosition = offset;
                    break;
            }
            try
            {
                _stream = _fileAccess == FileAccess.Read ? _ftpClient.OpenRead(_filePath, newPosition) : _ftpClient.OpenAppend(_filePath);
            }
            catch (FtpCommandException)
            {
                Thread.Sleep(10);
                _stream = _fileAccess == FileAccess.Read ? _ftpClient.OpenRead(_filePath, newPosition) : _ftpClient.OpenAppend(_filePath);
            }
            return _stream.Position;
        }

        public override void SetLength(long value) =>
            _stream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) =>
            _stream.Write(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            _stream?.Close();
            _ftpClient.Dispose();
            base.Dispose(disposing);
        }
    }
}
