#if GOOGLE_MOBILE_ADS

using GoogleMobileAds.Api;
using UnityEngine.Events;

namespace DTT.MinigameBase.Advertisements.AdMob
{
    /// <summary>
    /// Interface for AdMob ads that have rewards.
    /// </summary>
    public interface IAdMobRewardedAd
    {
        /// <summary>
        /// Invoked when the user received an award from the ad.
        /// </summary>
        UnityEvent<Reward> OnRewarded { get; }

        /// <summary>
        /// Called when the user received an award.
        /// </summary>
        /// <param name="reward">Received award.</param>
        void OnUserEarnedReward(Reward reward);
    }
}

#endif