using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Basic raycast action.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Decisions/Forward Raycast")]
    public class ForwardRaycast : Decision
    {
        [Tooltip("Layer Mask for what layers to raycast on.")]
        [SerializeField] private LayerMask layersToCast;

        [Tooltip("Length of the raycast.")]
        [SerializeField] private float length = Mathf.Infinity; 

        /// <summary>
        /// Fires raycast in forward movement direction, returns based on whether something is hit.
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> True if raycast hits something. </returns>
        public override bool Decide(BaseStateMachine stateMachine)
        {
            Vector2 smPos = stateMachine.transform.position;
            Vector2 smForward = stateMachine.GetComponent<Movement>().movementInput;
            RaycastHit2D raycastResult = Physics2D.Raycast(smPos, smPos + smForward, length, layersToCast);
            return raycastResult.collider != null;
        }
    }
}