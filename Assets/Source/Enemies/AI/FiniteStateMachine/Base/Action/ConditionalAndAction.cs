using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an action that executes only if several conditions return true
/// </summary>
[CreateAssetMenu(menuName = "FSM/Actions/Conditional AND Action")]
public class ConditionalAndAction : FSMAction
{
    [Tooltip("Decisions to evaluate with AND condition.")]
    [SerializeField] private List<FSMDecision> decisions;

    [Tooltip("Action to perform if decision is true.")]
    [SerializeField] private FSMAction trueAction;

    [Tooltip("Action to perform if decision is false.")]
    [SerializeField] private FSMAction falseAction;

    /// <summary>
    /// Evaluate the condition and execute the action if all listed conditions are true.
    /// </summary>
    /// <param name="stateMachine"> The state machine to be used. </param>
    public override void Execute(BaseStateMachine stateMachine)
    {
        var result = true;
        foreach (var decision in decisions)
        {
            result = result && decision.Decide(stateMachine);
        }

        if (result)
        {
            trueAction.Execute(stateMachine);
        }
        else
        {
            falseAction.Execute(stateMachine);
        }
    }
}