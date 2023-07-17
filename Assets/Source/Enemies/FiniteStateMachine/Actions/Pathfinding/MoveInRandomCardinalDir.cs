using System;
using System.Collections;
using UnityEngine;
using ChaseData = Cardificer.FiniteStateMachine.BaseStateMachine.ChaseData;
using Random = UnityEngine.Random;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that simply sets the movement input to a single random direction.
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Pathfinding/Move in Random Cardinal Direction")]
    public class MoveInRandomCardinalDir : SingleAction
    {
        [Tooltip("Delay after random move has been selected before another random move is allowed, in seconds.")]
        [SerializeField] private float randomMoveLockout;
        
        /// <summary>
        /// Picks a random dir and sets that as movement input
        /// </summary>
        /// <param name="stateMachine"> stateMachine to be used </param>
        /// <returns> Waits randomMoveLockout seconds before re-enabling cooldown </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            Vector2[] directions =
            {
                new(1, 0),  // right
                new(-1, 0), // left
                new(0, -1), // down
                new(0, 1) // up
            };
            
            Vector2 randomDirection = directions[Random.Range(0, directions.Length)];

            stateMachine.GetComponent<Movement>().movementInput = randomDirection;
            yield return new WaitForSeconds(randomMoveLockout);
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}