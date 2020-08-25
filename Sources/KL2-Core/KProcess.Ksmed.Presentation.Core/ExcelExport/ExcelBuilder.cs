using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using KProcess.Globalization;
using KProcess.Presentation.Windows;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;


namespace KProcess.Ksmed.Presentation.Core.ExcelExport
{
    /// <summary>
    /// Représente un créateur d'exports Excel.
    /// </summary>
    public partial class ExcelExporter
    {

        /// <summary>
        /// Le chemin vers le fichier excel de base
        /// </summary>
        private const string BaseExcelFilePath = @"Resources\Excel base.xlsm";

        private const double DefaultRowHeightInPixels = 20;

        private SpreadsheetDocument _package;

        #region Constructueurs

        /// <summary>
        /// Initialise une instance de la classe <see cref="ExcelExporter"/>.
        /// </summary>
        /// <param name="package">Le package Open XML.</param>
        private ExcelExporter(SpreadsheetDocument package)
        {
            _package = package;
            KeepCellDefaultStyles = false;
        }

        /// <summary>
        /// Crée un nouveau fichier.
        /// </summary>
        /// <param name="filePath">le chemin du nouveau fichier.</param>
        /// <returns>
        /// L'instance de l'exporter.
        /// </returns>
        public static async Task<ExcelExporter> Create(string filePath) =>
            await Create(filePath, Path.Combine(PresentationConstants.AssemblyDirectory, BaseExcelFilePath), true);

        /// <summary>
        /// Crée un nouveau fichier.
        /// </summary>
        /// <param name="filePath">le chemin du nouveau fichier.</param>
        /// <param name="baseFilePath">Le chemin vers le fichier de base.</param>
        /// <param name="deleteAllSheets">if set to <c>true</c> [delete all sheets].</param>
        /// <returns>
        /// L'instance de l'exporter.
        /// </returns>
        public static async Task<ExcelExporter> Create(string filePath, string baseFilePath, bool deleteAllSheets)
        {
            try
            {
                TryOpenFileInWriteMode(filePath);
            }
            catch(Exception e)
            {
                IoC.Resolve<IDialogFactory>().GetDialogView<IErrorDialog>().Show(LocalizationManager.GetString("Common_Error_AlreadyOpenMessage"), LocalizationManager.GetString("Common_Error"), e);

                // Permet aux consomateurs de Excel builder de vérifier que cette exception n'ait pas déjà été notifiée par l'application
                throw new FileAlreadyInUseExeption("The file is not accessible in writting mode. It seems that it is used by another process", e);
            }

            FileStream fileStream = null;
            try
            {
                // Copier les octets
                byte[] baseFile = File.ReadAllBytes(baseFilePath);
                fileStream = File.Create(filePath);
                await fileStream.WriteAsync(baseFile, 0, baseFile.Length);
                await fileStream.FlushAsync();
            }
            catch (Exception e)
            {
                TraceManager.TraceError(e, e.Message);
                throw new InvalidOperationException(e.Message, e);
            }
            finally
            {
                fileStream?.Dispose();
            }

            var package = SpreadsheetDocument.Open(filePath, true);
            var workbookPart = package.WorkbookPart;

            // Ne semble pas être correct : la validation lève des erreurs qui n'en sont en fait pas.
            //var errors = new OpenXmlValidator(FileFormatVersions.Office2007).Validate(package).ToArray();
            //if (errors.Any())
            //    throw new InvalidOperationException("Le fichier excel de base contient déjà des erreurs");

            if (deleteAllSheets)
            {
                // Supprimer les sheets par défaut
                workbookPart.DeleteParts(workbookPart.GetPartsOfType<WorksheetPart>());
                workbookPart.Workbook.Sheets.RemoveAllChildren();

                // Vider la shared string table
                var sharedStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (sharedStringPart != null)
                    sharedStringPart.SharedStringTable.RemoveAllChildren();
            }

            return new ExcelExporter(package);
        }

        #endregion

        #region Méthodes statiques

