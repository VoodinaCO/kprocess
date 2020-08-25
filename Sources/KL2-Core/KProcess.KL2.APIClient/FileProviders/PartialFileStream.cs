using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KProcess.KL2.APIClient
{
    public class PartialFileStream : Stream
    {
        readonly long _start;
        readonly long _end;
        readonly Stream _stream;

        public PartialFileStream(Stream stream, long start, long end)
        {
            _start = start;
            _position = start;
            _end = end;
            _stream = stream;

            if (start > 0)
                _stream.Seek(start, SeekOrigin.Begin);
        }

        public override bool CanRead =>
            _stream.CanRead;

        public override bool CanSeek =>
            _stream.CanSeek;

        public override bool CanWrite =>
            _stream.CanWrite;

        public override long Length =>
            _end - _start + 1;

        long _position;
        public override long Position
        {
            get => _position;
            set
            {
                _position = value;
                _stream.Seek(_position, SeekOrigin.Begin);
            }
        }

        public override void Flush() =>
            _stream.Flush();

        public override Task FlushAsync(CancellationToken cancellationToken) =>
            _stream.FlushAsync(cancellationToken);

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
            {
                _position = _start + offset;
                return _stream.Seek(_start + offset, origin);
            }
            if (origin == SeekOrigin.Current)
            {
                _position += offset;
                return _stream.Seek(_position + offset, origin);
            }
            throw new NotImplementedException("Seeking from SeekOrigin.End is not implemented");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int byteCountToRead = count;
            if (_position + count > _end)
                byteCountToRead = (int)(_end - _position) + 1;
            int result = _stream.Read(buffer, offset, byteCountToRead);
            _position += byteCountToRead;
            return result;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int byteCountToRead = count;
            if (_position + count > _end)
                byteCountToRead = (int)(_end - _position) + 1;
            int result = await _stream.ReadAsync(buffer, offset, byteCountToRead);
            _position += byteCountToRead;
            return result;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            int byteCountToRead = count;
            if (_position + count > _end)
                byteCountToRead = (int)(_end - _position);
            void internalCallback(IAsyncResult s)
            {
                _position += byteCountToRead;
                callback(s);
            }
            IAsyncResult result = _stream.BeginRead(buffer, offset, count, internalCallback, state);
            return result;
        }

        public override int EndRead(IAsyncResult asyncResult) =>
            _stream.EndRead(asyncResult);

        public override int ReadByte()
        {
            int result = _stream.ReadByte();
            _position++;
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _stream.Dispose();
            base.Dispose(disposing);
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int byteCountToWrite = count;
            if (_position + count > _end)
                byteCountToWrite = (int)(_end - _position) + 1;
            _stream.Write(buffer, offset, byteCountToWrite);
            _position += byteCountToWrite;
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int byteCountToWrite = count;
            if (_position + count > _end)
                byteCountToWrite = (int)(_end - _position) + 1;
            await _stream.WriteAsync(buffer, offset, byteCountToWrite, cancellationToken);
            _position += byteCountToWrite;
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            int byteCountToWrite = count;
            if (_position + count > _end)
                byteCountToWrite = (int)(_end - _position);
            void internalCallback(IAsyncResult s)
            {
                _position += byteCountToWrite;
                callback(s);
            }
            IAsyncResult result = _stream.BeginWrite(buffer, offset, count, internalCallback, state);
            return result;
        }

        public override void EndWrite(IAsyncResult asyncResult) =>
            _stream.EndWrite(asyncResult);

        public override void WriteByte(byte value)
        {
            _stream.WriteByte(value);
            _position++;
        }
    }
}
