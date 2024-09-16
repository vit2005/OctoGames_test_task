using System.Collections.Generic;

namespace Naninovel.Spreadsheet
{
    public class ScriptText
    {
        public Script Script { get; }
        public IReadOnlyList<string> TextLines { get; }
        public string Name => Script.Name;

        public ScriptText (Script script, string scriptText)
        {
            Script = script;
            TextLines = Parsing.ScriptParser.SplitText(scriptText);
            if (Script.Lines.Count != TextLines.Count)
                throw new Error($"Failed to parse `{Script.name}` script: line count is not equal.");
        }
    }
}
