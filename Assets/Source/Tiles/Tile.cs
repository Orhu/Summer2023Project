using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MovementType = Cardificer.RoomInterface.MovementType;

namespace Cardificer
{
    /// <summary>
    /// Tiles for use in the room grid. Holds information for pathfinding and spawning the tiles
    /// </summary>
    [System.Serializable] [ExecuteAlways]
    public class Tile : MonoBehaviour
    {
        [Tooltip("Movement types this tile supports")]
        public MovementType allowedMovementTypes = MovementType.Burrowing | MovementType.Walking | MovementType.Flying;

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