using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking if we currently have line of sight on the pathfinding target.
    /// </summary>
    [CreateAssetMenu(menuName= "FSM/Decisions/Pathfinding Target/Line of Sight on Pathfinding Target")]
    public class LineOfSightOnPathfindingTarget : Decision
    {
        [Tooltip("Which layers the raycast can hit, by their name")]
        [SerializeField] private LayerMask layerMask;

        [Tooltip("The width of the path to ensure a line of sight in.")] [Min(0)]
        [SerializeField] private float sightRadius;

        /// <summary>
        /// Evaluates whether the current target is within the requested range
        /// </summary>
        /// <param name="state"> The stateMachine to use </param>
        /// <returns> True if the target is at or below the specified range from this stateMachine, false otherwise </returns>
        public override bool Decide(BaseStateMachine state)
        {
            Vector2 currentPos = state.transform.position;
            Vector2 direction = state.currentPathfindingTarget - currentPos;
            RaycastHit2D hit;
            if (sightRadius == 0)
            {
                hit = Physics2D.Raycast(currentPos, direction, direction.magnitude, layerMask);
            }
            else
            {
                hit = Physics2D.CircleCast(currentPos, sightRadius, direction, direction.magnitude, layerMask);
            }

            return hit.collider == null || hit.collider.CompareTag("Player");
        }
    }
}