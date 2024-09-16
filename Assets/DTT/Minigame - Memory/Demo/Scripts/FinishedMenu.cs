using System;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using DTT.MinigameMemory;

namespace DTT.MinigameMemory.Demo
{
    /// <summary>
    /// Manages the UI for when the game is finished.
    /// </summary>
    public class FinishedMenu : MonoBehaviour
    {
        /// <summary>
        /// Used to display the results of the game on the UI.
        /// </summary>
        [SerializeField]
        private Text _results;

        /// <summary>
        /// Restart button of the menu.
        /// </summary>
        public Button _restartButton;

        /// <summary>
        /// Home button of the menu.
        /// </summary>
        public Button _homeButton;

        /// <summary>
        /// Sets the result text of the finished menu to the given <see cref="MemoryGameResults"/>.
        /// </summary>
        /// <param name="results">Results to show.</param>
        public void SetResultText(MemoryGameResults results)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Results");
            sb.Append('\n');
            sb.Append("Time: ");

            string format = results.timeTaken.Hours > 0 ? @"hh\:mm\:ss" : @"mm\:ss";
            sb.Append(results.timeTaken.ToString(format));

            sb.Append('\n');
            sb.Append("Turns: ");
            sb.Append(results.amountOfTurns);

            _results.text = sb.ToString();
        }
    }
}