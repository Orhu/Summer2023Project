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
        stateMachine.cooldownData.cooldownReady.Add(this, true);
        stateMachine.destinationReached = true;
        stateMachine.StartCoroutine(SetFarthestTilePos(stateMachine));
    }

    public override void OnStateExit(BaseStateMachine stateMachine)
    {
        stateMachine.cooldownData.cooldownReady.Remove(this);
    }

    private IEnumerator SetFarthestTilePos(BaseStateMachine stateMachine)
    {
        var curRoom = FloorGenerator.floorGeneratorInstance.currentRoom;
        var playerTileResult = RoomInterface.WorldPosToTileStatic(stateMachine.player.transform.position);

        if (!playerTileResult.Item2)
        {
            // if this bool is false, the tile was out of bounds of the grid
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }

        // TODO: Replace this algorithm with a more robust one in the future.
        Vector2Int roomMaxSize = curRoom.roomSize;
        Tile playerTile = playerTileResult.Item1;
        float greatestDistance = 0f;
        Tile tileWithGreatestDistance = null;

        for (int x = playerTile.gridLocation.x - 4; x <= playerTile.gridLocation.x + 4; x++)
        {
            for (int y = playerTile.gridLocation.y - 4; y <= playerTile.gridLocation.y + 4; y++)
            {
                if (x < 1 || x >= roomMaxSize.x - 1 || y < 1 || y >= roomMaxSize.y - 1)
                    continue; // Skip tiles outside the room bounds

                Tile tile = curRoom.roomGrid[x, y];
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
            stateMachine.currentTarget = RoomInterface.TileToWorldPos(tileWithGreatestDistance);
        }

        stateMachine.cooldownData.cooldownReady[this] = true;
        yield break;
    }
}
