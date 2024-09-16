using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Naninovel.Spreadsheet
{
    // OpenXML documentation: https://docs.microsoft.com/en-us/office/open-xml/spreadsheets

    internal static class OpenXML
    {
        public static SpreadsheetDocument OpenDocument (string path, bool editable)
        {
            try { return SpreadsheetDocument.Open(path, editable); }
            catch (IOException) { throw new IOException($"Failed to open '{path}'. Make sure it's not opened in another program (eg, Excel)."); }
        }

        public static SpreadsheetDocument CreateDocument (string path)
        {
            var document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook, true);
            var workbookPart = document.AddWorkbookPart();
            workbookPart.AddNewPart<WorksheetPart>().Worksheet = new Worksheet(new SheetData());
            workbookPart.Workbook = new Workbook();
            workbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            workbookPart.Workbook.Save();
            return document;
        }

        public static IEnumerable<Worksheet> GetAllSheets (this SpreadsheetDocument document)
        {
            var workbookPart = document.WorkbookPart;
            return workbookPart.Workbook.Descendants<Sheet>()
                .Select(s => ((WorksheetPart)workbookPart.GetPartById(s.Id)).Worksheet);
        }

        public static IEnumerable<string> GetSheetNames (this SpreadsheetDocument document)
        {
            return document.WorkbookPart.Workbook.Descendants<Sheet>().Select(s => s.Name.Value);
        }

        public static string GetSheetName (this Worksheet sheet, SpreadsheetDocument document)
        {
            var relationshipId = document.WorkbookPart.GetIdOfPart(sheet.WorksheetPart);
            return document.WorkbookPart.Workbook.Sheets.Elements<Sheet>()
                .FirstOrDefault(s => s.Id.HasValue && s.Id.Value == relationshipId)?.Name;
        }

        public static Worksheet GetSheet (this SpreadsheetDocument document, string sheetName)
        {
            var workbookPart = document.WorkbookPart;
            var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
            if (sheet is null) return null;
            return ((WorksheetPart)workbookPart.GetPartById(sheet.Id)).Worksheet;
        }

        public static Worksheet AddSheet (this SpreadsheetDocument document, string sheetName)
        {
            var worksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());
            var sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            var relationshipId = document.WorkbookPart.GetIdOfPart(worksheetPart);
            var sheetId = (uint)1;
            if (sheets.Elements<Sheet>().Any())
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            var sheet = new Sheet { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.AppendChild(sheet);
            return ((WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id)).Worksheet;
        }

        public static Cell GetCell (this Worksheet worksheet, string columnName, uint rowNumber)
        {
            var cellReference = columnName + rowNumber;
            return worksheet.GetFirstChild<SheetData>()
                .Elements<Row>().FirstOrDefault(r => r.RowIndex == rowNumber)?
                .Elements<Cell>().FirstOrDefault(c => c.CellReference.Value.EqualsFast(cellReference));
        }

        public static Cell AddCell (this Worksheet worksheet, string columnName, uint rowNumber)
        {
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var cellReference = columnName + rowNumber;

            var row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowNumber);
            if (row is null)
            {
                row = new Row { RowIndex = rowNumber };
                var previousRow = FindRowBefore(worksheet, rowNumber);
                if (previousRow != null)
                    sheetData.InsertAfter(row, previousRow);
                else sheetData.InsertAt(row, 0);
            }

            if (row.Elements<Cell>().Any(c => c.CellReference.Value == cellReference))
                return row.Elements<Cell>().First(c => c.CellReference.Value == cellReference);

            var refCell = default(Cell);
            foreach (var c in row.Elements<Cell>())
            {
                if (string.Compare(c.CellReference.Value, cellReference, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    refCell = c;
                    break;
                }
            }

            var newCell = new Cell { CellReference = cellReference };
            row.InsertBefore(newCell, refCell);
            return newCell;
        }

        public static string GetValue (this Cell cell, SpreadsheetDocument document)
        {
            if (cell.DataType is null || cell.InnerText.Length == 0) return null;

            if (cell.DataType.Value == CellValues.SharedString)
            {
                var stringTable = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                return stringTable?.SharedStringTable.ElementAt(int.Parse(cell.InnerText)).InnerText;
            }

            return cell.InnerText;
        }

        public static string GetCellValue (this SpreadsheetDocument document, Worksheet worksheet, string columnName, uint rowNumber)
        {
            return worksheet.GetCell(columnName, rowNumber)?.GetValue(document);
        }

        public static void SetValue (this Cell cell, SpreadsheetDocument document, string value)
        {
            var sharedIndex = GetOrAddSharedStringItem(document, value);
            cell.CellValue = new CellValue(sharedIndex.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        }

        public static void SetCellValue (this SpreadsheetDocument document, Worksheet worksheet, string columnName, uint rowNumber, string value)
        {
            var cell = worksheet.GetCell(columnName, rowNumber) ?? worksheet.AddCell(columnName, rowNumber);
            cell.SetValue(document, value);
        }

        public static IEnumerable<Cell> GetAllCellsInColumn (this Worksheet worksheet, string columnName)
        {
            var worksheetPart = worksheet.WorksheetPart;
            var rows = worksheetPart.Worksheet.GetFirstChild<SheetData>().Elements<Row>().ToArray();
            if (rows.Length == 0) return Array.Empty<Cell>();
            return rows.SelectMany(r => r.Elements<Cell>().Where(c => c.CellReference.Value == columnName + r.RowIndex));
        }

        public static void ClearAllCellsInColumn (this Worksheet worksheet, string columnName)
        {
            foreach (var cell in worksheet.GetAllCellsInColumn(columnName))
            {
                if (cell?.CellValue != null)
                    cell.CellValue.Text = string.Empty;
            }
        }

        public static string GetColumnNameFromNumber (int number)
        {
            if (number <= 26)
                return Convert.ToChar(number + 64).ToString();
            var div = number / 26;
            var mod = number % 26;
            if (mod == 0)
            {
                mod = 26;
                div--;
            }
            return GetColumnNameFromNumber(div) + GetColumnNameFromNumber(mod);
        }

        public static int GetColumnNumberFromName (string name)
        {
            var digits = new int[name.Length];
            for (int i = 0; i < name.Length; ++i)
                digits[i] = Convert.ToInt32(name[i]) - 64;
            var mul = 1;
            var res = 0;
            for (int pos = digits.Length - 1; pos >= 0; --pos)
            {
                res += digits[pos] * mul;
                mul *= 26;
            }
            return res;
        }

        private static int GetOrAddSharedStringItem (SpreadsheetDocument document, string value)
        {
            var sharedStringPart = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault() ??
                                   document.WorkbookPart.AddNewPart<SharedStringTablePart>();

            if (sharedStringPart.SharedStringTable is null)
                sharedStringPart.SharedStringTable = new SharedStringTable();

            var itemIndex = 0;
            foreach (var item in sharedStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == value)
                    return itemIndex;
                itemIndex++;
            }

            var textValue = new Text(value);
            textValue.Space = new EnumValue<SpaceProcessingModeValues>(SpaceProcessingModeValues.Preserve);
            var sharedItem = new SharedStringItem(textValue);
            sharedStringPart.SharedStringTable.AppendChild(sharedItem);
            sharedStringPart.SharedStringTable.Save();
            return itemIndex;
        }

        private static Row FindRowBefore (Worksheet worksheet, uint rowNumber)
        {
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var previousRow = default(Row);
            foreach (var row in sheetData.Elements<Row>().OrderBy(r => r.RowIndex))
            {
                if (row.RowIndex >= rowNumber) break;
                previousRow = row;
            }
            return previousRow;
        }
    }
}
