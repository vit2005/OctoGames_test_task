using System;
using UnityEngine;

namespace DTT.MinigameBase.LevelSelect
{
    /// <summary>
    /// The data about a level.
    /// </summary>
    [Serializable]
    public struct LevelData
    {
        /// <summary>
        /// The number of the level.
        /// </summary>
        public int levelNumber;
        
        /// <summary>
        /// The score of the level. (Value between 0 and 1.)
        /// </summary>
        [Range(0, 1)]
        public float score;
        
        /// <summary>
        /// Whether the level is locked.
        /// </summary>
        public bool locked;
    }
}