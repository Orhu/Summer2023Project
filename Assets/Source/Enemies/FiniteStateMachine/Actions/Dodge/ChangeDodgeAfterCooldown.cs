using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that enables dodging after a given delay
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Dodge/Enable or Disable Dodge After Delay")]
    public class ChangeDodgeAfterCooldown : SingleAction
    {
        [Tooltip("How long after this action is called should dodging be enabled (in seconds)?")]
        [SerializeField] private float delay = 1f;
        
        [Tooltip("What to set dodge var to (true is means able to dodge, false means cannot dodge)")]
        [SerializeField] private bool canDodge = true;

        /// <summary>
        /// Enables dodging after a given delay
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> Waits delay seconds before enabling dodging again </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            yield return new WaitForSeconds(delay);
            stateMachine.canDodge = canDodge;
        }
    }
}