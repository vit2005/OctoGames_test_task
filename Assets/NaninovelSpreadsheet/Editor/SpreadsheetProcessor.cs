using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using UnityEngine;

namespace Naninovel.Spreadsheet
{
    public class SpreadsheetProcessor
    {
        public class Parameters
        {
            public string SpreadsheetPath { get; set; }
            public bool SingleSpreadsheet { get; set; }
            public string ScriptFolderPath { get; set; }
            public string ManagedTextFolderPath { get; set; }
            public string LocalizationFolderPath { get; set; }
        }

        private const string scriptsCategory = "Scripts";
        private const string textCategory = "Text";
        private const string sheetPathSeparator = ">";
        private const string scriptSheetNamePrefix = scriptsCategory + sheetPathSeparator;
        private const string textSheetNamePrefix = textCategory + sheetPathSeparator;
        private const string scriptFileExtension = ".nani";
        private const string textFileExtension = ".txt";
        private const string scriptFilePattern = "*" + scriptFileExtension;
        private const string textFilePattern = "*" + textFileExtension;

        private readonly string spreadsheetPath;
        private readonly bool singleSpreadsheet;
        private readonly string scriptFolderPath;
        private readonly string textFolderPath;
        private readonly string localeFolderPath;
        private readonly Action<ProgressChangedArgs> onProgress;

        public SpreadsheetProcessor (Parameters parameters, Action<ProgressChangedArgs> onProgress)
        {
            spreadsheetPath = parameters.SpreadsheetPath;
            singleSpreadsheet = parameters.SingleSpreadsheet;
            scriptFolderPath = parameters.ScriptFolderPath;
            textFolderPath = parameters.ManagedTextFolderPath;
            localeFolderPath = parameters.LocalizationFolderPath;
            this.onProgress = onProgress;
        }

        public virtual void Export ()
        {
            if (Directory.Exists(scriptFolderPath))
                ExportScripts();
            if (Directory.Exists(textFolderPath))
                ExportManagedText();
        }

        public virtual void Import ()
        {
            var directory = singleSpreadsheet ? Path.GetDirectoryName(spreadsheetPath) : spreadsheetPath;
            var documentPaths = Directory.GetFiles(directory, "*.xlsx", SearchOption.AllDirectories);
            for (int i = 0; i < documentPaths.Length; i++)
            {
                var document = OpenXML.OpenDocument(documentPaths[i], false);
                var sheetsNames = document.GetSheetNames().ToArray();
                for (int j = 0; j < sheetsNames.Length; j++)
                {
                    if (singleSpreadsheet) NotifyProgressChanged(sheetsNames, j);
                    else NotifyProgressChanged(documentPaths, i);
                    try { ImportSheet(document, sheetsNames[j], documentPaths[i]); }
                    catch (Exception e) { throw new Error($"Failed to import tab `{sheetsNames[j]}` of sheet `{documentPaths[i]}`.", e); }
                }
                document.Dispose();
            }
        }

        protected virtual ProjectWriter CreateProjectWriter (CompositeSheet composite)
        {
            return new ProjectWriter(composite);
        }

        protected virtual SpreadsheetWriter CreateSpreadsheetWriter (CompositeSheet composite)
        {
            return new SpreadsheetWriter(composite);
        }

        protected virtual SpreadsheetReader CreateSpreadsheetReader (CompositeSheet composite)
        {
            return new SpreadsheetReader(composite);
        }

        protected virtual ScriptReader CreateScriptReader (CompositeSheet composite)
        {
            return new ScriptReader(composite);
        }

        protected virtual ManagedTextReader CreateManagedTextReader (CompositeSheet composite)
        {
            return new ManagedTextReader(composite);
        }

