using UnityEngine;

/// <summary>
/// Represents an action in a finite state machine
/// </summary>
public abstract class FSMAction : ScriptableObject
{
    /// <summary>
    /// Run each frame while this action's state is the current state (eg if the action belongs to attack state, this method is run every frame while attack state is active)
    /// </summary>
    /// <param name="stateMachine"> The state machine to be used. </param>
    public abstract void OnStateUpdate(BaseStateMachine stateMachine);

    /// <summary>
    /// Run when this action's state is entered (eg if the action belongs to attack state, this method is run once when attack state is first started)
    /// </summary>
    /// <param name="stateMachine"> The state machine to be used. </param>
    public abstract void OnStateEnter(BaseStateMachine stateMachine);

    /// <summary>
    /// Run when this action's state is exited (eg if the action belongs to attack state, this method is run once when attack state is exited)
    /// </summary>
    /// <param name="stateMachine"> The state machine to be used. </param>
    public abstract void OnStateExit(BaseStateMachine stateMachine);
}