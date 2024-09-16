using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MinigameBase.Timer
{
    /// <summary>
    /// Displays the time of a <see cref="ITimer"/> on a text object.
    /// </summary>
    [RequireComponent(typeof(Timer))]
    public class TimerUI : MonoBehaviour
    {
        /// <summary>
        /// The text to display the time on.
        /// </summary>
        [SerializeField]
        private Text _text;
        
        /// <summary>
        /// The timer used to display.
        /// </summary>
        private ITimer _timer;
        
        /// <summary>
        /// The refresh interval in seconds.
        /// </summary>
        private const int INTERVAL_SECONDS = 1;

        /// <summary>
        /// Whether the timer should start when the object is enabled.
        /// </summary>
        [SerializeField]
        private bool _startOnEnable;

        /// <summary>
        /// Retrieve references.
        /// </summary>
        private void Awake() => _timer = GetComponent<ITimer>();

        /// <summary>
        /// Kicks off the timer update loop.
        /// </summary>
        private void OnEnable()
        {
            if (_text == null)
            {
                Debug.LogError("Missing text object for UITimer.");
                return;
            }
            
            if(_startOnEnable)
                _timer.Begin();
            StartCoroutine(UpdateTimer());
        }

        /// <summary>
        /// Coroutine that updates the text as long as the object is enabled.
        /// </summary>
        private IEnumerator UpdateTimer()
        {
            WaitForSeconds interval = new WaitForSeconds(INTERVAL_SECONDS);
            while (enabled)
            {
                yield return interval;
                _text.text = _timer.TimePassed.ToString("mm':'ss");
            }
        }
    }
}