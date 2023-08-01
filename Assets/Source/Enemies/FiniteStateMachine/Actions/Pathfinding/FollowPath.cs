using System.Collections;
using UnityEngine;
using ChaseData = Cardificer.FiniteStateMachine.BaseStateMachine.ChaseData;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that follows whatever path is currently stored by the state machine.
    /// When leaving this state, enemies should stop the stateMachine.pathData.prevFollowCoroutine coroutine,
    /// as well as enabling stateMachine.pathData.ignorePathRequests
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Pathfinding/Follow Path")]
    public class FollowPath : SingleAction
    {
        /// <summary>
        /// Starts chase action
        /// </summary>
        /// <param name="stateMachine"> stateMachine to be used </param>
        /// <returns> Doesn't wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            while (!stateMachine.destinationReached)
            {
                stateMachine.GetComponent<Movement>().movementInput = (stateMachine.currentPathfindingTarget - stateMachine.GetFeetPos()).normalized;
                yield return null;
            }
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}