#if UNITY_ADS && UNITY_UNITYADS_API

using System;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace DTT.MinigameBase.Advertisements.UnityAds
{
    /// <summary>
    /// Handles showing a fullscreen rewarded ad.
    /// </summary>
    public class UnityRewardAd : UnityInterstitialAd, ICompletable
    {
        /// <summary>
        /// Invoked when the rewarded ad completed successfully.
        /// </summary>
        public UnityEvent onRewardComplete;

        /// <summary>
        /// Invoked when the rewarded ad was canceled before completion.
        /// </summary>
        public UnityEvent onRewardFailed;

        /// <summary>
        /// Invoked when the ad has completed.
        /// </summary>
        public event Action Completed;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string AndroidAdUnitId => "Rewarded_Android";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string IOSAdUnitId => "Rewarded_iOS";

        /// <summary>
        /// Invokes the reward events depending on the completion state.
        /// </summary>
        /// <param name="placementId"><inheritdoc/></param>
        /// <param name="showCompletionState"><inheritdoc/></param>
        public override void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            base.OnUnityAdsShowComplete(placementId, showCompletionState);

            bool adHasCompleted = showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED);
            if (placementId.Equals(AdUnitId) && adHasCompleted)
                onRewardComplete?.Invoke();
            else
                onRewardFailed?.Invoke();

            Completed?.Invoke();
        }
    }
}

#endif