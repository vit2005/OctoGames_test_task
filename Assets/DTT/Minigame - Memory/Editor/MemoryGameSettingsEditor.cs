using UnityEditor;
using System.Collections.Generic;
using DTT.PublishingTools;

namespace DTT.MinigameMemory.Editor
{
    /// <summary>
    /// Editor class for the MemoryGameSettings inspector.
    /// </summary>
    [CustomEditor(typeof(MemoryGameSettings))]
    [DTTHeader("dtt.minigame-memory", "Memory Game Settings")]
    public class MemoryGameSettingsEditor : DTTInspector
    {
        /// <summary>
        /// List of property names to exclude.
        /// </summary>
        private List<string> properties = new List<string>();

        /// <summary>
        /// Property holding the <see cref="BoardModes"/>.
        /// </summary>
        private SerializedProperty currentBoardMode;

        /// <summary>
        /// Property holding the <see cref="CardModes"/>
        /// </summary>
        private SerializedProperty currentCardMode;

        /// <summary>
        /// Sets the property fields.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            currentBoardMode = serializedObject.FindProperty("_boardMode");
            currentCardMode = serializedObject.FindProperty("_cardMode");

            RefreshInspector();
        }

        /// <summary>
        /// Refreshes the Inspector if values have been changed.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            DrawPropertiesExcluding(serializedObject, properties.ToArray());

            if (EditorGUI.EndChangeCheck())
                RefreshInspector();
        }

        /// <summary>
        /// Adds properties to be excluded based on enum values.
        /// </summary>
        private void RefreshInspector()
        {
            properties.Clear();

            BoardModes boardModeValue = (BoardModes)currentBoardMode.intValue;
            CardModes cardModeValue = (CardModes)currentCardMode.intValue;

            if (cardModeValue == CardModes.USE_CARDS_ONCE)
                properties.Add("_amountOfCards");

            if (boardModeValue == BoardModes.ALL_CARDS_ON_BOARD)
            {
                properties.Add("_cardsOnBoardLimit");
                properties.Add("_refillAtFoundPercentage");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}