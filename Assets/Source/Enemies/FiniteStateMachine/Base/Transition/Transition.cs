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
        [SerializeField] protected BaseState trueState;

        [Tooltip("Conditions to evaluate")]
        [SerializeField] private List<Decision.Combinable> decisions;

        /// <summary>
        /// Evaluate this transition
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        public void Evaluate(BaseStateMachine stateMachine)
        {
            if (decisions.Decide(stateMachine))
            {
                stateMachine.timeSinceTransition = 0f;
                stateMachine.currentState.OnStateExit(stateMachine);
                stateMachine.currentState = trueState.GetState();
                stateMachine.currentState.OnStateEnter(stateMachine);
            }
        }
    }
}