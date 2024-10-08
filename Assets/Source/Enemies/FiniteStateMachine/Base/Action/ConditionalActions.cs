﻿using System.Collections.Generic;
using Skaillz.EditInline;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that executes only if several conditions return true
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Conditional Action")]
    public class ConditionalActions : BaseAction
    {
        [Tooltip("Decisions to evaluate with.")]
        [SerializeField] private List<Decision.Combinable> decisions;

        [Tooltip("Action to perform if decision is true.")] [EditInline]
        [SerializeField] private BaseAction trueAction;

        /// <summary>
        /// Evaluate the condition and execute the action if all listed conditions are true.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        public override void Execute(BaseStateMachine stateMachine)
        {
            if (decisions.Decide(stateMachine))
            {
                trueAction.Execute(stateMachine);
            }
        }
    }
}