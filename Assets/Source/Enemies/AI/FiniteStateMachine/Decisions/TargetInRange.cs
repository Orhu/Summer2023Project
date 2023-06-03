using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/Target In Range")]
public class TargetInRange : FSMDecision
{
    [Tooltip("What range to check?")]
    [SerializeField] private float range;
    public override bool Decide(BaseStateMachine state)
    {
        return (Vector2.Distance(state.currentTarget.transform.position, state.transform.position) <= range);
    }
}
