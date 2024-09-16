using UnityEngine;

namespace DTT.MinigameBase
{
    /// <summary>
    /// Represents a custom yield instruction that waits for a mini game to be active.
    /// </summary>
    public class WaitForMinigame : CustomYieldInstruction
    {
        /// <summary>
        /// The minigame to wait for.
        /// </summary>
        private readonly IMinigame _minigame;

        /// <summary>
        /// Creates a new instance of this yield instruction.
        /// </summary>
        /// <param name="minigame">The minigame to wait for.</param>
        public WaitForMinigame(IMinigame minigame) => _minigame = minigame;

        /// <summary>
        /// Whether the game is active.
        /// </summary>
        public override bool keepWaiting => _minigame.IsGameActive;
    }
}


