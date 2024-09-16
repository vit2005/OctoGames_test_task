#if UNITY_ADS && UNITY_UNITYADS_API

using UnityEngine;
using UnityEngine.Advertisements;

namespace DTT.MinigameBase.Advertisements.UnityAds
{
    /// <summary>
    /// Handles initializing Unity ads.
    /// </summary>
    public class UnityAdsInitializer : BaseAdsInitializer<UnityAdsInitializer>, IUnityAdsInitializationListener
    {
        /// <summary>
        /// Game id for android.
        /// </summary>
        [SerializeField]
        private string _androidGameId;

        /// <summary>
        /// Game id for iOS.
        /// </summary>
        [SerializeField]
        private string _iOSGameId;

        /// <summary>
        /// Whether the ads should be in test mode.
        /// </summary>
        [SerializeField]
        private bool _testMode = true;

        /// <summary>
        /// Current platform game id.
        /// </summary>
        private string _gameId => (Application.platform == RuntimePlatform.IPhonePlayer)
                ? _iOSGameId
                : _androidGameId;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Initialize() => Advertisement.Initialize(_gameId, _testMode, this);

        /// <summary>
        /// Called when the initialization fails.
        /// </summary>
        /// <param name="error">Initialization error.</param>
        /// <param name="message">Error message.</param>
        public void OnInitializationFailed(UnityAdsInitializationError error, string message) =>
            Debug.LogError($"Unity Ads Initialization Failed: {error} - {message}");
    }
}

#endif