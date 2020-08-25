using System.Collections.Generic;
using System.Linq;

namespace Kprocess.KL2.TabletClient.Models
{
    public static class ExtensionsUtil
    {
        static readonly List<string> ImageExtensions = new List<string> { ".bmp", ".jpeg", ".jpg", ".png" };

        public static bool IsImageExtension(string ext) =>
            ImageExtensions.Any(_ => _.Equals(ext.ToLower()));
    }
}
