#if GOOGLE_MOBILE_ADS

using GoogleMobileAds.Api;
using UnityEngine;

namespace DTT.MinigameBase.Advertisements.AdMob
{
    /// <summary>
    /// Handles showing a banner ad for AdMob.
    /// </summary>
    public class AdMobBannerAd : AdMobAd
    {
        /// <summary>
        /// Position the banner should be placed at.
        /// </summary>
        [Header("Banner Settings")]
        [SerializeField]
        [Tooltip("Position the banner should be placed at.")]
        private AdPosition _bannerPosition = AdPosition.Bottom;

        /// <summary>
        /// Whether to use adaptive banner sizes or not.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether to use adaptive banner sizes or not.")]
        private bool _useAdaptiveBanner = false;

        /// <summary>
        /// Whether the banner should dynamically take up the full width of the screen.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether the banner should dynamically take up the full width of the screen.")]
        private bool _useFullWidth = true;

        /// <summary>
        /// The custom adaptive banner width.
        /// </summary>
        [SerializeField]
        [Tooltip("The custom adaptive banner width.")]
        private int _customWidth = 200;

        /// <summary>
        /// Current <see cref="GoogleMobileAds.Api.BannerView"/> being shown.
        /// </summary>
        public BannerView BannerView { get; private set; }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        private void OnDestroy() => BannerView?.Destroy();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ShowAd() => BannerView?.Show();

        /// <summary>
        /// Hides the banner.
        /// </summary>
        public void HideAd() => BannerView?.Hide();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="request"><inheritdoc/></param>
        /// <param name="showOnLoad"><inheritdoc/></param>
        public override void RequestAd(AdRequest request, bool showOnLoad)
        {
            AdSize adSize = _useAdaptiveBanner ?
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(_useFullWidth ? AdSize.FullWidth : _customWidth)
                : AdSize.Banner;

            BannerView bannerView = new BannerView(AdUnitId, adSize, _bannerPosition);
            RequestAd(bannerView, request, showOnLoad);
        }

        /// <summary>
        /// Requests the ad using the given <see cref="AdRequest"/> and <see cref="GoogleMobileAds.Api.BannerView"/>.
        /// </summary>
        /// <param name="banner">The new banner.</param>
        /// <param name="request">The ad request.</param>
        /// <param name="showOnLoad">Whether the ad should be shown on load.</param>
        public void RequestAd(BannerView banner, AdRequest request, bool showOnLoad)
        {
            if (BannerView != null) BannerView.Destroy();

            BannerView = banner;

            BannerView.OnAdClosed += OnAdClosed;
            BannerView.OnAdLoaded += OnAdLoaded;
            BannerView.OnAdOpening += OnAdOpened;
            BannerView.OnAdFailedToLoad += OnAdFailedToLoad;
            BannerView.OnPaidEvent += OnAdPaid;

            BannerView.LoadAd(request);

            if (!showOnLoad)
                BannerView.Hide();
        }
    }
}

#endif