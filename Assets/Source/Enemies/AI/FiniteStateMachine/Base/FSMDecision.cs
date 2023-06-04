
    using UnityEngine;

    /// <summary>
    /// Represents a condition to evaluate whether a transition should be true or false
    /// </summary>
    public abstract class FSMDecision : ScriptableObject
    {
        /// <summary>
        /// Given a state machine, returns whether the transition should be true or false
        /// </summary>
        /// <param name="state"> The state machine to use. </param>
        /// <returns> True if the expression evaluates to true, false otherwise </returns>
        public abstract bool Decide(BaseStateMachine state);
    }
