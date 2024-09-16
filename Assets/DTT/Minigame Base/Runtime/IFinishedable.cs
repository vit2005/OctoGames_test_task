using System;

namespace DTT.MinigameBase.UI
{
    /// <summary>
    /// Should be implemented on objects that can signal that they're finished.
    /// </summary>
    public interface IFinishedable
    {
        /// <summary>
        /// Should be called when the object has finished.
        /// </summary>
        event Action Finished;
    }
}