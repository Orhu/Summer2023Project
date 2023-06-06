using System.Collections;
using UnityEngine;

/// <summary>
/// Represents an action to update our target to be the player when the state is exited
/// </summary>
[CreateAssetMenu(menuName = "FSM/Actions/Set Target To Player On Exit")]
public class SetTargetToPlayerOnExit : FSMAction
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
    }

    /// <summary>
    /// Nothing to do here,required for FSMAction implementation
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateExit(BaseStateMachine stateMachine)
    {
        SetPlayerTarget(stateMachine);
    }

    /// <summary>
    /// Set the current target to be the player
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    void SetPlayerTarget(BaseStateMachine stateMachine)
    {
        stateMachine.currentTarget = stateMachine.player.transform.position;
        stateMachine.cooldownData.cooldownReady[this] = true;
    }
}