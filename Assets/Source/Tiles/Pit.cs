using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Determines the pit's sprite
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))] [RequireComponent(typeof(Tile))]
    public class Pit : MonoBehaviour
    {
        [Tooltip("The pit sprites to use")]
        [SerializeField] private PitSprites pitSprites;

        /// <summary>
        /// Enum that holds the values for the corner directions
        /// </summary>
        private enum CornerDirection
        {
            None = 0,
            TopRight = 1,
            TopLeft = 2,
            BottomLeft = 4,
            BottomRight = 8
        }

        /// <summary>
        /// Enum that holds the values for the edge directions
        /// </summary>
        private enum EdgeDirection
        {
            None = 0,
            Right = 16,
            Up = 32,
            Left = 64,
            Down = 128
        }

        /// <summary>
        /// Determines the pit's sprite
        /// </summary>
        void Start()
        {
            Vector2Int tileLocation = GetComponent<Tile>().gridLocation;
            Room room = GetComponent<Tile>().room;

            /// <summary>
            /// Checks the location in the room grid to see if it has a pit component or not
            /// </summary>
            /// <param name="location"> The location to check </param>
            /// <param name="room"> The room to check </param>
            /// <returns> True if the tile does not have a pit component, false otherwise </returns>
            bool CheckLocation(Vector2Int location, Room room)
            {
                if (location.x >= room.roomSize.x || location.x < 0 || location.y >= room.roomSize.y || location.y < 0)
                {
                    return false;
                }

                return (room.roomGrid[location.x, location.y].GetComponent<Pit>() == null);
            }

            // Get the edge directions
            EdgeDirection edgeDirection = EdgeDirection.None;

            if (CheckLocation(tileLocation + new Vector2Int(1, 0), room))
            {
                edgeDirection |= EdgeDirection.Right;
            }
            if (CheckLocation(tileLocation + new Vector2Int(0, 1), room))
            {
                edgeDirection |= EdgeDirection.Up;
            }
            if (CheckLocation(tileLocation + new Vector2Int(-1, 0), room))
            {
                edgeDirection |= EdgeDirection.Left;
            }
            if (CheckLocation(tileLocation + new Vector2Int(0, -1), room))
            {
                edgeDirection |= EdgeDirection.Down;
            }

            // Get the corner directions
            CornerDirection cornerDirection = CornerDirection.None;
            if (CheckLocation(tileLocation + new Vector2Int(1, 1), room))
            {
                cornerDirection |= CornerDirection.TopRight;
            }
            if (CheckLocation(tileLocation + new Vector2Int(-1, 1), room))
            {
                cornerDirection |= CornerDirection.TopLeft;
            }
            if (CheckLocation(tileLocation + new Vector2Int(-1, -1), room))
            {
                cornerDirection |= CornerDirection.BottomLeft;
            }
            if (CheckLocation(tileLocation + new Vector2Int(1, -1), room))
            {
                cornerDirection |= CornerDirection.BottomRight;
            }

            // Determine the pit number
            int pitNumber = (int) edgeDirection;

            if (!edgeDirection.HasFlag(EdgeDirection.Right) || edgeDirection.HasFlag(EdgeDirection.Up))
            {
                pitNumber += (int) (cornerDirection & CornerDirection.TopRight);
            }
            if (!edgeDirection.HasFlag(EdgeDirection.Up) || edgeDirection.HasFlag(EdgeDirection.Left))
            {
                pitNumber += (int) (cornerDirection & CornerDirection.TopLeft);
            }
            if (!edgeDirection.HasFlag(EdgeDirection.Left) || edgeDirection.HasFlag(EdgeDirection.Down))
            {
                pitNumber += (int) (cornerDirection & CornerDirection.BottomLeft);
            }
            if (!edgeDirection.HasFlag(EdgeDirection.Down) || edgeDirection.HasFlag(EdgeDirection.Right))
            {
                pitNumber += (int) (cornerDirection & CornerDirection.BottomRight);
            }

            GetComponent<SpriteRenderer>().sprite = pitSprites.GetSprite(pitNumber);
        }
    }
}