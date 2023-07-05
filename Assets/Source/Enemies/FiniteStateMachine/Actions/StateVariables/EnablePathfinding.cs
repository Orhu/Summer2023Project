
    using System.Collections;
    using UnityEngine;

    namespace Cardificer.FiniteStateMachine
    {
        /// <summary>
        /// Represents an action that enables following the current path and disables ignoring any new pathfinding requests
        /// </summary>
        [CreateAssetMenu(menuName = "FSM/Actions/State Variables/Enable Pathfinding")]
        public class EnablePathfinding : SingleAction
        {
            /// <summary>
            /// Updates the ignorePathRequests variable
            /// </summary>
            /// <param name="stateMachine"> The state machine to be used. </param>
            /// <returns> Ends when the action is complete. </returns>
            protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
            {
                stateMachine.pathData.keepFollowingPath = true;
                stateMachine.pathData.ignorePathRequests = false;
                stateMachine.cooldownData.cooldownReady[this] = true;
                yield break;
            }
        }
    }