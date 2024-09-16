using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Naninovel.Metadata;
using Naninovel.Parsing;

namespace Naninovel.Spreadsheet
{
    /// <summary>
    /// Represent a string template associated with arguments.
    /// </summary>
    public class Composite
    {
        public readonly string Value;
        public readonly string Template;
        public readonly IReadOnlyList<string> Arguments;

        private static readonly string[] emptyArgs = Array.Empty<string>();
        private static readonly Localizable[] emptyLocalizables = Array.Empty<Localizable>();
        private static readonly Regex argRegex = new Regex(@"(?<!\\)\{(\d+?)(?<!\\)\}", RegexOptions.Compiled);
        private static readonly MetadataProvider meta = CreateMetaProvider();

        private readonly ErrorCollector errors = new ErrorCollector();
        private readonly RangeMapper ranges = new RangeMapper();
        private readonly ScriptSerializer serializer = new ScriptSerializer();
        private readonly Parsing.CommandLineParser commandLineParser;
        private readonly GenericLineParser genericLineParser;

        public Composite ()
        {
            commandLineParser = new Parsing.CommandLineParser(errors, ranges);
            genericLineParser = new GenericLineParser(errors, ranges);
        }

        public Composite (string template, IEnumerable<string> args) : this()
        {
            Template = template;
            Arguments = args?.ToArray() ?? emptyArgs;
            Value = BuildTemplate(Template, Arguments);
        }

        public Composite (string lineText, LineType lineType, IReadOnlyList<Token> tokens) : this()
        {
            (Template, Arguments) = ParseScriptLine(lineText, lineType, tokens);
            Value = BuildTemplate(Template, Arguments);
        }

        public Composite (string managedTextLine) : this()
        {
            Value = managedTextLine;
            (Template, Arguments) = ParseManagedText(managedTextLine);
        }

        private static MetadataProvider CreateMetaProvider ()
        {
            var commands = MetadataGenerator.GenerateCommandsMetadata();
            var project = new Project { Commands = commands };
            return new MetadataProvider(project);
        }

        private static string BuildPlaceholder (int index) => $"{{{index}}}";

        private static string BuildTemplate (string template, IReadOnlyList<string> args)
        {
            if (args.Count == 0) return template;

            foreach (var match in argRegex.Matches(template).Cast<Match>())
            {
                var index = int.Parse(match.Value.GetBetween("{", "}"));
                var arg = args[index];
                template = template.Replace(match.Value, arg);
            }

            return template;
        }

        private (string template, IReadOnlyList<string> args) ParseScriptLine (string lineText, LineType lineType, IReadOnlyList<Token> tokens)
        {
            errors.Clear();
            ranges.Clear();
            switch (lineType)
            {
                case LineType.Label:
                case LineType.Comment:
                    return (lineText, emptyArgs);
                case LineType.Command:
                    var commandLine = commandLineParser.Parse(lineText, tokens);
                    if (errors.Count > 0) throw new Error($"Line `{lineText}` failed to parse: {errors[0]}");
                    return ParseCommandLine(commandLine, lineText);
                case LineType.Generic:
                    var genericLine = genericLineParser.Parse(lineText, tokens);
                    if (errors.Count > 0) throw new Error($"Line `{lineText}` failed to parse: {errors[0]}");
                    return ParseGenericLine(genericLine, lineText);
                default: throw new Error($"Unknown line type: {lineType}");
            }
        }

        private (string template, IReadOnlyList<string> args) ParseCommandLine (CommandLine model, string lineText)
        {
            var localizables = GetLocalizableParameters(model.Command);
            return ParseLocalizables(localizables, lineText);
        }

        private (string template, IReadOnlyList<string> args) ParseGenericLine (GenericLine line, string lineText)
        {
            var localizables = new List<Localizable>();
            foreach (var content in line.Content)
                if (content is MixedValue mixed) localizables.Add(CreateLocalizable(mixed, mixed, false));
                else localizables.AddRange(GetLocalizableParameters(((InlinedCommand)content).Command));
            return ParseLocalizables(localizables, lineText);
        }

        private (string template, IReadOnlyList<string> args) ParseLocalizables (IReadOnlyList<Localizable> localizables, string lineText)
        {
            var args = new string[localizables.Count];
            for (int i = localizables.Count - 1; i >= 0; --i)
            {
                var localizable = localizables[i];
                var placeholder = BuildPlaceholder(i);
                args[i] = localizable.Text;
                lineText = lineText
                    .Remove(localizable.Range.StartIndex, localizable.Range.Length)
                    .Insert(localizable.Range.StartIndex, placeholder);
            }
            return (lineText, args);
        }

        private IReadOnlyList<Localizable> GetLocalizableParameters (Parsing.Command command)
        {
            var commandMeta = meta.FindCommand(command.Identifier);
            if (commandMeta is null) throw new Error($"Unknown command: `{command.Identifier}`");
            if (!commandMeta.Localizable) return emptyLocalizables;

            var localizables = new List<Localizable>();
            foreach (var parameter in command.Parameters)
            {
                var paramMeta = meta.FindParameter(commandMeta.Id, parameter.Identifier);
                if (paramMeta is null) throw new Error($"Unknown parameter in `{command.Identifier}` command: `{parameter.Identifier}`");
                if (paramMeta.Localizable) localizables.Add(CreateLocalizable(parameter));
            }
            return localizables;
        }

        private Localizable CreateLocalizable (Parsing.Parameter parameter)
        {
            return CreateLocalizable(parameter.Value, parameter.Value, true);
        }

        private Localizable CreateLocalizable (ILineComponent component, MixedValue mixed, bool wrap)
        {
            var text = serializer.Serialize(mixed, wrap);
            if (!ranges.TryResolve(component, out var range))
                throw new Error($"Failed to resolve range for `{component}`.");
            return new Localizable(text, range);
        }

        private static (string template, IReadOnlyList<string> args) ParseManagedText (string line)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains(ManagedTextUtils.RecordIdLiteral))
                return (line, emptyArgs);

            var id = line.GetBefore(ManagedTextUtils.RecordIdLiteral);
            var lhsLength = id.Length + ManagedTextUtils.RecordIdLiteral.Length;
            var value = line.Substring(lhsLength);
            var template = line.Substring(0, lhsLength) + BuildPlaceholder(0);
            var args = new[] { value };
            return (template, args);
        }
    }
}
