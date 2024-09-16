using System.Collections.Generic;
using UnityEngine;

namespace Naninovel.Spreadsheet
{
    public class CompositeSheet
    {
        public const string TemplateHeader = "Template";
        public const string ArgumentHeader = "Arguments";

        public virtual Dictionary<string, List<string>> Columns { get; } = new Dictionary<string, List<string>>();

        public virtual List<string> GetColumnValues (string header)
        {
            if (!Columns.TryGetValue(header, out var values))
            {
                values = new List<string>();
                Columns[header] = values;
            }
            return values;
        }

        public virtual List<string> GetColumnValuesAt (string header, int startIndex, int endIndex)
        {
            var values = GetColumnValues(header);
            var length = Mathf.Min(values.Count - 1, endIndex) - startIndex + 1;
            return values.GetRange(startIndex, length);
        }
    }
}