        /// <summary>
        /// Obtient le nom du fichier combiné à l'extension par défaut.
        /// </summary>
        /// <param name="fileName">Le nom du fichier.</param>
        /// <param name="format">Le format.</param>
        /// <returns>
        /// le nom du fichier combiné à l'extension par défaut.
        /// </returns>
        public static string GetFileNameWithExtension(string fileName, ExcelFormat format = ExcelFormat.Xlsm)
        {
            string extension;
            switch (format)
            {
                case ExcelFormat.Xlsx:
                    extension = ".xlsx";
                    break;
                case ExcelFormat.Xlsm:
                    extension = ".xlsm";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format));
            }


            if (fileName.ToLower().EndsWith(extension))
                return fileName;
            else
                return fileName + extension;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le style des cellules doit toujours être conservé.
        /// </summary>
        public bool KeepCellDefaultStyles { get; set; }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Supprime une feuille.
        /// </summary>
        /// <param name="name">Le nom de la feuille.</param>
        public void DeleteWorksheet(string name)
        {
            var part = GetWorkSheetPart(name);
            var sheet = GetSheet(part);

            if (_package.WorkbookPart.CalculationChainPart != null &&
                _package.WorkbookPart.CalculationChainPart.CalculationChain != null)
            {
                var calcCells = _package.WorkbookPart.CalculationChainPart.CalculationChain
                    .OfType<CalculationCell>()
                    .Where(c => c.SheetId != null && c.SheetId.HasValue && c.SheetId.Value == sheet.SheetId)
                    .ToArray();

                foreach (var calcCell in calcCells)
                    calcCell.Remove();

                if (!_package.WorkbookPart.CalculationChainPart.CalculationChain.Any())
                    _package.WorkbookPart.DeletePart(_package.WorkbookPart.CalculationChainPart);
            }

            _package.WorkbookPart.DeletePart(part);
            sheet.Remove();
        }

        /// <summary>
        /// Supprime les CalculationCell potentiellement invalides de la CalculationChain.
        /// </summary>
        public void ClearInvalidCalcCells()
        {
            if (_package.WorkbookPart.CalculationChainPart != null &&
                _package.WorkbookPart.CalculationChainPart.CalculationChain != null)
            {
                var calcCells = _package.WorkbookPart.CalculationChainPart.CalculationChain
                    .OfType<CalculationCell>()
                    .Where(c => c.SheetId == null || !c.SheetId.HasValue)
                    .ToArray();

                foreach (var calcCell in calcCells)
                    calcCell.Remove();

                if (!_package.WorkbookPart.CalculationChainPart.CalculationChain.Any())
                    _package.WorkbookPart.DeletePart(_package.WorkbookPart.CalculationChainPart);
            }
        }

        /// <summary>
        /// Crée une feuille.
        /// </summary>
        /// <param name="name">Le nom de la feuille.</param>
        /// <returns>La feuille créée.</returns>
        public WorksheetPart CreateSheet(string name)
        {
            var sheetPart = _package.WorkbookPart.AddNewPart<WorksheetPart>();

            // Taille limite
            name = new string(name.Take(30).ToArray());

            Worksheet workSheet = new Worksheet();
            workSheet.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

            workSheet.Append(new SheetData());

            sheetPart.Worksheet = workSheet;

            // Get a unique ID for the new worksheet.
            Sheets sheets = _package.WorkbookPart.Workbook.Sheets;
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Any())
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            var sheet = new Sheet() { Name = MakeValidSheetName(name), Id = _package.WorkbookPart.GetIdOfPart(sheetPart), SheetId = sheetId };
            _package.WorkbookPart.Workbook.Sheets.Append(sheet);

