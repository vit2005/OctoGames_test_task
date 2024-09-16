namespace DTT.MinigameBase.Advertisements
{
    /// <summary>
    /// Enum holding settings for the OnAwake behaviour of ads.
    /// </summary>
    public enum AdAwakeAction
    {
        /// <summary>
        /// The ad won't do anything on awake.
        /// </summary>
        None = 0,

        /// <summary>
        /// The ad will load on awake.
        /// </summary>
        Load = 1,

        /// <summary>
        /// The ad will load and show on awake.
        /// </summary>
        LoadAndShow = 2
    }
}