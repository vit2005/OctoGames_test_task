using DTT.MinigameBase.Advertisements;
#if UNITY_ADS && UNITY_UNITYADS_API
using DTT.MinigameBase.Advertisements.UnityAds;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace DTT.MinigameBase.Advertisements
{
    /// <summary>
    /// Handles displaying advertisements when playing minigames.
    /// </summary>
    public abstract class MinigameAdHandler<TConfig, TResult, TMinigame> : MonoBehaviour where TMinigame : Object, IMinigame<TConfig, TResult>
    {
        /// <summary>
        /// The GameObject that contains the Minigame manager.
        /// </summary>
        [SerializeField]
        private GameObject _minigameGameObject;

        /// <summary>
        /// The minigame interface that we want to handle ads on.
        /// </summary>
        private TMinigame _minigame;
        
        /// <summary>
        /// The ad to load when the minigame has finished.
        /// </summary>
        private BaseAd _ad;

        /// <summary>
        /// Whether to show an ad when the minigame starts.
        /// </summary>
        [SerializeField]
        [Tooltip("If applicable, shows an ad at the start of the game.")]
        private bool _ableToShowAdOnStart;
        
        /// <summary>
        /// Pause minigame when showing ad at start.
        /// </summary>
        [SerializeField]
        [Tooltip("Pause minigame when showing ad at start. This can be useful for interstitial ads.")]
        private bool _pauseGameWithAdAtStart;
        
        /// <summary>
        /// Whether to show an ad when the minigame finishes.
        /// </summary>
        [SerializeField]
        [Tooltip("If applicable, shows an ad at the end of the game.")]
        private bool _ableToShowAdOnFinish = true;
        
        /// <summary>
        /// The interval at which the ads will be displayed. Every time an ad is eligible to be displayed it checks if it matches this interval and otherwise increments the counter.
        /// </summary>
        [Min(0)]
        [SerializeField]
        [Tooltip("The interval at which the ads will be displayed. Every time an ad is eligible to be displayed it checks if it matches this interval and otherwise increments the counter.")]
        private int _adInterval = 3;

        /// <summary>
        /// The unique key used for keeping track of the interval on this component.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private string _uniqueKey = "";

        /// <summary>
        /// The key used to access the persistent ad interval state.
        /// </summary>
        private string _intervalKey => $"{GetType().FullName}_AD_INTERVAL_{_uniqueKey}";

        /// <summary>
        /// Retrieves components.
        /// </summary>
        private void Awake()
        {
            _ad = GetComponent<BaseAd>();
            _minigame = _minigameGameObject.GetComponent<TMinigame>();
            AddScript();
        }

        /// <summary>
        /// Subscribes to the finish method.
        /// </summary>
        private void OnEnable()
        {
            _minigame.Started += OnStart;
            _minigame.Finish += OnFinished;
        }

        /// <summary>
        /// Cleans up state.
        /// </summary>
        private void OnDisable()
        {
            _minigame.Started -= OnStart;
            _minigame.Finish -= OnFinished;
        }

        /// <summary>
        /// Called when the game starts.
        /// </summary>
        private void OnStart()
        {
            if (!_ableToShowAdOnStart)
                return;
            
            bool load = TryLoadAd();

            if (_pauseGameWithAdAtStart && load)
            {
                _minigame.Pause();

                if (_ad is ICompletable completable)
                {
                    completable.Completed += OnAdClosed;

                    void OnAdClosed()
                    {
                        completable.Completed -= OnAdClosed;
                        
                        _minigame.Continue();
                    }
                }
            }
        }

        /// <summary>
        /// Loads the when the minigame has finished.
        /// </summary>
        private void OnFinished(TResult result)
        {
            if (!_ableToShowAdOnFinish)
                return;

            TryLoadAd();
        }

        /// <summary>
        /// Increments the current counter value.
        /// </summary>
        private void IncrementInterval()
        {
            int current = GetInterval();
            PlayerPrefs.SetInt(_intervalKey, current + 1);
        }
        
        /// <summary>
        /// Retrieves the current interval count.
        /// </summary>
        /// <returns>The current interval count.</returns>
        private int GetInterval() => PlayerPrefs.GetInt(_intervalKey, 0);
        
        /// <summary>
        /// Resets the interval to zero.
        /// </summary>
        private void ResetInterval() => PlayerPrefs.SetInt(_intervalKey, 0);

        /// <summary>
        /// Tries to load an ad based on the counter.
        /// </summary>
        private bool TryLoadAd()
        {
            bool load = GetInterval() % _adInterval == 0;
            if(load)
                _ad.LoadAd(true);
            
            IncrementInterval();
            return load;
        }

        /// <summary>
        /// Add the ads script to the game object if plugins are found.
        /// </summary>
        public void AddScript()
        {
#if UNITY_ADS && UNITY_UNITYADS_API
            if (_ad == null)
                _ad = gameObject.AddComponent<UnityInterstitialAd>();

            UnityAdsInitializer initializer = GetComponent<UnityAdsInitializer>();
            if (initializer == null)
                initializer = gameObject.AddComponent<UnityAdsInitializer>();
#elif GOOGLE_MOBILE_ADS
            if (_ad == null)
                _ad = gameObject.AddComponent<AdMobInterstitialAd>();

            AdMobInitializer initializer = GetComponent<AdMobInitializer>();
            if (initializer == null)
                initializer = gameObject.AddComponent<AdMobInitializer>();
#endif
        }
    }
}