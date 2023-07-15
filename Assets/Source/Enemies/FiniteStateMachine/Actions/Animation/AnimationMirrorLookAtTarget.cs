using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets the animation state variables.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Animation/Animation Mirror Look At Target")]
    public class AnimationMirrorLookAtTarget : SingleAction
    {
        [Tooltip("The name of the property to set")]
        [SerializeField] private string propertyName;

        [Tooltip("Whether or not to invert the look direction")]
        [SerializeField] private bool invert;

        [Tooltip("Which target to look at")]
        [SerializeField] private TargetType targetToLookAt;
        private enum TargetType { AttackTarget, PathfindingTarget, ForwardMovement };

        /// <summary>
        /// Sets the given mirror property to look at the state machine's target. 
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            Vector2 target;
            
            switch (targetToLookAt)
            {
                case TargetType.AttackTarget:
                    target = stateMachine.currentAttackTarget;
                    break;
                case TargetType.PathfindingTarget:
                    target = stateMachine.currentPathfindingTarget;
                    break;
                default:
                    target = (Vector2)stateMachine.transform.position + stateMachine.GetComponent<Movement>().movementInput;
                    break;
            }
            
            bool lookDirection = (stateMachine.transform.position.x - target.x) < 0;
            stateMachine.GetComponent<AnimatorController>().SetMirror(propertyName, invert != lookDirection);

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
