using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision in a Finite State Machine
    /// </summary>
    public abstract class BaseDecision : ScriptableObject
    {
        /// <summary>
        /// Decide this decision
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        public abstract bool Decide(BaseStateMachine stateMachine);
    }
}