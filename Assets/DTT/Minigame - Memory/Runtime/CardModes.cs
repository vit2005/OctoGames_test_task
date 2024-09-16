using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.MinigameMemory
{
    /// <summary>
    /// The different types for how cards are used in the game.
    /// </summary>
    public enum CardModes
    {
        /// <summary>
        /// Each card has only one other card to match.
        /// </summary>
        [InspectorName("Use cards once.")]
        USE_CARDS_ONCE = 0,

        /// <summary>
        /// Each card has multiple other cards to match.
        /// </summary>
        [InspectorName("Reuse cards.")]
        REUSE_CARDS = 1
    }
}
