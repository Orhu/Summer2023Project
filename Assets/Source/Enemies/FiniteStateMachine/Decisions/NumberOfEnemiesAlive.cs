using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking whether our current velocity is below a certain amount
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Decisions/Number Of Enemies Alive")]
    public class NumberOfEnemiesAlive : Decision
    {
        [Tooltip("The minimum (inclusive) number of enemies that need to be alive for this to be true.")] [Min(1)]
        [SerializeField] private int minNumberOfEnemies = 1;

        /// <summary>
        /// Returns true if the number of living enemies is below the provided threshold.
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use. </param>
        /// <returns> True if the number of living enemies is below the provided threshold, false otherwise </returns>
        public override bool Decide(BaseStateMachine stateMachine)
        {
            return FloorGenerator.currentRoom.livingEnemies.Count <= minNumberOfEnemies;
        }
    }
}