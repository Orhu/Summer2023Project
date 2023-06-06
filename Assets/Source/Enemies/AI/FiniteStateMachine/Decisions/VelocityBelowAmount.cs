using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/Velocity Below Amount")]
public class VelocityBelowAmount : FSMDecision
{
    [SerializeField] private float velocityThreshold;
    
    public override bool Decide(BaseStateMachine stateMachine)
    {
        return invert
            ? !(stateMachine.GetComponent<Rigidbody2D>().velocity.magnitude <= velocityThreshold)
            : stateMachine.GetComponent<Rigidbody2D>().velocity.magnitude <= velocityThreshold;
    }
}
