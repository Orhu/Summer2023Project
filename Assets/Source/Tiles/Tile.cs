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
        [HideInInspector] public Vector2Int gridLocation;

        [Tooltip("Whether or not this tile should disable it's components on start")]
        public bool shouldDisable = true;

        // The room that this tile is a part of 
        [HideInInspector] public Room room;

        /// <summary>
        /// Disables the game object from starting before it's ready to
        /// </summary>
        private void Start()
        {
            if (!shouldDisable) { return; }

            // Disable all the components except this and the sprite renderer
            foreach (MonoBehaviour component in gameObject.GetComponents<MonoBehaviour>())
            {
                component.enabled = false;
            }

            enabled = true;

            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }
        }

        /// <summary>
        /// Enables the game object
        /// </summary>
        public void Enable()
        {
            foreach (MonoBehaviour component in gameObject.GetComponents<MonoBehaviour>())
            {
                component.enabled = true;
            }
        }
    }
}