#if GOOGLE_MOBILE_ADS

using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

namespace DTT.MinigameBase.Advertisements.AdMob
{
    /// <summary>
    /// Handles showing an app open ad for AdMob.
    /// </summary>
    public class AdMobAppOpenAd : AdMobAd
    {
        /// <summary>
        /// Current <see cref="GoogleMobileAds.Api.AppOpenAd"/> being shown.
        /// </summary>
        public AppOpenAd AppOpenAd { get; private set; }

        /// <summary>
        /// Whether the ad should be shown after it loaded.
        /// </summary>
        private bool _showOnLoad = false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="request"><inheritdoc/></param>
        /// <param name="showOnLoad"><inheritdoc/></param>
        public override void RequestAd(AdRequest request, bool showOnLoad)
        {
            _showOnLoad = showOnLoad;

            if (AppOpenAd != null) 
                AppOpenAd.Destroy();

            AppOpenAd.LoadAd(AdUnitId, Screen.orientation, request, AdLoadCallback);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ShowAd() => AppOpenAd.Show();

        /// <summary>
        /// Called when the <see cref="GoogleMobileAds.Api.AppOpenAd"/> finished loading.
        /// </summary>
        /// <param name="ad">Loaded ad.</param>
        /// <param name="error">Loading error. Null if no errors were thrown.</param>
        private void AdLoadCallback(AppOpenAd ad, AdFailedToLoadEventArgs error)
        {
            if (error != null)
            {
                OnAdFailedToLoad(this, error);
                return;
            }

            AppOpenAd = ad;

            AppOpenAd.OnAdDidDismissFullScreenContent += OnAdClosed;
            AppOpenAd.OnAdDidPresentFullScreenContent += OnAdOpened;
            AppOpenAd.OnPaidEvent += OnAdPaid;
            AppOpenAd.OnAdFailedToPresentFullScreenContent += OnAdFailedToShow;

            if (_showOnLoad)
                AppOpenAd.Show();

            _showOnLoad = false;
        }
    }
}

#endif
