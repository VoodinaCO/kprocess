using System.IO;
using System.Security.Cryptography;

namespace TusDotNet.Extensions
{
	internal static class FileStreamExtensions
	{
        public static byte[] CalculateSha1(this FileStream fileStream, long chunkStartPosition)
        {
            byte[] fileHash;
            using (var sha1 = new SHA1Managed())
            {
                var originalPos = fileStream.Position;
                fileStream.Seek(chunkStartPosition, SeekOrigin.Begin);
                fileHash = sha1.ComputeHash(fileStream);
                fileStream.Seek(originalPos, SeekOrigin.Begin);
            }

            return fileHash;
        }
    }
}
