using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.MinigameMemory
{
    /// <summary>
    /// The different types for how cards can be placed on the board.
    /// </summary>
    public enum BoardModes
    {
        /// <summary>
        /// The board will show all the cards at once.
        /// </summary>
        [InspectorName("All cards on board.")]
        ALL_CARDS_ON_BOARD = 0,

        /// <summary>
        /// The board will show a limited amount of cards at once.
        /// </summary>
        [InspectorName("Limited amount of cards on board")]
        LIMIT_CARDS_ON_BOARD = 1
    }
}