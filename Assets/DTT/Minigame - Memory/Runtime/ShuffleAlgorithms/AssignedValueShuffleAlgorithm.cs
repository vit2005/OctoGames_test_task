using System.Collections.Generic;
using System.Linq;

namespace DTT.MinigameMemory
{
    /// <summary>
    /// Assigned value implementation of a shuffle algorithm.
    /// </summary>
    public class AssignedValueShuffleAlgorithm : IShuffleAlgorithm
    {
        /// <summary>
        /// Random to get random values from.
        /// </summary>
        private System.Random _random = new System.Random();

        /// <summary>
        /// List of random values used to randomly sort a given list.
        /// </summary>
        private List<double> _randoms = new List<double>();

        /// <summary>
        /// Shuffles the list to get a random ordered list.
        /// </summary>
        /// <typeparam name="T">Objects in the list.</typeparam>
        /// <param name="list">The list that needs to be shuffled.</param>
        /// <returns>The shuffled list.</returns>
        public List<T> Shuffle<T>(List<T> list)
        {
            _randoms.Clear();

            while (_randoms.Count < list.Count)
            {
                bool newUniqueValue = true;

                double randomValue = _random.NextDouble();
                foreach (double value in _randoms)
                {
                    if (value == randomValue)
                    {
                        newUniqueValue = false;
                        break;
                    }
                }

                if (!newUniqueValue)
                    continue;

                _randoms.Add(randomValue);
            }

            Dictionary<double, T> keyValuePairs = new Dictionary<double, T>();
            for (int i = 0; i < list.Count; i++)
                keyValuePairs.Add(_randoms[i], list[i]);

            var sortedDict = (from entry in keyValuePairs orderby entry.Key ascending select entry);

            for (int i = 0; i < list.Count; i++)
                list[i] = sortedDict.ElementAt(i).Value;

            return list;
        }
    }
}