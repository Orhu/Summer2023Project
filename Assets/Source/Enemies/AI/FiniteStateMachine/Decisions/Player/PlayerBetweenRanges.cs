using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Represents a decision checking whether our target is in a certain range
/// </summary>
[CreateAssetMenu(menuName = "FSM/Decisions/Player/Player Between Ranges")]
public class PlayerBetweenRanges : FSMDecision
{
    [Tooltip("What min range for our target?")]
    [SerializeField] private float minRange;

    [Tooltip("What max range for our target?")]
    [SerializeField] private float maxRange;

    /// <summary>
    /// Evaluates whether the current target is within the requested range
    /// </summary>
    /// <param name="state"> The stateMachine to use </param>
    /// <returns> True if the target is within the specified range from this unit, false otherwise </returns>
    public override bool Evaluate(BaseStateMachine state)
    {
        var dist = Vector2.Distance(Player.Get().transform.position, state.transform.position);
        return dist >= minRange && dist <= maxRange;
    }
}