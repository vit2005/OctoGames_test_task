using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
namespace DTT.MinigameBase.Editor
{
    /// <summary>
    /// Information popup for the add plugin dependencies.
    /// </summary>
    public class PluginPopup : EditorWindow
    {
        /// <summary>
        /// Request the unity Ads package.
        /// </summary>
        private static AddRequest _request;
        
        /// <summary>
        /// Display the information Popup with button to install UnityAds and google mod Ad.
        /// </summary>
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Unity Ads", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("It appears that you don't have an advertisement plugin installed. The advertisement implementation require a plugin to work. Click the button below to install Unity ads plugin.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Unity Ads"))
                OpenUnityAds();
            
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Unity Ads", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("It appears that you don't have an advertisement plugin installed. The advertisement implementation require a plugin to work. Click the button below to install AdMob plugin.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Google AdMob"))
                OpenAdModSetup();
            
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Download Unity Ads package.
        /// </summary>
        private void OpenUnityAds()
        {
            _request = Client.Add("com.unity.ads");
            EditorApplication.update += Progress;
            Close();
        }

        /// <summary>
        /// Open link to AdMod quick set up guide.
        /// </summary>
        private void OpenAdModSetup() => Application.OpenURL(Constants.GOOGLE_AD_MOD_LINK);
        
        /// <summary>
        /// Install package progress.
        /// </summary>
        private static void Progress()
        {
            if (_request.IsCompleted)
            {
                if (_request.Status == StatusCode.Success)
                    Debug.Log("Installed: " + _request.Result.packageId);
                else if (_request.Status >= StatusCode.Failure)
                    Debug.Log(_request.Error.message);

                EditorApplication.update -= Progress;
            }
        }
    }
}