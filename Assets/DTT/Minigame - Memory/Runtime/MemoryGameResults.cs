using System.Text;
using System;

namespace DTT.MinigameMemory
{
    /// <summary>
    /// Class to contain the results of a Memory game.
    /// </summary>
    public class MemoryGameResults
    {
        /// <summary>
        /// Time it took to finish the game in seconds.
        /// </summary>
        public readonly TimeSpan timeTaken;

        /// <summary>
        /// Amount of times two cards have been flipped.
        /// </summary>
        public readonly int amountOfTurns;

        /// <summary>
        /// Sets the result information.
        /// </summary>
        /// <param name="timeTaken">Time the player took to finish the game in seconds.</param>
        /// <param name="amountOfTurns">Amount of times the player has restarted a level.</param>
        public MemoryGameResults(TimeSpan timeTaken, int amountOfTurns)
        {
            this.timeTaken = timeTaken;
            this.amountOfTurns = amountOfTurns;
        }

        /// <summary>
        /// Returns result info in string format for debugging.
        /// </summary>
        /// <returns>Result in string format</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Time taken (s): ");
            sb.Append(timeTaken.ToString(@"hh\:mm\:ss"));
            sb.Append('\t');
            sb.Append("Amount of turns taken: ");
            sb.Append(amountOfTurns);

            return sb.ToString();
        }
    }
}