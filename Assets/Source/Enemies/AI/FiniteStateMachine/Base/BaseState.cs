using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Represents a state in a finite state machine
    /// </summary>
    public class BaseState : ScriptableObject
    {
        /// <summary>
        /// Runs every frame while we are in this state
        /// </summary>
        /// <param name="machine"> The state machine to be used. </param>
        public virtual void OnStateUpdate(BaseStateMachine machine)
        {
        }

        /// <summary>
        /// Runs when this state is entered
        /// </summary>
        /// <param name="machine"> The state machine to be used. </param>
        public virtual void OnStateEnter(BaseStateMachine machine)
        {
        }

        /// <summary>
        /// Runs when this state is exited
        /// </summary>
        /// <param name="machine"> The state machine to be used. </param>
        public virtual void OnStateExit(BaseStateMachine machine)
        {
        }
    }
}