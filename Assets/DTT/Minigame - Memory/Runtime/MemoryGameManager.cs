using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DTT.MinigameBase;
using DTT.MinigameBase.Timer;
using DTT.MinigameBase.UI;

namespace DTT.MinigameMemory
{
    /// <summary>
    /// Class that functions as the minigame manager.
    /// </summary>
    public class MemoryGameManager : MonoBehaviour, IMinigame<MemoryGameSettings, MemoryGameResults>
    {
        /// <summary>
        /// Is called when the game has started.
        /// </summary>
        public event Action Started;

        /// <summary>
        /// Is called when the game is paused.
        /// Provides a bool to indicate the paused state of the game.
        /// </summary>
        public event Action<bool> Paused;

        /// <summary>
        /// Is called when the game has finished.
        /// </summary>
        public event Action<MemoryGameResults> Finish;

        /// <summary>
        /// Is true when the game is paused.
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// Is true when the game is active.
        /// </summary>
        public bool IsGameActive => _isGameActive;

        /// <summary>
        /// Time that has passed in the game.
        /// </summary>
        public TimeSpan Time => _timer.TimePassed;

        /// <summary>
        /// The <see cref="Board"/> used for the minigame.
        /// </summary>
        [SerializeField]
        private Board _board;

        /// <summary>
        /// Game timer.
        /// </summary>
        [SerializeField]
        private Timer _timer;

        /// <summary>
        /// Is true when the game is paused.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Is true when the game has started and isn't finished.
        /// </summary>
        private bool _isGameActive;

        /// <summary>
        /// The GameSettings.
        /// </summary>
        private MemoryGameSettings _settings;

        /// <summary>
        /// The amount a player has tried to match two cards during the game.
        /// </summary>
        private int _amountOfTurns = 0;

        /// <summary>
        /// Starts the game with the given settings.
        /// </summary>
        /// <param name="settings">The settings used for this play session.</param>
        public void StartGame(MemoryGameSettings settings)
        {
            _settings = settings;
            _amountOfTurns = 0;
            _isPaused = false;
            _isGameActive = true;
            _timer.Begin();

            _board.SetupGame(_settings);
            Started?.Invoke();
        }

        /// <summary>
        /// Stops the game activities and timer.
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
            _timer.Pause();
            Paused?.Invoke(_isPaused);
        }

        /// <summary>
        /// Continues the game.
        /// </summary>
        public void Continue()
        {
            _isPaused = false;
            _timer.Resume();
            Paused?.Invoke(_isPaused);
        }

        /// <summary>
        /// Restarts the current game.
        /// </summary>
        public void Restart()
        {
            if (_isPaused)
                Continue();

            StartGame(_settings);
        }

        /// <summary>
        /// Finishes the current game.
        /// </summary>
        public void ForceFinish()
        {
            _timer.Stop();
            _isGameActive = false;
            Finish?.Invoke(new MemoryGameResults(_timer.TimePassed, _amountOfTurns));
        }

        /// <summary>
        /// Adds a <see cref="Timer"/> to the gameobject if there was not timer assigned.
        /// </summary>
        private void Awake() => _timer = (_timer == null) ? this.gameObject.AddComponent<Timer>() : _timer;

        /// <summary>
        /// Subscribe to board events.
        /// </summary>
        private void OnEnable()
        {
            _board.CardsTurned += IncreaseTurnAmount;
            _board.AllCardsMatched += ForceFinish;
        }

        /// <summary>
        /// Unsubscribe from board events.
        /// </summary>
        private void OnDisable()
        {
            _board.CardsTurned -= IncreaseTurnAmount;
            _board.AllCardsMatched -= ForceFinish;
        }

        /// <summary>
        /// Increases the amount of turns taken by one.
        /// </summary>
        private void IncreaseTurnAmount() => _amountOfTurns++;

        /// <summary>
        /// Stop the game.
        /// </summary>
        public void Stop()
        {
            _isGameActive = false;
            _timer.Stop();
        }
    }
}