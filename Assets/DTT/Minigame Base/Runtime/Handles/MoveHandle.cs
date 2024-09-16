using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DTT.MinigameBase.Handles
{
    /// <summary>
    /// A handle used for moving the object on the mouse.
    /// This makes sure the object isn't snapped to mouse but just translates with it.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class MoveHandle : Handle
    {
        /// <summary>
        /// The transform of the object to move.
        /// </summary>
        private RectTransform _rectTransform;

        /// <summary>
        /// The most upper parent of the handle.
        /// </summary>
        private Canvas _canvasHolder;

        /// <summary>
        /// Retrieves component references.
        /// </summary>
        private void Awake() => _rectTransform = (RectTransform)transform;

        /// <summary>
        /// Initializes the most upper <see cref="Canvas"/> parent component of the handle.
        /// </summary>
        private void Start()
        {
            Canvas[] canvasParents = GetComponentsInParent<Canvas>();
            if (canvasParents.Length > 0)
                _canvasHolder = canvasParents.Last();
            else
                Debug.LogWarning($"The handle '{name}' is not present in a canvas. Input to the handle may be ignored.");
        }

        /// <summary>
        /// Subscribes to events.
        /// </summary>
        private void OnEnable()
        {
            Drag += OnDrag;
        }

        /// <summary>
        /// Cleans up subscribed events.
        /// </summary>
        private void OnDisable()
        {
            Drag -= OnDrag;
        }

        /// <summary>
        /// Adds the position change to the object.
        /// </summary>
        /// <param name="eventData">The data about the pointer event.</param>
        private void OnDrag(PointerEventData eventData)
        {
            if (_canvasHolder == null)
                return;

            switch(_canvasHolder.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    _rectTransform.position += (Vector3)eventData.delta;
                    break;
                case RenderMode.ScreenSpaceCamera:
                case RenderMode.WorldSpace:
                    Vector2 cursorPosition = eventData.position;
                    Camera canvasCamera = _canvasHolder.worldCamera ?? Camera.main;

                    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, cursorPosition, canvasCamera, out Vector3 rectPosition))
                        _rectTransform.position = rectPosition;

                    break;
            }
        }
    }
}