using System.Collections.Generic;
using Skaillz.EditInline;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that executes only if one of several conditions return true
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Conditional OR Action")]
    public class ConditionalOrAction : BaseAction
    {
        [Tooltip("Decisions to evaluate with OR condition.")] [EditInline]
        [SerializeField] private List<BaseDecision> decisions;

        [Tooltip("Action to perform if decision is true.")] [EditInline]
        [SerializeField] private BaseAction trueAction;

        /// <summary>
        /// Evaluate the condition and execute the action if one of the listed conditions is true.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        public override void Execute(BaseStateMachine stateMachine)
        {
            var result = false;
            foreach (var decision in decisions)
            {
                result = result || decision.Decide(stateMachine);
            }

            if (result)
            {
                trueAction.Execute(stateMachine);
            }
        }
    }
}