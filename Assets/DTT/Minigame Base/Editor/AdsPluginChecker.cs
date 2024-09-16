#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace DTT.MinigameBase.Editor
{
    /// <summary>
    /// Check if there is an Ad plugin.
    /// </summary>
    [InitializeOnLoad]
    public class AdsPluginChecker : UnityEditor.Editor
    {

        /// <summary>
        /// Check if advertisement plugins are present at each compilation.
        /// </summary>
        static AdsPluginChecker()
        {
            bool unityFound = ArePluginsFound("com.unity.ads");
            bool googleFound = ArePluginsFound("com.google.modAd");
            
            bool alreadyShown = EditorPrefs.GetBool(Constants.POPUP_SHOWN, false);
            if (!unityFound && !googleFound && !alreadyShown)
            {
                PluginPopup window = (PluginPopup) EditorWindow.GetWindow(typeof(PluginPopup));
                window.maxSize = new Vector2(500, 200);
                window.minSize = new Vector2(500, 200);
                window.Show();
                EditorPrefs.SetBool(Constants.POPUP_SHOWN,true);
            }

            ModifyDefineSymbols(unityFound, Constants.UNITY_ADS_SYMBOL);
            ModifyDefineSymbols(googleFound, Constants.GOOGLE_ADS_SYMBOL);
        }

        /// <summary>
        /// Check if the necessary plugin is in the Assets somewhere.
        /// <param name="plugin">Name of the plugin to check for.</param>
        /// </summary>
        /// <returns>True if the expected plugin is in the project.</returns>
        private static bool ArePluginsFound(string plugin)
        {
            // Find all package files in the Assets and package folder.
            List<string> packages = AssetDatabase.FindAssets("package", new[] { "Assets","Packages" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(x => AssetDatabase.LoadAssetAtPath<TextAsset>(x) != null)
                .ToList();
            List<string> packageNames = new List<string>();
            if (packages != null)
            {
                packageNames.AddRange(from package in packages where package.EndsWith(".json")
                    select File.ReadAllText(package)
                    into jsonString where !string.IsNullOrEmpty(jsonString)
                    select JsonUtility.FromJson<string>(jsonString));
            }
            
            // Check if the necessary plugins are present.
            return packageNames.Contains(plugin);
        }
        
        /// <summary>
        /// Modify the define symbols depending on whether plugins were found or not.
        /// </summary>
        /// <param name="pluginsFound">Whether the plugins are present in the project.</param>
        /// <param name="symbol">Define symbol of that plugin.</param>
        internal static void ModifyDefineSymbols(bool pluginsFound,string symbol)
        {
            // Get the current define symbols.
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();

            bool pluginInDefines = allDefines.Contains(symbol);
            switch (pluginsFound)
            {
                // If the plugins are present, add symbol to the EditorPrefs symbols.
                case true when !pluginInDefines:
                    allDefines.Add(symbol);
                    break;
                // If not, remove it.
                case false when pluginInDefines:
                    allDefines.Remove(symbol);
                    break;
                default:
                    return;
            }

            // Update the define symbols.
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));

#if UNITY_2019_3_OR_NEWER
            CompilationPipeline.RequestScriptCompilation();
#elif UNITY_2017_1_OR_NEWER
            Assembly editorAssembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            Type editorCompilationInterfaceType = editorAssembly.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface");
            MethodInfo dirtyAllScriptsMethod = editorCompilationInterfaceType.GetMethod("DirtyAllScripts", BindingFlags.Static | BindingFlags.Public);
            dirtyAllScriptsMethod.Invoke(editorCompilationInterfaceType, null);
#endif
        }
    }
}
#endif
