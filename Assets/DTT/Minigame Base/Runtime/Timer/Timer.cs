using System;
using System.Diagnostics;
using UnityEngine;

namespace DTT.MinigameBase.Timer
{
    /// <summary>
    /// Component that can time.
    /// </summary>
    public class Timer : MonoBehaviour, ITimer
    {
        /// <summary>
        /// Used to time the time passed.
        /// </summary>
        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// The amount of time passed.
        /// </summary>
        public TimeSpan TimePassed => _stopwatch.Elapsed;
        
        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Begin() => _stopwatch.Restart();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop() => _stopwatch.Stop();

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        public void Pause() => _stopwatch.Stop();

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        public void Resume() => _stopwatch.Start();
    }
}