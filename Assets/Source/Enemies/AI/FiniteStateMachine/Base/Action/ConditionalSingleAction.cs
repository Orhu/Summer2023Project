using System.Collections;
using Cardificer;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that executes only if a condition returns true
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Conditional Single Action")]
    public class ConditionalSingleAction : BaseAction
    {
        [Tooltip("Decision to evaluate.")]
        [SerializeField] private BaseDecision decision;

        [Tooltip("Action to perform if decision is true.")]
        [SerializeField] private BaseAction trueAction;

        [Tooltip("Action to perform if decision is false.")]
        [SerializeField] private BaseAction falseAction;

        /// <summary>
        /// Evaluate the condition and execute the action if the condition is true.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        public override void Execute(BaseStateMachine stateMachine)
        {
            if (decision.Decide(stateMachine))
            {
                trueAction.Execute(stateMachine);
            }
            else
            {
                falseAction.Execute(stateMachine);
            }
        }
    }
}