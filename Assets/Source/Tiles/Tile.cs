using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementType = Cardificer.RoomInterface.MovementType;

namespace Cardificer
{
    /// <summary>
    /// Tiles for use in the room grid. Holds information for pathfinding and spawning the tiles
    /// </summary>
    [System.Serializable]
    public class Tile : MonoBehaviour
    {
        [Tooltip("Movement types this tile supports")]
        public MovementType allowedMovementTypes;

        [Tooltip("How much this tile costs to walk on (higher is avoided more, lower is preferred)")]
        public int walkMovementPenalty;

        [Tooltip("How much this tile costs to fly over (higher is avoided more, lower is preferred)")]
        public int flyMovementPenalty;

        [Tooltip("How much this tile costs to burrow below (higher is avoided more, lower is preferred)")]
        public int burrowMovementPenalty;

        // the x and y location of this tile within the 2D array grid
        [HideInInspector] public Vector2Int gridLocation;
    }
}