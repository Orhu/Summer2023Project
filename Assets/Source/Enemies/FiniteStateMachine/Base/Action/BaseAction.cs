using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action in a Finite State Machine
    /// </summary>
    public abstract class BaseAction : ScriptableObject
    {
        /// <summary>
        /// Run this action
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        public abstract void Execute(BaseStateMachine stateMachine); 
    }
}
