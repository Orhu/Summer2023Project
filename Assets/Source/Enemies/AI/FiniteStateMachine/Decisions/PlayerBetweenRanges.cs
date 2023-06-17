using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Represents a decision checking whether our target is in a certain range
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Decisions/Player Between Ranges")]
    public class PlayerBetweenRanges : FSMDecision
    {
        [Tooltip("What min range for our target?")]
        [SerializeField] private float minRange;

        [Tooltip("What max range for our target?")]
        [SerializeField] private float maxRange;

        /// <summary>
        /// Evaluates whether the current target is within the requested range
        /// </summary>
        /// <param name="state"> The stateMachine to use </param>
        /// <returns> True if the target is at or below the specified range from this stateMachine, false otherwise </returns>
        public override bool Decide(BaseStateMachine state)
        {
            var dist = Vector2.Distance(state.player.transform.position, state.transform.position);
            return invert
                ? !(dist >= minRange && dist <= maxRange)
                : (dist >= minRange && dist <= maxRange);
        }
    }
}