            return sheetPart;
        }

        /// <summary>
        /// Copie une feuille.
        /// </summary>
        /// <param name="sheetName">Le nom de la source.</param>
        /// <param name="clonedSheetName">Le nom de la destination.</param>
        /// <param name="insertAfter">La feuille après laquelle insérer la nouvelle feuille. Si <c>null</c>, ajoute après la source.</param>
        /// <returns>
        /// La nouvelle feuille.
        /// </returns>
        public WorksheetPart CopySheet(string sheetName, string clonedSheetName, WorksheetPart insertAfter = null)
        {
            WorksheetPart sourceSheetPart = GetWorkSheetPart(sheetName);

            // AddPart fait du deep cloning
            SpreadsheetDocument tempSheet = SpreadsheetDocument.Create(new MemoryStream(), _package.DocumentType);
            WorkbookPart tempWorkbookPart = tempSheet.AddWorkbookPart();
            WorksheetPart tempWorksheetPart = tempWorkbookPart.AddPart(sourceSheetPart);

            // Ajouter le clone
            WorksheetPart clonedSheet = _package.WorkbookPart.AddPart(tempWorksheetPart);

            int numTableDefParts = sourceSheetPart.GetPartsOfType<TableDefinitionPart>().Count();

            // Corriger les ids
            if (numTableDefParts != 0)
                FixupTableParts(clonedSheet, (uint)numTableDefParts);

            // Vérifier qu'une seule vue a le focus
            CleanView(clonedSheet);

            Sheets sheets = _package.WorkbookPart.Workbook.GetFirstChild<Sheets>();

            Sheet copiedSheet = new Sheet
            {
                Name = MakeValidSheetName(clonedSheetName),
                Id = _package.WorkbookPart.GetIdOfPart(clonedSheet),
                SheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1
            };
            sheets.InsertAfter(copiedSheet, GetSheet(insertAfter ?? sourceSheetPart));

            return clonedSheet;
        }

        /// <summary>
        /// Obtient une feuille à partir de son nom.
        /// </summary>
        /// <param name="sheetName">Le nom de la feuille.</param>
        /// <returns>La feuille.</returns>
        public WorksheetPart GetWorkSheetPart(string sheetName)
        {
            //Get the relationship id of the sheetname
            string relId = _package.WorkbookPart.Workbook.Descendants<Sheet>()
.First(s => s.Name.Value.Equals(sheetName)).Id;
            return (WorksheetPart)_package.WorkbookPart.GetPartById(relId);
        }

        /// <summary>
        /// Renomme une feuille.
        /// </summary>
        /// <param name="worksheetPart">Le worksheetPart.</param>
        /// <param name="name">Le nouveau nom.</param>
        public void RenameSheet(WorksheetPart worksheetPart, string name)
        {
            GetSheet(worksheetPart).Name = name;
        }

        /// <summary>
        /// Ajoute des données dans la feuille.
        /// </summary>
        /// <param name="part">La feuille.</param>
        /// <param name="headers">Les entêtes.</param>
        /// <param name="data">Les données.</param>
        public void AddTable(WorksheetPart part, ColumnFormat[] headers, CellContent[][] data)
        {
            AddTable(part, headers, data, new CellReference());
        }

        /// <summary>
        /// Ajoute des données dans la feuille.
        /// </summary>
        /// <param name="part">La feuille.</param>
        /// <param name="headers">Les entêtes.</param>
        /// <param name="data">Les données.</param>
        /// <param name="startCellReference">La référence de la cellule où poser les données.</param>
        public void AddTable(WorksheetPart part, ColumnFormat[] headers, CellContent[][] data, CellReference startCellReference)
        {
            Assertion.NotNull(part.Worksheet, "part.Worksheet");

            var sheetData = part.Worksheet.GetFirstChild<SheetData>();

            // Ajouter les entêtes
            if (headers != null)
            {
                var rowHeaders = new Row() { RowIndex = startCellReference.RowIndex };
                foreach (var header in headers)
                {
                    var cell = new Cell()
                    {
                        CellReference = startCellReference.Reference,
                    };
                    FormatCellContent(cell, header.Header);

                    startCellReference.MoveRight();
                    rowHeaders.Append(cell);
                }
                sheetData.Append(rowHeaders);
                startCellReference.NewLine();
            }

            // Ajouter les valeurs
            int columnsLength = headers.Length;
            foreach (var datarow in data)
            {
                var row = new Row() { RowIndex = startCellReference.RowIndex };

                for (int i = 0; i < columnsLength; i++)
                {
                    var format = headers[i];
                    var cell = new Cell()
                    {
                        CellReference = startCellReference.Reference,
                    };
                    FormatCellContent(cell, datarow[i]);

                    startCellReference.MoveRight();
                    row.Append(cell);
                }

                sheetData.Append(row);

                startCellReference.NewLine();
            }

            var sheetDimension = new SheetDimension() { Reference = startCellReference.Dimensions };
        }

        /// <summary>
        /// Définit la valeur à l'emplacement spécifié d'une cellule.
        /// </summary>
        /// <param name="part">La feuille.</param>
        /// <param name="cellReference">La référence vers la cellule.</param>
        /// <param name="dataType">Le type de la donnée.</param>
        /// <param name="data">La donnée.</param>
        public void SetCellValue(WorksheetPart part, CellReference cellReference, CellContent content)
        {
            var cell = InsertCellInWorksheet(part, cellReference);
            FormatCellContent(cell, content);
        }

        /// <summary>
        /// Obtient la hauteur d'une ligne.
        /// </summary>
        /// <param name="part">La feuille.</param>
        /// <param name="rowIndex">L'index de la ligne.</param>
        /// <returns>La hauteur de la ligne ou <c>null</c> si la ligne n'existe pas.</returns>
        public double? GetRowHeight(WorksheetPart part, uint rowIndex)
        {
            Worksheet worksheet = part.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            Row row = GetRow(sheetData, rowIndex, false);

            if (row != null)
                return row.Height;
            return null;
        }

        /// <summary>
        /// Définit la hauteur personnalisée d'une ligne.
        /// </summary>
        /// <param name="part">La feuille.</param>
        /// <param name="rowIndex">L'index de la ligne.</param>
        /// <param name="height">La hauteur.</param>
        public void SetRowCustomHeight(WorksheetPart part, uint rowIndex, double height)
        {
            Worksheet worksheet = part.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            Row row = GetRow(sheetData, rowIndex, false);
            if (row == null)
                throw new InvalidOperationException("La ligne n'existe pas");

            row.Height = height;
            row.CustomHeight = true;
        }

        /// <summary>
        /// Sauvegarde le fichier et le ferme.
        /// L'excel builder n'est plus utilisable par la suite.
        /// </summary>
        public void SaveAndClose()
        {
            // Ne semble pas être correct : la validation lève des erreurs qui n'en sont en fait pas.
            //var errors = new OpenXmlValidator(FileFormatVersions.Office2007).Validate(_package).ToArray();
            //if (errors.Any())
            //    throw new InvalidOperationException("Des erreurs ont eu lieu car le fichier produit est incorrect");

            _package.Dispose();
        }

        /// <summary>
        /// Invalide les résultats des calculs des cellules dans les plages spécifiées.
        /// Au prochain lancement d'Excel, ces cellules seront automatiquement recalculées.
        /// </summary>
        /// <param name="worksheetPart">La feuille.</param>
        /// <param name="ranges">Les plages.</param>
        public void InvalidateCellFormulaResults(WorksheetPart worksheetPart, params CellRange[] ranges)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            foreach (var range in ranges)
            {
                for (uint rowIndex = range.From.RowIndex; rowIndex <= range.To.RowIndex; rowIndex++)
                {
                    var row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
                    if (row != null)
                    {
                        for (uint columnIndex = range.From.ColumnIndex; columnIndex <= range.To.ColumnIndex; columnIndex++)
                        {
                            var cellReference = new CellReference(columnIndex, rowIndex);
                            var cell = row.Elements<Cell>().FirstOrDefault(c => c.CellReference.Value == cellReference.Reference);
                            if (cell != null && cell.CellFormula != null && cell.CellValue != null)
                            {
                                cell.CellValue.Remove();
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Ajoute une image dans une feuille.
        /// </summary>
        /// <param name="worksheetPart">La feuille.</param>
        /// <param name="image">Les données de l'image.</param>
        /// <param name="imageType">Le type de l'image.</param>
        /// <param name="filename">Le nom du fichier.</param>
        /// <param name="pictureName">Le nom de l'image.</param>
        /// <param name="dimensions">Les dimensions de l'image, en pixels.</param>
        /// <param name="position">La position de l'image dans la feuille.</param>
        /// <returns>Le nombre de ligne que l'image a prise.</returns>
        public uint AddImage(WorksheetPart worksheetPart, byte[] image, ImagePartType imageType, string filename, string pictureName, System.Windows.Size dimensions, CellReference position)
        {
            if (worksheetPart.DrawingsPart == null)
                worksheetPart.AddNewPart<DrawingsPart>();

            var imageId = "rId" + Guid.NewGuid().ToString("N");

            ImagePart imgp = worksheetPart.DrawingsPart.AddImagePart(imageType, imageId);

            using (var ms = new MemoryStream(image))
            {
                imgp.FeedData(ms);
            }

            uint fromCol = position.ColumnIndex - 1;
            int fromColOffset = 0;
            uint fromRow = position.RowIndex - 1;
            int fromRowOffset = 0;

            int widthEmu = PixelsToEmus(dimensions.Width);
            int heightEmu = PixelsToEmus(dimensions.Height);

            string oneCellAnchor = @"<xdr:oneCellAnchor xmlns:xdr='http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing' xmlns:a='http://schemas.openxmlformats.org/drawingml/2006/main'>
    <xdr:from>
      <xdr:col>" + fromCol + @"</xdr:col>
      <xdr:colOff>" + fromColOffset + @"</xdr:colOff>
      <xdr:row>" + fromRow + @"</xdr:row>
      <xdr:rowOff>" + fromRowOffset + @"</xdr:rowOff>
    </xdr:from>    
    <xdr:ext cx='" + widthEmu + "' cy='" + heightEmu + @"' />
    <xdr:pic>
      <xdr:nvPicPr>
        <xdr:cNvPr id='4' name='" + pictureName + @"'/>
        <xdr:cNvPicPr>
          <a:picLocks noChangeAspect='1'/>
        </xdr:cNvPicPr>
      </xdr:nvPicPr>
      <xdr:blipFill>
        <a:blip xmlns:r='http://schemas.openxmlformats.org/officeDocument/2006/relationships' r:embed='" + imageId + @"' cstate='print'>
          <a:extLst>
            <a:ext uri='{28A0092B-C50C-407E-A947-70E740481C1C}'>
              <a14:useLocalDpi xmlns:a14='http://schemas.microsoft.com/office/drawing/2010/main' val='0'/>
            </a:ext>
          </a:extLst>
        </a:blip>
        <a:stretch>
          <a:fillRect/>
        </a:stretch>
      </xdr:blipFill>
      <xdr:spPr>
        <a:xfrm>
          <a:off x='0' y='0'/>
          <a:ext cx='0' cy='0'/>
        </a:xfrm>
        <a:prstGeom prst='rect'>
          <a:avLst/>
        </a:prstGeom>
      </xdr:spPr>
    </xdr:pic>
    <xdr:clientData/>
  </xdr:oneCellAnchor>";

            if (worksheetPart.DrawingsPart.WorksheetDrawing == null)
            {
                worksheetPart.DrawingsPart.WorksheetDrawing = new Xdr.WorksheetDrawing();
            }

            worksheetPart.DrawingsPart.WorksheetDrawing.Append(new Xdr.OneCellAnchor(oneCellAnchor));

            var drawingsPartId = worksheetPart.GetIdOfPart(worksheetPart.DrawingsPart);
            if (!worksheetPart.Worksheet.Elements<Drawing>().Any(d => d.Id == drawingsPartId))
                worksheetPart.Worksheet.Append(new Drawing() { Id = drawingsPartId });

            var rows = (uint)Math.Ceiling(dimensions.Height / DefaultRowHeightInPixels);
            return rows;
        }

        /// <summary>
        /// Crée une zone nommée.
        /// </summary>
        /// <param name="sheet">La feuille.</param>
        /// <param name="reference">La zone.</param>
        /// <param name="name">Le nom.</param>
        public void CreateDefinedName(WorksheetPart sheet, CellReference reference, string name)
        {
            if (_package.WorkbookPart.Workbook.DefinedNames == null)
                _package.WorkbookPart.Workbook.DefinedNames = new DefinedNames();

            string content = string.Format("'{0}'!${1}${2}", GetSheet(sheet).Name, reference.ColumnReference, reference.RowIndex);

            _package.WorkbookPart.Workbook.DefinedNames.Append(new DefinedName
            {
                Name = name,
                Text = content,
            });
        }

        /// <summary>
        /// Ajoute un hyperlien vers une zone nommée.
        /// </summary>
        /// <param name="sheet">La feuille.</param>
        /// <param name="reference">La cellule où sera situé l'hyperlien.</param>
        /// <param name="location">Le nom de la zone.</param>
        /// <param name="tooltip">L'info bulle.</param>
        public void AddHyperlinkToDefinedName(WorksheetPart sheet, CellReference reference, string location, string tooltip)
        {
            var hls = sheet.Worksheet.GetFirstChild<Hyperlinks>();
            if (hls == null)
            {
                hls = new Hyperlinks();
                sheet.Worksheet.Append(hls);
            }

            var hl = new Hyperlink
            {
                Reference = reference.Reference,
                Location = location,
                Display = tooltip,
            };

            hls.Append(hl);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Détermine s'il est possible d'écrire/créer dans un fichier au chemain spécifié
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static void TryOpenFileInWriteMode(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            using (FileStream stream = File.OpenWrite(filePath))
            { }
        }

        /// <summary>
        /// Insère une cellule dans la feuille ou la renvoie si elle existe déjà.
        /// </summary>
        /// <param name="worksheetPart">La feuille.</param>
        /// <param name="cellReference">La référence de la cellule.</param>
        /// <returns>La cellule.</returns>
        private Cell InsertCellInWorksheet(WorksheetPart worksheetPart, CellReference cellReference)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            Row row = GetRow(sheetData, cellReference.RowIndex, true);

            var ce = row.Elements<Cell>().FirstOrDefault(c => c.CellReference.Value == cellReference.Reference);
            if (ce != null)
                return ce;
            else
            {
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference.Reference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference.Reference };
                row.InsertBefore(newCell, refCell);

                return newCell;
            }
        }

        /// <summary>
        /// Obtient un Row à partir de la référence de la cellule.
        /// </summary>
        /// <param name="sheetData">Les données de la feuille.</param>
        /// <param name="rowIndex">L'index de la ligne.</param>
        /// <param name="autoCreate">if set to <c>true</c> [auto create].</param>
        /// <returns>
        /// Le row
        /// </returns>
        private static Row GetRow(SheetData sheetData, uint rowIndex, bool autoCreate)
        {
            Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            if (row == null && autoCreate)
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }
            return row;
        }

        /// <summary>
        /// Obtient un Sheet à partir d'un WorksheetPart.
        /// </summary>
        /// <param name="worksheetPart">Le worksheet part.</param>
        /// <returns>Le sheet</returns>
        private Sheet GetSheet(WorksheetPart worksheetPart)
        {
            var sheets = _package.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            var sheetId = _package.WorkbookPart.GetIdOfPart(worksheetPart);
            var sheet = sheets.OfType<Sheet>().FirstOrDefault(s => s.Id == sheetId);
            return sheet;
        }

        /// <summary>
        /// Corrige les ids des tables en auto increment.
        /// </summary>
        /// <param name="worksheetPart">La feuille.</param>
        /// <param name="numTableDefParts">L'id de départ (par ex le plus grand actuel).</param>
        private void FixupTableParts(WorksheetPart worksheetPart, uint numTableDefParts)
        {
            //Every table needs a unique id and name
            foreach (TableDefinitionPart tableDefPart in worksheetPart.TableDefinitionParts)
            {
                numTableDefParts++;
                tableDefPart.Table.Id = numTableDefParts;
                tableDefPart.Table.DisplayName = "CopiedTable" + numTableDefParts;
                tableDefPart.Table.Name = "CopiedTable" + numTableDefParts;
                tableDefPart.Table.Save();
            }
        }

        /// <summary>
        /// Supprime la vue de la feuille.
        /// </summary>
        /// <param name="worksheetPart">La feuille.</param>
        private void CleanView(WorksheetPart worksheetPart)
        {
            //There can only be one sheet that has focus
            SheetViews views = worksheetPart.Worksheet.GetFirstChild<SheetViews>();
            if (views != null)
            {
                views.Remove();
                worksheetPart.Worksheet.Save();
            }
        }

        /// <summary>
        /// Insère une chaîne de caractères partagées dans la table.
        /// </summary>
        /// <param name="text">Le text.</param>
        /// <returns>L'identifiant de la chaîne.</returns>
        private string InsertSharedStringItem(string text)
        {
            var sharedStringPart = _package.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            if (sharedStringPart == null)
                sharedStringPart = _package.WorkbookPart.AddNewPart<SharedStringTablePart>();

            // If the part does not contain a SharedStringTable, create one.
            if (sharedStringPart.SharedStringTable == null)
                sharedStringPart.SharedStringTable = new SharedStringTable();

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in sharedStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                    return i.ToString();

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            sharedStringPart.SharedStringTable.AppendChild(new SharedStringItem(
                new DocumentFormat.OpenXml.Spreadsheet.Text(text) { Space = SpaceProcessingModeValues.Preserve }));
            sharedStringPart.SharedStringTable.Save();

            return i.ToString();
        }

        /// <summary>
        /// Formatte le contenu de la cellule.
        /// </summary>
        /// <param name="cell">La cellule.</param>
        /// <param name="dataType">Le type de la donnée.</param>
        /// <param name="data">La donnée.</param>
        /// <param name="keepStyle"><c>true</c> pour conserver le style déjà présent sur cette cellule.</param>
        public void FormatCellContent(Cell cell, CellContent content)
        {
            CellValues cellValues;
            CellValue value = null;

            if (content == null)
                return;

            switch (content.DataType)
            {
                case CellDataType.String:
                case CellDataType.Hyperlink:
                    cellValues = CellValues.SharedString;
                    if (!string.IsNullOrEmpty(content.Value))
                        value = new CellValue(InsertSharedStringItem(content.Value));
                    break;

                case CellDataType.Number:
                case CellDataType.Percentage:
                case CellDataType.Date:
                case CellDataType.TimeSpan:
                    cellValues = CellValues.Number;
                    value = new CellValue(content.Value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();

            }

            if (!string.IsNullOrEmpty(content.Value))
            {
                cell.DataType = cellValues;
                cell.CellValue = value;
            }

            if (!this.KeepCellDefaultStyles)
            {
                uint cellStyleIndex = GetCellStyleIndex(content.DataType, content.Value != null ? content.Value.Contains(Environment.NewLine) : false);
                cell.StyleIndex = cellStyleIndex;
            }
        }

     
        /// <summary>
        /// Obtient l'index du style pour le type de données spécifié.
        /// </summary>
        /// <param name="dataType">le type de données.</param>
        /// <param name="wrapText"><c>true</c> si le texte doit retourner à la ligne.</param>
        /// <returns>
        /// L'index du style.
        /// </returns>
        private uint GetCellStyleIndex(CellDataType dataType, bool wrapText)
        {
            var stylesheet = _package.WorkbookPart.WorkbookStylesPart.Stylesheet;

            var numberingFormats = stylesheet.NumberingFormats;
            if (numberingFormats == null)
            {
                numberingFormats = new NumberingFormats();
                stylesheet.NumberingFormats = numberingFormats;
            }

            var cellFormats = stylesheet.CellFormats;
            if (cellFormats == null)
            {
                cellFormats = new CellFormats();
                stylesheet.Append(cellFormats);
            }


            string numberFormatCode = null;
            uint numberFormatId = 0; // Les format ids spécifiés correspondent à ceux définis par défaut dans Excel. Voir la liste dans la section 18.8.30 des specs ECMA OPEN XML
            int cellStyleId = -1;

            switch (dataType)
            {
                case CellDataType.String:
                case CellDataType.Number:
                    break;

                case CellDataType.TimeSpan:
                    numberFormatId = 46; // [h]:mm:ss
                    break;

                case CellDataType.Date:
                    numberFormatId = 14; // mm-dd-yy
                    break;

                case CellDataType.Hyperlink:
                    cellStyleId = 42; // Non présent dans les specs ECMA, mais présents dans Office > 2007
                    break;

                case CellDataType.Percentage:
                    numberFormatId = 10; // 0.00%
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Rechercher le number format
            if (numberFormatCode != null)
            {
                var numberingFormat = numberingFormats.Elements<NumberingFormat>().FirstOrDefault(nf => nf.FormatCode == numberFormatCode);
                if (numberingFormat == null)
                {
                    // Créer le numbering format
                    numberingFormat = new NumberingFormat()
                    {
                        FormatCode = numberFormatCode,
                    };

                    if (numberingFormats.Elements<NumberingFormat>().Any())
                        numberingFormat.NumberFormatId = numberingFormats.Elements<NumberingFormat>().Select(nf => nf.NumberFormatId).Max() + 1;
                    else
                        numberingFormat.NumberFormatId = 100; // Commencer à 100 pour ne pas écraser les formats livrés par défaut, qui sont connus par excel mais non présents dans le fichier.

                    numberingFormats.Append(numberingFormat);
                }
                numberFormatId = numberingFormat.NumberFormatId;
            }

            if (cellStyleId != -1)
            {
                var cellFormat = cellFormats.ChildElements.OfType<CellFormat>().FirstOrDefault(cf => cf.FormatId == 42);
                if (cellFormat != null)
                    return (uint)cellFormats.IndexOf(cellFormat);
                else
                    return 1;
            }

            if (numberFormatId != 0)
            {
                CellFormat cellFormat;
                // rechercher le Cell format
                if (wrapText)
                    cellFormat = cellFormats.Elements<CellFormat>().FirstOrDefault(cf => cf.NumberFormatId == numberFormatId && cf.ApplyAlignment != null && cf.ApplyAlignment == true && cf.Alignment != null && cf.Alignment.WrapText);
                else
                    cellFormat = cellFormats.Elements<CellFormat>().FirstOrDefault(cf => cf.NumberFormatId == numberFormatId);

                if (cellFormat == null)
                {
                    cellFormat = new CellFormat()
                    {
                        NumberFormatId = numberFormatId,
                        FontId = 0,
                        FillId = 0,
                        BorderId = 0,
                        FormatId = 0,
                        ApplyNumberFormat = numberFormatId != 0,
                    };

                    if (wrapText)
                    {
                        cellFormat.ApplyAlignment = true;
                        cellFormat.Alignment = new Alignment() { WrapText = true };
                    }

                    cellFormats.Append(cellFormat);
                }

                return (uint)cellFormats.IndexOf(cellFormat);
            }
            else if (wrapText)
            {
                // Rechercher :
                //<x:xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0" applyAlignment="1">
                //  <x:alignment wrapText="1" />
                //</x:xf>

                var cellFormat = cellFormats.Elements<CellFormat>().FirstOrDefault(cf => cf.NumberFormatId == 0 && cf.ApplyAlignment != null && cf.ApplyAlignment == true && cf.Alignment != null && cf.Alignment.WrapText);
                if (cellFormat == null)
                {

                    cellFormat = new CellFormat()
                    {
                        NumberFormatId = 0,
                        FontId = 0,
                        FillId = 0,
                        BorderId = 0,
                        FormatId = 0,
                        ApplyNumberFormat = false,
                        ApplyAlignment = true,
                        Alignment = new Alignment() { WrapText = true },
                    };

                    cellFormats.Append(cellFormat);
                }

                return (uint)cellFormats.IndexOf(cellFormat);
            }
            else
                return 0;
        }

        /// <summary>
        /// Crée un nom valide pour une feuille.
        /// </summary>
        /// <param name="name">Le nom voulu.</param>
        /// <returns>Le nom valide</returns>
        private string MakeValidSheetName(string name)
        {
            if (name.Length > 31)
                return name.Substring(0, 31);
            else
                return name;
        }

        /// <summary>
        /// Convertit des Pixels en Emus.
        /// </summary>
        /// <param name="pixels">Les pixels.</param>
        /// <returns>Les emus.</returns>
        private int PixelsToEmus(double pixels)
        {
            return (int)((pixels / 96.0) * 914400.0);
        }

        #endregion

    }
}
