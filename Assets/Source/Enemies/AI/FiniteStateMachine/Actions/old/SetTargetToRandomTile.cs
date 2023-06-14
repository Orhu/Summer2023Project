using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Animations;

// This code is not currently in use due to the enemy rework, but it may be useful for making a future action.
// Hence, it will remain here until I deem it delete-worthy or not.

/*
/// <summary>
/// Represents an action to set our target to a random tile
/// </summary>
[CreateAssetMenu(menuName = "FSM/Actions/Set Target To Random Tile")]
public class SetTargetToRandomTile : FSMAction
{
    [Tooltip("Condition to evaluate before picking new tile")]
    [SerializeField] private FSMDecision whenToPickNewTile;
    
    /// <summary>
    /// Sets random tile pos when condition is met
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateUpdate(BaseStateMachine stateMachine)
    {
        if (stateMachine.cooldownData.cooldownReady[this] && whenToPickNewTile.Decide(stateMachine))
        {
            stateMachine.cooldownData.cooldownReady[this] = false;
            stateMachine.StartCoroutine(SetRandomTilePos(stateMachine));
        }
    }

    /// <summary>
    /// Sets random tile pos
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateEnter(BaseStateMachine stateMachine)
    {
        stateMachine.cooldownData.cooldownReady.Add(this, true);
        stateMachine.destinationReached = true;
        RoomInterface.instance.GrabCurrentRoom();
        stateMachine.StartCoroutine(SetRandomTilePos(stateMachine));
    }

    /// <summary>
    /// Nothing to do here, required for FSMAction implementation
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateExit(BaseStateMachine stateMachine)
    {
        stateMachine.cooldownData.cooldownReady.Remove(this);
    }

    /// <summary>
    /// Sets the current target to a random tile in the room
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    /// <returns></returns>
    private IEnumerator SetRandomTilePos(BaseStateMachine stateMachine)
    {
        var curRoomSize = RoomInterface.instance.myRoomSize;
        var tileX = Random.Range(0, curRoomSize.x);
        var tileY = Random.Range(0, curRoomSize.y);
        var newTile = RoomInterface.instance.myRoomGrid[tileX, tileY];
        if (newTile != null)
        {
            if (newTile.walkable)
            {
                stateMachine.currentTarget = RoomInterface.instance.TileToWorldPos(RoomInterface.instance.myRoomGrid[tileX, tileY]);
                stateMachine.cooldownData.cooldownReady[this] = true;
                yield break;
            }
            else
            {
                stateMachine.cooldownData.cooldownReady[this] = true;
                yield break;
            }
        }

        yield break;
    }
}
*/