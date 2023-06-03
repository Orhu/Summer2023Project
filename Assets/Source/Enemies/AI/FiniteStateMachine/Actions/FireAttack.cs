using System.Collections;
using System.Runtime.InteropServices;
using Skaillz.EditInline;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "FSM/Actions/Perform Attack")]
public class FireAttack : FSMAction
{
    [Tooltip("Can this enemy launch an attack?")]
    [SerializeField] private bool canAttack;

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
    
    public override void Execute(BaseStateMachine stateMachine)
    {
        if (canAttack)
        {
            var coroutine = PerformAttack(stateMachine);
            stateMachine.StartCoroutine(coroutine);
        }
    }
    
    /// <summary>
    /// Performs an attack
    /// </summary>
    /// <param name="stateMachine"> The stateMachine performing the attack </param>
    public IEnumerator PerformAttack(BaseStateMachine stateMachine)
    {
        canAttack = false;
        beforeChargeUp?.Invoke();
        yield return new WaitForSeconds(actionChargeUpTime);
        beforeAction?.Invoke();
        foreach (var action in actions)
        {
            // TODO when merged with Mabel's branch use this commented out code
           // action.Play(stateMachine.GetComponent<Controller>(), FloorGenerator.instance.ListOfEnemies);
            action.Play(stateMachine.GetComponent<Controller>());
        }
        afterAction?.Invoke();
        yield return new WaitForSeconds(actionCooldownTime);
        afterCooldown?.Invoke();
        canAttack = true;
    }
}
