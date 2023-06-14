using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a state in a finite state machine. Performs actions and transitions as needed in this state.
/// </summary>
[CreateAssetMenu(menuName = "FSM/State")]
public sealed class State : BaseState
{
    [Tooltip("Actions run when the state is entered.")]
    public List<FSMAction> enterActions = new List<FSMAction>();
    
    [Tooltip("Actions run when the state is exited.")]
    public List<FSMAction> exitActions = new List<FSMAction>();
    
    [Tooltip("Actions run every frame when in this state.")]
    public List<FSMAction> updateActions = new List<FSMAction>();
    
    [Tooltip("This state's transitions. Evaluated every frame.")]
    public List<BaseFSMTransition> transitions = new List<BaseFSMTransition>();

    /// <summary>
    /// Run through all of this state's actions and transitions, executing them.
    /// </summary>
    /// <param name="machine"> The state machine to be used. </param>
    public override void OnStateUpdate(BaseStateMachine machine)
    {
        foreach (var action in updateActions)
        {
            action.Execute(machine);
        }

        foreach (var transition in transitions)
        {
            transition.Execute(machine);
        }
    }

    /// <summary>
    /// Runs through all of this state's actions, executing their OnStateEnter methods.
    /// </summary>
    /// <param name="stateMachine"> The state machine to be used. </param>
    public override void OnStateEnter(BaseStateMachine stateMachine)
    {
        foreach (var action in enterActions)
        {
            stateMachine.cooldownData.cooldownReady.TryAdd(action, true);
            action.Execute(stateMachine);
        }
    }

    /// <summary>
    /// Runs through all of this state's actions, executing their OnStateExit methods.
    /// </summary>
    /// <param name="stateMachine"> The state machine to be used. </param>
    public override void OnStateExit(BaseStateMachine stateMachine)
    {
        foreach (var action in exitActions)
        {
            action.Execute(stateMachine);
        }
    }
}