        protected virtual void ImportSheet (SpreadsheetDocument document, string sheetName, string docPath)
        {
            if (!TryGetCategoryFromSheetName(sheetName, out var category))
            {
                Debug.LogWarning($"Sheet `{sheetName}` in `{docPath}` is not recognized and will be ignored.");
                return;
            }
            var localPath = singleSpreadsheet
                ? SheetNameToLocalPath(sheetName, category)
                : DocumentPathToLocalPath(docPath, category);
            var sheet = document.GetSheet(sheetName);
            var fullPath = LocalToFullPath(localPath);
            var localizations = LocateLocalizationsFor(localPath, false);
            var composite = new CompositeSheet();
            CreateSpreadsheetReader(composite).Read(document, sheet);
            CreateProjectWriter(composite).Write(fullPath, localizations, IsManagedText(fullPath));
        }

        protected virtual SpreadsheetDocument OpenOrCreateDocument (string category, string localPath)
        {
            if (singleSpreadsheet) return OpenXML.OpenDocument(spreadsheetPath, true);
            var directory = Path.Combine(spreadsheetPath, category, Path.GetDirectoryName(localPath));
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, Path.GetFileNameWithoutExtension(localPath) + ".xlsx");
            return OpenXML.CreateDocument(path);
        }

        protected virtual void ExportScripts ()
        {
            var scriptPaths = Directory.GetFiles(scriptFolderPath, scriptFilePattern, SearchOption.AllDirectories);
            for (int pathIndex = 0; pathIndex < scriptPaths.Length; pathIndex++)
            {
                NotifyProgressChanged(scriptPaths, pathIndex);
                try { ExportScript(scriptPaths[pathIndex]); }
                catch (Exception e) { throw new Error($"Failed to export script `{scriptPaths[pathIndex]}`.", e); }
            }
        }

        protected virtual void ExportScript (string scriptPath)
        {
            var script = LoadScriptAtPath(scriptPath);
            var localPath = FullToLocalPath(scriptPath);
            var document = OpenOrCreateDocument(scriptsCategory, localPath);
            var sheetName = LocalPathToSheetName(localPath);
            var sheet = document.GetSheet(sheetName) ?? document.AddSheet(sheetName);
            var localizations = LocateLocalizationsFor(localPath).Select(LoadScriptAtPath).ToArray();
            var composite = new CompositeSheet();
            CreateScriptReader(composite).Read(script, localizations);
            CreateSpreadsheetWriter(composite).Write(document, sheet);
            document.Dispose();
        }

        protected virtual void ExportManagedText ()
        {
            var managedTextPaths = Directory.GetFiles(textFolderPath, textFilePattern, SearchOption.AllDirectories);
            for (int pathIndex = 0; pathIndex < managedTextPaths.Length; pathIndex++)
            {
                NotifyProgressChanged(managedTextPaths, pathIndex);
                try { ExportManagedText(managedTextPaths[pathIndex]); }
                catch (Exception e) { throw new Error($"Failed to export managed text `{managedTextPaths[pathIndex]}`.", e); }
            }
        }

        protected virtual void ExportManagedText (string docPath)
        {
            var docText = File.ReadAllText(docPath);
            var localPath = FullToLocalPath(docPath);
            var document = OpenOrCreateDocument(textCategory, localPath);
            var sheetName = LocalPathToSheetName(localPath);
            var sheet = document.GetSheet(sheetName) ?? document.AddSheet(sheetName);
            var localizations = LocateLocalizationsFor(localPath).Select(File.ReadAllText).ToArray();
            var composite = new CompositeSheet();
            CreateManagedTextReader(composite).Read(docText, localizations);
            CreateSpreadsheetWriter(composite).Write(document, sheet);
            document.Dispose();
        }

        protected virtual string FullToLocalPath (string fullPath)
        {
            var prefix = fullPath.EndsWithFast(scriptFileExtension) ? scriptFolderPath : textFolderPath;
            return fullPath.Remove(prefix).TrimStart('\\').TrimStart('/');
        }

