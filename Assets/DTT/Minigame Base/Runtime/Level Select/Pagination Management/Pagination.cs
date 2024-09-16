using System;
using UnityEngine;

namespace DTT.MinigameBase.LevelSelect.PaginationManagement
{
    /// <summary>
    /// Main pagination logic controller.
    /// <remarks>Use <see cref="PaginationUI"/> for displaying visuals in Unity canvas.</remarks>
    /// </summary>
    internal class Pagination : MonoBehaviour
    {
        /// <summary>
        /// Called when the page has changed, passing the new page number.
        /// </summary>
        public event Action<int> PageChanged;
        
        /// <summary>
        /// Called when control enabled state has changed, passing the current state.
        /// </summary>
        public event Action<bool> ControlsEnabled;

        /// <summary>
        /// The current page of the pagination.
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// The total amount of pages available.
        /// </summary>
        public int TotalPages => _totalPages;

        /// <summary>
        /// The amount of items per page.
        /// </summary>
        public int ItemsPerPage => _itemsPerPage;

        /// <summary>
        /// The page that was used before the current. Is -1 initially.
        /// </summary>
        public int LastPage { get; private set; } = -1;

        /// <summary>
        /// Whether the pagination controls are enabled and can move between pages.
        /// </summary>
        public bool ControlsAreEnabled
        {
            get => _controlsAreEnabled;
            set
            {
                if(value != _controlsAreEnabled)
                    ControlsEnabled?.Invoke(value);
                _controlsAreEnabled = value;
            }
        }

        /// <summary>
        /// The amount of items per page.
        /// </summary>
        [SerializeField]
        [Tooltip("The amount of items per page.")]
        private int _itemsPerPage;

        /// <summary>
        /// The total amount of pages available.
        /// </summary>
        [SerializeField]
        [Tooltip("The total amount of pages available.")]
        private int _totalPages;

        /// <summary>
        /// Whether the pagination controls are enabled and can move between pages.
        /// </summary>
        private bool _controlsAreEnabled = true;

        /// <summary>
        /// Set the page to a new page number. If controls are disabled or given page is out of bounds, nothing happens.
        /// </summary>
        /// <param name="pageNumber">The new page that should be transitioned to.</param>
        public void SetPage(int pageNumber)
        {
            if (!ControlsAreEnabled || (pageNumber < 0 || pageNumber > TotalPages - 1))
                return;

            LastPage = CurrentPage;
            CurrentPage = pageNumber;
            PageChanged?.Invoke(pageNumber);
        }
        
        /// <summary>
        /// Go to the next page.
        /// </summary>
        public void NextPage() => SetPage(CurrentPage + 1);

        /// <summary>
        /// Go to the previous page.
        /// </summary>
        public void PreviousPage() => SetPage(CurrentPage - 1);

        /// <summary>
        /// Populate the pagination with the given amount of pages and amount of items per page.
        /// </summary>
        /// <param name="totalPages">Total amount of pages in the pagination.</param>
        /// <param name="itemsPerPage">The amount of items per page.</param>
        public void Populate(int totalPages, int itemsPerPage)
        {
            _totalPages = totalPages;
            _itemsPerPage = itemsPerPage;
        }
    }
}