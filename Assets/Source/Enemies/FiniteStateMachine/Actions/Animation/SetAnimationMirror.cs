using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets the animation state variables.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Animation/Set Animation Mirror")]
    public class SetAnimationMirror : SingleAction
    {
        [Tooltip("The name of the property to set")]
        [SerializeField] private string propertyName;

        [Tooltip("The name of the property to set")]
        [SerializeField] private bool propertyValue;

        /// <summary>
        /// Sets the given mirror property 
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.GetComponent<AnimatorController>().SetMirror(propertyName, propertyValue);
            yield break;
        }
    }
}
