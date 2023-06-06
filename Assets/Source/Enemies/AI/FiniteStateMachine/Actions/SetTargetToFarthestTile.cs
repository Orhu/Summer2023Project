using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(menuName = "FSM/Actions/Set Target To Farthest Tile From Player")]
public class SetTargetToFarthestTile : FSMAction
{
    public override void OnStateUpdate(BaseStateMachine stateMachine)
    {
        if (stateMachine.cooldownData.cooldownReady[this])
        {
            stateMachine.cooldownData.cooldownReady[this] = false;
            stateMachine.StartCoroutine(SetFarthestTilePos(stateMachine));
        }
    }

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

    public override void OnStateExit(BaseStateMachine stateMachine)
    {
    }

    private IEnumerator SetFarthestTilePos(BaseStateMachine stateMachine)
    {
        var curRoom = FloorGenerator.floorGeneratorInstance.currentRoom;
        var playerTileResult = RoomInterface.instance.WorldPosToTile(stateMachine.player.transform.position);

        if (!playerTileResult.Item2)
        {
            // if this bool is false, the tile was out of bounds of the grid
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }

        // TODO: Replace this algorithm with a more robust one in the future.
        Vector2Int roomMaxSize = curRoom.roomSize;
        RoomInterface.PathfindingTile playerTile = playerTileResult.Item1;
        float greatestDistance = 0f;
        RoomInterface.PathfindingTile tileWithGreatestDistance = null;

        for (int x = playerTile.gridLocation.x - 4; x <= playerTile.gridLocation.x + 4; x++)
        {
            for (int y = playerTile.gridLocation.y - 4; y <= playerTile.gridLocation.y + 4; y++)
            {
                if (x < 1 || x >= roomMaxSize.x - 1 || y < 1 || y >= roomMaxSize.y - 1)
                    continue; // Skip tiles outside the room bounds

                RoomInterface.PathfindingTile tile = RoomInterface.instance.myRoomGrid[x, y];
                if (!tile.walkable)
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
