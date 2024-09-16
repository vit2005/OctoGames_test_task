#if UNITY_ADS && UNITY_UNITYADS_API

using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace DTT.MinigameBase.Advertisements.UnityAds
{
    /// <summary>
    /// Handles showing an full screen ad on screen.
    /// </summary>
    public class UnityInterstitialAd : UnityAd, IUnityAdsLoadListener, IUnityAdsShowListener, ICompletable
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string AndroidAdUnitId => "Interstitial_Android";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string IOSAdUnitId => "Interstitial_iOS";

        /// <summary>
        /// Invoked when the ad has completed.
        /// </summary>
        public event Action Completed;

        /// <summary>
        /// Whether the ad should show when the ad has been loaded.
        /// </summary>
        private bool _showOnLoad;

        /// <summary>
        /// Invoked when the full screen ad has been completed.
        /// </summary>
        public UnityEvent<UnityAdsShowCompletionState> onComplete;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ShowAd() => Advertisement.Show(AdUnitId, this);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void LoadAd(bool showOnLoad)
        {
            _showOnLoad = showOnLoad;
            Advertisement.Load(AdUnitId, this);
        }

        /// <summary>
        /// Invokes the onLoad event of the ad.
        /// </summary>
        /// <param name="placementId"><inheritdoc/></param>
        public virtual void OnUnityAdsAdLoaded(string placementId)
        {
            onLoad?.Invoke();
            if (_showOnLoad)
                ShowAd();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="placementId"><inheritdoc/></param>
        /// <param name="error"><inheritdoc/></param>
        /// <param name="message"><inheritdoc/></param>
        public virtual void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"Error loading Ad Unit: {placementId} - {error} - {message}");
            _showOnLoad = false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="placementId"><inheritdoc/></param>
        /// <param name="error"><inheritdoc/></param>
        /// <param name="message"><inheritdoc/></param>
        public virtual void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) =>
            Debug.LogError($"Error showing Ad Unit {placementId}: {error} - {message}");

        /// <summary>
        /// Invokes the onShow event of the ad.
        /// </summary>
        /// <param name="placementId"><inheritdoc/></param>
        public virtual void OnUnityAdsShowStart(string placementId) => onShow?.Invoke();

        /// <summary>
        /// Invokes the onClick event of the ad.
        /// </summary>
        /// <param name="placementId"><inheritdoc/></param>
        public virtual void OnUnityAdsShowClick(string placementId) => onClick?.Invoke();

        /// <summary>
        /// Invokes the onComplete event of the ad.
        /// </summary>
        /// <param name="placementId"><inheritdoc/></param>
        /// <param name="showCompletionState"><inheritdoc/></param>
        public virtual void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if (!placementId.Equals(AdUnitId)) 
                return;
            
            onComplete?.Invoke(showCompletionState);
            Completed?.Invoke();
        }
    }
}

#endif