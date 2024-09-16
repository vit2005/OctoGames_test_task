using System.Collections.Generic;
using DTT.MinigameBase.LevelSelect;
using UnityEngine;

namespace DTT.MinigameMemory.Demo
{
    /// <summary>
    /// Handle the level selection.
    /// </summary>
    public class MemoryGameLevelSelectHandler : LevelSelectHandler<MemoryGameSettings,MemoryGameResults,MemoryGameManager>
    {
        /// <summary>
        /// List of configuration for the game.
        /// </summary>
        [SerializeField] 
        [Tooltip("List of level in your game.")]
        private List<MemoryGameSettings> _settings;
        
        /// <summary>
        /// Get the configuration for a level number.
        /// </summary>
        /// <param name="levelNumber">Level number to load.</param>
        /// <returns>Return the configuration of the level.</returns>
        protected override MemoryGameSettings GetConfig(int levelNumber)
        {
            return _settings[(levelNumber-1) % _settings.Count];
        }

        /// <summary>
        /// Calculate the score of the player.
        /// </summary>
        /// <param name="result">Result of the game.</param>
        /// <returns>Return the score float between 0 and 1.</returns>
        protected override float CalculateScore(MemoryGameResults result)
        {
            if (result.amountOfTurns == 0)
            {
                return 0;
            }
            return Mathf.InverseLerp(3, 0,result.timeTaken.Seconds / 20);
        }
    }
}