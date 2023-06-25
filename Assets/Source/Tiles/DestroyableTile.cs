using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cardificer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cardificer
{
    /// <summary>
    /// Component that makes this GameObject change its PathfindingTile to "destroyed" when it is destroyed.
    /// A "destroyed" tile allows all movement types.
    /// </summary>
    public class DestroyableTile : MonoBehaviour
    {
        // Stores a shared list of destroyed tile positions
        public static HashSet<Vector2> destroyedTiles;

        /// <summary>
        /// Adds the room change listener and initializes destroyedTiles if needed
        /// </summary>
        void Start()
        {
            FloorGenerator.floorGeneratorInstance.onRoomChange.AddListener(OnRoomEnter);
            destroyedTiles ??=
                SaveManager.savedDestroyedTiles.ToHashSet(); // if destroyedTiles doesnt exist, load it from the save
        }

        /// <summary>
        /// If destroyedTiles is not initialized, initializes it based on save data.
        /// Then, if the tile is in the loaded list, destroy it.
        /// </summary>
        private void OnRoomEnter()
        {
            // if our pos is in the HashSet, destroy immediately
            if (destroyedTiles.Contains(transform.position))
            {
                // true because we are not already destroyed by something else
                InitiateDestruction(true);
            }
        }

        /// <summary>
        /// Grabs the tile at this GameObject's position, and allows walking on it.
        /// OnDisable is called before the GameObject is fully destroyed. We need its transform, so we use OnDisable.
        /// </summary>
        private void OnDestroy()
        {
            // if scene is loaded
            if (gameObject.scene.isLoaded)
            {
                // false because OnDestroy means something else destroyed us
                InitiateDestruction(false);
            }
        }

        /// <summary>
        /// Sets the tile this object is on to be movable by all movement types, adds it to the destroyed tiles static list, and optionally Destroys the object itself
        /// </summary>
        /// <param name="shouldDestroy"> If true, destroys the game object. If false, does not. Should only destroy game object if it is not being destroyed by some other source. </param>
        private void InitiateDestruction(bool shouldDestroy)
        {
            var myWorldPos = transform.position;
            (PathfindingTile, bool) grabbedTile = RoomInterface.instance.WorldPosToTile(myWorldPos);
            if (grabbedTile.Item2)
            {
                grabbedTile.Item1.allowedMovementTypes |=
                    RoomInterface.MovementType.Walk | RoomInterface.MovementType.Fly |
                    RoomInterface.MovementType.Burrow;
                destroyedTiles.Add(myWorldPos);
                if (shouldDestroy)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogWarning("Attempted to update grid on destroy, but Tile at " + myWorldPos + " does not exist.");
            }
        }
    }
}