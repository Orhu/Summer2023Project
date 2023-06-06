﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Represents a transition between states in a finite state machine
/// </summary>
[CreateAssetMenu(menuName = "FSM/Transition/AND")]
public class FSMTransitionAND : ScriptableObject
{
    // the condition to evaluate
    public List<FSMDecision> decisions;

    // what state to enter on true
    public BaseState trueState;

    // what state to enter on false
    public BaseState falseState;

    /// <summary>
    /// Evaluate the decision, then change state depending on result
    /// </summary>
    /// <param name="machine"> The state machine to be used. </param>
    public void Execute(BaseStateMachine machine)
    {
        var result = true;

        foreach (var d in decisions)
        {
            result = result && d.Decide(machine);
        }

        // if the condition evaluates to true and true isn't just "remain in same state"
        if (result && trueState is not RemainInState)
        {
            machine.currentState.OnStateExit(machine);
            machine.currentState = trueState;
            machine.currentState.OnStateEnter(machine);
        }
        // if the condition evaluates to false and false isn't just "remain in same state"
        else if (!result && falseState is not RemainInState)
        {
            machine.currentState.OnStateExit(machine);
            machine.currentState = falseState;
            machine.currentState.OnStateEnter(machine);
        }
    }
}