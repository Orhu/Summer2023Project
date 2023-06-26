using Skaillz.EditInline;
using UnityEngine;


namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a transition between states in a finite state machine
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Transitions/One Condition")]
    public class Transition : BaseTransition
    {
        [Tooltip("Decision to evaluate")] [EditInline]
        public BaseDecision decision;

        /// <summary>
        /// Evaluate the decision, then change state depending on result
        /// </summary>
        /// <param name="machine"> The state machine to be used. </param>
        protected override bool Execute(BaseStateMachine machine)
        {
            return decision.Decide(machine);
        }
    }
}