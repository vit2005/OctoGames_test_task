#if GOOGLE_MOBILE_ADS

using GoogleMobileAds.Api;

namespace DTT.MinigameBase.Advertisements.AdMob
{
    /// <summary>
    /// Handles initializing advertisements for AdMob.
    /// </summary>
    public class AdMobInitializer : BaseAdsInitializer<AdMobInitializer>
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Initialize() => MobileAds.Initialize(initStatus => OnInitializationComplete());
    }
}

#endif