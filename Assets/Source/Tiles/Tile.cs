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

        [Tooltip("the x and y location of this tile within the 2D array grid")]
        public Vector2Int gridLocation;

        // The room that this tile is a part of 
        public Room room;

        /// <summary>
        /// Disables the game object from starting before it's ready to
        /// </summary>
        private void Start()
        {
            // Disable all the components except this and the sprite rendere
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