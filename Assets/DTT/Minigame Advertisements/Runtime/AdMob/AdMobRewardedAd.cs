#if GOOGLE_MOBILE_ADS

using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

namespace DTT.MinigameBase.Advertisements.AdMob
{
    /// <summary>
    /// Handles showing a banner ad for AdMob.
    /// </summary>
    public class AdMobRewardedAd : AdMobAd, IAdMobRewardedAd
    {
        /// <summary>
        /// Invoked when the user received a reward from the ad.
        /// </summary>
        [Header("Rewarded Ad Events")]
        public UnityEvent<Reward> onRewarded;

        /// <summary>
        /// Current <see cref="GoogleMobileAds.Api.RewardedAd"/> being shown.
        /// </summary>
        public RewardedAd RewardedAd { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public UnityEvent<Reward> OnRewarded => onRewarded;

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        private void OnDestroy() => RewardedAd.Destroy();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ShowAd() => RewardedAd?.Show();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="request"><inheritdoc/></param>
        /// <param name="showOnLoad"><inheritdoc/></param>
        public override void RequestAd(AdRequest request, bool showOnLoad)
        {
            if (RewardedAd != null) 
                RewardedAd.Destroy();

            RewardedAd = new RewardedAd(AdUnitId);
            RewardedAd.OnAdFailedToLoad += OnAdFailedToLoad;
            RewardedAd.OnAdFailedToShow += OnAdFailedToShow;
            RewardedAd.OnAdLoaded += OnAdLoaded;
            RewardedAd.OnAdOpening += OnAdOpened;
            RewardedAd.OnAdClosed += OnAdClosed;
            RewardedAd.OnUserEarnedReward += OnUserEarnedReward;
            RewardedAd.OnPaidEvent += OnAdPaid;

            RewardedAd.LoadAd(request);

            if (showOnLoad)
                RewardedAd.Show();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="reward">The reward that was earned.</param>
        public virtual void OnUserEarnedReward(Reward reward) => onRewarded?.Invoke(reward);

        /// <summary>
        /// Called when the user earned a reward from watching the ad.
        /// </summary>
        /// <param name="sender">Object the event triggered from.</param>
        /// <param name="reward">Reward received from watching the ad.</param>
        private void OnUserEarnedReward(object sender, Reward reward) => OnUserEarnedReward(reward);
    }
}

#endif