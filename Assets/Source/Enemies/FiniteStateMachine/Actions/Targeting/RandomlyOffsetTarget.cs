using System;
using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action to update our target to be the player
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Targeting/Randomly Offset Target")]
    public class RandomlyOffsetTarget : SingleAction
    {
        [Tooltip("The radius of the circle to offset the target in.?")] [Min(0f)]
        [SerializeField] private float offsetRadius;

        [Tooltip("Target type to use. Should we set the pathfinding target, attack target, or both for this unit?")]
        [SerializeField] private TargetType targetType;

        /// <summary>
        /// Represents a targeting mode
        /// </summary>
        [Flags]
        private enum TargetType
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
            Vector2 offset = UnityEngine.Random.insideUnitCircle * offsetRadius;
            
            if (targetType.HasFlag(TargetType.Pathfinding))
            {
                Vector2 newPathfindingTarget = stateMachine.currentPathfindingTarget + offset;
                (PathfindingTile, bool) newPathfindingTargetTile = RoomInterface.instance.WorldPosToTile(newPathfindingTarget);
                
                // need to check that the new tile is valid
                if (newPathfindingTargetTile.Item2 && newPathfindingTargetTile.Item1.allowedMovementTypes.HasFlag(stateMachine.currentMovementType))
                {
                    stateMachine.currentPathfindingTarget = newPathfindingTarget;
                }
                else
                {
                    Debug.LogWarning(stateMachine.gameObject.name + ": Randomly offset tile was not a valid tile! No offset will be applied.");
                }
            }

            if (targetType.HasFlag(TargetType.Attack))
            {
                stateMachine.currentAttackTarget += offset;
            }

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}