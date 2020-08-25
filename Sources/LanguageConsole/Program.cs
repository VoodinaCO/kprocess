using KProcess.KL2.Languages.Provider;
using KProcess.KL2.Languages.Service;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace LanguageConsole
{
    class Program
    {
        const string spreadsheetId = "1jAsDrLB4X9uTlp4t1eBGo5Va5tnwY1oztytOK4vCEuo";
        const int bufferLength = 256;

        static void Main()
        {
            var languageProvider = new SQLiteLanguageStorageProvider(SQLiteLanguageStorageProvider.kConnectionString);
            ILocalizationService localizationService = new LocalizationService(languageProvider);

            //Get GoogleLocalization.xlsx from Google Spreadsheet
            var answer = localizationService.ImportLocalizedStringsFromSpreadsheet(spreadsheetId);
            ReadWriteStream(answer, File.Create("GoogleLocalization.xlsx"));

            languageProvider.CreateDatabase();

            //import
            localizationService.ImportLocalizedStringsFromExcel("GoogleLocalization.xlsx");
            var dictionaries = localizationService.ImportJSONLocalizedStringsFromExcel("GoogleLocalization.xlsx");
            if (dictionaries != null)
            {
                foreach(var kvp in dictionaries)
                {
                    var singleDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
                    {
                        [kvp.Key] = kvp.Value
                    };
                    using (var jsonFile = File.Create($"{kvp.Key}.json"))
                    using (var jsonWriter = new StreamWriter(jsonFile))
                    {
                        JsonSerializer serializer = new JsonSerializer()
                        {
                            Formatting = Formatting.Indented
                        };
                        serializer.Serialize(jsonWriter, singleDictionary);
                    }
                }
            }

            var sourceInfo = new FileInfo(languageProvider.DatabaseFile);
            var destInfo = new FileInfo("..\\..\\..\\KL2-Core\\Localization.sqlite");
            sourceInfo.CopyTo(destInfo.FullName, true);

            destInfo = new FileInfo("..\\..\\..\\KProcess.KL2.WebAdmin\\App_Data\\Localization.sqlite");
            sourceInfo.CopyTo(destInfo.FullName, true);

            foreach (var language in dictionaries.Keys)
            {
                var srcInfo = new FileInfo($"{language}.json");
                var dstInfo = new FileInfo($"..\\..\\..\\KProcess.KL2.WebAdmin\\Scripts\\ej2\\{language}.json");
                srcInfo.CopyTo(dstInfo.FullName, true);
            }
        }

        static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            byte[] buffer = new byte[bufferLength];
            int bytesRead = readStream.Read(buffer, 0, bufferLength);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, bufferLength);
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}
