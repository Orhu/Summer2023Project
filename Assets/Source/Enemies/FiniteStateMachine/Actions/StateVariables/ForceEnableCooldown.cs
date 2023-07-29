using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that adds to the max speed.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Force Enable Cooldown")]
    public class ForceEnableCooldown : SingleAction
    {
        [Tooltip("The ScriptableObject representing the action of the cooldown that should be enabled.")]
        [SerializeField] private SingleAction cooldownToEnable;

        /// <summary>
        /// Enables the cooldown of the object serialized
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.cooldownData.cooldownReady.TryAdd(cooldownToEnable, true);
            stateMachine.cooldownData.cooldownReady[cooldownToEnable] = true;
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
