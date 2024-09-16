using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MinigameBase.LevelSelect.PaginationManagement
{
    /// <summary>
    /// Handles the UI for the pagination.
    /// </summary>
    internal class PaginationUI : MonoBehaviour
    {
        /// <summary>
        /// The main pagination object that handles the pagination logic.
        /// </summary>
        [SerializeField]
        [Tooltip("The main pagination object that handles the pagination logic.")]
        private Pagination _pagination;
        
        /// <summary>
        /// The button for returning to the previous page.
        /// </summary>
        [SerializeField]
        [Tooltip("The button for returning to the previous page.")]
        private Button _previousButton;
        
        /// <summary>
        /// The button for going to the next page.
        /// </summary>
        [SerializeField]
        [Tooltip("The button for going to the next page.")]
        private Button _nextButton;

        /// <summary>
        /// The page selection UI component, for handling page selection.
        /// </summary>
        [SerializeField]
        [Tooltip("The page selection UI component, for handling page selection.")]
        private PageSelectorUI _pageSelector;

        /// <summary>
        /// Sets up pagination UI.
        /// <remarks>Is setup as a coroutine since UI needs some initial delay as to not interfere with its own initialization.</remarks>
        /// </summary>
        /// <returns>Coroutine.</returns>
        private IEnumerator Start()
        {
            yield return null;
            _pageSelector.Populate(_pagination);

            _previousButton.gameObject.SetActive(_pagination.CurrentPage > 0);
            _nextButton.gameObject.SetActive(_pagination.CurrentPage < _pagination.TotalPages - 1);
        }

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable()
        {
            _previousButton.onClick.AddListener(_pagination.PreviousPage);
            _nextButton.onClick.AddListener(_pagination.NextPage);
            _pagination.PageChanged += SetPageUI;
            _pageSelector.PageSelected += _pagination.SetPage;
            _pagination.ControlsEnabled += SetButtonControl;
        }

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable()
        {
            _previousButton.onClick.RemoveListener(_pagination.PreviousPage);
            _nextButton.onClick.RemoveListener(_pagination.NextPage);
            _pagination.PageChanged -= SetPageUI;
            _pageSelector.PageSelected += _pagination.SetPage;
            _pagination.ControlsEnabled -= SetButtonControl;
        }

        /// <summary>
        /// Called when the controls are enabled/disabled for the pagination.
        /// </summary>
        /// <param name="enabled">The enabled state.</param>
        private void SetButtonControl(bool enabled)
        {
            _nextButton.enabled = enabled;
            _previousButton.enabled = enabled;
        }

        /// <summary>
        /// Sets the user interface of the page to given page number.
        /// </summary>
        /// <param name="pageNumber">The page to set to.</param>
        private void SetPageUI(int pageNumber)
        {
            _pageSelector.SetPageActive(_pagination.LastPage, false);
            _pageSelector.SetPageActive(pageNumber, true);

            _previousButton.gameObject.SetActive(_pagination.CurrentPage > 0);
            _nextButton.gameObject.SetActive(_pagination.CurrentPage < _pagination.TotalPages - 1);
        }
    }
}