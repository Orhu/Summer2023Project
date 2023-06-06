using System.Collections;
using System.Runtime.InteropServices;
using Skaillz.EditInline;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents an action that fires an attack
/// </summary>
[CreateAssetMenu(menuName = "FSM/Actions/Perform Attack With Condition")]
public class FireAttackWithCondition : FSMAction
{
    [Tooltip("What decision must be true before the attack will be fired?")]
    public FSMDecision decision;
    
    [Tooltip("After requesting an action, how long does it take for the action to be performed?")]
    public float actionChargeUpTime;

    [Tooltip("After the action is performed, what is the delay before the action can be performed again?")]
    public float actionCooldownTime;

    [Tooltip("The actions that will be taken when the enemy attempts to issue an action.")] [EditInline]
    public Action[] actions;

    [Tooltip("Before the action charge up is started, do you want to do anything else?")]
    public UnityEvent<BaseStateMachine> beforeChargeUp;

    [Tooltip("Before the action is performed, do you want to do anything else?")]
    public UnityEvent<BaseStateMachine> beforeAction;

    [Tooltip("Immediately after the action is performed, do you want to do anything else?")]
    public UnityEvent<BaseStateMachine> afterAction;

    [Tooltip("After the cooldown ends, do you want to do anything else?")]
    public UnityEvent<BaseStateMachine> afterCooldown;

    /// <summary>
    /// Launches an attack whenever the cooldown is available
    /// </summary>
    /// <param name="stateMachine"> The stateMachine performing the attack </param>
    public override void OnStateUpdate(BaseStateMachine stateMachine)
    {
        if (stateMachine.cooldownData.cooldownReady[this] && decision.Decide(stateMachine))
        {
            stateMachine.cooldownData.cooldownReady[this] = false;
            var coroutine = PerformAttack(stateMachine);
            stateMachine.StartCoroutine(coroutine);
        }
    }

    /// <summary>
    /// Sets the attack cooldown to true 
    /// </summary>
    /// <param name="stateMachine"> The stateMachine performing the attack </param>
    public override void OnStateEnter(BaseStateMachine stateMachine)
    {
        // sometimes, because transitions can occur every frame, rapid transitions cause the key not to be deleted properly and error. this check prevents that error
        if (!stateMachine.cooldownData.cooldownReady.ContainsKey(this))
        {
            // if the key has not yet been added, add it with 0 cooldown
            stateMachine.cooldownData.cooldownReady.Add(this, true);
        }
        else
        {
            // in this case, there may be a cooldown still running from a previous state exit/re-entry so don't set the value at all
        }
    }

    /// <summary>
    /// Not needed for this action, but demanded due to FSMAction inheritance
    /// </summary>
    /// <param name="stateMachine"> The stateMachine performing the attack </param>
    public override void OnStateExit(BaseStateMachine stateMachine)
    {
    }

    /// <summary>
    /// Performs an attack
    /// </summary>
    /// <param name="stateMachine"> The stateMachine performing the attack </param>
    IEnumerator PerformAttack(BaseStateMachine stateMachine)
    {
        beforeChargeUp?.Invoke(stateMachine);
        yield return new WaitForSeconds(actionChargeUpTime);
        beforeAction?.Invoke(stateMachine);
        foreach (var action in actions)
        {
            BaseStateMachine.print(name + ": Firing!");
            action.Play(stateMachine.GetComponent<Controller>(), FloorGenerator.floorGeneratorInstance.currentRoom.livingEnemies);
        }

        afterAction?.Invoke(stateMachine);
        yield return new WaitForSeconds(actionCooldownTime);
        afterCooldown?.Invoke(stateMachine);
        stateMachine.cooldownData.cooldownReady[this] = true;
    }
}