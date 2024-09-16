using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.ObjectModel;

namespace DTT.MinigameMemory
{
    /// <summary>
    /// Class to handle the board behaviour.
    /// </summary>
    public class Board : MonoBehaviour
    {
        /// <summary>
        /// Is fired when two cards are turned.
        /// </summary>
        public event Action CardsTurned;

        /// <summary>
        /// Is fired when all cards have been matched.
        /// </summary>
        public event Action AllCardsMatched;

        /// <summary>
        /// The grid the cards are placed on.
        /// </summary>
        [SerializeField]
        [Tooltip("GridLayoutGroup used to position the cards.")]
        private GridLayoutGroup _grid;

        /// <summary>
        /// The space between the cards in percentage of card size.
        /// </summary>
        [SerializeField]
        [Tooltip("Percentage of card size used for space between te cards.")]
        private float _cardSpacing;

        /// <summary>
        /// The maximum size in pixels on the screen the card can be.
        /// Leaving the value 0 will ignore using a maximum size.
        /// </summary>
        [SerializeField]
        private float _maximumCardSize = 0f;

        /// <summary>
        /// Prefab to make cards from.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab used to create the cards shown on the board.")]
        private GameObject _cardPrefab;

        /// <summary>
        /// List of all the cards in the game.
        /// </summary>
        private List<Sprite> _cardsInGame = new List<Sprite>();

        /// <summary>
        /// List of all the cards on the board.
        /// </summary>
        private List<Card> _cardsOnBoard = new List<Card>();

        /// <summary>
        /// List of all the active cards in the game.
        /// </summary>
        private List<Card> _activeCards = new List<Card>();

        /// <summary>
        /// List of all the active cards in the game.
        /// </summary>
        private List<Card> _inactiveCards = new List<Card>();

        /// <summary>
        /// Suffle algorithm used to shuffle the cards.
        /// </summary>
        private IShuffleAlgorithm _shuffleAlgorithm;

        /// <summary>
        /// The maximum amount of cards that fit in a row on the grid.
        /// </summary>
        private int _maxCardsInRow;

        /// <summary>
        /// The first card that was turned by the player.
        /// </summary>
        private Card _firstSelectedCard;

        /// <summary>
        /// Percentage of matched found untill the board is refilled.
        /// </summary>
        private int _activateAtMatchesFoundPecentage;

        /// <summary>
        /// Time the card animations are played in, in seconds.
        /// </summary>
        private float _cardAnimationSpeed;

        /// <summary>
        /// Create's the cards and places them on the board.
        /// </summary>
        /// <param name="settings">Game settings used for this play session.</param>
        public void SetupGame(MemoryGameSettings settings)
        {
            _shuffleAlgorithm = settings.ShuffleAlgorithm;
            _activateAtMatchesFoundPecentage = settings.RefillAtFoundPercentage;
            _cardAnimationSpeed = settings.CardAnimationSpeed;

            ClearCards();

            CreateCards(settings.AmountOfCardsInGame, settings.CardSprites);

            SetupGrid(settings.AmountOfCardsOnBoard);

            CreateBoardCards(settings.AmountOfCardsOnBoard, settings.CardBacks);

            if (settings.AlignLastRow)
                AlignLastRowCards(settings.CardBacks);

            ActivateCards();
        }

        /// <summary>
        /// Subscribing to card events.
        /// </summary>
        private void OnEnable()
        {
            foreach (Card card in _cardsOnBoard)
            {
                card.Clicked += OnCardClicked;
                card.GoodMatch += RemoveMatchedCards;
                card.BadMatch += FlipMatchedCards;
            }
        }

        /// <summary>
        /// Unsubscribing from card events.
        /// </summary>
        private void OnDisable()
        {
            foreach (Card card in _cardsOnBoard)
            {
                card.Clicked -= OnCardClicked;
                card.GoodMatch -= RemoveMatchedCards;
                card.BadMatch -= FlipMatchedCards;
            }
        }

        /// <summary>
        /// Creates a list of all the cards sprites used in the game.
        /// </summary>
        /// <param name="amountOfCards">Amount of cards to create</param>
        /// <param name="CardSprites">List of sprites to use.</param>
        private void CreateCards(int amountOfCards, ReadOnlyCollection<Sprite> CardSprites)
        {
            int index;

            for (int i = 0; i < (amountOfCards / 2); i++)
            {
                index = Mathf.FloorToInt( Mathf.Repeat(i, CardSprites.Count));

                _cardsInGame.Add(CardSprites[index]);
                _cardsInGame.Add(CardSprites[index]);
            }

        }

