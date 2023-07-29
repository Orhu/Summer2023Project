using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking whether our target is in a certain range
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Decisions/Player/Player In Range")]
    public class PlayerInRange : Decision
    {
        [Tooltip("What range to check?")]
        [SerializeField] private float range;

        /// <summary>
        /// Evaluates whether the current target is within the requested range
        /// </summary>
        /// <param name="state"> The stateMachine to use </param>
        /// <returns> True if the target is at or below the specified range from this stateMachine, false otherwise </returns>
        public override bool Decide(BaseStateMachine state)
        {
            return Vector2.Distance(Player.Get().transform.position, state.transform.position) <= range;
        }
    }
}