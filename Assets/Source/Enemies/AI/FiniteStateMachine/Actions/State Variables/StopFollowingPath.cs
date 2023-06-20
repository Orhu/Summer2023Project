
    using System.Collections;
    using UnityEngine;

    namespace Cardificer.FiniteStateMachine
    {
        /// <summary>
        /// Represents an action that disables the follow path coroutine
        /// </summary>
        [CreateAssetMenu(menuName = "FSM/Actions/State Variables/Stop Following Path")]
        public class StopFollowingPath : SingleAction
        {
            /// <summary>
            /// Stops the follow path coroutine 
            /// </summary>
            /// <param name="stateMachine"> The state machine to be used. </param>
            /// <returns> Ends when the action is complete. </returns>
            protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
            {
                stateMachine.StopCoroutine(stateMachine.pathData.prevFollowCoroutine);
                yield break;
            }
        }
    }
