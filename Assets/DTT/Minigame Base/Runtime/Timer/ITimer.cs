using System;

namespace DTT.MinigameBase.Timer
{
    /// <summary>
    /// Objects that can time should implement this interface.
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// The amount of time passed.
        /// </summary>
        TimeSpan TimePassed { get; }
        
        /// <summary>
        /// Should start the timer.
        /// </summary>
        void Begin();
        
        /// <summary>
        /// Should stop the timer from running.
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Should pause the timer.
        /// </summary>
        void Pause();
        
        
        /// <summary>
        /// Should resume the timer.
        /// </summary>
        void Resume();
    }
}