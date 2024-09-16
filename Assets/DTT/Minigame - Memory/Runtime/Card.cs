using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace DTT.MinigameMemory 
{
    /// <summary>
    /// Class to handle card behaviour.
    /// </summary>
    public class Card : MonoBehaviour
    {
        /// <summary>
        /// Is fired when clicked.
        /// </summary>
        public event Action<Card> Clicked;

        /// <summary>
        /// Is fired when the compared card is a match.
        /// </summary>
        public event Action<Card, Card> GoodMatch;

        /// <summary>
        /// Is fired when the compared card is not a match.
        /// </summary>
        public event Action<Card, Card> BadMatch;

        /// <summary>
        /// True if the front of the card is showing.
        /// </summary>
        public bool IsShowing => _isShowing;

        /// <summary>
        /// True if the card can be clicked.
        /// </summary>
        public bool canClick = true;

        /// <summary>
        /// Sprite used as the card front.
        /// </summary>
        private Sprite _frontSprite;

        /// <summary>
        /// Sprite used as the card back.
        /// </summary>
        private Sprite _backSprite;

        /// <summary>
        /// Reference to the button of the card.
        /// </summary>
        [SerializeField]
        private GameObject _cardContent;

        /// <summary>
        /// Reference to the button of the card.
        /// </summary>
        [SerializeField]
        private Button _cardButton;

        /// <summary>
        /// True if the front of the card is showing.
        /// </summary>
        private bool _isShowing = false;

        /// <summary>
        /// Sets the sprites for the card.
        /// </summary>
        /// <param name="frontSprite">The sprite shown on the front of the card.</param>
        /// <param name="backSprite">The sprite shown on the back of the card.</param>
        public void Init(Sprite backSprite)
        {
            _backSprite = backSprite;

            _cardButton.image.sprite = _backSprite;
        }

        /// <summary>
        /// Sets the frontsprite of the card.
        /// </summary>
        /// <param name="frontSprite">Sprite to be set as the front sprite.</param>
        public void SetFrontsprite(Sprite frontSprite) => _frontSprite = frontSprite;

        /// <summary>
        /// Swithes the cards side that is facing up.
        /// </summary>
        public void FlipCard(float speed)
        {
            this.StartCoroutine(Flip(Quaternion.Euler(0, (_isShowing ? 0 : 180), 0), _isShowing ? _backSprite : _frontSprite, speed));
            _isShowing = !_isShowing;
        }

        /// <summary>
        /// Comapairs this cards front sprite to a given cards front sprite.
        /// </summary>
        /// <param name="otherCard">The card to compair with.</param>
        public void CompairToCard(Card otherCard)
        {
            if (this._frontSprite == otherCard._frontSprite)
                GoodMatch?.Invoke(this, otherCard);
            else
                BadMatch?.Invoke(this, otherCard);
        }

        /// <summary>
        /// Moves the card content to a given position.
        /// </summary>
        /// <param name="target">Position to move too</param>
        public void MoveToPosition(Vector3 target) => _cardContent.transform.localPosition = target;

        /// <summary>
        /// Disables the content of the card.
        /// </summary>
        public void DisableCard(float speed) => this.StartCoroutine(FadeOutCard(speed));

        /// <summary>
        /// Enables the content of the card.
        /// </summary>
        public void EnableCard(float speed) => this.StartCoroutine(FadeInCard(speed));

        /// <summary>
        /// Adding listener to the card button.
        /// </summary>
        private void OnEnable() => _cardButton.onClick.AddListener(OnClick);

        /// <summary>
        /// Removing listener from the card button.
        /// </summary>
        private void OnDisable() => _cardButton.onClick.RemoveListener(OnClick);

        /// <summary>
        /// Called when the card is clicked.
        /// </summary>
        private void OnClick()
        {
            if (!canClick)
                return;

            Clicked?.Invoke(this);
        }

        /// <summary>
        /// Flips the card over time.
        /// </summary>
        /// <param name="targetRotation">Rotation to rotate to.</param>
        /// <param name="sprite">Sprite to be shown after flip.</param>
        /// <param name="time">Time to flip in seconds.</param>
        private IEnumerator Flip(Quaternion targetRotation, Sprite sprite, float time)
        {
            Quaternion myRotation = _cardContent.transform.localRotation;

            for (float t = 0; t < 1; t += Time.deltaTime / time) 
            {
                _cardContent.transform.localRotation = Quaternion.Lerp(myRotation, targetRotation, t);

                if (t > 0.5f && _cardButton.image.sprite != sprite)
                {
                    _cardButton.image.sprite = sprite;
                    _cardButton.transform.localRotation = targetRotation;
                }

                yield return null;
            }

            _cardContent.transform.localRotation = targetRotation;
        }

        /// <summary>
        /// Fades the card to invisible.
        /// </summary>
        /// <param name="time">Time to fade in seconds.</param>
        private IEnumerator FadeOutCard(float time)
        {
            _cardButton.enabled = false;

            for (float t = 0; t < 1; t += Time.deltaTime / time)
            {
                Color color = _cardButton.image.color;
                _cardButton.image.color = new Color(color.r, color.g, color.b, Mathf.Lerp(1f, 0f, t));

                yield return null;
            }
        }

        /// <summary>
        /// Fades the card to visible.
        /// </summary>
        /// <param name="time">Time to fade in seconds.</param>
        private IEnumerator FadeInCard(float time)
        {
            for (float t = 0; t < 1; t += Time.deltaTime / time)
            {
                Color color = _cardButton.image.color;
                _cardButton.image.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0f, 1f, t));

                yield return null;
            }

            _cardButton.enabled = true;
        }
    }
}