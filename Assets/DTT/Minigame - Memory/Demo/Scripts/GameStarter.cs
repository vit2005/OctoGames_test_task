using UnityEngine;
using DTT.MinigameMemory;
using System;

namespace DTT.MinigameMemory.Demo
{
    /// <summary>
    /// Used to start the memory minigame throught the <see cref="MemoryGameManager"/>.
    /// </summary>
    public class GameStarter : MonoBehaviour
    {
        /// <summary>
        /// Used to manipulate the game flow.
        /// </summary>
        [SerializeField]
        private MemoryGameManager _gameManager;

        /// <summary>
        /// Settings used for this game.
        /// </summary>
        [SerializeField]
        private MemoryGameSettings _gameSettings;

        /// <summary>
        /// Results from the game.
        /// </summary>
        private MemoryGameResults _gameResults;

        /// <summary>
        /// Subscribes to the game finished event to store results.
        /// </summary>
        private void OnEnable() => _gameManager.Finish += StoreResults;

        /// <summary>
        /// Unsubscribes from the game finished event.
        /// </summary>
        private void OnDisable() => _gameManager.Finish += StoreResults;

        /// <summary>
        /// Starts a new memory minigame.
        /// </summary>
        private void Start() => _gameManager.StartGame(_gameSettings);

        /// <summary>
        /// Sums the existing results and the new results.
        /// </summary>
        /// <param name="results">new results to be added.</param>
        private void StoreResults(MemoryGameResults results)
        {
            if (_gameResults == null)
            {
                _gameResults = results;
                return;
            }

            TimeSpan totalTimeTaken = _gameResults.timeTaken + results.timeTaken;
            int totalTurnsTaken = _gameResults.amountOfTurns + results.amountOfTurns;
            _gameResults = new MemoryGameResults(totalTimeTaken, totalTurnsTaken);
        }
    }
}