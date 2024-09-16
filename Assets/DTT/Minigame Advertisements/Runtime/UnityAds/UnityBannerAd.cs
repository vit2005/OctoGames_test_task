#if UNITY_ADS && UNITY_UNITYADS_API

using UnityEngine;
using UnityEngine.Advertisements;

namespace DTT.MinigameBase.Advertisements.UnityAds
{
    /// <summary>
    /// Handles showing a banner ad on screen.
    /// </summary>
    public class UnityBannerAd : UnityAd
    {
        /// <summary>
        /// Anchor position of the banner ad.
        /// </summary>
        [SerializeField]
        [Tooltip("Anchor position of the banner ad.")]
        private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string AndroidAdUnitId => "Banner_Android";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string IOSAdUnitId => "Banner_iOS";

        /// <summary>
        /// Whether the ad should be shown when loaded.
        /// </summary>
        private bool _showOnLoad = false;

        /// <summary>
        /// Sets the banner position.
        /// </summary>
        private void Start() => Advertisement.Banner.SetPosition(_bannerPosition);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="showOnLoad"><inheritdoc/></param>
        public override void LoadAd(bool showOnLoad)
        {
            _showOnLoad = showOnLoad;

            BannerLoadOptions options = new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerError
            };

            Advertisement.Banner.Load(AdUnitId, options);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ShowAd()
        {
            BannerOptions options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };

            Advertisement.Banner.Show(AdUnitId, options);
        }

        /// <summary>
        /// Hides the banner ad.
        /// </summary>
        /// <param name="destroy">Whether the banner ad should be destroyed, requiring it to be loaded again.</param>
        public void HideAd(bool destroy = false) => Advertisement.Banner.Hide(destroy);

        /// <summary>
        /// Called when the banner loaded.
        /// </summary>
        protected virtual void OnBannerLoaded()
        {
            onLoad?.Invoke();
            if (_showOnLoad) ShowAd();
        }

        /// <summary>
        /// Called when the banner load failed.
        /// </summary>
        /// <param name="message">Error message.</param>
        protected virtual void OnBannerError(string message) => Debug.Log($"Banner Error: {message}");

        /// <summary>
        /// Called when the banner is clicked.
        /// </summary>
        protected virtual void OnBannerClicked() => onClick?.Invoke();

        /// <summary>
        /// Called when the banner is shown.
        /// </summary>
        protected virtual void OnBannerShown() => onShow?.Invoke();

        /// <summary>
        /// Called when the banner is hidden.
        /// </summary>
        protected virtual void OnBannerHidden() { }
    }
}

#endif