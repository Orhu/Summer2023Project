
    using System.Collections;
    using UnityEngine;

    namespace Cardificer.FiniteStateMachine
    {
        /// <summary>
        /// Sets the ignore path requests variable to serialized bool
        /// </summary>
        [CreateAssetMenu(menuName = "FSM/Actions/State Variables/Ignore Path Requests")]
        public class IgnorePathRequests : Action
        {
            [Tooltip("The value to set ignorePathRequests on this enemy to")]
            [SerializeField] private bool newIgnorePathRequests;

            /// <summary>
            /// Updates the ignorePathRequests variable
            /// </summary>
            /// <param name="stateMachine"> The state machine to be used. </param>
            /// <returns></returns>
            protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
            {
                stateMachine.pathData.ignorePathRequests = newIgnorePathRequests;
                yield break;
            }
        }
    }