        /// <summary>
        /// Sets the cell size of the grid depending on the amount of cards.
        /// </summary>
        /// <param name="numberOfCards">Amount of cards that will be placed on the board.</param>
        private void SetupGrid(int numberOfCards)
        {
            // Retrieve values of the containing parent.
            Rect gridRectangle= ((RectTransform)_grid.transform).rect;
            float availableWidth = gridRectangle.width;
            float availableHeight = gridRectangle.height;
            bool isLandscapeOrientation = availableWidth > availableHeight;

            // Initial occupied rows/columns values.
            int occupiedRows = isLandscapeOrientation ? 1 : numberOfCards;
            int occupiedColumns = isLandscapeOrientation ? numberOfCards : 1;

            // Initiate a loop that keeps dividing the amount by either 2 or 3 (2 preferred) of columns/rows as long as one axis is
            // higher than the other, according to the current orientation.
            while(
                (isLandscapeOrientation ? occupiedColumns > occupiedRows : occupiedRows > occupiedColumns) &&
                (isLandscapeOrientation ?
                    ((occupiedColumns % 2 == 0 && occupiedColumns / 2 != 1) || (occupiedColumns % 3 == 0 && occupiedColumns / 3 != 1)) :
                    ((occupiedRows % 2 == 0 && occupiedRows / 2 != 1) || (occupiedRows % 3 == 0 && occupiedRows / 3 != 1))
                    )
                )
            {
                if (isLandscapeOrientation)
                {
                    int division = occupiedColumns % 2 == 0 ? 2 : 3;
                    occupiedRows *= division;
                    occupiedColumns /= division;
                }
                else
                {
                    int division = occupiedRows % 2 == 0 ? 2 : 3;
                    occupiedRows /= division;
                    occupiedColumns *= division;
                }
            }

            // Set the constraint for amount of cards in a row.
            // Uses Mathf.Max to potentially flip the axises that better fits the aspect ratio.
            _maxCardsInRow = Mathf.Max(occupiedRows, occupiedColumns);

            // Calculate the best-fitting size for the cards.
            float widthSize = availableWidth / occupiedColumns;
            float heightSize = availableHeight / occupiedRows;
            widthSize -= widthSize * _cardSpacing;
            heightSize -= heightSize * _cardSpacing;
            float minimumSize = Mathf.Min(widthSize, heightSize);
            if (_maximumCardSize > 0f && minimumSize > _maximumCardSize)
                minimumSize = _maximumCardSize;

            // Set the grid layout's values.
            _grid.cellSize = Vector2.one * minimumSize;
            _grid.spacing = Vector2.one * minimumSize * _cardSpacing;

            GridLayoutGroup.Constraint gridConstraint =
                isLandscapeOrientation ?
                GridLayoutGroup.Constraint.FixedColumnCount :
                GridLayoutGroup.Constraint.FixedRowCount;

            _grid.constraint = gridConstraint;
            _grid.constraintCount = _maxCardsInRow;
        }

        /// <summary>
        /// Creates the playing cards and subscribes to their events.
        /// </summary>
        /// <param name="amountOfCardsOnBoard">Amount of cards on the board.</param>
        /// <param name="backSprites">List of sprites used for the card backs.</param>
        private void CreateBoardCards(int amountOfCardsOnBoard, ReadOnlyCollection<Sprite> backSprites)
        {
            int row;
            int cardbackIndex;
            
            _cardsOnBoard.Clear();
            _inactiveCards.Clear();

            for (int i = 0; i < amountOfCardsOnBoard; i++)
            {
                Card card = Instantiate(_cardPrefab, _grid.transform).GetComponent<Card>();

                row = Mathf.FloorToInt(i / _maxCardsInRow);
                cardbackIndex = ((i % _maxCardsInRow) + (row % backSprites.Count)) % backSprites.Count;

                card.Init(backSprites[cardbackIndex]);
                _cardsOnBoard.Add(card);
                _inactiveCards.Add(card);

                card.Clicked += OnCardClicked;
                card.GoodMatch += RemoveMatchedCards;
                card.BadMatch += FlipMatchedCards;
            }
        }

        /// <summary>
        /// Activates the inactive cards with available cards left in the game.
        /// </summary>
        private void ActivateCards()
        {
            List<Sprite> sprites = new List<Sprite>();
            if (_inactiveCards.Count < _cardsInGame.Count)
                sprites = _cardsInGame.GetRange(0, _inactiveCards.Count);
            else
                sprites = _cardsInGame;

            sprites = _shuffleAlgorithm.Shuffle(sprites);

            for (int i = 0; i < sprites.Count; i++)
            {
                Card card = _inactiveCards[i];
                card.SetFrontsprite(sprites[i]);
                _activeCards.Add(card);

                if (card.IsShowing)
                    card.FlipCard(_cardAnimationSpeed);

                card.EnableCard(_cardAnimationSpeed);
            }

            if (_inactiveCards.Count < _cardsInGame.Count)
                _cardsInGame.RemoveRange(0, _inactiveCards.Count);
            else
                _cardsInGame.Clear();
            
            _inactiveCards.Clear();
        }

