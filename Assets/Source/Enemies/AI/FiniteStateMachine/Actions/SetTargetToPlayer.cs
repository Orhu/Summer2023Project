using System.Collections;
using UnityEngine;

/// <summary>
/// Represents an action to update our target to be the player
/// </summary>
[CreateAssetMenu(menuName = "FSM/Actions/Set Target To Player")]
public class SetTargetToPlayer : FSMAction
{
    /// <summary>
    /// Updates the currentTarget to be the player when cooldown is ready
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateUpdate(BaseStateMachine stateMachine)
    {
        if (stateMachine.cooldownData.cooldownReady[this])
        {
            stateMachine.cooldownData.cooldownReady[this] = false;
            stateMachine.StartCoroutine(SetPlayerTarget(stateMachine));
        }
    }

    /// <summary>
    /// Updates the currentTarget to be the player
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateEnter(BaseStateMachine stateMachine)
    {
        stateMachine.cooldownData.cooldownReady.Add(this, true);
        stateMachine.StartCoroutine(SetPlayerTarget(stateMachine));
    }

    /// <summary>
    /// Nothing to do here,required for FSMAction implementation
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    public override void OnStateExit(BaseStateMachine stateMachine)
    {
        stateMachine.cooldownData.cooldownReady.Remove(this);
    }

    /// <summary>
    /// Sets the current target to the player
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to use </param>
    /// <returns></returns>
    IEnumerator SetPlayerTarget(BaseStateMachine stateMachine)
    {
        stateMachine.currentTarget = stateMachine.player.transform.position;
        stateMachine.cooldownData.cooldownReady[this] = true;
        yield break;
    }
}