using Kprocess.KL2.FileTransfer;
using Murmur;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Web.Script.Serialization;

namespace KProcess.Ksmed.Models
{
    public partial class CloudFile
    {
        [ScriptIgnore]
        public byte[] Data
        {
            get
            {
                string filePath = Path.Combine(Preferences.SyncDirectory, FileName);
                if (!File.Exists(filePath))
                {
                    try
                    {
                        WebRequest request = WebRequest.Create(new Uri($"{Preferences.FileServerUri}/GetFile/{FileName}", UriKind.Absolute));
                        request.Timeout = -1;
                        WebResponse response = request.GetResponse();
                        int bytesRead = 0;
                        byte[] bytebuffer = new byte[Preferences.BufferSize];
                        using (Stream responseStream = response.GetResponseStream())
                        using (BinaryReader reader = new BinaryReader(responseStream))
                        using (FileStream fileStream = File.OpenWrite(Path.Combine(Preferences.SyncDirectory, FileName)))
                        {
                            do
                            {
                                bytesRead = reader.Read(bytebuffer, 0, bytebuffer.Length);
                                fileStream.Write(bytebuffer, 0, bytesRead);
                            } while (bytesRead > 0);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                if (File.Exists(filePath))
                    return File.ReadAllBytes(filePath);
                return null;
            }
            set
            {
                string filePath = Path.Combine(Preferences.SyncDirectory, FileName);
                if (!File.Exists(filePath) && value != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream(value))
                    using (BinaryReader reader = new BinaryReader(memoryStream))
                    using (FileStream fileStream = File.OpenWrite(Path.Combine(Preferences.SyncDirectory, FileName)))
                    {
                        int bytesRead = 0;
                        byte[] bytebuffer = new byte[Preferences.BufferSize];
                        do
                        {
                            bytesRead = reader.Read(bytebuffer, 0, bytebuffer.Length);
                            fileStream.Write(bytebuffer, 0, bytesRead);
                        } while (bytesRead > 0);
                    }
                }
            }
        }

        public string FileName =>
            $"{Hash}{Extension}";

        public Uri Uri =>
            new Uri($"{Preferences.FileServerUri}/GetFile/{FileName}");

        [ScriptIgnore]
        public Uri LocalUri =>
            new Uri($"{Preferences.SyncDirectory}/{FileName}");

        public CloudFile(byte[] data, string extension)
        {
            HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
            using (var ms = new MemoryStream(data))
            {
                var streamHash = murmur128.ComputeHash(ms);
                Hash = streamHash.ToHashString();
                Extension = extension;
                Data = data;
            }
        }

        public CloudFile(string hash, string extension)
        {
            Hash = hash;
            Extension = extension;
        }
    }
}
