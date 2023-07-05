using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets the animation state variables.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Animation/Set Animation Bool")]
    public class SetAnimationBool : SingleAction
    {
        [Tooltip("The name of the property to set")]
        [SerializeField] private string PropertyName;

        [Tooltip("The name of the property to set")]
        [SerializeField] private bool PropertyValue;

        /// <summary>
        /// Sets the given animation bool. 
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.GetComponent<AnimatorController>().SetBool(PropertyName, PropertyValue);
            yield break;
        }
    }
}