        protected virtual string LocalToFullPath (string localPath)
        {
            var prefix = localPath.EndsWithFast(scriptFileExtension) ? scriptFolderPath : textFolderPath;
            return $"{prefix}/{localPath}";
        }

        protected virtual string LocalPathToSheetName (string localPath)
        {
            var namePrefix = localPath.EndsWithFast(scriptFileExtension) ? scriptSheetNamePrefix : textSheetNamePrefix;
            var name = namePrefix + localPath.Replace("\\", sheetPathSeparator).Replace("/", sheetPathSeparator).GetBeforeLast(".");
            if (name.Length > 31)
            {
                name = name.Substring(0, 31);
                if (singleSpreadsheet)
                    throw new FormatException($"Resource `{name}` name is too long, " +
                                              "Excel doesn't support sheet names longer than 31 characters. " +
                                              "Either shorten the name or disable `Single Spreadsheet`.");
            }
            return name;
        }

        protected virtual bool TryGetCategoryFromSheetName (string sheetName, out string category)
        {
            if (sheetName.StartsWithFast(scriptSheetNamePrefix)) category = scriptsCategory;
            else if (sheetName.StartsWithFast(textSheetNamePrefix)) category = textCategory;
            else category = null;
            return category != null;
        }

        protected virtual string SheetNameToLocalPath (string sheetName, string category)
        {
            var fileExtension = category == scriptsCategory ? scriptFileExtension : textFileExtension;
            return sheetName.GetAfterFirst(sheetPathSeparator).Replace(sheetPathSeparator, "/") + fileExtension;
        }

        protected virtual string DocumentPathToLocalPath (string docPath, string category)
        {
            var basePath = Path.Combine(spreadsheetPath, category + "/");
            var localPath = new Uri(basePath).MakeRelativeUri(new Uri(docPath)).OriginalString;
            var fileExtension = category == scriptsCategory ? scriptFileExtension : textFileExtension;
            return localPath.GetBeforeLast(".") + fileExtension;
        }

        protected virtual ScriptText LoadScriptAtPath (string scriptPath)
        {
            var scriptText = File.ReadAllText(scriptPath);
            var script = Script.FromScriptText(scriptPath, scriptText);
            if (script == null) throw new Error($"Failed to load `{scriptPath}` script.");
            return new ScriptText(script, scriptText);
        }

        protected virtual IReadOnlyCollection<string> LocateLocalizationsFor (string localPath, bool skipMissing = true)
        {
            if (!Directory.Exists(localeFolderPath)) return Array.Empty<string>();

            var prefix = localPath.EndsWithFast(scriptFileExtension)
                ? ScriptsConfiguration.DefaultPathPrefix
                : ManagedTextConfiguration.DefaultPathPrefix;

            // Localized script resources are flatted in a single folder.
            if (localPath.EndsWithFast(scriptFileExtension))
                localPath = Path.GetFileName(localPath);

            var paths = new List<string>();
            foreach (var localeDir in Directory.EnumerateDirectories(localeFolderPath))
            {
                var localizationPath = Path.Combine(localeDir, prefix, localPath).Replace('\\', '/');
                if (skipMissing && !File.Exists(localizationPath))
                {
                    Debug.LogWarning($"Missing localization resource for `{localPath}` (expected in `{localizationPath}`).");
                    continue;
                }
                paths.Add(localizationPath);
            }
            return paths;
        }

        protected virtual void NotifyProgressChanged (IReadOnlyList<string> paths, int index)
        {
            if (onProgress is null) return;

            var path = paths[index];
            var name = path.Contains(sheetPathSeparator) ? path : Path.GetFileNameWithoutExtension(path);
            var progress = index / (float)paths.Count;
            var info = $"Processing `{name}`...";
            var args = new ProgressChangedArgs(info, progress);
            onProgress.Invoke(args);
        }

        protected virtual bool IsManagedText (string fullPath)
        {
            return fullPath.EndsWithFast(textFileExtension);
        }
    }
}
