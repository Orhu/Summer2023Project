using System.Collections;
using System.Runtime.InteropServices;
using Skaillz.EditInline;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that fires an attack
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Attacking/Perform Attack")]
    public class FireAttack : FSMAction
    {
        [Tooltip("After requesting an action, how long does it take for the action to be performed?")]
        public float actionChargeUpTime;

        [Tooltip("After the action is performed, what is the delay before the action can be performed again?")]
        public float actionCooldownTime;

        [Tooltip(
            "How many seconds to apply the Exhaust status effect. Note: Total cooldown of an attack is equal to exhaust duration + cooldown duration")]
        public float exhaustDuration;

        [Tooltip("The actions that will be taken when the enemy attempts to issue an action.")] [EditInline]
        public Action[] actions;

        // [Tooltip("Before the action charge up is started, do you want to do anything else?")]
        //  public UnityEvent<BaseStateMachine> beforeChargeUp;

        //  [Tooltip("Before the action is performed, do you want to do anything else?")]
        //  public UnityEvent<BaseStateMachine> beforeAction;

        //  [Tooltip("Immediately after the action is performed, do you want to do anything else?")]
        // public UnityEvent<BaseStateMachine> afterAction;

        // [Tooltip("After the cooldown ends, do you want to do anything else?")]
        // public UnityEvent<BaseStateMachine> afterCooldown;

        public override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            if (stateMachine.canAct)
            {
                stateMachine.StartCoroutine(PerformAttack(stateMachine));
            }
            else
            {
                stateMachine.cooldownData.cooldownReady[this] = true;
            }

            yield break;
        }

        /// <summary>
        /// Performs an attack
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack </param>
        IEnumerator PerformAttack(BaseStateMachine stateMachine)
        {
            // beforeChargeUp?.Invoke(stateMachine);
            yield return new WaitForSeconds(actionChargeUpTime);
            // beforeAction?.Invoke(stateMachine);
            foreach (var action in actions)
            {
                action.Play(stateMachine, FloorGenerator.floorGeneratorInstance.currentRoom.livingEnemies);
            }

            // afterAction?.Invoke(stateMachine);
            // var exhaustableComponent = stateMachine.GetComponent<Exhaustable>();
            // if (exhaustableComponent != null)
            // {
            //     exhaustableComponent.ExhaustMe(exhaustDuration);
            // }

            yield return new WaitForSeconds(actionCooldownTime);
            // afterCooldown?.Invoke(stateMachine);
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}