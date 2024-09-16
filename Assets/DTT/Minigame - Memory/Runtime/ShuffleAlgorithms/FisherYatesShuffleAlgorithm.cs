using System.Collections.Generic;

namespace DTT.MinigameMemory
{
    /// <summary>
    /// Fisher–Yates implementation of a shuffle algorithm.
    /// </summary>
    public class FisherYatesShuffleAlgorithm : IShuffleAlgorithm
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
            int lastUnshuffledIndex = list.Count - 1;

            while (lastUnshuffledIndex > 0)
            {
                int randomIndex = _random.Next(0, lastUnshuffledIndex);
                T temporary = list[lastUnshuffledIndex];
                list[lastUnshuffledIndex] = list[randomIndex];
                list[randomIndex] = temporary;
                lastUnshuffledIndex--;
            }

            return list; ;
        }
    }
}