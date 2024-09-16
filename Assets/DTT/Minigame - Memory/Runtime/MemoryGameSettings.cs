using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MinigameMemory
{
    /// <summary>
    /// Scriptable object storing the settings for a Memory game.
    /// </summary>
    [CreateAssetMenu(fileName = "Memory_GameSettings_Template", menuName = "DTT/MiniGame/Memory/GameSettings")]
    public class MemoryGameSettings : ScriptableObject
    {
        /// <summary>
        /// Algorithm used to shuffle the cards.
        /// </summary>
        public IShuffleAlgorithm ShuffleAlgorithm => GetAlgorithm();

        /// <summary>
        /// Mode to use card sprites.
        /// </summary>
        public CardModes CardMode => _cardMode;

        /// <summary>
        /// Mode to place cards on the board.
        /// </summary>
        public BoardModes BoardMode => _boardMode;

        /// <summary>
        /// If true aligns the last row of cards to the center.
        /// </summary>
        public bool AlignLastRow => _alignLastRow;

        /// <summary>
        /// Maximum amount of cards on the table.
        /// </summary>
        public int AmountOfCardsInGame => GetAmountOfCardsInGame();

        /// <summary>
        /// Maximum amount of cards on the table.
        /// </summary>
        public int AmountOfCardsOnBoard => GetAmountOfCardsOnBoard();

        /// <summary>
        /// Percentage of matches found untill refilling the board.
        /// </summary>
        public int RefillAtFoundPercentage => _refillAtFoundPercentage;

        /// <summary>
        /// Speed of the animations in seconds.
        /// </summary>
        public float CardAnimationSpeed =>_cardAnimationSpeed;

        /// <summary>
        /// Sprite used for the card backs.
        /// </summary>
        public ReadOnlyCollection<Sprite> CardBacks => _cardBacks.AsReadOnly();

        /// <summary>
        /// Sprites used for the card fronts.
        /// </summary>
        public ReadOnlyCollection<Sprite> CardSprites => _cardSprites.AsReadOnly();

        // Algorithm Settings header.
        [Header("Algorithm Settings")]
        [Space(10)]

        /// <summary>
        /// Algorithm used to shuffle the cards.
        /// </summary>
        [SerializeField]
        [Tooltip("Determines the difficulty the game is played in.")]
        private ShuffleAlgorithms _shuffleAlgorithm;

        /// <summary>
        /// If true aligns the last row of cards to the center.
        /// </summary>
        [SerializeField]
        [Tooltip("True: Alligns the bottom row of cards with the center.")]
        private bool _alignLastRow = true;

        // Game Settings header.
        [Header("Game Settings")]
        [Space(10)]

        /// <summary>
        /// Mode to use card sprites.
        /// </summary>
        [SerializeField]
        [Tooltip("Determines the use of card sprites.")]
        private CardModes _cardMode;

        /// <summary>
        /// Amount of cards the game is played with.
        /// </summary>
        [SerializeField]
        [Tooltip("Amount of cards the game is played with.")]
        private int _amountOfCards = 10;

        // Board Settings header.
        [Header("Board Settings")]
        [Space(10)]

        /// <summary>
        /// Mode to place cards on the board.
        /// </summary>
        [SerializeField]
        [Tooltip("Determines the amount of cards placed on the board.")]
        private BoardModes _boardMode;

        /// <summary>
        /// Maximum amount of cards on the table.
        /// </summary>
        [SerializeField]
        [Tooltip("Limit for the amount of cards on the board.")]
        private int _cardsOnBoardLimit = 4;

        /// <summary>
        /// Percentage of matches found untill refilling the board.
        /// </summary>
        [SerializeField]
        [Tooltip("Percentage of matches found untill refilling the board.")]
        [Range(0, 100)]
        private int _refillAtFoundPercentage = 100;

        // Card Settings header.
        [Header("Card Settings")]
        [Space(10)]

        /// <summary>
        /// Speed of the animations in seconds.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed of the animations in seconds.")]
        [Min(0f)]
        private float _cardAnimationSpeed = 1f;

        /// <summary>
        /// Sprite used for the card backs.
        /// </summary>
        [SerializeField]
        [Tooltip("The sprite used on the back of the cards.")]
        private List<Sprite> _cardBacks;

        /// <summary>
        /// Sprites used for the card fronts.
        /// </summary>
        [SerializeField]
        [Tooltip("The sprites used to match cards with each other.")]
        private List<Sprite> _cardSprites;

        /// <summary>
        /// Sets the default values on creation or reset of the objects.
        /// </summary>
        private void Reset()
        {
            _shuffleAlgorithm = ShuffleAlgorithms.FISHER_YATES;
            _cardMode = CardModes.USE_CARDS_ONCE;
            _boardMode = BoardModes.ALL_CARDS_ON_BOARD;
            _amountOfCards = 10;
            _cardsOnBoardLimit = 4;
            _refillAtFoundPercentage = 100;
            _cardAnimationSpeed = 1f;
            _alignLastRow = true;
            _cardBacks = new List<Sprite>();
            _cardSprites = new List<Sprite>();
        }

        /// <summary>
        /// The amount of cards in the game based on <see cref="CardModes"/>.
        /// </summary>
        /// <returns>Amount of cards in the game.</returns>
        private int GetAmountOfCardsInGame()
        {
            switch (_cardMode)
            {
                case CardModes.USE_CARDS_ONCE:
                    return _cardSprites.Count * 2;

                case CardModes.REUSE_CARDS:
                    if (_amountOfCards % 2 != 0)
                        _cardsOnBoardLimit--;

                    return _amountOfCards;

                default:
                    return _cardSprites.Count * 2;
            }
        }

        /// <summary>
        /// Maximum amount of cards on the table based on <see cref="BoardModes"/>.
        /// </summary>
        /// <returns>Maximum amount of cards on the table.</returns>
        private int GetAmountOfCardsOnBoard()
        {
            int cardsInGame = GetAmountOfCardsInGame();
            int cardsOnBoard = 0;

            switch (_boardMode)
            {
                case BoardModes.ALL_CARDS_ON_BOARD:
                    cardsOnBoard = AmountOfCardsInGame;
                    break;

                case BoardModes.LIMIT_CARDS_ON_BOARD:
                    if (_cardsOnBoardLimit < 4)
                        _cardsOnBoardLimit = 4;

                    if (_cardsOnBoardLimit % 2 != 0)
                        _cardsOnBoardLimit--;

                    cardsOnBoard = _cardsOnBoardLimit;
                    break;

                default:
                    cardsOnBoard = CardSprites.Count * 2;
                    break;
            }

            if (cardsOnBoard > cardsInGame)
                cardsOnBoard = cardsInGame;

            return cardsOnBoard;
        }

        /// <summary>
        /// Provides a shuffle algorithm based on <see cref="ShuffleAlgorithms"/>.
        /// Uses a random algorithm if no fitting algorithm was found.
        /// </summary>
        /// <returns>Shuffle algorithm to use.</returns>
        private IShuffleAlgorithm GetAlgorithm()
        {
            IShuffleAlgorithm algorithm;

            switch (_shuffleAlgorithm)
            {
                case ShuffleAlgorithms.FISHER_YATES:
                    algorithm = new FisherYatesShuffleAlgorithm();
                    break;

                case ShuffleAlgorithms.KNUTHS_SHUFFLE:
                    algorithm = new KnuthShuffleAlgorithm();
                    break;

                case ShuffleAlgorithms.ASSIGNED_VALUE:
                    algorithm = new AssignedValueShuffleAlgorithm();
                    break;

                default:
                    Debug.LogError("[DTT] - [Minigame - Memory] - [GameSettings] - " +
                        "ShuffleAlgorithm not recognised, using Fisher-Yates shuffle instead.");
                    algorithm = new FisherYatesShuffleAlgorithm();
                    break;
            }

            return algorithm;
        }
    }
}