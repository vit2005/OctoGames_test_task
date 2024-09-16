using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Naninovel.Spreadsheet
{
    public class SpreadsheetReader
    {
        protected virtual CompositeSheet Composite { get; }
        protected virtual SpreadsheetDocument Document { get; set; }
        protected virtual Worksheet Sheet { get; set; }

        public SpreadsheetReader (CompositeSheet composite)
        {
            Composite = composite;
        }

        public virtual void Read (SpreadsheetDocument document, Worksheet sheet)
        {
            ResetState(document, sheet);

            for (int columnNumber = 1;; columnNumber++)
            {
                var columnName = OpenXML.GetColumnNameFromNumber(columnNumber);
                var cells = GetCells(columnName);
                if (cells.Length == 0) break;

                var header = cells[0].GetValue(document);
                if (columnNumber > 2 && !LanguageTags.ContainsTag(header)) break;

                for (int rowIndex = 1; rowIndex < cells.Length; rowIndex++)
                    ReadRow(cells[rowIndex], header);
            }
        }

        protected virtual void ResetState (SpreadsheetDocument document, Worksheet sheet)
        {
            Document = document;
            Sheet = sheet;
        }

        protected virtual Cell[] GetCells (string columnName)
        {
            return Sheet
                .GetAllCellsInColumn(columnName)
                .OrderBy(c => c.Ancestors<Row>().FirstOrDefault()?.RowIndex ?? uint.MaxValue)
                .ToArray();
        }

        protected virtual void ReadRow (Cell cell, string header)
        {
            var cellValue = cell.GetValue(Document);
            Composite.GetColumnValues(header).Add(cellValue);
        }
    }
}
