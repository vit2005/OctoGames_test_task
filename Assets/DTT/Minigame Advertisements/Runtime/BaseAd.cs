using UnityEngine;
using UnityEngine.Events;

namespace DTT.MinigameBase.Advertisements
{
    /// <summary>
    /// Base class for ads. Handles loading and showing the ad.
    /// </summary>
    public abstract class BaseAd : MonoBehaviour
    {
        /// <summary>
        /// Whether the add should be shown on Awake.
        /// </summary>
        [SerializeField]
        private AdAwakeAction _onAwakeAction = AdAwakeAction.None;

        /// <summary>
        /// Invoked when the ad has been loaded.
        /// </summary>
        [Header("General Events")]
        public UnityEvent onLoad;

        /// <summary>
        /// Invoked when the ad has been shown.
        /// </summary>
        public UnityEvent onShow;

        /// <summary>
        /// Invoked when the ad has been clicked.
        /// </summary>
        public UnityEvent onClick;

        /// <summary>
        /// Loads and shows the add.
        /// </summary>
        protected virtual void Awake()
        {
            if (_onAwakeAction == AdAwakeAction.None)
                return;
            
            LoadAd(_onAwakeAction == AdAwakeAction.LoadAndShow);
        }

        /// <summary>
        /// Loads the ad.
        /// </summary>
        /// <param name="showOnLoad">Whether the ad should open after loading.</param>
        public abstract void LoadAd(bool showOnLoad);

        /// <summary>
        /// Shows the ad. Make sure to call <see cref="LoadAd(bool)"/> before showing an ad.
        /// </summary>
        public abstract void ShowAd();
    }
}