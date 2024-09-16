#if UNITY_ADS && UNITY_UNITYADS_API

namespace DTT.MinigameBase.Advertisements.UnityAds
{
    /// <summary>
    /// Base class for Unity ads.
    /// </summary>
    public abstract class UnityAd : BaseAd
    {
        /// <summary>
        /// Ad unit ID on Android.
        /// </summary>
        public abstract string AndroidAdUnitId { get; }

        /// <summary>
        /// Ad unit ID on iOS.
        /// </summary>
        public abstract string IOSAdUnitId { get; }

        /// <summary>
        /// Ad unit ID of this ad.
        /// </summary>
        public string AdUnitId =>
#if UNITY_IOS
            IOSAdUnitId;
#elif UNITY_ANDROID
            AndroidAdUnitId;
#else
            "unexpected_platform";
#endif
    }
}

#endif