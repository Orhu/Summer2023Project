using System.Collections;
using UnityEngine;
using ChaseData = Cardificer.FiniteStateMachine.BaseStateMachine.ChaseData;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// An action that causes this to immediately teleport to the pathfinding target position
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Pathfinding/Teleport to Pathfinding Target")]
    public class TeleportToTarget : SingleAction
    {
        /// <summary>
        /// Starts chase action
        /// </summary>
        /// <param name="stateMachine"> stateMachine to be used </param>
        /// <returns> Doesn't wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.transform.position = stateMachine.currentPathfindingTarget;
            
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}