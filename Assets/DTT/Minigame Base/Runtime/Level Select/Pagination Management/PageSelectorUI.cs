using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MinigameBase.LevelSelect.PaginationManagement
{
    /// <summary>
    /// Manages the UI for making a page selection.
    /// </summary>
    internal class PageSelectorUI : MonoBehaviour
    {
        /// <summary>
        /// Called when a page is selected, passing the page number.
        /// </summary>
        public event Action<int> PageSelected;

        /// <summary>
        /// The prefab to use for the page dots.
        /// </summary>
        [SerializeField]
        [Tooltip("The prefab to use for the page dots.")]
        private PageDotUI _pageDotPrefab;

        /// <summary>
        /// The layout group where the page dots will be placed in.
        /// </summary>
        [SerializeField]
        [Tooltip("The layout group where the page dots will be placed in.")]
        private HorizontalLayoutGroup _horizontalLayoutGroup;

        /// <summary>
        /// The active instances of the page dots created from the <see cref="_pageDotPrefab"/>.
        /// </summary>
        private readonly List<PageDotUI> _pageDotInstances = new List<PageDotUI>();

        /// <summary>
        /// Populates the page selection based on the given pagination object.
        /// </summary>
        /// <param name="pagination">The pagination this page selection UI should be created for.</param>
        /// <param name="activePage">The page that initially should be active.</param>
        public void Populate(Pagination pagination, int activePage = 0)
        {
            // Clean up active instances.
            for (int i = 0; i < _pageDotInstances.Count; i++)
            {
                _pageDotInstances[i].OnClick -= PageSelected;
                Destroy(_pageDotInstances[i]);
            }
            _pageDotInstances.Clear();

            // Create new instances.
            for (int i = 0; i < pagination.TotalPages; i++)
            {
                PageDotUI dot = Instantiate(_pageDotPrefab, transform);
                _pageDotInstances.Add(dot);
                dot.Index = i;
                dot.OnClick += PageSelected;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_horizontalLayoutGroup.transform);
            
            SetPageActive(activePage, true);
        }

        /// <summary>
        /// Sets the given page at the given index to be active.
        /// </summary>
        /// <param name="pageIndex">The index of the page to be affected.</param>
        /// <param name="active">Enable/disable this page.</param>
        public void SetPageActive(int pageIndex, bool active) => _pageDotInstances[pageIndex].SetDotActive(active);
    }
}