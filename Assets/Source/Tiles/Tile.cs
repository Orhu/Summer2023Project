using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Tiles for use in the room grid. Holds information for pathfinding and spawning the tiles
    /// </summary>
    [System.Serializable] [ExecuteAlways]
    public class Tile : MonoBehaviour
    {
        [Tooltip("How much this tile costs to walk on (higher is avoided more, lower is preferred)")]
        public int walkMovementPenalty;

        [Tooltip("How much this tile costs to fly over (higher is avoided more, lower is preferred)")]
        public int flyMovementPenalty;

        [Tooltip("How much this tile costs to burrow below (higher is avoided more, lower is preferred)")]
        public int burrowMovementPenalty;

        [Tooltip("the x and y location of this tile within the 2D array grid")]
        public Vector2Int gridLocation;

        // The room that this tile is a part of 
        [HideInInspector] public Room room;
    }
}