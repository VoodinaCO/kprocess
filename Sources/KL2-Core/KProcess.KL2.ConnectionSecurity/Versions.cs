using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace KProcess.KL2.ConnectionSecurity
{
    public static class Versions
    {
        /// <summary>
        /// Dictionnaire des versions de KL²
        /// aussi utilisé pour le MSI de mise à jour de KL²
        /// </summary>
        public static Dictionary<Version, Guid> List
        {
            get
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KProcess.KL2.ConnectionSecurity.Versions.xml");
                var result = new Dictionary<Version, Guid>();
                try
                {
                    var doc = XDocument.Load(stream);
                    result = doc.Root.Elements("version").ToDictionary(k => Version.Parse(k.Attribute("Id").Value), v => Guid.Parse(v.Attribute("Guid").Value));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return result;
            }
        }
    }
}
