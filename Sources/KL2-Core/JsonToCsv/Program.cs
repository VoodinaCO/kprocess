using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonToCsv
{
    class Program
    {
        /* Get json files on https://github.com/syncfusion/ej2-locale/tree/master/src */
        static void Main(string[] args)
        {
            var languages = new[] { "fr", "en", "de", "es", "pt", "pl" };

            foreach(var language in languages)
            {
                using (var jsonFile = File.OpenRead($"{language}.json"))
                using (var jsonReader = new StreamReader(jsonFile, Encoding.UTF8))
                using (var csvFile = File.Create($"{language}.csv"))
                using (var csvWriter = new StreamWriter(csvFile, Encoding.UTF8))
                {
                    var json = jsonReader.ReadToEnd();
                    var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json);

                    foreach (var component in jsonObject[language])
                    {
                        foreach (var label in component.Value)
                        {
                            csvWriter.WriteLine($"{component.Key}.{label.Key};{label.Value.Replace("\"", "\\\"")}");
                        }
                    }
                }
            }
        }
    }
}
