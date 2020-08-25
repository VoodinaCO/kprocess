using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace KProcess.KL2.Server.Setup
{
    public static class Versions
    {
        public static Dictionary<Version, Guid> API_List
        {
            get
            {
                var result = new Dictionary<Version, Guid>();
                using (var stream = File.OpenRead(Path.GetFullPath(@"..\KL2-Core\KProcess.KL2.API\Properties\Versions.xml")))
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

        public static Dictionary<Version, Guid> FileServer_List
        {
            get
            {
                var result = new Dictionary<Version, Guid>();
                using (var stream = File.OpenRead(Path.GetFullPath(@"..\KL2-Core\Kprocess.KL2.FileServer\Properties\Versions.xml")))
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

        public static Dictionary<Version, Guid> Notification_List
        {
            get
            {
                var result = new Dictionary<Version, Guid>();
                using (var stream = File.OpenRead(Path.GetFullPath(@"..\KProcess.KL2.Notification\Properties\Versions.xml")))
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

        public static Dictionary<Version, Guid> WebAdmin_List
        {
            get
            {
                var result = new Dictionary<Version, Guid>();
                using (var stream = File.OpenRead(Path.GetFullPath(@"..\KProcess.KL2.WebAdmin\Properties\Versions.xml")))
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
