using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking whether an amount of time has passed in the current state based on some health value
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Hero of Black Petals/Time Since Transition (Health Threshold)")]
    public class TimeSinceTransitionBasedOnHealth : Decision
    {
        [Tooltip("The time to wait until this returns true")]
        [SerializeField] private float timeIfAboveThreshold;
        
        [Tooltip("The time to wait until this returns true")]
        [SerializeField] private float timeIfBelowThreshold;
        
        [Tooltip("Health % threshold")] [Range(0f, 1f)]
        [SerializeField] private float healthPercentThreshold;

        /// <summary>
        /// Returns true if the time has passed since this state was entered.
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> timeSinceTransition >= time </returns>
        public override bool Decide(BaseStateMachine stateMachine)
        {
            Health health = stateMachine.GetComponent<Health>();
            float healthPercent = (float)health.currentHealth / health.maxHealth;
            
            return healthPercent >= healthPercentThreshold
                ? stateMachine.timeSinceTransition >= timeIfAboveThreshold
                : stateMachine.timeSinceTransition >= timeIfBelowThreshold;
        }
    }
}