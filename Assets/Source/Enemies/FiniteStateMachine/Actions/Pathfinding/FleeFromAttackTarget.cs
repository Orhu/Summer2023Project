using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets the pathfinding target away from the current attack target.
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Pathfinding/Flee from Attack Target")]
    public class FleeFromAttackTarget : SingleAction
    {
        [Tooltip("The maximum number of tiles to flee at a time. Larger number means its better at getting out of dead ends.")] [Range(1,5)]
        [SerializeField] private int fleeTiles = 1;

        /// <summary>
        /// Sets the pathfinding target to the furthest tile within flee tiles of the state machine.
        /// </summary>
        /// <param name="stateMachine"> StateMachine to be used </param>
        /// <returns> Updates move input every frame until duration has elapsed. </returns>
        protected sealed override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.currentPathfindingTarget = (stateMachine.GetFeetPos() - stateMachine.currentAttackTarget).normalized * fleeTiles;

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}