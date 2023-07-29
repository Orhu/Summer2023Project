using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision that rolls a random chance to return true
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Decisions/Random/Random Chance")]
    public class RandomChance : Decision
    {
        [Tooltip("Odds of this roll returning true")] [Range(0f, 1f)]
        [SerializeField] private float chance;
        
        [Tooltip("Delay between rolls (in seconds).")]
        [SerializeField] private float rollTimeLockout;

        /// <summary>
        /// Evaluates whether the current target meets the random chance criteria.
        /// </summary>
        /// <param name="state"> The stateMachine to use. </param>
        /// <returns> True if the target rolls the given chance, false otherwise. </returns>
        public override bool Decide(BaseStateMachine state)
        {
            state.trackedVariables.TryAdd("LastRollTime", Time.time);
            float lastRollTime = (float)state.trackedVariables["LastRollTime"];
            
            // Check if enough time has passed since the last roll
            if (Time.time - lastRollTime < rollTimeLockout)
            {
                return false;
            }
            
            // Perform the random chance roll
            if (Random.Range(0f, 1f) <= chance)
            {
                state.trackedVariables["LastRollTime"] = Time.time; // Update the last roll time
                return true;
            }
            
            return false;
        }
    }
}