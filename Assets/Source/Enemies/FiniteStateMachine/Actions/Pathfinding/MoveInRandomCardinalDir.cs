using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChaseData = Cardificer.FiniteStateMachine.BaseStateMachine.ChaseData;
using Random = UnityEngine.Random;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that simply sets the movement input to a single random direction.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Pathfinding/Move in Random Cardinal Direction")]
    public class MoveInRandomCardinalDir : SingleAction
    {
        [Tooltip("Delay after random move has been selected before another random move is allowed, in seconds.")]
        [SerializeField] private float randomMoveLockout;
        
        [Tooltip("Which layers the raycast can hit, by their name")]
        [SerializeField] private LayerMask layerMask;

        [Tooltip("What range to check with raycast to tell if direction is allowed?")]
        [SerializeField] private float raycastRange = 1f;

        /// <summary>
        /// Picks a random unblocked dir and sets that as movement input
        /// </summary>
        /// <param name="stateMachine"> stateMachine to be used </param>
        /// <returns> Waits randomMoveLockout seconds before re-enabling cooldown </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            // initialize variables
            Vector2 currentDir = stateMachine.GetComponent<Movement>().movementInput;
            Vector2 clampedDir = new Vector2(Mathf.Clamp01(currentDir.x), Mathf.Clamp01(currentDir.y));
            
            Vector2 right = new(1, 0);
            Vector2 left = new(-1, 0);
            Vector2 up = new(0, -1);
            Vector2 down = new(0, 1);

            // populate viable directions list with all directions the enemy is not currently moving in
            List<Vector2> viableDirections = new List<Vector2>();
            Vector2[] allDirections = { right, left, up, down };

            foreach (Vector2 direction in allDirections)
            {
                // check if this is not the already moving direction, and there is a raycast that can make it at least raycastRange units
                if (clampedDir != direction && Physics2D.Raycast(stateMachine.GetFeetPos(), direction, raycastRange, layerMask).collider == null)
                {
                    viableDirections.Add(direction);
                }
            }

            if (viableDirections.Count == 0)
            {
                // set movement to zero
                Debug.LogWarning(stateMachine.gameObject.name + ": No viable random movement direction found!");
                stateMachine.GetComponent<Movement>().movementInput = Vector2.zero;
                yield return new UnityEngine.WaitForSeconds(randomMoveLockout);
                stateMachine.cooldownData.cooldownReady[this] = true;
            }
            else
            {
                // pick a random direction and set that as movement input
                Vector2 randomDirection = viableDirections[Random.Range(0, viableDirections.Count)];

                stateMachine.GetComponent<Movement>().movementInput = randomDirection;
                yield return new UnityEngine.WaitForSeconds(randomMoveLockout);
                stateMachine.cooldownData.cooldownReady[this] = true;  
            }
        }
    }
}