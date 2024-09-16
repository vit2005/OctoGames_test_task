using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Naninovel.Spreadsheet
{
    public class SpreadsheetWindow : EditorWindow
    {
        private string spreadsheetPath { get => PlayerPrefs.GetString(GetPrefName()); set => DoAndValidatePaths(() => PlayerPrefs.SetString(GetPrefName(), value)); }
        private bool singleSpreadsheet { get => PlayerPrefs.GetInt(GetPrefName(), 0) == 1; set => DoAndValidatePaths(() => PlayerPrefs.SetInt(GetPrefName(), value ? 1 : 0)); }
        private string scriptFolderPath { get => PlayerPrefs.GetString(GetPrefName()); set => PlayerPrefs.SetString(GetPrefName(), value); }
        private string managedTextFolderPath { get => PlayerPrefs.GetString(GetPrefName()); set => PlayerPrefs.SetString(GetPrefName(), value); }
        private string localizationFolderPath { get => PlayerPrefs.GetString(GetPrefName()); set => PlayerPrefs.SetString(GetPrefName(), value); }

        private static readonly GUIContent spreadsheetPathSingleContent = new GUIContent("Spreadsheet", "The spreadsheet file (.xlsx).");
        private static readonly GUIContent spreadsheetPathContent = new GUIContent("Spreadsheets", "Folder with the spreadsheet files (.xlsx).");
        private static readonly GUIContent singleSpreadsheetContent = new GUIContent("Single Spreadsheet", "Whether to combine all the localizable documents into single spreadsheet.");
        private static readonly GUIContent scriptFolderPathContent = new GUIContent("Scripts", "Folder containing naninovel script files (optional).");
        private static readonly GUIContent textFolderPathContent = new GUIContent("Managed Text", "Folder containing managed text files (optional).");
        private static readonly GUIContent localizationFolderPathContent = new GUIContent("Localization", "Folder containing localization resources (optional).");

        private SpreadsheetProcessor.Parameters Parameters => new SpreadsheetProcessor.Parameters {
            SpreadsheetPath = spreadsheetPath,
            SingleSpreadsheet = singleSpreadsheet,
            ScriptFolderPath = scriptFolderPath,
            ManagedTextFolderPath = managedTextFolderPath,
            LocalizationFolderPath = localizationFolderPath
        };

        private bool pathsValid;

        [MenuItem("Naninovel/Tools/Spreadsheet")]
        private static void OpenWindow ()
        {
            var position = new Rect(100, 100, 500, 210);
            GetWindowWithRect<SpreadsheetWindow>(position, true, "Spreadsheet", true);
        }

        private static string GetPrefName ([CallerMemberName] string name = "") => $"Naninovel.{nameof(SpreadsheetWindow)}.{name}";

        private void OnEnable ()
        {
            ValidatePaths();
        }

        private void DoAndValidatePaths (Action action)
        {
            action();
            ValidatePaths();
        }

        private void OnGUI ()
        {
            EditorGUILayout.LabelField("Naninovel Spreadsheet", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("The tool to export/import scenario script, managed text and localization data to/from spreadsheets.", EditorStyles.wordWrappedMiniLabel);

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                spreadsheetPath = EditorGUILayout.TextField(singleSpreadsheet ? spreadsheetPathSingleContent : spreadsheetPathContent, spreadsheetPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                    spreadsheetPath = singleSpreadsheet
                        ? EditorUtility.OpenFilePanel(spreadsheetPathSingleContent.text, "", "xlsx")
                        : EditorUtility.OpenFolderPanel(spreadsheetPathContent.text, "", "");
            }
            singleSpreadsheet = EditorGUILayout.Toggle(singleSpreadsheetContent, singleSpreadsheet);

            using (new EditorGUILayout.HorizontalScope())
            {
                scriptFolderPath = EditorGUILayout.TextField(scriptFolderPathContent, scriptFolderPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                    scriptFolderPath = EditorUtility.OpenFolderPanel(scriptFolderPathContent.text, "", "");
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                managedTextFolderPath = EditorGUILayout.TextField(textFolderPathContent, managedTextFolderPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                    managedTextFolderPath = EditorUtility.OpenFolderPanel(textFolderPathContent.text, "", "");
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                localizationFolderPath = EditorGUILayout.TextField(localizationFolderPathContent, localizationFolderPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                    localizationFolderPath = EditorUtility.OpenFolderPanel(localizationFolderPathContent.text, "", "");
            }

            GUILayout.FlexibleSpace();

            if (pathsValid)
            {
                if (GUILayout.Button("Export", GUIStyles.NavigationButton))
                    Export();

                if (GUILayout.Button("Import", GUIStyles.NavigationButton))
                    Import();
            }
            else if (singleSpreadsheet) EditorGUILayout.HelpBox("Spreadsheet path is not valid; make sure it points to an existing .xlsx file.", MessageType.Error);
            else EditorGUILayout.HelpBox("Spreadsheet path is not valid; make sure it points to an existing folder.", MessageType.Error);

            EditorGUILayout.Space();
        }

        private void ValidatePaths ()
        {
            if (singleSpreadsheet)
                pathsValid = File.Exists(spreadsheetPath) && Path.GetExtension(spreadsheetPath) == ".xlsx";
            else pathsValid = Directory.Exists(spreadsheetPath);
        }

        private void Export ()
        {
            if (!EditorUtility.DisplayDialog("Export data to spreadsheet?",
                    "Are you sure you want to export the scenario scripts, managed text and localization data to spreadsheets?\n\n" +
                    "The spreadsheets content will be overwritten, existing data could be lost. The effect of this action is permanent and can't be undone, so make sure to backup the spreadsheet file before confirming.\n\n" +
                    "In case a spreadsheet is currently open in another program, close the program before proceeding.", "Export", "Cancel")) return;
            try
            {
                var processor = CreateProcessor(Parameters,
                    p => EditorUtility.DisplayProgressBar("Exporting Naninovel Scripts", p.Info, p.Progress));
                processor.Export();
            }
            finally { EditorUtility.ClearProgressBar(); }
        }

        private void Import ()
        {
            if (!EditorUtility.DisplayDialog("Import data from spreadsheet?",
                    "Are you sure you want to import the spreadsheets data to this project?\n\n" +
                    "Affected scenario scripts, managed text and localization documents will be overwritten, existing data could be lost. The effect of this action is permanent and can't be undone, so make sure to backup the project before confirming.\n\n" +
                    "In case a spreadsheet is currently open in another program, close the program before proceeding.", "Import", "Cancel")) return;
            try
            {
                var processor = CreateProcessor(Parameters,
                    p => EditorUtility.DisplayProgressBar("Importing Naninovel Scripts", p.Info, p.Progress));
                processor.Import();
            }
            finally { EditorUtility.ClearProgressBar(); }
        }

        private SpreadsheetProcessor CreateProcessor (SpreadsheetProcessor.Parameters parameters, Action<ProgressChangedArgs> onProgress)
        {
            var customType = TypeCache.GetTypesWithAttribute<SpreadsheetProcessorAttribute>().FirstOrDefault();
            if (customType is null) return new SpreadsheetProcessor(parameters, onProgress);
            try { return (SpreadsheetProcessor)Activator.CreateInstance(customType, parameters, onProgress); }
            catch (Exception e) { throw new Error($"Custom processor `{customType.Name}` is invalid: {e.Message}"); }
        }
    }
}
