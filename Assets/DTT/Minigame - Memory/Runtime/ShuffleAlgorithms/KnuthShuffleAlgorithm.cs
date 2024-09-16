using System.Collections.Generic;

namespace DTT.MinigameMemory
{
    /// <summary>
    /// Knut's implementation of a shuffle algorithm.
    /// </summary>
    public class KnuthShuffleAlgorithm : IShuffleAlgorithm
    {
        /// <summary>
        /// Random to get random values from.
        /// </summary>
        private System.Random _random = new System.Random();

        /// <summary>
        /// Shuffles the list to get a random ordered list.
        /// </summary>
        /// <typeparam name="T">Objects in the list.</typeparam>
        /// <param name="list">The list that needs to be shuffled.</param>
        /// <returns>The shuffled list.</returns>
        public List<T> Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temporary = list[i];
                int randomIndex = _random.Next(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temporary;
            }

            return list;
        }
    }
}