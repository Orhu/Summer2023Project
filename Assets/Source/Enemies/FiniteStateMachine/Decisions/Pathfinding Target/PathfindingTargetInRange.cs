using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking whether our target is in a certain range
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Decisions/Pathfinding Target/Pathfinding Target In Range")]
    public class PathfindingTargetInRange : Decision
    {
        [Tooltip("What range to check?")]
        [SerializeField] private float range;

        /// <summary>
        /// Evaluates whether the current target is within the requested range
        /// </summary>
        /// <param name="state"> The stateMachine to use </param>
        /// <returns> True if the target is at or below the specified range from this stateMachine, false otherwise </returns>
        protected override bool Evaluate(BaseStateMachine state)
        {
            return Vector2.Distance(state.currentPathfindingTarget, state.transform.position) <= range;
        }
    }
}