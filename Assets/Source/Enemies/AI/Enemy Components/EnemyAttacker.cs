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

    private bool canAttack = true;
    
    public static List<GameObject> enemies = new List<GameObject>();

    void Awake()
    {
       enemies.Add(gameObject);
    }

    /// <summary>
    /// Performs an attack
    /// </summary>
    /// <param name="agent"> The agent performing the attack </param>
    public IEnumerator PerformAttack(IActor agent)
    {
        if (!canAttack) yield break;

        canAttack = false;
        beforeChargeUp?.Invoke();
        yield return new WaitForSeconds(actionChargeUpTime);
        beforeAction?.Invoke();
        foreach (var action in actions)
        {
            action.Play(agent, enemies);
        }

        afterAction?.Invoke();
        yield return new WaitForSeconds(actionCooldownTime);
        afterCooldown?.Invoke();
        canAttack = true;
    }

    void OnDestroy()
    {
        enemies.Remove(gameObject);
    }
}
