using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/Destination Reached")]
public class DestinationReached : FSMDecision
{
    public override bool Decide(BaseStateMachine stateMachine)
    {
        return invert ? !stateMachine.destinationReached : stateMachine.destinationReached;
    }
}
