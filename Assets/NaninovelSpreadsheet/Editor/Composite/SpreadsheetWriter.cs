using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Naninovel.Spreadsheet
{
    public class SpreadsheetWriter
    {
        protected virtual CompositeSheet Composite { get; }
        protected virtual SpreadsheetDocument Document { get; set; }
        protected virtual Worksheet Sheet { get; set; }

        public SpreadsheetWriter (CompositeSheet composite)
        {
            Composite = composite;
        }

        public virtual void Write (SpreadsheetDocument document, Worksheet sheet)
        {
            ResetState(document, sheet);
            for (int i = 0; i < Composite.Columns.Count; i++)
            {
                var kv = Composite.Columns.ElementAt(i);
                var columnName = OpenXML.GetColumnNameFromNumber(i + 1);
                WriteColumn(kv.Key, columnName, kv.Value);
            }
            sheet.Save();
        }

        protected virtual void ResetState (SpreadsheetDocument document, Worksheet sheet)
        {
            Document = document;
            Sheet = sheet;
        }

        protected virtual void WriteColumn (string header, string columnName, IEnumerable<string> values)
        {
            var rowNumber = (uint)1;
            Sheet.ClearAllCellsInColumn(columnName);
            Document.SetCellValue(Sheet, columnName, rowNumber, header);
            foreach (var value in values)
            {
                rowNumber++;
                Document.SetCellValue(Sheet, columnName, rowNumber, value);
            }
        }
    }
}
