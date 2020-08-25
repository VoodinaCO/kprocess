using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SyncfusionHelper.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var resourcesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");
            SfDataGridXmlDto xmlDto = new SfDataGridXmlDto();
            string xmlContent_original;
            string xmlContent_computed;

            Console.WriteLine("Test XML deserialization :");
            xmlContent_original = File.ReadAllText(Path.Combine(resourcesFolder, "formation.xml"), Encoding.UTF8);
            using (var fileStream = File.OpenRead(Path.Combine(resourcesFolder, "formation.xml")))
            {
                var doc = XDocument.Load(fileStream);
                xmlDto.Deserialize(doc.Root);
            }

            Console.WriteLine("Test XML serialization :");
            string tempFile = Path.GetTempFileName();
            using (var fileStream = File.OpenWrite(tempFile))
            using (var writer = XmlWriter.Create(fileStream, new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t",
                OmitXmlDeclaration = true
            }))
            {
                var doc = new XDocument(xmlDto.Serialize());
                doc.WriteTo(writer);
            }
            xmlContent_computed = File.ReadAllText(tempFile, Encoding.UTF8).Replace(" />", "/>");

            Console.WriteLine($"Xml {(xmlContent_original == xmlContent_computed ? string.Empty : "doesn't ")}files match.\n");

            string jsonContent_original;
            string jsonContent_computed;

            Console.WriteLine("Test JSON deserialization :");
            jsonContent_original = File.ReadAllText(Path.Combine(resourcesFolder, "formation.json"), Encoding.UTF8);
            SfDataGridJsonDto jsonDto = JsonConvert.DeserializeObject<SfDataGridJsonDto>(jsonContent_original);

            Console.WriteLine("Test JSON serialization :\n");
            jsonContent_computed = JsonConvert.SerializeObject(jsonDto, JsonHelper.Settings);

            Console.WriteLine("Test JSON to XML conversion :\n");
            SfDataGridJsonDto sourceJsonDto = JsonConvert.DeserializeObject<SfDataGridJsonDto>(jsonContent_original);
            SfDataGridXmlDto destinationXmlDto = sourceJsonDto.ToXmlDto();
            tempFile = Path.GetTempFileName();
            using (var fileStream = File.OpenWrite(tempFile))
            using (var writer = XmlWriter.Create(fileStream, new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t",
                OmitXmlDeclaration = true
            }))
            {
                var doc = new XDocument(destinationXmlDto.Serialize());
                doc.WriteTo(writer);
            }
            xmlContent_computed = File.ReadAllText(tempFile, Encoding.UTF8).Replace(" />", "/>");

            Console.WriteLine("Test XML to JSON conversion :\n");
            SfDataGridXmlDto sourceXmlDto = destinationXmlDto;
            SfDataGridJsonDto destinationJsonDto = sourceXmlDto.ToJsonDto(new Dictionary<string, string>
            {
                ["Label"] = "#labelTemplate"
            });
            jsonContent_computed = JsonConvert.SerializeObject(destinationJsonDto, JsonHelper.Settings);

            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
    }
}
