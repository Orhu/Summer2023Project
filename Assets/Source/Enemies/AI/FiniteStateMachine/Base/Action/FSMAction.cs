using UnityEngine;

/// <summary>
/// Represents an action in a finite state machine
/// </summary>
public abstract class FSMAction : ScriptableObject
{
    /// <summary>
    /// Execute this action
    /// </summary>
    /// <param name="stateMachine"> The state machine to be used. </param>
    public abstract void Execute(BaseStateMachine stateMachine);
}