using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action to update our target to be the player
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Targeting/Set Target To Direction")]
    public class SetTargetToDirection : SingleAction
    {
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
        /// Enum representing a direction to aim in
        /// </summary>
        enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        [Tooltip("What direction should be aimed in?")]
        [SerializeField] private Direction direction;
        
        [Tooltip("Target type to use. Should we set the pathfinding target, attack target, or both for this unit?")]
        [SerializeField] private TargetType targetType;

        /// <summary>
        /// Sets pathfinding and/or attack target depending on the requested targeting type
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            if (targetType.HasFlag(TargetType.Pathfinding))
            {
                switch (direction)
                {
                    case Direction.Up:
                        stateMachine.currentPathfindingTarget = (Vector2)stateMachine.transform.position + Vector2.up;
                        break;
                    case Direction.Down:
                        stateMachine.currentPathfindingTarget = (Vector2)stateMachine.transform.position + Vector2.down;
                        break;
                    case Direction.Left:
                        stateMachine.currentPathfindingTarget = (Vector2)stateMachine.transform.position + Vector2.left;
                        break;
                    case Direction.Right:
                        stateMachine.currentPathfindingTarget = (Vector2)stateMachine.transform.position + Vector2.right;
                        break;
                }
            }

            if (targetType.HasFlag(TargetType.Attack))
            {
                switch (direction)
                {
                    case Direction.Up:
                        stateMachine.currentAttackTarget = (Vector2)stateMachine.transform.position + Vector2.up;
                        break;
                    case Direction.Down:
                        stateMachine.currentAttackTarget = (Vector2)stateMachine.transform.position + Vector2.down;
                        break;
                    case Direction.Left:
                        stateMachine.currentAttackTarget = (Vector2)stateMachine.transform.position + Vector2.left;
                        break;
                    case Direction.Right:
                        stateMachine.currentAttackTarget = (Vector2)stateMachine.transform.position + Vector2.right;
                        break;
                }
            }

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}