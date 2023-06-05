using System.Collections;
using System.Runtime.InteropServices;
using Skaillz.EditInline;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents an action that fires an attack
/// </summary>
[CreateAssetMenu(menuName = "FSM/Actions/Perform Attack")]
public class FireAttack : FSMAction
{
    [Tooltip("After requesting an action, how long does it take for the action to be performed?")]
    public float actionChargeUpTime;

    [Tooltip("After the action is performed, what is the delay before the action can be performed again?")]
    public float actionCooldownTime;
    
    [Tooltip("The actions that will be taken when the enemy attempts to issue an action.")] [EditInline]
    public Action[] actions;

    [Tooltip("Before the action charge up is started, do you want to do anything else?")]
    public UnityEvent beforeChargeUp;
    
    [Tooltip("Before the action is performed, do you want to do anything else?")]
    public UnityEvent beforeAction;
    
    [Tooltip("Immediately after the action is performed, do you want to do anything else?")]
    public UnityEvent afterAction;
    
    [Tooltip("After the cooldown ends, do you want to do anything else?")]
    public UnityEvent afterCooldown;
    
    // determines if attack is available or not
    private bool attackReady;
    
    /// <summary>
    /// Launches an attack whenever the cooldown is available
    /// </summary>
    /// <param name="stateMachine"> The stateMachine performing the attack </param>
    public override void OnStateUpdate(BaseStateMachine stateMachine)
    {
        if (attackReady)
        {
            attackReady = false;
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
        attackReady = true;
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
        beforeChargeUp?.Invoke();
        yield return new WaitForSeconds(actionChargeUpTime);
        beforeAction?.Invoke();
        foreach (var action in actions)
        {
           action.Play(stateMachine.GetComponent<Controller>(), FloorGenerator.floorGeneratorInstance.currentRoom.livingEnemies);
        }
        afterAction?.Invoke();
        yield return new WaitForSeconds(actionCooldownTime);
        afterCooldown?.Invoke();
        attackReady = true;
    }
}
