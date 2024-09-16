using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Naninovel.Spreadsheet.CompositeSheet;

namespace Naninovel.Spreadsheet
{
    public class ManagedTextReader
    {
        protected virtual CompositeSheet Composite { get; }
        protected virtual LocaleTagsCache LocaleTagsCache { get; }
        protected virtual TemplateBuilder TemplateBuilder { get; }
        protected virtual string[][] LocalizationLines { get; set; }

        public ManagedTextReader (CompositeSheet composite)
        {
            Composite = composite;
            LocaleTagsCache = new LocaleTagsCache();
            TemplateBuilder = new TemplateBuilder(composite);
        }

        public virtual void Read (string managedText, IReadOnlyCollection<string> localizations)
        {
            ResetState(localizations);
            foreach (var line in managedText.SplitByNewLine())
                ReadLine(line);
            if (TemplateBuilder.Length > 0) FillSheet();
        }

        protected virtual void ResetState (IReadOnlyCollection<string> localizations)
        {
            LocalizationLines = localizations.Select(l => l.SplitByNewLine()).ToArray();
        }

        protected virtual void ReadLine (string line)
        {
            if (!line.Contains(ManagedTextUtils.RecordIdLiteral) ||
                line.StartsWithFast(ManagedTextUtils.RecordCommentLiteral)) return;

            var composite = new Composite(line);
            TemplateBuilder.Append(composite, false);

            if (composite.Arguments.Count == 0) return;
            foreach (var localization in LocalizationLines)
            {
                var locale = ExtractManagedTextLocaleTag(localization);
                var localizedValue = GetLocalizedValue(line, locale, localization);
                Composite.GetColumnValues(locale).Add(localizedValue);
            }
        }

        protected virtual void FillSheet ()
        {
            var lastTemplateValue = TemplateBuilder.Build().TrimEnd(StringUtils.NewLineChars);
            Composite.GetColumnValues(TemplateHeader).Add(lastTemplateValue);
        }

        protected virtual string ExtractManagedTextLocaleTag (string[] localization)
        {
            var firstCommentLine = localization.FirstOrDefault(l => l.StartsWithFast(ManagedTextUtils.RecordCommentLiteral));
            return LocaleTagsCache.Get(localization.GetHashCode(), firstCommentLine);
        }

        protected virtual string GetLocalizedValue (string line, string locale, string[] localization)
        {
            var id = line.GetBefore(ManagedTextUtils.RecordIdLiteral);
            var localizedLine = localization.FirstOrDefault(l => l.StartsWithFast(id));
            if (localizedLine is null)
            {
                Debug.LogWarning($"`{locale}` localization for `{id}` managed text is not found. Try re-generating localization documents.");
                return string.Empty;
            }
            return localizedLine.Substring(id.Length + ManagedTextUtils.RecordIdLiteral.Length);
        }
    }
}
