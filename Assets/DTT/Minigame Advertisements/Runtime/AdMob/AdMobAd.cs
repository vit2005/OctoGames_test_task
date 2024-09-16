#if GOOGLE_MOBILE_ADS

using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace DTT.MinigameBase.Advertisements
{
    /// <summary>
    /// Base class handling building, loading and showing ads for AdMob.
    /// </summary>
    public abstract class AdMobAd : BaseAd, ICompletable
    {
        /// <summary>
        /// Invoked when the ad is closed.
        /// </summary>
        [Header("AdMob Settings")]
        public UnityEvent OnClosed;

        /// <summary>
        /// Invoked when the ad is completed.
        /// </summary>
        public event Action Completed;

        /// <summary>
        /// Invoked when the ad received a paid event.
        /// </summary>
        public UnityEvent<AdValueEventArgs> OnPaid;

        /// <summary>
        /// Ad unit ID for android.
        /// </summary>
        [SerializeField]
        private string _androidAdUnitId;

        /// <summary>
        /// Ad unit ID for iOS.
        /// </summary>
        [SerializeField]
        private string _iOSAdUnitId;

        /// <summary>
        /// Ad unit ID of this ad.
        /// </summary>
        public string AdUnitId =>
#if UNITY_IOS
            _iOSAdUnitId;
#elif UNITY_ANDROID
            _androidAdUnitId;
#else
            "unexpected_platform";
#endif

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="showOnLoad"><inheritdoc/></param>
        public override void LoadAd(bool showOnLoad)
        {
            AdRequest request = new AdRequest.Builder().Build();
            RequestAd(request, showOnLoad);
        }

        /// <summary>
        /// Requests the ad using the given <see cref="AdRequest"/>.
        /// </summary>
        /// <param name="request">The ad request.</param>
        /// <param name="showOnLoad">Whether the ad should be shown on load.</param>
        public abstract void RequestAd(AdRequest request, bool showOnLoad);

        /// <summary>
        /// Called when the ad has been loaded.
        /// </summary>
        /// <param name="sender">Object the event invoked from.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnAdLoaded(object sender, EventArgs args) => onLoad?.Invoke();

        /// <summary>
        /// Called when the ad failed to load.
        /// </summary>
        /// <param name="sender">Object the event invoked from.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) =>
            Debug.LogError("Ad failed to load: " + args.LoadAdError.GetMessage());

        /// <summary>
        /// Called when the ad failed to show.
        /// </summary>
        /// <param name="sender">Object the event invoked from.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnAdFailedToShow(object sender, AdErrorEventArgs args) =>
            Debug.LogError("Ad failed to show: " + args.AdError.GetMessage());

        /// <summary>
        /// Called when the add is clicked on.
        /// </summary>
        /// <param name="sender">Object the event invoked from.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnAdOpened(object sender, EventArgs args) => onClick?.Invoke();

        /// <summary>
        /// Called when the ad is closed.
        /// </summary>
        /// <param name="sender">Object the event invoked from.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnAdClosed(object sender, EventArgs args)
        {
            OnClosed?.Invoke();
            Completed?.Invoke();
        }

        /// <summary>
        /// Called when the ad received a paid event.
        /// </summary>
        /// <param name="sender">Object the event triggered from.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnAdPaid(object sender, AdValueEventArgs args) => OnPaid?.Invoke(args);
    }
}

#endif