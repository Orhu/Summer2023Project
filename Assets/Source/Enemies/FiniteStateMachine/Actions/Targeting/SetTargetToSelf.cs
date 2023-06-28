using System;
using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action to update our target to be at the enemy's current position.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Targeting/Set Target To Self")]
    public class SetTargetToSelf : SingleAction
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
                stateMachine.currentPathfindingTarget = stateMachine.GetFeetPos();
            }

            if (targetType.HasFlag(TargetType.Attack))
            {
                stateMachine.currentAttackTarget = stateMachine.GetFeetPos();
            }

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}