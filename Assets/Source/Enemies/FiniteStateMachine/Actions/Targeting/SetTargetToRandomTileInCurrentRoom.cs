using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action to set target to a random tile in the current room
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Targeting/Set Target To Random in Current Room")]
    public class SetTargetToRandomTileInCurrentRoom : SingleAction
    {
        [Tooltip("Target type to use. Should we set the pathfinding target, attack target, or both for this unit?")]
        [SerializeField] private TargetType targetType;
        
        [Tooltip("Cooldown between target acquisitions")] [Min(0f)]
        [SerializeField] private float cooldown = 0f;

        /// <summary>
        /// Enum representing targeting modes for setting a target
        /// </summary>
        [Flags]
        enum TargetType
        {
            None = 0,
            Pathfinding = 1,
            Attack = 2,
        }

        /// <summary>
        /// Sets pathfinding and/or attack target depending on the requested targeting type
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            // Get Random Tile
            Vector2 tileLocation = Vector2.zero;
            Vector2Int roomSize = RoomInterface.instance.myRoomSize;
            bool tileNavicable = false;

            for (int i = 0; i < RoomInterface.instance.GetMaxRoomSize() && !tileNavicable; i++)
            {
                tileLocation = new Vector2(Mathf.RoundToInt(Random.Range(roomSize.x / -2f, roomSize.x / 2f)), Mathf.RoundToInt(Random.Range(roomSize.y / -2f, roomSize.y / 2f)));

                (PathfindingTile, bool) tileReturnData = RoomInterface.instance.WorldPosToTile(tileLocation + RoomInterface.instance.myWorldPosition);

                if (tileReturnData.Item2) // only assign if we get a valid tile
                {
                    tileNavicable = tileReturnData.Item1.allowedMovementTypes.HasFlag(stateMachine.currentMovementType);
                }
            }
            if (!tileNavicable)
            {
                Debug.LogError("No valid random tile was found.");
            }

            tileLocation += RoomInterface.instance.myWorldPosition;
            
            
            // Set Target
            if (targetType.HasFlag(TargetType.Pathfinding))
            {
                stateMachine.currentPathfindingTarget = tileLocation;
            }

            if (targetType.HasFlag(TargetType.Attack))
            {
                stateMachine.currentAttackTarget = tileLocation;
            }

            yield return new WaitForSeconds(cooldown);
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}