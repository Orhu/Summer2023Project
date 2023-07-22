using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets the animation state variables.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Animation/Set Animation Trigger (on Attack)")]
    public class SetAnimationTriggerOnAttack : SingleAction
    {
        [Tooltip("The name of the property to set")]
        [SerializeField] private string propertyName;

        /// <summary>
        /// Sets the given animation trigger when the next attack is played by the state machine.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables["OnAttack"] = (stateMachine.trackedVariables["OnAttack"] as System.Action) + PlayAnim;

            void PlayAnim()
            {
                stateMachine.trackedVariables["OnAttack"] = (stateMachine.trackedVariables["OnAttack"] as System.Action) - PlayAnim;
                stateMachine.GetComponent<AnimatorController>().SetTrigger(propertyName);
                Debug.Log("ANIMIATE!");
                stateMachine.cooldownData.cooldownReady[this] = true;
            }
            yield break;
        }
    }
}
