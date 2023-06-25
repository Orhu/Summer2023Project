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
        [Tooltip("Which layers the raycast can hit, by their name")]
        [SerializeField] private LayerMask layerMask;

        /// <summary>
        /// Evaluates whether the current target is within the requested range
        /// </summary>
        /// <param name="state"> The stateMachine to use </param>
        /// <returns> True if the target is at or below the specified range from this stateMachine, false otherwise </returns>
        protected override bool Evaluate(BaseStateMachine state)
        {
            var currentPos = state.transform.position;
            Vector2 direction = Player.Get().transform.position - currentPos;
            RaycastHit2D hit = Physics2D.Raycast(currentPos, direction, direction.magnitude, layerMask);

            return hit.collider == null || hit.collider.CompareTag("Player");
        }
    }
}