using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an action that sets movement input to a target when a condition is met
/// </summary>
[CreateAssetMenu(menuName = "FSM/Actions/Set Movement Input To Target With Condition")]
public class SetMovementInputToTargetWithCondition : FSMAction
{
    [Tooltip("Condition to evaluate before executing the action")]
    [SerializeField] private FSMDecision decision;
    
    /// <summary>
    /// Move towards the current target when the condition is met
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateUpdate(BaseStateMachine stateMachine)
    {
        if (decision.Decide(stateMachine))
        {
            stateMachine.GetComponent<Controller>().MoveTowards(stateMachine.currentTarget);
        }
    }

    /// <summary>
    /// Does nothing, required for FSMAction implementation
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateEnter(BaseStateMachine stateMachine)
    {
    }

    /// <summary>
    /// Does nothing, required for FSMAction implementation
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateExit(BaseStateMachine stateMachine)
    {
    }
}
