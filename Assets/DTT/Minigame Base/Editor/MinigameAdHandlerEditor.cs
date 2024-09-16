using System;
using System.Reflection;
using DTT.MinigameBase.Advertisements;
using DTT.PublishingTools;
using DTT.Utils.EditorUtilities;
using DTT.Utils.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DTT.MinigameBase.Editor.Advertisements.AdMob
{
    /// <summary>
    /// The base editor class for AdHandler.
    /// </summary>
    /// <typeparam name="TConfig">The type of configuration used in the minigame.</typeparam>
    /// <typeparam name="TResult">The type of result used in the minigame.</typeparam>
    /// <typeparam name="TMinigame">The type of minigame manager.</typeparam>
    [DTTHeader("dtt.minigame-base")]
    public abstract class MinigameAdHandlerEditor<TConfig, TResult, TMinigame> : DTTInspector where TMinigame : Object, IMinigame<TConfig, TResult>
    {
        /// <summary>
        /// The target of the editor.
        /// </summary>
        private MinigameAdHandler<TConfig, TResult, TMinigame> _adHandler;
        
        /// <summary>
        /// The property cache of the ad handler for displaying values.
        /// </summary>
        private MinigameAdHandlerSerializedPropertyCache _propertyCache;
        
        /// <summary>
        /// Initializes data members.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _adHandler = (MinigameAdHandler<TConfig, TResult, TMinigame>)target;
            _propertyCache = new MinigameAdHandlerSerializedPropertyCache(serializedObject);

            if (_propertyCache.UniqueKey.stringValue.IsNullOrEmpty())
                _propertyCache.UniqueKey.stringValue = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Draws the editor.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
#if UNITY_ADS && UNITY_UNITYADS_API
            EditorGUI.BeginChangeCheck();
            
            // Manually draw script field.
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

            // Draw the Minigame Gameobject field manually, so we can add editor-time checking of the reference entered.
            GameObject gameObject = (GameObject)EditorGUILayout.ObjectField(
                new GUIContent(_propertyCache.MinigameGameObject.displayName, _propertyCache.MinigameGameObject.tooltip),
                _propertyCache.MinigameGameObject.objectReferenceValue, typeof(GameObject), true);

            // Tests whether the gameobject contains a minigame object.
            TestGameObjectReference(gameObject);

            // Display a message if no ad is found on the Gameobject.
            if (!_adHandler.TryGetComponent(out BaseAd _))
            {
                EditorGUILayout.HelpBox("Press the button to add the missing advertisement scripts!", MessageType.Error);
                if (GUILayout.Button("Add missing scripts"))
                {
                    _adHandler.AddScript();
                }
            }
            else
            {
                // Draw remaining properties in the editor.
                EditorGUILayout.PropertyField(_propertyCache.AbleToShowAdOnStart);
                EditorGUILayout.PropertyField(_propertyCache.PauseGameWithAdAtStart);
                EditorGUILayout.PropertyField(_propertyCache.AbleToShowAdOnFinish);
                EditorGUILayout.PropertyField(_propertyCache.AdInterval);


                // Shows a helpbox displaying when an ad will popup.
                DrawAdIntervalHelpBox();
            }
            
            if (EditorGUI.EndChangeCheck()) 
                serializedObject.ApplyModifiedProperties();
#else
            EditorGUILayout.HelpBox("For advertisement you need to import UnityAds or AdMob. \n Afterwards required scripts can be added by using the button that will appear in this inspector.",MessageType.Info);
            EditorGUILayout.HelpBox("For advertisement you need to change BuildSettings to Android or IOS.",MessageType.Info);
            
#endif
        }

        /// <summary>
        /// Shows a helpbox displaying when an ad will popup.
        /// </summary>
        private void DrawAdIntervalHelpBox()
        {
            // If interval is 0, no need to show extra information.
            if (_propertyCache.AdInterval.intValue == 0)
                return;

            // Uses reflection to gain access to methods to get information about the interval.
            int count = 0;
            try
            {
                MethodInfo infoGetCount = typeof(MinigameAdHandler<TConfig, TResult, TMinigame>)
                    .GetMethod("GetInterval", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                
                count = (int)infoGetCount.Invoke(_adHandler, new object[] { });
            }
            catch (InvalidCastException e)
            {
                Debug.LogError($"Casting result of GetInterval method failed, check API to make sure Reflection call still matches implementation.\n{e}");
                return;
            }
            catch (Exception e)
            {
                Debug.LogError($"Something failed during Reflection of GetInterval.\n{e}");
                return;
            }

            int wrappedCount = count % _propertyCache.AdInterval.intValue;
            EditorGUILayout.HelpBox($"Interval at count: {count}\n{(wrappedCount == 0 ? 0 : _propertyCache.AdInterval.intValue - wrappedCount)} more until new ad displays.", MessageType.Info);
            
            if (GUILayout.Button("Reset interval"))
            {
                try
                {
                    MethodInfo infoResetInterval = typeof(MinigameAdHandler<TConfig, TResult, TMinigame>)
                        .GetMethod("ResetInterval", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                
                    infoResetInterval.Invoke(_adHandler, new object[] { });
                }
                catch (Exception e)
                {
                    Debug.LogError($"Something failed during Reflection of ResetInterval.\n{e}");
                    return;
                }
            }
        }

        /// <summary>
        /// Tests whether the gameobject contains a minigame object.
        /// </summary>
        /// <param name="gameObject">The new gameobject reference assigned.</param>
        private void TestGameObjectReference(GameObject gameObject)
        {
            if (gameObject == null)
                _propertyCache.MinigameGameObject.objectReferenceValue = null;
            else if (gameObject != _propertyCache.MinigameGameObject.objectReferenceValue)
                if (gameObject.TryGetComponent(out TMinigame _))
                    _propertyCache.MinigameGameObject.objectReferenceValue = gameObject;
        }
    }

    /// <summary>
    /// Contains all the serialized properties on <see cref="MinigameAdHandler{TConfig,TResult,TMinigame}"/>.
    /// </summary>
    internal class MinigameAdHandlerSerializedPropertyCache : SerializedPropertyCache
    {
        /// <summary>
        /// Serialized property for MinigameGameObject.
        /// </summary>
        public SerializedProperty MinigameGameObject => base["_minigameGameObject"];
        
        /// <summary>
        /// Serialized property for AbleToShowAdOnStart.
        /// </summary>
        public SerializedProperty AbleToShowAdOnStart => base["_ableToShowAdOnStart"];
        
        /// <summary>
        /// Serialized property for PauseGameWithAdAtStart.
        /// </summary>
        public SerializedProperty PauseGameWithAdAtStart => base["_pauseGameWithAdAtStart"];
        
        /// <summary>
        /// Serialized property for AbleToShowAdOnFinish.
        /// </summary>
        public SerializedProperty AbleToShowAdOnFinish => base["_ableToShowAdOnFinish"];
        
        /// <summary>
        /// Serialized property for AdInterval.
        /// </summary>
        public SerializedProperty AdInterval => base["_adInterval"];
        
        /// <summary>
        /// Serialized property for UniqueKey.
        /// </summary>
        public SerializedProperty UniqueKey => base["_uniqueKey"];
        
        /// <summary>
        /// Creates a new MinigameAdHandlerSerializedPropertyCache.
        /// </summary>
        /// <param name="serializedObject">The object to base it off.</param>
        public MinigameAdHandlerSerializedPropertyCache(SerializedObject serializedObject) : base(serializedObject)
        {
        }
    }
}