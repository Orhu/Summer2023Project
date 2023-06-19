using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action to update our target to be the player
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Set Target To Player")]
    public class SetTargetToPlayer : Action
    {
        /// <summary>
        /// Enum representing targeting modes for setting a target
        /// </summary>
        enum TargetType
        {
            Pathfinding,
            Attack,
            Both
        }

        [Tooltip("Target type to use. Should we set the pathfinding target, attack target, or both for this unit?")]
        [SerializeField] private TargetType targetType;

        /// <summary>
        /// Sets pathfinding and/or attack target depending on the requested targeting type
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns></returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            switch (targetType)
            {
                case TargetType.Both:
                    stateMachine.currentPathfindingTarget = Player.GetFeet().transform.position;
                    stateMachine.currentAttackTarget = Player.Get().transform.position;
                    break;
                case TargetType.Pathfinding:
                    stateMachine.currentPathfindingTarget = Player.GetFeet().transform.position;
                    break;
                case TargetType.Attack:
                    stateMachine.currentAttackTarget = Player.Get().transform.position;
                    break;
            }

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}