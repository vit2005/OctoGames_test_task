#if GOOGLE_MOBILE_ADS

using GoogleMobileAds.Api;

namespace DTT.MinigameBase.Advertisements.AdMob
{
    /// <summary>
    /// Handles showing an interstitial ad for AdMob.
    /// </summary>
    public class AdMobInterstitialAd : AdMobAd
    {
        /// <summary>
        /// Current <see cref="GoogleMobileAds.Api.InterstitialAd"/> being shown.
        /// </summary>
        public InterstitialAd InterstitialAd { get; private set; }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        private void OnDestroy() => InterstitialAd?.Destroy();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ShowAd() => InterstitialAd?.Show();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="request"><inheritdoc/></param>
        /// <param name="showOnLoad"><inheritdoc/></param>
        public override void RequestAd(AdRequest request, bool showOnLoad)
        {
            if (InterstitialAd != null)
                InterstitialAd.Destroy();

            InterstitialAd = new InterstitialAd(AdUnitId);
            InterstitialAd.OnAdClosed += OnAdClosed;
            InterstitialAd.OnAdFailedToLoad += OnAdFailedToLoad;
            InterstitialAd.OnAdFailedToShow += OnAdFailedToShow;
            InterstitialAd.OnAdLoaded += OnAdLoaded;
            InterstitialAd.OnAdOpening += OnAdOpened;
            InterstitialAd.OnPaidEvent += OnAdPaid;

            InterstitialAd.LoadAd(request);

            if (showOnLoad)
                InterstitialAd.Show();
        }
    }
}

#endif