        /// <summary>
        /// Alligns the cards in the last row to be in the middle.
        /// Changes card back if needed.
        /// </summary>
        /// <param name="backSprites">List of sprites used for the card backs.</param>
        private void AlignLastRowCards(ReadOnlyCollection<Sprite> backSprites) 
        {
            int cardsInLastRow = _cardsOnBoard.Count % _maxCardsInRow;
            int row = Mathf.FloorToInt(_cardsOnBoard.Count / _maxCardsInRow);

            if (cardsInLastRow == _maxCardsInRow)
                return;

            int emptySlotsInLastRow = _maxCardsInRow - cardsInLastRow;
            float NumberOfCardWidths = emptySlotsInLastRow / 2f;
            float distance = (NumberOfCardWidths * _grid.cellSize.x) + (NumberOfCardWidths * _grid.spacing.x);
            int cardIndex;
            int cardbackIndex;

            for (int i = 0; i < cardsInLastRow; i++)
            {
                cardIndex = row * _maxCardsInRow + i;
                Vector3 newPosition = _cardsOnBoard[cardIndex].transform.localPosition + new Vector3(distance, 0, 0);
                _cardsOnBoard[cardIndex].MoveToPosition(new Vector3(distance, 0, 0));

                cardbackIndex = (i + (row + 1 % backSprites.Count)) % backSprites.Count;
                _cardsOnBoard[cardIndex].Init( backSprites[cardbackIndex]);
            }
        }

        /// <summary>
        /// Destroys all cards still on the board and empties the list.
        /// </summary>
        private void ClearCards()
        {
            _cardsOnBoard.ForEach(card => Destroy(card.gameObject));
            foreach (Card card in _cardsOnBoard)
            {
                card.Clicked -= OnCardClicked;
                card.GoodMatch -= RemoveMatchedCards;
                card.BadMatch -= FlipMatchedCards;

                Destroy(card.gameObject);
            }

            _cardsInGame.Clear();
            _cardsOnBoard.Clear();
            _activeCards.Clear();
        }

        /// <summary>
        /// Saves a reference to the clicked card,
        /// if a second card is clicked they are compared for a match.
        /// </summary>
        /// <param name="clickedCard">Rederence to the <see cref="Card"/> that has been clicked.</param>
        private void OnCardClicked(Card clickedCard)
        {
            clickedCard.FlipCard(_cardAnimationSpeed);

            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = clickedCard;
                _firstSelectedCard.canClick = false;
                return;
            }

            LockCards(true);

            StartCoroutine(CompareDelay(clickedCard));
        }

        /// <summary>
        /// Removes the matched cards from the cards list.
        /// </summary>
        /// <param name="card1">First <see cref="Card"/> of the match.</param>
        /// <param name="card2">Second <see cref="Card"/> of the match.</param>
        private void RemoveMatchedCards(Card card1, Card card2)
        {
            _activeCards.Remove(card1);
            _inactiveCards.Add(card1);
            card1.DisableCard(_cardAnimationSpeed);

            _activeCards.Remove(card2);
            _inactiveCards.Add(card2);
            card2.DisableCard(_cardAnimationSpeed);

            if (_cardsInGame.Count == 0 && _activeCards.Count == 0)
                AllCardsMatched?.Invoke();
            if (_inactiveCards.Count >= Mathf.FloorToInt(_cardsOnBoard.Count * (_activateAtMatchesFoundPecentage / 100f)))
                ActivateCards();
            
            LockCards(false);
        }

        /// <summary>
        /// Flip the matched cards.
        /// </summary>
        /// <param name="card1">First <see cref="Card"/> of the match.</param>
        /// <param name="card2">Second <see cref="Card"/> of the match.</param>
        private void FlipMatchedCards(Card card1, Card card2)
        {
            card1.FlipCard(_cardAnimationSpeed);
            card2.FlipCard(_cardAnimationSpeed);

            LockCards(false);
        }

        /// <summary>
        /// Sets the clickability of the cards.
        /// </summary>
        /// <param name="lockCards">Whether the cards can be clicked or not.</param>
        private void LockCards(bool lockCards) => _activeCards.ForEach(card => card.canClick = !lockCards);

        /// <summary>
        /// After a second compairs the first clicked card with the clicked card.
        /// </summary>
        /// <param name="clickedCard">The card to be compared.</param>
        private IEnumerator CompareDelay(Card clickedCard)
        {
            float delay = _cardAnimationSpeed;
            if (delay == 0)
                delay = 0.5f;

            yield return new WaitForSeconds(delay);

            CardsTurned?.Invoke();
            _firstSelectedCard.CompairToCard(clickedCard);
            _firstSelectedCard = null;
        }
    }
}