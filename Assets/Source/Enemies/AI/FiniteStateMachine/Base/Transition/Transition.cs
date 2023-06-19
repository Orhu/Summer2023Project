using UnityEngine;


namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a transition between states in a finite state machine
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Transitions/One Condition")]
    public class Transition : BaseTransition
    {
        [Tooltip("Decision to evaluate")]
        public BaseDecision decision;
        
        [Tooltip("State to enter if result is true")]
        public BaseState trueState;

        [Tooltip("State to enter if result is false")]
        public BaseState falseState;

        /// <summary>
        /// Evaluate the decision, then change state depending on result
        /// </summary>
        /// <param name="machine"> The state machine to be used. </param>
        public override void Execute(BaseStateMachine machine)
        {
            var result = decision.Decide(machine);

            // if the condition evaluates to true and true isn't just "remain in same state"
            if (result && trueState is not RemainInState)
            {
                machine.currentState.OnStateExit(machine);
                machine.currentState = trueState;
                machine.currentState.OnStateEnter(machine);
            }
            // if the condition evaluates to false and false isn't just "remain in same state"
            else if (!result && falseState is not RemainInState)
            {
                machine.currentState.OnStateExit(machine);
                machine.currentState = falseState;
                machine.currentState.OnStateEnter(machine);
            }
        }
    }
}