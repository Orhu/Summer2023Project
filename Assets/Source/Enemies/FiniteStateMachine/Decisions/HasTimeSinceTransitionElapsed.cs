using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking whether an amount of time has passed in the current state
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Decisions/Has Time Since Transition Elapsed")]
    public class HasTimeSinceTransitionElapsed : Decision
    {
        [Tooltip("The time to wait until this returns true")]
        [SerializeField] private float time;

        /// <summary>
        /// Returns true if the time has passed since this state was entered.
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> timeSinceTransition >= time </returns>
        protected override bool Evaluate(BaseStateMachine stateMachine)
        {
            return stateMachine.timeSinceTransition >= time;
        }
    }
}