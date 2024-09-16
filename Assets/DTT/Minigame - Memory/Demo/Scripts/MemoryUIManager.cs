using UnityEngine;
using UnityEngine.UI;
using System.Text;
using DTT.MinigameMemory;

namespace DTT.MinigameMemory.Demo
{
    /// <summary>
    /// Manages the UI for the memory game.
    /// </summary>
    public class MemoryUIManager : MonoBehaviour
    {
        /// <summary>
        /// Used to get the state of the game.
        /// </summary>
        [SerializeField]
        private MemoryGameManager _gameManager;
        
        /// <summary>
        /// The _levelSelectHandler field is used to navigate back to the level selection from this popup.
        /// </summary>
        [SerializeField]
        private MemoryGameLevelSelectHandler _levelSelectHandler;

        /// <summary>
        /// Used to show UI for when the game is paused.
        /// </summary>
        [SerializeField]
        private PausedMenu _pausedMenu;

        /// <summary>
        /// <see cref="FinishedMenu"/> used to display the game results.
        /// </summary>
        [SerializeField]
        private FinishedMenu _finishedMenu;

        /// <summary>
        /// Subscrive to game events.
        /// </summary>
        private void OnEnable()
        {
            _gameManager.Started += SetFinisedMenuInactive;
            _gameManager.Started += SetPausedMenuInactive;
            _gameManager.Paused += SetPausedMenuActive;
            _gameManager.Finish += SetFinisedMenuActive;
            
            _finishedMenu._homeButton.onClick.AddListener(Home);
            _finishedMenu._restartButton.onClick.AddListener(Restart);
            _pausedMenu._restartButton.onClick.AddListener(Restart);
            _pausedMenu._homeButton.onClick.AddListener(Home);
            _pausedMenu._resumeButton.onClick.AddListener(Resume);
        }

        /// <summary>
        /// Unsubscrive from game events.
        /// </summary>
        private void OnDisable()
        {
            _gameManager.Started -= SetFinisedMenuInactive;
            _gameManager.Paused -= SetPausedMenuActive;
            _gameManager.Finish -= SetFinisedMenuActive;
        }

        /// <summary>
        /// Sets the active state of the <see cref="PausedMenu"/> gameobject.
        /// </summary>
        /// <param name="active">Whether the state should be active or not.</param>
        private void SetPausedMenuActive(bool active) => _pausedMenu.gameObject.SetActive(active);

        /// <summary>
        /// Sets the active state of the <see cref="PausedMenu"/> gameobject.
        /// </summary>
        private void SetPausedMenuInactive()
        {
            _pausedMenu.gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets the active state of the <see cref="FinishedMenu"/> gameobject to active.
        /// </summary>
        /// <param name="results">Results to be shown on the _finishedMenu.</param>
        private void SetFinisedMenuActive(MemoryGameResults results)
        {
            _finishedMenu.SetResultText(results);
            _finishedMenu.gameObject.SetActive(true);
        }

        /// <summary>
        /// Sets the active state of the <see cref="FinishedMenu"/> gameobject to inactive.
        /// </summary>
        private void SetFinisedMenuInactive() => _finishedMenu.gameObject.SetActive(false);

        /// <summary>
        /// Show home menu.
        /// </summary>
        private void Home()
        {
            Hide();
            _gameManager.Stop();
            _levelSelectHandler.ShowLevelSelect();
        }

        /// <summary>
        /// Restart the game.
        /// </summary>
        private void Restart()
        {
            Hide();
            _gameManager.Restart();
        }

        /// <summary>
        /// Hide menu UI.
        /// </summary>
        private void Hide()
        {
            SetFinisedMenuInactive();
            SetPausedMenuInactive();
        }

        /// <summary>
        /// Resume the game.
        /// </summary>
        private void Resume()
        {
            SetPausedMenuActive(false);
            _gameManager.Continue();
        }
    }
}