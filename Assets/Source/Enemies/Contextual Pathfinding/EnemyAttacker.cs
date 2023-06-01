using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skaillz.EditInline;
using UnityEngine.Events;

/// <summary>
/// Represents component to handle issuing enemy attacks
/// </summary>
public class EnemyAttacker : MonoBehaviour
{ 
    [Header("Attacking/Actions")]

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


    /// <summary>
    /// Performs an attack
    /// </summary>
    /// <param name="agent"> The agent performing the attack </param>
    public void PerformAttack(IActor agent)
    {
        beforeChargeUp?.Invoke();
        // wait actionChargeUpTime
        beforeAction?.Invoke();
        foreach (var action in actions)
        {
            action.Play(agent);
        }
        afterAction?.Invoke();
        // wait cooldown time
        afterCooldown?.Invoke();
    }
}
