using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that raycasts in all cardinal directions, then sets movement input to be one of those valid directions
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Dodge/Set MovementInput to Open Cardinal Dir")]
    public class RaycastAndDodge : SingleAction
    {
        [Tooltip("Layer Mask for what layers to raycast on.")]
        [SerializeField] private LayerMask layersToCast;

        [Tooltip("Length of the raycast.")]
        [SerializeField] private float length = 2f; 

        /// <summary>
        /// Fires raycast in four cardinal directions, then sets movement input randomly to one of the open directions
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> Does not wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            Vector2 smPos = stateMachine.transform.position;
            Vector2 right = Vector2.right;
            Vector2 up = Vector2.up;
            Vector2 down = Vector2.down;
            Vector2 left = Vector2.left;
            Vector2[] possibleMovementDirs = new Vector2[] { right, up, down, left };

            List<Vector2> validMovementDirs = new List<Vector2>();
            Vector2 longestMovementDir = smPos;
            
            foreach(Vector2 possibleMovement in possibleMovementDirs)
            {
                RaycastHit2D raycastResult = Physics2D.Raycast(smPos, smPos + possibleMovement, length, layersToCast);
                if (raycastResult.collider == null)
                {
                    validMovementDirs.Add(possibleMovement);
                }
                else
                {
                    if (((Vector2)raycastResult.transform.position - smPos).sqrMagnitude > (longestMovementDir - smPos).sqrMagnitude)
                    {
                        longestMovementDir = possibleMovement;
                    }
                }
            }

            if (validMovementDirs.Count > 0)
            {
                stateMachine.GetComponent<Movement>().movementInput = validMovementDirs[Random.Range(0, validMovementDirs.Count - 1)];
            }
            else
            {
                stateMachine.GetComponent<Movement>().movementInput = longestMovementDir;
            }

            yield break;
        }
    }
}