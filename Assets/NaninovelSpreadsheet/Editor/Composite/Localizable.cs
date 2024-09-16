using Naninovel.Parsing;

namespace Naninovel.Spreadsheet
{
    public class Localizable
    {
        public string Text { get; }
        public LineRange Range { get; }

        public Localizable (string text, LineRange range)
        {
            Text = text;
            Range = range;
        }
    }
}
