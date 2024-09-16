using System;

namespace DTT.MinigameBase
{
    /// <summary>
    /// Contains the interface for handling a minigame.
    /// </summary>
    /// <typeparam name="TConfig">The configuration of the mini game.</typeparam>
    /// <typeparam name="TResult">The result of the mini game, typically used for statistics of the user.</typeparam>
    public interface IMinigame<in TConfig, out TResult> : IMinigame
    {
        /// <summary>
        /// Is called when the game has started.
        /// </summary>
        event Action Started;
        
        /// <summary>
        /// Is called when the game has finished.
        /// </summary>
        event Action<TResult> Finish;

        /// <summary>
        /// Is called when the user wants to start the game.
        /// </summary>
        void StartGame(TConfig config);
    }
    
    /// <summary>
    /// Contains the interface for handling a minigame.
    /// </summary>
    public interface IMinigame 
    {
        /// <summary>
        /// Is true when the game is paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Is true when the game has started and isn't finished.
        /// </summary>
        bool IsGameActive { get; }

        /// <summary>
        /// Stops the game activities and timer.
        /// </summary>
        void Pause();

        /// <summary>
        /// Continues the game.
        /// </summary>
        void Continue();

        /// <summary>
        /// Is called when the user want to force finish the game.
        /// </summary>
        void ForceFinish();
    }
}