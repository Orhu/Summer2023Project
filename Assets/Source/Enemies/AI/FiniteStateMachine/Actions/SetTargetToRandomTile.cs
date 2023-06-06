using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(menuName = "FSM/Actions/Set Target To Random Tile")]
public class SetTargetToRandomTile : FSMAction
{
    [SerializeField] private FSMDecision whenToPickNewTile;
    
    public override void OnStateUpdate(BaseStateMachine stateMachine)
    {
        if (stateMachine.cooldownData.cooldownReady[this] && whenToPickNewTile.Decide(stateMachine))
        {
            stateMachine.cooldownData.cooldownReady[this] = false;
            stateMachine.StartCoroutine(SetRandomTilePos(stateMachine));
        }
    }

    public override void OnStateEnter(BaseStateMachine stateMachine)
    {
        stateMachine.cooldownData.cooldownReady.Add(this, true);
        stateMachine.destinationReached = true;
        stateMachine.StartCoroutine(SetRandomTilePos(stateMachine));
    }

    public override void OnStateExit(BaseStateMachine stateMachine)
    {
        stateMachine.cooldownData.cooldownReady.Remove(this);
    }

    private IEnumerator SetRandomTilePos(BaseStateMachine stateMachine)
    {
        var curRoom = FloorGenerator.floorGeneratorInstance.currentRoom;
        var tileX = Random.Range(0, curRoom.roomSize.x);
        var tileY = Random.Range(0, curRoom.roomSize.y);
        var newTile = curRoom.roomGrid[tileX, tileY];
        if (newTile != null)
        {
            if (newTile.walkable)
            {
                stateMachine.currentTarget = RoomInterface.TileToWorldPos(curRoom.roomGrid[tileX, tileY],
                    curRoom.transform.position, curRoom.roomSize);
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
