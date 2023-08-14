using System;
using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action to update our target to be at position of the variable (works with vector & gameObject vars).
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Targeting/Set Target To Tracked Variable")]
    public class SetTargetToTrackedVariable : SingleAction
    {
        [Tooltip("The variable to get the location of.")]
        [SerializeField] string variableName;

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
            if (stateMachine.trackedVariables.TryGetValue(variableName, out object value))
            {
                if (value is GameObject gameObject)
                {
                    SetTarget(gameObject.transform.position);
                }
                else if (value is Vector2 vector)
                {
                    SetTarget(vector);
                }
                else if (value != null)
                {
                    Debug.LogError($"Tried to set target to {variableName} but it is not a game object or vector 2 on {stateMachine}.");
                }
            }
            else
            {
                Debug.LogError($"Tried to set target to {variableName} which does not exist on {stateMachine}.");
            }

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;

            // Sets the appropriate state machine targets to the given value.
            void SetTarget(Vector2 value)
            {
                if (targetType.HasFlag(TargetType.Pathfinding))
                {
                    stateMachine.currentPathfindingTarget = value;
                }

                if (targetType.HasFlag(TargetType.Attack))
                {
                    stateMachine.currentAttackTarget = value;
                }
            }
        }
    }
}