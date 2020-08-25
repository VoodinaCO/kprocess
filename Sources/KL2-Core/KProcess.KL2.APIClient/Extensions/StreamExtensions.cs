using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KProcess.KL2.APIClient
{
    public static class StreamExtensions
    {
        public static readonly int BufferSize = 80 * 1024; // 80k seems to be the best value for speed

        public static async Task CopyToAsync(this Stream source, Stream destination, IProgress<double> progress = null, CancellationToken cancellationToken = default(CancellationToken), long totalLength = 0, long alreadyReadedLength = 0)
        {
            long inputSize = totalLength == 0 ? source.Length : totalLength;
            var buffer = new byte[BufferSize];
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                alreadyReadedLength += bytesRead;
                progress?.Report(100 * alreadyReadedLength / inputSize);
            }
        }
    }
}
