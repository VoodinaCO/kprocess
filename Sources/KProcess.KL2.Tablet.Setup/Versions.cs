using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace KProcess.KL2.Tablet.Setup
{
    public static class Versions
    {
        /// <summary>
        /// Dictionnaire des versions de KL² Tablet
        /// aussi utilisé pour le MSI de mise à jour de KL² Tablet
        /// </summary>
        public static Dictionary<Version, Guid> List
        {
            get
            {
                var result = new Dictionary<Version, Guid>();
                using (var stream = File.OpenRead(Path.GetFullPath(@"..\KL2-Core\Kprocess.KL2.TabletClient\Properties\Versions.xml")))
                {
                    try
                    {
                        var doc = XDocument.Load(stream);
                        result = doc.Root.Elements("version").ToDictionary(k => Version.Parse(k.Attribute("Id").Value), v => Guid.Parse(v.Attribute("Guid").Value));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                return result;
            }
        }
    }
}
