using System;
using DTT.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MinigameBase.LevelSelect.PaginationManagement
{
    /// <summary>
    /// Handles the UI for the selectable dot in the pagination menu. Can be clicked on to move to that page.
    /// </summary>
    internal class PageDotUI : MonoBehaviour
    {
        /// <summary>
        /// The image component of that dot, used for changing the color and shape.
        /// </summary>
        [SerializeField]
        [Tooltip("The image component of that dot, used for changing the color and shape.")]
        private Image _image;

        /// <summary>
        /// The button component, so we can receive click callbacks.
        /// </summary>
        [SerializeField]
        [Tooltip("The button component, so we can receive click callbacks.")]
        private Button _button;

        /// <summary>
        /// Color when the page is selected.
        /// </summary>
        [SerializeField]
        [Tooltip("Color when the page is selected.")]
        private Color _activeColor;
        
        /// <summary>
        /// Color when the page is unselected.
        /// </summary>
        [SerializeField]
        [Tooltip("Color when the page is unselected.")]
        private Color _inactiveColor;

        /// <summary>
        /// The color of the page dot. Can be changed to display whether it's highlighted.
        /// </summary>
        public Color Color
        {
            get => _image.color;
            set => _image.color = value;
        }

        /// <summary>
        /// The index of this dot in the page selection.
        /// </summary>
        internal int Index { get; set; }
        
        /// <summary>
        /// The RectTransform component of this UI element.
        /// </summary>
        public RectTransform RectTransform => (RectTransform)transform;

        /// <summary>
        /// Called when the dot is clicked.
        /// </summary>
        public event Action<int> OnClick;

        /// <summary>
        /// The animation that is currently active. Can be used to cancel the animation.
        /// </summary>
        private Coroutine _activeAnimation;
        
        /// <summary>
        /// Sets the dot to be the active dot. Used for when a page is selected.
        /// </summary>
        /// <param name="active">Whether this should be the active page.</param>
        public void SetDotActive(bool active)
        {
            // Cancel the animation if one is ongoing.
            if(_activeAnimation != null)
                StopCoroutine(_activeAnimation);
            
            // Sets the active button to not be interactable.
            _button.interactable = !active;
            
            Color = active ? _activeColor : _inactiveColor;
            
            if (active)
            {
                _activeAnimation = DTTween.Value(RectTransform.sizeDelta.x, 68, 0.2f, Easing.EASE_IN_OUT_QUAD, val =>
                {
                    RectTransform.sizeDelta = new Vector2(val, 30);
                    _activeAnimation = null;
                });
            }
            else
            {
                _activeAnimation = DTTween.Value(RectTransform.sizeDelta.x, 30, 0.2f, Easing.EASE_IN_OUT_QUAD, val =>
                {
                    RectTransform.sizeDelta = new Vector2(val, 30);
                    _activeAnimation = null;
                });
            }
        }

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable() => _button.onClick.AddListener(OnButtonClicked);

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable() => _button.onClick.RemoveListener(OnButtonClicked);

        /// <summary>
        /// Invokes public API click event when button is pressed.
        /// </summary>
        private void OnButtonClicked() => OnClick?.Invoke(Index);
    }
}