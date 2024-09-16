using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.MinigameMemory
{
    /// <summary>
    /// Different shuffle algorithms.
    /// </summary>
    public enum ShuffleAlgorithms
    {
        /// <summary>
        /// The game uses the modern Fisher-Yates algorithm to shuffle the cards.
        /// </summary>
        [InspectorName("Modern Fisher-Yates Shuffle")]
        FISHER_YATES = 0,

        /// <summary>
        /// The game uses Knuth's algorithm based on the old Fisher-Yates algorithm to shuffle the cards.
        /// </summary>
        [InspectorName("Knuth's Shuffle")]
        KNUTHS_SHUFFLE = 1,

        /// <summary>
        /// The game uses the randomly assigned value algorithm to sort the cards.
        /// </summary>
        [InspectorName("Random assigned values")]
        ASSIGNED_VALUE = 2
    }
}