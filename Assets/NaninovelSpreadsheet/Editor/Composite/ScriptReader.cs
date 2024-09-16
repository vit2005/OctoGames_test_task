using System.Collections.Generic;
using System.Linq;
using Naninovel.Parsing;
using UnityEngine;
using static Naninovel.Spreadsheet.CompositeSheet;

namespace Naninovel.Spreadsheet
{
    public class ScriptReader
    {
        protected virtual CompositeSheet Composite { get; }
        protected virtual LocaleTagsCache LocaleTagsCache { get; }
        protected virtual TemplateBuilder TemplateBuilder { get; }
        protected virtual List<Token> Tokens { get; }
        protected virtual Lexer Lexer { get; }
        protected virtual ScriptText Script { get; set; }
        protected virtual IReadOnlyCollection<ScriptText> Localizations { get; set; }

        public ScriptReader (CompositeSheet composite)
        {
            Composite = composite;
            LocaleTagsCache = new LocaleTagsCache();
            TemplateBuilder = new TemplateBuilder(composite);
            Tokens = new List<Token>();
            Lexer = new Lexer();
        }

        public virtual void Read (ScriptText script, IReadOnlyCollection<ScriptText> localizations)
        {
            ResetState(script, localizations);
            for (int i = 0; i < script.TextLines.Count; i++)
                ReadLine(script.TextLines[i], script.Script.Lines[i]);
            if (TemplateBuilder.Length > 0) FillSheet();
        }

        protected virtual void ResetState (ScriptText script, IReadOnlyCollection<ScriptText> localizations)
        {
            Script = script;
            Localizations = localizations;
        }

        protected virtual void ReadLine (string lineText, ScriptLine line)
        {
            Tokens.Clear();
            var lineType = Lexer.TokenizeLine(lineText, Tokens);
            var composite = new Composite(lineText, lineType, Tokens);
            TemplateBuilder.Append(composite);

            if (composite.Arguments.Count > 0)
                foreach (var localizationScript in Localizations)
                    ReadArgument(localizationScript, line, composite);
        }

        protected virtual void ReadArgument (ScriptText localizationScript, ScriptLine line, Composite composite)
        {
            var locale = ExtractScriptLocaleTag(localizationScript);
            var localizedValues = GetLocalizedValues(line, locale, localizationScript, composite.Arguments.Count);
            Composite.GetColumnValues(locale).AddRange(localizedValues);
        }

        protected virtual void FillSheet ()
        {
            var lastTemplateValue = TemplateBuilder.Build().TrimEnd(StringUtils.NewLineChars);
            Composite.GetColumnValues(TemplateHeader).Add(lastTemplateValue);
        }

        protected virtual string ExtractScriptLocaleTag (ScriptText localizationScript)
        {
            var firstCommentText = localizationScript.Script.Lines.OfType<CommentScriptLine>().FirstOrDefault()?.CommentText;
            return LocaleTagsCache.Get(localizationScript.GetHashCode(), firstCommentText);
        }

        protected virtual IReadOnlyList<string> GetLocalizedValues (ScriptLine line, string locale, ScriptText localizationScript, int argsCount)
        {
            var startIndex = localizationScript.Script.GetLineIndexForLabel(line.LineHash);
            if (startIndex == -1)
                throw new Error($"Failed to find `{locale}` localization for `{Script.Name}` script at line #{line.LineNumber}. Try re-generating localization documents.");
            var endIndex = localizationScript.Script.FindLine<LabelScriptLine>(l => l.LineIndex > startIndex)?.LineIndex ?? localizationScript.Script.Lines.Count;
            var localizationLines = localizationScript.Script.Lines
                .Where(l => (l is CommandScriptLine || l is GenericTextScriptLine gl && gl.InlinedCommands.Count > 0) && l.LineIndex > startIndex && l.LineIndex < endIndex).ToArray();
            if (localizationLines.Length > 1)
                Debug.LogWarning($"Multiple `{locale}` localization lines found for `{Script.Name}` script at line #{line.LineNumber}. Only the first one will be exported to the spreadsheet.");
            if (localizationLines.Length == 0)
                return Enumerable.Repeat(string.Empty, argsCount).ToArray();

            var localizationLineText = localizationScript.TextLines[localizationLines.First().LineIndex];
            Tokens.Clear();
            var lineType = Lexer.TokenizeLine(localizationLineText, Tokens);
            var localizedComposite = new Composite(localizationLineText, lineType, Tokens);
            if (localizedComposite.Arguments.Count != argsCount)
                throw new Error($"`{locale}` localization for `{Script.Name}` script at line #{line.LineNumber} is invalid. Make sure it preserves original commands.");
            return localizedComposite.Arguments;
        }
    }
}
