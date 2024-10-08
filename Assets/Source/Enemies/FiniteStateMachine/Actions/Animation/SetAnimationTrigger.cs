﻿using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets the animation state variables.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Animation/Set Animation Trigger")]
    public class SetAnimationTrigger : SingleAction
    {
        [Tooltip("The name of the property to set")]
        [SerializeField] private string propertyName;

        [Tooltip("After requesting an action, how long does it take for the action to be performed?")] [Min(0f)]
        public float delay;

        /// <summary>
        /// States the coroutine to trigger the given animation trigger 
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.StartCoroutine(PlayTrigger(stateMachine));
            yield break;
        }

        /// <summary>
        /// Triggers the given trigger.
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack. </param>
        /// <returns> The time to wait to trigger. </returns>
        IEnumerator PlayTrigger(BaseStateMachine stateMachine)
        {
            if (delay != 0)
            {
                yield return new UnityEngine.WaitForSeconds(delay);
            }
            stateMachine.GetComponent<AnimatorController>().SetTrigger(propertyName);
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}
