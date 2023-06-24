using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cardificer;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Component that makes this GameObject change its PathfindingTile to "destroyed" when it is destroyed.
/// A "destroyed" tile allows all movement types.
/// </summary>
public class TileMoveableOnDestroy : MonoBehaviour
{
    [HideInInspector] public static HashSet<Vector2> destroyedTiles;

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
            GoodbyeCruelWorldAndDestroy();
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
            GoodbyeCruelWorld();
        }
    }

    void GoodbyeCruelWorld()
    {
        var myWorldPos = transform.position;
        (PathfindingTile, bool) grabbedTile = RoomInterface.instance.WorldPosToTile(myWorldPos);
        if (grabbedTile.Item2)
        {
            grabbedTile.Item1.allowedMovementTypes |= RoomInterface.MovementType.Walk | RoomInterface.MovementType.Fly |
                                                      RoomInterface.MovementType.Burrow;
            destroyedTiles.Add(myWorldPos);
        }
        else
        {
            Debug.LogWarning("Attempted to update grid on destroy, but Tile at " + myWorldPos + " does not exist.");
        }
    }
    
    void GoodbyeCruelWorldAndDestroy()
    {
        var myWorldPos = transform.position;
        (PathfindingTile, bool) grabbedTile = RoomInterface.instance.WorldPosToTile(myWorldPos);
        if (grabbedTile.Item2)
        {
            grabbedTile.Item1.allowedMovementTypes |= RoomInterface.MovementType.Walk | RoomInterface.MovementType.Fly |
                                                      RoomInterface.MovementType.Burrow;
            destroyedTiles.Add(myWorldPos);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Attempted to update grid on destroy, but Tile at " + myWorldPos + " does not exist.");
        }
    }
}