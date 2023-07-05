using Skaillz.EditInline;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that executes only if a condition returns true
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Conditional Single Action")]
    public class ConditionalSingleAction : BaseAction
    {
        [Tooltip("Decision to evaluate.")] [EditInline]
        [SerializeField] private BaseDecision decision;

        [Tooltip("Invert decision result?")] 
        [SerializeField] private bool invert;

        [Tooltip("Action to perform if decision is true.")] [EditInline]
        [SerializeField] private BaseAction trueAction;

        /// <summary>
        /// Evaluate the condition and execute the action if the condition is true.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        public override void Execute(BaseStateMachine stateMachine)
        {
            if (invert != decision.Decide(stateMachine))
            {
                trueAction.Execute(stateMachine);
            }
        }
    }
}