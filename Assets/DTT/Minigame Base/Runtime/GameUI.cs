using DTT.MinigameBase.LevelSelect;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MinigameBase.UI
{
    /// <summary>
    /// Standardized user interface for minigames.
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        /// <summary>
        /// The popup used for pausing and finishing.
        /// </summary>
        [SerializeField]
        private GamePopupUI _popup;

        /// <summary>
        /// The button for pausing the game.
        /// </summary>
        [SerializeField]
        private Button _pauseButton;

        /// <summary>
        /// The game object on which the minigame is placed.
        /// </summary>
        [SerializeField]
        private GameObject _minigameGameObject;

        /// <summary>
        /// The minigame instance.
        /// </summary>
        private IMinigame _minigame;
        
        /// <summary>
        /// The part of the minigame that can be restarted.
        /// </summary>
        private IRestartable _restartable;
        
        /// <summary>
        /// The part of the minigame that will let us know when it's finished.
        /// </summary>
        private IFinishedable _finishedable;
        
        /// <summary>
        /// The level select currently active.
        /// </summary>
        private LevelSelectHandlerBase _levelSelectHandler;

        /// <summary>
        /// Retrieves interface implementations.
        /// </summary>
        private void Awake()
        {
            _minigame = _minigameGameObject.GetComponent<IMinigame>();
            _restartable = _minigameGameObject.GetComponent<IRestartable>();
            _finishedable = _minigameGameObject.GetComponent<IFinishedable>();
            _levelSelectHandler = FindObjectOfType<LevelSelectHandlerBase>();
        }

        /// <summary>
        /// Subscribes to events.
        /// </summary>
        private void OnEnable()
        {
            _pauseButton.onClick.AddListener(PauseGame);
            _finishedable.Finished += FinishGame;
            _popup.ResumeButtonPressed += ResumeGame;
            _popup.RestartButtonPressed += RestartGame;
            _popup.HomeButtonPressed += ToHome;
        }

        /// <summary>
        /// Removes events.
        /// </summary>
        private void OnDisable()
        {
            _pauseButton.onClick.RemoveListener(PauseGame);
            _finishedable.Finished -= FinishGame;
            _popup.ResumeButtonPressed -= ResumeGame;
            _popup.RestartButtonPressed -= RestartGame;
            _popup.HomeButtonPressed -= ToHome;
        }

        /// <summary>
        /// Sets the UI in a state for when the game finishes.
        /// </summary>
        private void FinishGame()
        {
            _popup.Show(true);
            _popup.SetTitleToFinished();
            _popup.EnableResumeButton(false);
        }

        /// <summary>
        /// Sets the UI in a state for when the game restarts.
        /// </summary>
        private void RestartGame()
        {
            _minigame.Continue();
            
            _restartable.Restart();
            
            _popup.Show(false);
        }

        /// <summary>
        /// Sets the UI in a state for when the game resumes.
        /// </summary>
        private void ResumeGame()
        {
            _minigame.Continue();
            _popup.Show(false);
        }

        /// <summary>
        /// Sets the UI in a state for when the game resumes.
        /// </summary>
        private void PauseGame()
        {
            _minigame.Pause();
            _popup.Show(true);
            _popup.SetTitleToPaused();
            _popup.EnableResumeButton(true);
        }

        /// <summary>
        /// Sets the UI in a state for when the game goes back to home.
        /// </summary>
        private void ToHome()
        {
            _popup.Show(false);
            _levelSelectHandler.ShowLevelSelect();
        }
    }
}