using UnityEngine;
using UnityEditor;
using DTT.MinigameBase.LevelSelect;

namespace DTT.MinigameBase.Editor
{
    /// <summary>
    /// Makes the <see cref="LevelDatabase.Data"/> read-only in the inspector.
    /// </summary>
    [CustomEditor(typeof(LevelDatabase))]
    public class DatabaseEditor : UnityEditor.Editor
    {
        /// <summary>
        /// The <see cref="LevelData"/> array property.
        /// </summary>
        private SerializedProperty _readonlyData;

        /// <summary>
        /// The <see cref="LevelData"/> level count property.
        /// </summary>
        private SerializedProperty _levelCount;

        /// <summary>
        /// The <see cref="LevelData"/> initial unlocks property.
        /// </summary>
        private SerializedProperty _intialUnlocks;

        /// <summary>
        /// The max value the data list can be.
        /// </summary>
        private const int _MAX_LEVEL_COUNT = 1000;

        /// <summary>
        /// Gets the <see cref="LevelData"/> array property.
        /// </summary>
        private void OnEnable() => GetData();

        /// <summary>
        /// Gets the properties of the selected <see cref="LevelDatabase"/>.
        /// </summary>
        private void GetData()
        {
            _readonlyData = serializedObject.FindProperty("_data");
            _levelCount = serializedObject.FindProperty("_levelCount");
            _intialUnlocks = serializedObject.FindProperty("_initialUnlocks");
        }

        /// <summary>
        /// Draws in the default inspector. The data array gets drawed in as readonly.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_levelCount);
            _levelCount.intValue = Mathf.Clamp(_levelCount.intValue, 0, _MAX_LEVEL_COUNT);
            EditorGUILayout.PropertyField(_intialUnlocks);

            _readonlyData.arraySize = _levelCount.intValue;
            LevelDatabase script = (LevelDatabase)target;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_readonlyData);
            EditorGUI.EndDisabledGroup();
         
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                script.UpdateList();
            }
        }
    }
}
