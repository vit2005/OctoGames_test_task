using System;
using UnityEngine;

namespace DTT.MinigameBase.Advertisements
{
    /// <summary>
    /// Base class for initializing ad handlers.
    /// </summary>
    public abstract class BaseAdsInitializer<T> : MonoBehaviour, IAdsInitializer
    {
        /// <summary>
        /// Whether the advertisements should be initialized on awake.
        /// </summary>
        [SerializeField]
        private bool _initializeOnAwake;

        /// <summary>
        /// Whether ads have been initialized.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// Invoked when the initializer is initialized.
        /// </summary>
        public static event Action Initialized;

        /// <summary>
        /// Initializes the ads if specified.
        /// </summary>
        void Awake()
        {
            IsInitialized = false;
            if (_initializeOnAwake) 
                Initialize();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void OnInitializationComplete()
        {
            IsInitialized = true;
            Initialized?.Invoke();
        }
    }
}