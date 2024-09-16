using UnityEngine;
using UnityEngine.UI;

namespace DTT.MinigameBase.Background
{
    /// <summary>
    /// Moves the background pattern in a direction.
    /// </summary>
    public class BackgroundMover : MonoBehaviour
    {
        /// <summary>
        /// Background pattern.
        /// </summary>
        [SerializeField]
        [Tooltip("Background pattern.")]
        private RawImage _pattern;

        /// <summary>
        /// Background pattern move speed.
        /// </summary>
        [SerializeField]
        [Tooltip("Background pattern move speed.")]
        private float _speed = 0.0005f;

        /// <summary>
        /// Background pattern direction.
        /// </summary>
        [SerializeField]
        [Tooltip("Background pattern direction.")]
        private PatternDirection _patternDirection;

        /// <summary>
        /// Moves the pattern in the selected direction.
        /// </summary>
        private void Update()
        {
            switch (_patternDirection)
            {
                case PatternDirection.NORTH:
                    _pattern.uvRect = new Rect(_pattern.uvRect.x, _pattern.uvRect.y - _speed * Time.deltaTime, _pattern.uvRect.width, _pattern.uvRect.height);
                    break;
                case PatternDirection.EAST:
                    _pattern.uvRect = new Rect(_pattern.uvRect.x - _speed * Time.deltaTime, _pattern.uvRect.y, _pattern.uvRect.width, _pattern.uvRect.height);
                    break;
                case PatternDirection.SOUTH:
                    _pattern.uvRect = new Rect(_pattern.uvRect.x, _pattern.uvRect.y + _speed * Time.deltaTime, _pattern.uvRect.width, _pattern.uvRect.height);
                    break;
                case PatternDirection.WEST:
                    _pattern.uvRect = new Rect(_pattern.uvRect.x + _speed * Time.deltaTime, _pattern.uvRect.y, _pattern.uvRect.width, _pattern.uvRect.height);
                    break;
                case PatternDirection.NORTHEAST:
                    _pattern.uvRect = new Rect(_pattern.uvRect.x - _speed * Time.deltaTime, _pattern.uvRect.y - _speed * Time.deltaTime, _pattern.uvRect.width, _pattern.uvRect.height);
                    break;
                case PatternDirection.NORTHWEST:
                    _pattern.uvRect = new Rect(_pattern.uvRect.x + _speed * Time.deltaTime, _pattern.uvRect.y - _speed * Time.deltaTime, _pattern.uvRect.width, _pattern.uvRect.height);
                    break;
                case PatternDirection.SOUTHEAST:
                    _pattern.uvRect = new Rect(_pattern.uvRect.x - _speed * Time.deltaTime, _pattern.uvRect.y + _speed * Time.deltaTime, _pattern.uvRect.width, _pattern.uvRect.height);
                    break;
                case PatternDirection.SOUTHWEST:
                    _pattern.uvRect = new Rect(_pattern.uvRect.x + _speed * Time.deltaTime, _pattern.uvRect.y + _speed * Time.deltaTime, _pattern.uvRect.width, _pattern.uvRect.height);
                    break;
                default:
                    break;
            }
        }
    }
}
