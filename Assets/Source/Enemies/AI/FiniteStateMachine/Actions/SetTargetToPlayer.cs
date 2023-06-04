using UnityEngine;

/// <summary>
/// Represents an action to update our target to be the player
/// </summary>
[CreateAssetMenu(menuName = "FSM/Actions/Set Target To Player")]
public class SetTargetToPlayer : FSMAction
{
    /// <summary>
    /// Nothing to do here, required for FSMAction implementation
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateUpdate(BaseStateMachine stateMachine)
    {
    }

    /// <summary>
    /// Updates the currentTarget to be the player
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateEnter(BaseStateMachine stateMachine)
    {
        stateMachine.currentTarget = stateMachine.player;
    }

    /// <summary>
    /// Nothing to do here,required for FSMAction implementation
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateExit(BaseStateMachine stateMachine)
    {
    }
}