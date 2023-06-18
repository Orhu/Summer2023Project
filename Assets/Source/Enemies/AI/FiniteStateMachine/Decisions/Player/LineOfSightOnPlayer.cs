using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking if we currently have line of sight on the player
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Decisions/Player/Line of Sight on Player")]
    public class LineOfSightOnPlayer : FSMDecision
    {
        [Tooltip("What range to check?")]
        [SerializeField] private float range = 4.0f;

        [Tooltip("Which layers to raycast, by their integer identifier")]
        [SerializeField] private List<int> raycastLayers;

        /// <summary>
        /// Evaluates whether the current target is within the requested range
        /// </summary>
        /// <param name="state"> The stateMachine to use </param>
        /// <returns> True if the target is at or below the specified range from this stateMachine, false otherwise </returns>
        public override bool Evaluate(BaseStateMachine state)
        {
            // use layer mask to exclude provided int layers from the raycast
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
                if (hit.collider.CompareTag("Player"))
                {
                    // Line of sight is unobstructed and player is hit
                    return true;
                }
                else
                {
                    // Obstacle or other object hit
                    return false;
                }
            }

            // No objects hit
            return false;
        }
    }
}