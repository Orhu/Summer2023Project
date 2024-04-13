
    using System.Collections;
    using UnityEngine;

    namespace Cardificer.FiniteStateMachine
    {
        /// <summary>
        /// Represents an action that disables following the current path and enables ignoring any new pathfinding requests
        /// </summary>
        [CreateAssetMenu(menuName = "FSM/Actions/State Variables/Disable Pathfinding")]
        public class DisablePathfinding : SingleAction
        {
            /// <summary>
            /// Stops the follow path coroutine 
            /// </summary>
            /// <param name="stateMachine"> The state machine to be used. </param>
            /// <returns> Ends when the action is complete. </returns>
            protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
            {
                if (stateMachine.pathData.prevFollowCoroutine != null)
                {
                    stateMachine.StopCoroutine(stateMachine.pathData.prevFollowCoroutine);
                }
                stateMachine.pathData.keepFollowingPath = false;
                stateMachine.pathData.ignorePathRequests = true;
                stateMachine.GetComponent<SimpleMovement>().movementInput = Vector2.zero;
                stateMachine.cooldownData.cooldownReady[this] = true;
                yield break;
            }
        }
    }
