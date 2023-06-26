using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Skaillz.EditInline;
using UnityEditorInternal;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents the base class of a finite state machine transition. If false, the state machine remains in the same state
    /// </summary>
    public abstract class BaseTransition : ScriptableObject
    {
        [Tooltip("Invert the result of decision evaluation?")]
        [SerializeField] private bool invert;
        
        [Tooltip("What state to enter on true")] [EditInline]
        [SerializeField] protected State trueState;
        
        /// <summary>
        /// Evaluate this transition
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        public void Evaluate(BaseStateMachine stateMachine)
        {
            if (Execute(stateMachine) != invert)
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
            stateMachine.currentState.OnStateEnter(stateMachine);
        }
        
        /// <summary>
        /// Execute this transition
        /// </summary>
        /// <param name="machine"> The stateMachine to use </param>
        protected abstract bool Execute(BaseStateMachine machine);
    }
}