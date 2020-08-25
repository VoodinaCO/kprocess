using KProcess.KL2.Languages.Model;
using KProcess.KL2.Languages.Provider;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace KProcess.KL2.Languages.Service
{
    public class LocalizationService : ILocalizationService
    {
        readonly ILanguageStorageProvider _languageStorageProvider;

        public LocalizationService(ILanguageStorageProvider languageStorageProvider)
        {
            _languageStorageProvider = languageStorageProvider;
        }


        /// <summary>
        /// Import excel file to SQL Lite database
        /// </summary>
        /// <param name="filePath">Filepath of the excel file</param>
        public void ImportLocalizedStringsFromExcel(string filePath)
        {
            try
            {
                //var localizedStrings = ReadExcelFile(filePath);
                var localizedStrings = ReadPlainExcelFile(filePath);
                _languageStorageProvider.Save(localizedStrings);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// Import excel file to JSON files
        /// </summary>
        /// <param name="filePath">Filepath of the excel file</param>
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> ImportJSONLocalizedStringsFromExcel(string filePath)
        {
            try
            {
                var localizedStrings = ReadPlainExcelFileForJson(filePath);
                var cultures = localizedStrings.First().Value.Select(_ => _.Culture);
                var dictionaries = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                foreach (var culture in cultures)
                {
                    var values = localizedStrings.Select(_ => _.Value.Single(v => v.Culture == culture))
                        .Select(_ => new 
                        {
                            Language = _.Culture.Split('-')[0],
                            Component = _.Key.Split('.')[0],
                            Key = _.Key.Split('.')[1],
                            Value = _.Value
                        });
                    var dictionary = values.GroupBy(_ => _.Component).ToDictionary(k => k.Key, v => v.ToDictionary(k2 => k2.Key, v2 => v2.Value));
                    dictionaries.Add(culture.Split('-')[0], dictionary);
                }
                return dictionaries;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Export SQL Lite database to Excel file
        /// </summary>
        /// <returns></returns>
        public byte[] ExportLocalizedStringsToExcel()
        {
            try
            {
                var localizedStrings = _languageStorageProvider.LoadLocalizedStrings();

                var numberOfRows = localizedStrings.Keys.Count;
                //var numberOfColumns = localizedStrings.Any() ? localizedStrings.First().Value.Count + 1 : 0;

                using (var xlPackage = new ExcelPackage())
                {
                    // add a sheet
                    var ws = xlPackage.Workbook.Worksheets.Add("LocalizedStrings");

                    // select the range that will be included in the table
                    //var range = ws.Cells[1, 1, numberOfRows+1, numberOfColumns];

                    // add the excel table entity
                    //var table = ws.Tables.Add(range, "LocalizedStringsTable");
                    //table.TableStyle = TableStyles.Light2;
                    //table.ShowHeader = true;
                    
                    if (numberOfRows == 0)
                        return xlPackage.GetAsByteArray();

                    var columnHeaders = new List<string> { "Key" };
                    columnHeaders.AddRange(localizedStrings.Keys);

                    for(int i=0; i < columnHeaders.Count; i++)
                    {
                        //table not used anymore because google spreadsheet doesn't have table feature
                        //table.Columns[i].Name = columnHeaders[i];
                        ws.Cells[1, i + 1].Value = columnHeaders[i];
                    }

                    var keys = localizedStrings.First().Value.Keys.OrderBy(u => u).ToList();
                    for (int i=0; i< keys.Count; i++) // For each string key
                    {
                        var key = keys[i];
                        // First column of the table, 
                        // i + 2 => Because indexing in EFPlus start from 1 (+1) + the first rows is used for header (+1)
                        // 1     => this is key (start from 1)
                        ws.Cells[i + 2, 1].Value = key;

                        // Loop on all the values for each culture
                        for (int j=0; j < columnHeaders.Count; j++)
                        {
                            var value = localizedStrings[columnHeaders[j]][keys[i]];
                            ws.Cells[i+2, j+2].Value = value;
                        }
                    }             
                    return xlPackage.GetAsByteArray();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get GoogleLocalization.xlsx from Google Spreadsheet
        /// </summary>
        /// <param name="spreadsheetId"></param>
        public Stream ImportLocalizedStringsFromSpreadsheet(string spreadsheetId)
        {
            try
            {   
                var url = $"https://docs.google.com/feeds/download/spreadsheets/Export?key={spreadsheetId}&exportFormat=xlsx";
                var myReq = (HttpWebRequest)WebRequest.Create(url);
                myReq.Method = "GET";
                var WebResp = (HttpWebResponse)myReq.GetResponse();

                return WebResp.GetResponseStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        /*IDictionary<string, IList<LocalizedStringValue>> ReadExcelFile(string filePath)
        {
            try
            {
                var dictionary = new Dictionary<string, IList<LocalizedStringValue>>();

                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    var excel = new ExcelPackage(fileStream);
                    var ws = excel.Workbook.Worksheets.FirstOrDefault(u => u.Name == "LocalizedStrings");
                    if (ws == null)
                        throw new Exception("Worksheet with name LocalizedStrings not found");

                    var table = ws.Tables.FirstOrDefault(u => u.Name == "LocalizedStringsTable");
                    if (table == null)
                        throw new Exception("Table with name LocalizedStringsTable not found");


                    // select the range that will be included in the table
                    var start = table.Address.Start;
                    var end = table.Address.End;
                    for (int row = start.Row + 1; row <= end.Row; row++)
                    {
                        var key = ws.Cells[row, 1].Text;
                        if (dictionary.ContainsKey(key))
                            throw new Exception($"Table contains duplicated key with value={key}");

                        dictionary[key] = new List<LocalizedStringValue>();

                        for (int col = start.Column; col < end.Column; col++)
                        {
                            var cell = ws.Cells[row, col + 1];
                            dictionary[key].Add(new LocalizedStringValue
                            {
                                Key = key,
                                Culture = table.Columns[col].Name,
                                Value = cell.Text,
                            });
                        }
                    }
                }
                return dictionary;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }*/

        IDictionary<string, IList<LocalizedStringValue>> ReadPlainExcelFile(string filePath)
        {
            try
            {
                var dictionary = new Dictionary<string, IList<LocalizedStringValue>>();
                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    var excel = new ExcelPackage(fileStream);
                    var ws = excel.Workbook.Worksheets.FirstOrDefault(u => u.Name == "LocalizedStrings");

                    // select the range that will be included in the table
                    if (ws == null)
                        throw new Exception("Worksheet with name LocalizedStrings not found");
                    var dimension = ws.Dimension;
                    int realEndRow = 0;
                    while (!string.IsNullOrEmpty(ws.Cells[realEndRow + 1, 1].Text))
                        realEndRow++;
                    int realEndColumn = 0;
                    while (!string.IsNullOrEmpty(ws.Cells[1, realEndColumn + 1].Text))
                        realEndColumn++;
                    var start = dimension.Start;
                    var end = new ExcelCellAddress(realEndRow, realEndColumn);
                    for (int row = start.Row + 1; row <= end.Row; row++)
                    {
                        var key = ws.Cells[row, 1].Text;
                        if (dictionary.ContainsKey(key))
                            throw new Exception($"Table contains duplicated key with value={key}");

                        dictionary[key] = new List<LocalizedStringValue>();

                        for (int col = start.Column; col < end.Column; col++)
                        {
                            var cell = ws.Cells[row, col + 1];
                            dictionary[key].Add(new LocalizedStringValue
                            {
                                Key = key,
                                Culture = ws.Cells[1, col + 1].Text, //table.Columns[col].Name,
                                Value = cell.Text,
                            });
                        }
                    }
                }
                return dictionary;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        IDictionary<string, IList<LocalizedStringValue>> ReadPlainExcelFileForJson(string filePath)
        {
            try
            {
                var dictionary = new Dictionary<string, IList<LocalizedStringValue>>();
                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    var excel = new ExcelPackage(fileStream);
                    var ws = excel.Workbook.Worksheets.FirstOrDefault(u => u.Name == "JSON");

                    // select the range that will be included in the table
                    if (ws == null)
                        throw new Exception("Worksheet with name JSON not found");
                    var dimension = ws.Dimension;
                    int realEndRow = 0;
                    while (!string.IsNullOrEmpty(ws.Cells[realEndRow + 1, 1].Text))
                        realEndRow++;
                    int realEndColumn = 0;
                    while (!string.IsNullOrEmpty(ws.Cells[1, realEndColumn + 1].Text))
                        realEndColumn++;
                    var start = dimension.Start;
                    var end = new ExcelCellAddress(realEndRow, realEndColumn);
                    for (int row = start.Row + 1; row <= end.Row; row++)
                    {
                        var key = ws.Cells[row, 1].Text;
                        if (dictionary.ContainsKey(key))
                            throw new Exception($"Table contains duplicated key with value={key}");

                        dictionary[key] = new List<LocalizedStringValue>();

                        for (int col = start.Column; col < end.Column; col++)
                        {
                            var cell = ws.Cells[row, col + 1];
                            dictionary[key].Add(new LocalizedStringValue
                            {
                                Key = key,
                                Culture = ws.Cells[1, col + 1].Text, //table.Columns[col].Name,
                                Value = cell.Text,
                            });
                        }
                    }
                }
                return dictionary;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public class ExcelMap
        {
            public string Name { get; set; }
            public string MappedTo { get; set; }
            public int Index { get; set; }
        }

    }


}
