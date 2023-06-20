using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking if we currently have line of sight on the player
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Decisions/Player/Line of Sight on Player")]
    public class LineOfSightOnPlayer : Decision
    {
        [Tooltip("Maximum raycast range?")]
        [SerializeField] private float range = 4.0f;

        [Tooltip("Which layers the raycast can hit, by their integer identifier")]
        [SerializeField] private List<int> raycastLayers;

        /// <summary>
        /// Evaluates whether the current target is within the requested range
        /// </summary>
        /// <param name="state"> The stateMachine to use </param>
        /// <returns> True if the target is at or below the specified range from this stateMachine, false otherwise </returns>
        protected override bool Evaluate(BaseStateMachine state)
        {
            // use layer mask to include provided int layers in the raycast
            int layerMask = 0;
            foreach (int layer in raycastLayers)
            {
                layerMask |= 1 << layer;
            }

            var currentPos = state.transform.position;
            Vector2 direction = Player.Get().transform.position - currentPos;
            RaycastHit2D hit = Physics2D.Raycast(currentPos, direction, range, layerMask);

            if (hit.collider != null)
            {
                // return if we hit the player
                return hit.collider.CompareTag("Player");
            }

            // No objects hit
            return false;
        }
    }
}