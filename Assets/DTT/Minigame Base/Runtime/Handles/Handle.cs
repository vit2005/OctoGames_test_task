using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DTT.MinigameBase.Handles
{
    /// <summary>
    /// Makes use of the UI callbacks for pointer events and passes these in actual events.
    /// Can be used to create pointer behaviour.
    /// </summary>
    public class Handle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        /// <summary>
        /// Called when the pointer is pressed on the UI element.
        /// </summary>
        public event Action<PointerEventData> PointerDown;
        
        /// <summary>
        /// Called when the pointer is released on the UI element.
        /// </summary>
        public event Action<PointerEventData> PointerUp;
        
        /// <summary>
        /// Called when the pointer is being dragged on the UI element.
        /// </summary>
        public event Action<PointerEventData> Drag;
        
        /// <summary>
        /// Called when the pointer has exited an UI element.
        /// </summary>
        public event Action<PointerEventData> Exit;
        
        /// <summary>
        /// Called when the pointer has entered an UI element.
        /// </summary>
        public event Action<PointerEventData> Enter;
        
        /// <summary>
        /// Called when the pointer click on a UI element.
        /// </summary>
        public event Action<PointerEventData> Click;
        
        /// <summary>
        /// Called by Unity UI system when the pointer is pressed on the UI element.
        /// </summary>
        /// <param name="eventData">The data about the pointer event.</param>
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) => PointerDown?.Invoke(eventData);

        /// <summary>
        /// Called by Unity UI system when the pointer is released on the UI element.
        /// </summary>
        /// <param name="eventData">The data about the pointer event.</param>
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) => PointerUp?.Invoke(eventData);

        /// <summary>
        /// Called by Unity UI system when the pointer is dragged on the UI element.
        /// </summary>
        /// <param name="eventData">The data about the pointer event.</param>
        void IDragHandler.OnDrag(PointerEventData eventData) => Drag?.Invoke(eventData);

        /// <summary>
        /// Called by Unity UI system when the pointer entered an UI element.
        /// </summary>
        /// <param name="eventData">The data about the pointer event.</param>
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => Enter?.Invoke(eventData);

        /// <summary>
        /// Called by Unity UI system when the pointer exited an UI element.
        /// </summary>
        /// <param name="eventData">The data about the pointer event.</param>
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => Exit?.Invoke(eventData);

        /// <summary>
        /// Called by Unity UI system when the pointer clicked on a UI element.
        /// </summary>
        /// <param name="eventData">The data about the pointer event.</param>
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) => Click?.Invoke(eventData);
    }
}