using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KProcess.Ksmed.Models.Helpers
{
    public static class FilesHelper
    {
        const string Backup_Passphrase = "oN'2X+znvpr_f=B9$%SW&KXB";
        const string Backup_Iv = "FE4625E5B79D6B9C";

        static readonly IEnumerable<FileSignatureInfo> FileSignatureInfos = new List<FileSignatureInfo>
        {
            new FileSignatureInfo(new byte?[] { 0x42, 0x4D },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".bmp", FileType.Picture), (".dib", FileType.Picture) }),
            new FileSignatureInfo(new byte?[] { 0x47, 0x49, 0x46, 0x38 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".gif", FileType.Picture) }),
            new FileSignatureInfo(new byte?[] { 0xFF, 0xD8, 0xFF },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".jpg", FileType.Picture), (".jpeg", FileType.Picture), (".jpe", FileType.Picture), (".jfif", FileType.Picture) }),
            new FileSignatureInfo(new byte?[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".png", FileType.Picture) }),
            new FileSignatureInfo(new byte?[] { 0x49, 0x49, 0x2A, 0x00 }, // Little endian
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".tiff", FileType.Picture), (".tif", FileType.Picture) }),
            new FileSignatureInfo(new byte?[] { 0x4D, 0x4D, 0x00, 0x2A }, // Big endian
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".tiff", FileType.Picture), (".tif", FileType.Picture) }),
            /*new FileSignatureInfo(new byte?[] { 0x52, 0x49, 0x46, 0x46, null, null, null, null, 0x57, 0x45, 0x42, 0x50 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".webp", FileType.Picture) }),*/

            new FileSignatureInfo(new byte?[] { null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x33, 0x67, 0x70 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".3gp", FileType.Video), (".3gg", FileType.Video), (".3g2", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { 0x52, 0x49, 0x46, 0x46, null, null, null, null, 0x41, 0x56, 0x49, 0x20, 0x4C, 0x49, 0x53, 0x54 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".avi", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { 0x46, 0x4C, 0x56, 0x01 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".flv", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x56, 0x20 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".flv", FileType.Video), (".m4v", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { 0x47 },
                CompareAlgorithms.RecurrenceAlgorithm(4, 192),
                new [] { (".m2ts", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x6D, 0x70, 0x34 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".m4v", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { 0x1A, 0x45, 0xDF, 0xA3 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".mkv", FileType.Video), (".webm", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x71, 0x74, 0x20, 0x20 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".mov", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { null, null, null, null, 0x6D, 0x6F, 0x6F, 0x76 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".mov", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x53, 0x4E, 0x56 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".mp4", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x61, 0x76, 0x63, 0x31 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".mp4", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".mp4", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { 0x00, 0x00, 0x01, 0xBA },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".mpg", FileType.Video), (".mpeg", FileType.Video), (".vob", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { 0x47 },
                CompareAlgorithms.RecurrenceAlgorithm(0, 188),
                new [] { (".ts", FileType.Video), (".mts", FileType.Video), (".tsa", FileType.Audio), (".tsv", FileType.Video) }),
            new FileSignatureInfo(new byte?[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11, 0xA6, 0xD9, 0x00, 0xAA, 0x00, 0x62, 0xCE, 0x6C },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".wmv", FileType.Video), (".wma", FileType.Audio), (".asf", FileType.Video) }),

            new FileSignatureInfo(new byte?[] { 0xFF, 0xF1 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".aac", FileType.Audio) }),
            new FileSignatureInfo(new byte?[] { 0xFF, 0xF9 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".aac", FileType.Audio) }),
            new FileSignatureInfo(new byte?[] { 0x52, 0x49, 0x46, 0x46, null, null, null, null, 0x43, 0x44, 0x44, 0x41, 0x66, 0x6D, 0x74, 0x20 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".cda", FileType.Audio) }),
            new FileSignatureInfo(new byte?[] { 0x49, 0x44, 0x33 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".mp3", FileType.Audio) }),
            new FileSignatureInfo(new byte?[] { 0x52, 0x49, 0x46, 0x46, null, null, null, null, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".wav", FileType.Audio) }),

            new FileSignatureInfo(new byte?[] { 0x54, 0x41, 0x50, 0x45 },
                CompareAlgorithms.SequenceEqualAlgorithm,
                new [] { (".bak3", FileType.Backup), (".bak", FileType.Backup) }),
            new FileSignatureInfo(new byte?[] { 0x54, 0x41, 0x50, 0x45 },
                CompareAlgorithms.CryptAlgorithm(Backup_Passphrase, Backup_Iv),
                new [] { (".bak3", FileType.Backup), (".bak", FileType.Backup) })
        }
            .Concat(FileSignatureInfoRangeGenerator.Generate(new byte?[] { 0x00, 0x00, 0x01, 0xB0 }, 3, 0xB0, 0xB9,
                new[] { (".mpg", FileType.Video), (".mpeg", FileType.Video) }))
            .Concat(FileSignatureInfoRangeGenerator.Generate(new byte?[] { 0x00, 0x00, 0x01, 0xBB }, 3, 0xBB, 0xBF,
                new[] { (".mpg", FileType.Video), (".mpeg", FileType.Video) }))
            .Concat(FileSignatureInfoRangeGenerator.Generate(new byte?[] { 0x00, 0xE0 }, 1, 0xE0, 0xF0,
                new[] { (".mp3", FileType.Audio), (".mpg", FileType.Audio), (".mpeg", FileType.Audio) }))
            .Concat(FileSignatureInfoRangeGenerator.Generate(new byte?[] { 0x00, 0xF2 }, 1, 0xF2, 0xF8,
                new[] { (".mp3", FileType.Audio), (".mpg", FileType.Audio), (".mpeg", FileType.Audio) }))
            .Concat(FileSignatureInfoRangeGenerator.Generate(new byte?[] { 0x00, 0xFA }, 1, 0xFA, 0xFF,
                new[] { (".mp3", FileType.Audio), (".mpg", FileType.Audio), (".mpeg", FileType.Audio) }));

        public static string GetSyncFilesLocation()
        {
            if (Assembly.GetEntryAssembly() == null)
                return null;
            var syncFilesLocation = ConfigurationManager.AppSettings["SyncPath"];
            if (syncFilesLocation.StartsWith(@".\"))
                syncFilesLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), syncFilesLocation.TrimStart('.', '\\'));
            if (!new Uri(syncFilesLocation).IsAbsoluteUri)
                syncFilesLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), syncFilesLocation);
            Directory.CreateDirectory(syncFilesLocation);
            return syncFilesLocation;
        }

        public static void SetSyncFilesLocation(string path, string exePath = null)
        {
            Configuration config = string.IsNullOrEmpty(exePath) ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None) : ConfigurationManager.OpenExeConfiguration(exePath);
            config.AppSettings.Settings["SyncPath"].Value = path;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static bool TestSyncDirectoryRights()
        {
            try
            {
                var testFile = Path.Combine(GetSyncFilesLocation(), "testRights.tmp");
                using (var testFileStream = File.Create(testFile))
                {
                    // Its OK
                }
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetImportedFilesLocation()
        {
            var importedFilesLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "K-Process", "KL²", "ImportedFiles");
            Directory.CreateDirectory(importedFilesLocation);
            return importedFilesLocation;
        }

        /// <summary>
        /// Récupère le dossier courant reservé aux opérations de base de données
        /// </summary>
        /// <returns></returns>
        public static string GetBackupDir()
        {
            var result = Path.Combine(Path.GetTempPath(), "K-Process", "KL²", "SQL");

            var networkServiceAccountName = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null).Translate(typeof(NTAccount)).Value;
            Directory.CreateDirectory(result);
            DirectoryInfo directory = new DirectoryInfo(result);
            DirectorySecurity security = directory.GetAccessControl();
            security.AddAccessRule(new FileSystemAccessRule(networkServiceAccountName,
                FileSystemRights.FullControl | FileSystemRights.Write,
                InheritanceFlags.ObjectInherit,
                PropagationFlags.InheritOnly,
                AccessControlType.Allow));
            directory.SetAccessControl(security);

            return result;
        }

        public static Task CleanSyncFiles() =>
            CleanFiles(GetSyncFilesLocation());

        public static Task CleanImportedFiles() =>
            CleanFiles(GetImportedFilesLocation());

        public static Task CleanFiles(string directory)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    Console.WriteLine($"Can not deleting '{file}'");
                }
            }
            return Task.CompletedTask;
        }

        public static Size GetThumbnailSize(this Uri uri)
        {
            using (var imageStream = File.OpenRead(uri.LocalPath))
            {
                var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile,
                BitmapCacheOption.Default);
                var height = decoder.Frames[0].PixelHeight;
                var width = decoder.Frames[0].PixelWidth;
                return new Size(width, height);
            }
        }

        static FileSignatureInfo GetSignatureInfo(this Stream fileStream)
        {
            foreach (var fileSignature in FileSignatureInfos)
                if (fileSignature.Algorithm(fileStream, fileSignature.Signature))
                    return fileSignature;

            return null;
        }

        public static string GetFileExtension(this Stream fileStream, FileType fileType) =>
            fileStream.GetSignatureInfo()?.DefaultExtension(fileType) ?? string.Empty;

        public static FileType GetFileType(this Stream fileStream) =>
            fileStream.GetSignatureInfo()?.FileType ?? FileType.Unknown;

        public static bool IsFileType(this Stream fileStream, FileType fileType) =>
            (fileStream.GetFileType() & fileType) == fileType;

        public static bool IsFileType(string filePath, FileType fileType)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                return fileStream.IsFileType(fileType);
            }
        }

        public static IEnumerable<string> GetAllExtensions(FileType fileType) =>
            FileSignatureInfos
                .SelectMany(_ => _.Extensions)
                .Where(_ => (_.FileType & fileType) == fileType)
                .Select(_ => _.Extension)
                .Distinct()
                .OrderBy(_ => _);

        public static IEnumerable<string> GetDefaultExtension(this FileType fileType) =>
            FileSignatureInfos
                .Where(_ => _.Extensions.Any(e => (e.FileType & fileType) == fileType))
                .Select(_ => _.DefaultExtension(fileType))
                .Distinct()
                .OrderBy(_ => _);
    }

    [Flags]
    public enum FileType
    {
        Unknown = 0x00,
        Picture = 0x01,
        Video = 0x02,
        Audio = 0x04,
        Backup = 0x08
    }

    class FileSignatureInfo
    {
        public delegate bool CompareAlgorithm(Stream stream, byte?[] signatureBytes);

        public byte?[] Signature { get; }

        public CompareAlgorithm Algorithm { get; }

        public FileType FileType
        {
            get
            {
                var result = FileType.Unknown;
                foreach (var extension in Extensions)
                    result |= extension.FileType;
                return result;
            }
        }

        public (string Extension, FileType FileType)[] Extensions { get; }

        public string DescriptionResourceKey { get; set; }

        public string DefaultExtension(FileType fileType) =>
            Extensions.Any(_ => (_.FileType & fileType) == fileType) ?
                Extensions.First(_ => (_.FileType & fileType) == fileType).Extension
                : null;

        public FileSignatureInfo(byte?[] signature, CompareAlgorithm algorithm, (string, FileType)[] extensions, string descriptionResourceKey = null)
        {
            Signature = signature;
            Algorithm = algorithm;
            Extensions = extensions;
            DescriptionResourceKey = descriptionResourceKey;
        }
    }

    static class FileSignatureInfoRangeGenerator
    {
        public static List<FileSignatureInfo> Generate(byte?[] pattern, int index, byte min, byte max, (string, FileType)[] extensions, string descriptionResourceKey = null)
        {
            if (min > max)
                throw new InvalidOperationException($"Le paramètre {nameof(min)}:{min} doit être inférieur au paramètre {nameof(max)}:{max}");
            if (min > max)
                throw new IndexOutOfRangeException($"L'index {index} est hors limite du tableau {nameof(pattern)}[{pattern.Length}]");
            var result = new List<FileSignatureInfo>();
            var counter = min;
            do
            {
                var computedPattern = pattern.ToArray();
                computedPattern[index] = counter;
                result.Add(new FileSignatureInfo(computedPattern, CompareAlgorithms.SequenceEqualAlgorithm, extensions, descriptionResourceKey));
                if (counter == 0xFF)
                    return result;
                counter++;
            } while (counter <= max);
            return result;
        }
    }

    static class CompareAlgorithms
    {
        public static readonly FileSignatureInfo.CompareAlgorithm SequenceEqualAlgorithm = (stream, signatureBytes) =>
        {
            var buffer = new byte[signatureBytes.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);

            var maskedBuffer = buffer.Cast<byte?>().ToArray();
            for (int i = 0; i < maskedBuffer.Count(); i++)
                if (signatureBytes[i] == null)
                    maskedBuffer[i] = null;

            return maskedBuffer.SequenceEqual(signatureBytes);
        };

        public static FileSignatureInfo.CompareAlgorithm CryptAlgorithm(string passphrase, string iv) =>
            (stream, signatureBytes) =>
            {
                CryptoStream csDecrypt = new CryptoStream(stream,
                    new TripleDESCryptoServiceProvider().CreateDecryptor(Encoding.Default.GetBytes(passphrase), Encoding.Default.GetBytes(iv)),
                    CryptoStreamMode.Read);

                byte[] buffer = new byte[signatureBytes.Length];
                stream.Seek(0, SeekOrigin.Begin);
                csDecrypt.Read(buffer, 0, buffer.Length);

                var maskedBuffer = buffer.Cast<byte?>().ToArray();
                for (int i = 0; i < maskedBuffer.Count(); i++)
                    if (signatureBytes[i] == null)
                        maskedBuffer[i] = null;

                return maskedBuffer.SequenceEqual(signatureBytes);
            };

        public static FileSignatureInfo.CompareAlgorithm RecurrenceAlgorithm(long startOffset, long recurrency) =>
            (stream, signatureBytes) =>
            {
                var buffer = new byte[signatureBytes.Length];
                int passCounter = 5;
                long offset = startOffset;
                do
                {
                    stream.Seek(offset, SeekOrigin.Begin);
                    stream.Read(buffer, 0, buffer.Length);
                    var maskedBuffer = buffer.Cast<byte?>().ToArray();
                    for (int i = 0; i < maskedBuffer.Count(); i++)
                        if (signatureBytes[i] == null)
                            maskedBuffer[i] = null;
                    if (!maskedBuffer.SequenceEqual(signatureBytes))
                        return false;
                    offset += recurrency;
                    passCounter--;
                } while (passCounter > 0 && offset < stream.Length);

                return true;
            };
    }
}