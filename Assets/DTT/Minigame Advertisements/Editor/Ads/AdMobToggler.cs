using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#if UNITY_2019_3_OR_NEWER
using UnityEditor.Compilation;
#elif UNITY_2017_1_OR_NEWER
using System.Reflection;
using Assembly = System.Reflection.Assembly;
#endif

namespace DTT.MinigameBase.Editor.Advertisements.AdMob
{
    /// <summary>
    /// Handles toggling the usage of AdMob on and off.
    /// </summary>
    internal static class AdMobToggler
    {
        /// <summary>
        /// Name of the define.
        /// </summary>
        public const string GOOGLE_MOBILE_ADS_SYMBOL = "GOOGLE_MOBILE_ADS";

        /// <summary>
        /// Enables AdMob.
        /// </summary>
        [MenuItem("Tools/DTT/Minigame Ads/Enable AdMob")]
        public static void EnableAdMob() => ModifyDefineSymbols(true);

        /// <summary>
        /// Disables AdMob.
        /// </summary>
        [MenuItem("Tools/DTT/Minigame Ads/Disable AdMob")]
        public static void DisableAdMob() => ModifyDefineSymbols(false);

        /// <summary>
        /// Modify the define symbols depending on whether plugins were found or not.
        /// </summary>
        /// <param name="pluginsFound">Whether the plugins are present in the project</param>
        internal static void ModifyDefineSymbols(bool pluginsFound)
        {
            // Get the current define symbols.
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();

            bool pluginInDefines = allDefines.Contains(GOOGLE_MOBILE_ADS_SYMBOL);
            if (pluginsFound && !pluginInDefines)
            {
                // If the plugins are present, add the Google Plugin symbol to the symbols.
                allDefines.Add(GOOGLE_MOBILE_ADS_SYMBOL);
            }
            else if (!pluginsFound && pluginInDefines)
            {
                // If not, remove it.
                allDefines.Remove(GOOGLE_MOBILE_ADS_SYMBOL);
            }
            else
            {
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
