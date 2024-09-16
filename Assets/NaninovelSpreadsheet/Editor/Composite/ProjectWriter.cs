using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Naninovel.Parsing;
using UnityEngine;
using static Naninovel.Spreadsheet.CompositeSheet;

namespace Naninovel.Spreadsheet
{
    public class ProjectWriter
    {
        protected virtual CompositeSheet Composite { get; }
        protected virtual Dictionary<string, StringBuilder> Builders { get; }
        protected virtual int LastTemplateIndex { get; set; }
        protected virtual bool ManagedText { get; set; }

        public ProjectWriter (CompositeSheet composite)
        {
            Composite = composite;
            Builders = new Dictionary<string, StringBuilder>();
        }

        public virtual void Write (string path, IReadOnlyCollection<string> localizations, bool managedText)
        {
            ResetState(managedText);

            var maxLength = Composite.Columns.Values.Max(v => v.Count);
            for (int i = 0; i < maxLength; i++)
            {
                var template = Composite.GetColumnValues(TemplateHeader).ElementAtOrDefault(i);
                if (string.IsNullOrWhiteSpace(template)) continue;
                if (LastTemplateIndex > -1)
                    WriteLine(i - 1);
                LastTemplateIndex = i;
            }
            WriteLine(maxLength - 1, true);

            foreach (var kv in Builders)
            {
                var header = kv.Key;
                var builder = kv.Value;
                if (header == ArgumentHeader)
                {
                    File.WriteAllText(path, GetBuilder(ArgumentHeader).ToString());
                    continue;
                }

                var localizationPath = localizations.FirstOrDefault(p => p.Contains($"/{header}/"));
                if (localizationPath is null || !File.Exists(localizationPath))
                    throw new Error($"Localization document `{localizationPath?.GetAfter(Application.dataPath) ?? header}` not found. " +
                                    $"Try re-generating the localization documents.");
                var localeHeader = File.ReadAllText(localizationPath).SplitByNewLine()[0];
                if (managedText) localeHeader += Environment.NewLine;
                File.WriteAllText(localizationPath, localeHeader + Environment.NewLine + builder);
            }
        }

        protected virtual void ResetState (bool managedText)
        {
            Builders.Clear();
            LastTemplateIndex = -1;
            ManagedText = managedText;
        }

        protected virtual void WriteLine (int lastArgIndex, bool lastLine = false)
        {
            var template = Composite.GetColumnValues(TemplateHeader)[LastTemplateIndex];
            var sourceArgs = Composite.GetColumnValuesAt(ArgumentHeader, LastTemplateIndex, lastArgIndex);
            foreach (var header in Composite.Columns.Keys)
            {
                if (header == TemplateHeader) continue;
                var builder = GetBuilder(header);
                if (header == ArgumentHeader)
                {
                    builder.Append(new Composite(template, sourceArgs).Value);
                    if (lastLine || ManagedText) builder.AppendLine();
                    continue;
                }

                var localizedArgs = Composite.GetColumnValuesAt(header, LastTemplateIndex, lastArgIndex);
                if (localizedArgs.Count > 0)
                    AppendLocalizationLine(builder, template, localizedArgs, sourceArgs);
            }
        }

        protected virtual StringBuilder GetBuilder (string header)
        {
            if (!Builders.TryGetValue(header, out var builder))
            {
                builder = new StringBuilder();
                Builders[header] = builder;
            }
            return builder;
        }

        protected virtual void AppendLocalizationLine (StringBuilder builder, string template,
            IReadOnlyList<string> localizedArgs, IReadOnlyList<string> sourceArgs)
        {
            var localizableTemplate = template.TrimEnd(StringUtils.NewLineChars).SplitByNewLine().Last();
            var localizedLine = new Composite(localizableTemplate, localizedArgs).Value;
            if (ManagedText)
            {
                builder.AppendLine(localizedLine);
                return;
            }
            var sourceLine = new Composite(localizableTemplate, sourceArgs).Value;
            var lineHash = CryptoUtils.PersistentHexCode(sourceLine.TrimFull());
            builder.AppendLine()
                .AppendLine($"{Identifiers.LabelLine} {lineHash}")
                .AppendLine($"{Identifiers.CommentLine} {sourceLine}");
            if (!localizedArgs.All(string.IsNullOrWhiteSpace))
                builder.AppendLine(localizedLine);
        }
    }
}
