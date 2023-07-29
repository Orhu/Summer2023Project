using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking whether our current health is below a certain amount
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Decisions/Health Below Amount")]
    public class HealthBelowAmount : Decision
    {
        [Tooltip("The max health for which this will be true")] [Min(1)]
        [SerializeField] private int healthThreshold = 4;

        /// <summary>
        /// Returns true if the health is below the provided threshold
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> True if the health is below the provided threshold, false otherwise </returns>
        public override bool Decide(BaseStateMachine stateMachine)
        {
            return stateMachine.GetComponent<Health>().currentHealth <= healthThreshold;
        }
    }
}