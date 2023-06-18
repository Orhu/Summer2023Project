using UnityEngine;

namespace Cardificer.FiniteStateMachine
{

    /// <summary>
    /// Represents a condition to evaluate whether a transition should be true or false
    /// </summary>
    public abstract class FSMDecision : ScriptableObject
    {
        [Tooltip("Indicates whether to invert the result of this decision as the final result")]
        [SerializeField] protected bool invert;

        /// <summary>
        /// Given a state machine, returns whether the transition should be true or false
        /// </summary>
        /// <param name="state"> The state machine to use. </param>
        /// <returns> True if the expression evaluates to true, false otherwise </returns>
        public bool Decide(BaseStateMachine state)
        {
            return invert ? !Evaluate(state) : Evaluate(state);
        }

        public abstract bool Evaluate(BaseStateMachine state);
    }
}