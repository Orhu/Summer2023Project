using System.Collections.Generic;
using Skaillz.EditInline;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents the base class of a finite state machine transition. If false, the state machine remains in the same state
    /// </summary>
    [CreateAssetMenu(fileName = "NewTransition", menuName = "FSM/Transition")]
    public class Transition : ScriptableObject
    {
        [Tooltip("What state to enter on true")] [EditInline]
        [SerializeField] protected State trueState;

        [Tooltip("Conditions to evaluate")]
        [SerializeField] private List<Decision.Combinable> decisions;

        

        /// <summary>
        /// Evaluate this transition
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        public void Evaluate(BaseStateMachine stateMachine)
        {
            if (Execute(stateMachine))
            {
                // true and not inverted, change to true state
                StateTransition(stateMachine);
            }
        }

        /// <summary>
        /// Transitions to the true state
        /// </summary>
        /// <param name="stateMachine"> The state machine to use </param>
        private void StateTransition(BaseStateMachine stateMachine)
        {
            stateMachine.currentState.OnStateExit(stateMachine);
            stateMachine.currentState = trueState;
            stateMachine.timeSinceTransition = 0f;
            stateMachine.currentState.OnStateEnter(stateMachine);
        }

        /// <summary>
        /// Execute this transition
        /// </summary>
        /// <param name="machine"> The stateMachine to use </param>
        protected bool Execute(BaseStateMachine machine)
        {
            return decisions.Decide(machine);
        }
    }
}