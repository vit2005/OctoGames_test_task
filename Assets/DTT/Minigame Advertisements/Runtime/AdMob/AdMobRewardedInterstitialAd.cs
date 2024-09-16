#if GOOGLE_MOBILE_ADS

using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

namespace DTT.MinigameBase.Advertisements.AdMob
{
    /// <summary>
    /// Handles showing an rewarded interstitial ad for AdMob.
    /// </summary>
    public class AdMobRewardedInterstitialAd : AdMobAd, IAdMobRewardedAd
    {
        /// <summary>
        /// Invoked when the user received a reward from the ad.
        /// </summary>
        [Header("Rewarded Ad Events")]
        public UnityEvent<Reward> onRewarded;

        /// <summary>
        /// Current <see cref="GoogleMobileAds.Api.RewardedInterstitialAd"/> being shown.
        /// </summary>
        public RewardedInterstitialAd RewardedInterstitialAd { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public UnityEvent<Reward> OnRewarded => onRewarded;

        /// <summary>
        /// Whether the ad should be shown after it loaded.
        /// </summary>
        private bool _showOnLoad = false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ShowAd() => RewardedInterstitialAd?.Show(OnUserEarnedReward);

        /// <summary>
        /// Called when the user received an award.
        /// </summary>
        /// <param name="reward">Received award.</param>
        public virtual void OnUserEarnedReward(Reward reward) => onRewarded?.Invoke(reward);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="request"><inheritdoc/></param>
        /// <param name="showOnLoad"><inheritdoc/></param>
        public override void RequestAd(AdRequest request, bool showOnLoad)
        {
            _showOnLoad = showOnLoad;

            if (RewardedInterstitialAd != null)
                RewardedInterstitialAd.Destroy();

            RewardedInterstitialAd.LoadAd(AdUnitId, request, AdLoadCallback);
        }

        /// <summary>
        /// Called when the <see cref="GoogleMobileAds.Api.RewardedInterstitialAd"/> finished loading.
        /// </summary>
        /// <param name="ad">Loaded ad.</param>
        /// <param name="error">Loading error. Null if no errors were thrown.</param>
        private void AdLoadCallback(RewardedInterstitialAd ad, AdFailedToLoadEventArgs error)
        {
            if (error != null)
            {
                OnAdFailedToLoad(this, error);
                return;
            }

            RewardedInterstitialAd = ad;

            RewardedInterstitialAd.OnAdDidDismissFullScreenContent += OnAdClosed;
            RewardedInterstitialAd.OnAdDidPresentFullScreenContent += OnAdOpened;
            RewardedInterstitialAd.OnPaidEvent += OnAdPaid;
            RewardedInterstitialAd.OnAdFailedToPresentFullScreenContent += OnAdFailedToShow;

            if (_showOnLoad)
                RewardedInterstitialAd.Show(OnUserEarnedReward);

            _showOnLoad = false;
        }
    }
}

#endif