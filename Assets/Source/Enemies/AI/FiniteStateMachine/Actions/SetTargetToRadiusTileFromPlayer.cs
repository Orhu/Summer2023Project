using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// Represents an action that sets target to the farthest tile in the room
/// </summary>
[CreateAssetMenu(menuName = "FSM/Actions/Set Target To Radius Tile From Player")]
public class SetTargetToRadiusTileFromPlayer : FSMAction
{
    [Tooltip("Radius from the player to set the target to")]
    [SerializeField] private int radius = 4;
    
    /// <summary>
    /// Starts the set farthest tile pos coroutine if cooldown is ready
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateUpdate(BaseStateMachine stateMachine)
    {
        if (stateMachine.cooldownData.cooldownReady[this])
        {
            stateMachine.cooldownData.cooldownReady[this] = false;
            stateMachine.StartCoroutine(SetFarthestTilePos(stateMachine));
        }
    }

    /// <summary>
    /// Adds to cooldown list in state machine if necessary
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateEnter(BaseStateMachine stateMachine)
    {
        // sometimes, because transitions can occur every frame, rapid transitions cause the key not to be deleted properly and error. this check prevents that error
        if (!stateMachine.cooldownData.cooldownReady.ContainsKey(this))
        {
            // if the key has not yet been added, add it with 0 cooldown
            stateMachine.cooldownData.cooldownReady.Add(this, true);
        }
        else
        {
            // in this case, there may be a cooldown still running from a previous state exit/re-entry so don't set the value at all
        }
    }

    /// <summary>
    /// Nothing to do here, required for FSMAction implementation
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateExit(BaseStateMachine stateMachine)
    {
    }

    /// <summary>
    /// Picks a random tile position that is within a certain number of tiles
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    /// <returns></returns>
    private IEnumerator SetFarthestTilePos(BaseStateMachine stateMachine)
    {
        var curRoom = FloorGenerator.floorGeneratorInstance.currentRoom;
        var playerTileResult = RoomInterface.instance.WorldPosToTile(stateMachine.player.transform.position, stateMachine.currentMovementType);

        if (!playerTileResult.Item2)
        {
            // if this bool is false, the tile was out of bounds of the grid
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }

        // TODO: Replace this algorithm with a more robust one in the future.
        Vector2Int roomMaxSize = curRoom.roomSize;
       PathfindingTile playerTile = playerTileResult.Item1;
        float greatestDistance = 0f;
        PathfindingTile tileWithGreatestDistance = null;

        for (int x = playerTile.gridLocation.x - radius; x <= playerTile.gridLocation.x + radius; x++)
        {
            for (int y = playerTile.gridLocation.y - radius; y <= playerTile.gridLocation.y + radius; y++)
            {
                if (x < 1 || x >= roomMaxSize.x - 1 || y < 1 || y >= roomMaxSize.y - 1)
                    continue; // Skip tiles outside the room bounds

                PathfindingTile tile;
                switch (stateMachine.currentMovementType)
                {
                    case RoomInterface.MovementType.Walk:
                        tile = RoomInterface.instance.walkRoomGrid[x, y];
                        break;
                    case RoomInterface.MovementType.Burrow:
                        tile = RoomInterface.instance.burrowRoomGrid[x, y];
                        break;
                    case RoomInterface.MovementType.Fly:
                        tile = RoomInterface.instance.flyRoomGrid[x, y];
                        break;
                    default:
                        Debug.LogError("Attempting to select a tile with invalid movement type!");
                        tile = null;
                        break;
                }
                if (!tile.moveable)
                    continue; // Skip non-walkable tiles

                float distance = Vector2.Distance(playerTile.gridLocation, tile.gridLocation);
                if (distance > greatestDistance)
                {
                    greatestDistance = distance;
                    tileWithGreatestDistance = tile;
                }
            }
        }

        if (tileWithGreatestDistance != null)
        {
            stateMachine.currentTarget = RoomInterface.instance.TileToWorldPos(tileWithGreatestDistance);
        }

        stateMachine.cooldownData.cooldownReady[this] = true;
        yield break;
    }
}
