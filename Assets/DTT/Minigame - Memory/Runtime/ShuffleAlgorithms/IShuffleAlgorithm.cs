using System.Collections.Generic;

namespace DTT.MinigameMemory
{
    /// <summary>
    /// Interface for shuffle algorithms.
    /// </summary>
    public interface IShuffleAlgorithm
    {
        /// <summary>
        /// Shuffles the list to get a random ordered list.
        /// </summary>
        /// <typeparam name="T">Objects in the list.</typeparam>
        /// <param name="list">The list that needs to be shuffled.</param>
        /// <returns>The shuffled list.</returns>
        List<T> Shuffle<T>(List<T> list);
    }
}