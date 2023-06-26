using System.Collections.Generic;
using Skaillz.EditInline;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a transition between states in a finite state machine, where there are more than one condition and one must be true for the decision to be true
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Transitions/OR")]
    public class TransitionOr : BaseTransition
    {
        [Tooltip("Conditions to evaluate")] [EditInline]
        public List<BaseDecision> decisions;

        /// <summary>
        /// Evaluate the decision, then change state depending on result
        /// </summary>
        /// <param name="machine"> The state machine to be used. </param>
        protected override bool Execute(BaseStateMachine machine)
        {
            var result = false;

            foreach (var d in decisions)
            {
                result = result || d.Decide(machine);
            }

            return result;
        }
    }
}