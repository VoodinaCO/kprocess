using System.Collections.Generic;
using System.IO;

namespace KProcess.KL2.Languages.Service
{
    public interface ILocalizationService
    {
        /// <summary>
        /// Import excel file to SQL Lite database
        /// </summary>
        /// <param name="filePath">Filepath of the excel file</param>
        void ImportLocalizedStringsFromExcel(string filePath);

        /// <summary>
        /// Import excel file to JSON files
        /// </summary>
        /// <param name="filePath">Filepath of the excel file</param>
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> ImportJSONLocalizedStringsFromExcel(string filePath);

        /// <summary>
        /// Export SQL Lite database to Excel file
        /// </summary>
        /// <returns></returns>
        byte[] ExportLocalizedStringsToExcel();

        /// <summary>
        /// Get GoogleLocalization.xlsx from Google Spreadsheet
        /// </summary>
        /// <param name="spreadsheetId">Spreadsheet Id of the Google spreadsheet</param>
        Stream ImportLocalizedStringsFromSpreadsheet(string spreadsheetId);
    }
}
