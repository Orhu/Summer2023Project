using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Actions/Set Movement Input To Target With Condition")]
public class SetMovementInputToTargetWithCondition : FSMAction
{
    [Tooltip("Condition to evaluate before executing the action")]
    [SerializeField] private FSMDecision decision;
    public override void OnStateUpdate(BaseStateMachine stateMachine)
    {
        if (decision.Decide(stateMachine))
        {
            stateMachine.GetComponent<Controller>().MoveTowards(stateMachine.currentTarget);
        }
    }

    public override void OnStateEnter(BaseStateMachine stateMachine)
    {
    }

    public override void OnStateExit(BaseStateMachine stateMachine)
    {
    }
}
