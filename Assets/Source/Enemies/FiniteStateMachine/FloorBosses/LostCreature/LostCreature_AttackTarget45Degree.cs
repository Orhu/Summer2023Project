using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets the attack target to face in whichever 45 degree angle is the farthest
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Lost Creature/Attack Target Farthest 45 Degree")]
    public class LostCreature_AttackTarget45Degree : SingleAction
    {
        [Tooltip("When determining distance, what things should block the raycast?")]
        [SerializeField] private LayerMask raycastCheckLayers;
        
        /// <summary>
        /// Sets pathfinding target to the longest diagonal
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            Vector2 statePos = stateMachine.transform.position;
            Vector2 upLeft = statePos + Vector2.up + Vector2.left;
            Vector2 upRight = statePos + Vector2.up + Vector2.right;
            Vector2 downLeft = statePos + Vector2.down + Vector2.left;
            Vector2 downRight = statePos + Vector2.down + Vector2.right;
            Vector2[] possiblePicks = { upLeft, upRight, downLeft, downRight };

            Vector2 longestVector = Vector2.zero;

            foreach (Vector2 diagonalDir in possiblePicks)
            { 
                RaycastHit2D hit = Physics2D.Raycast(statePos, diagonalDir - statePos, raycastCheckLayers);
                
                    // Calculate the squared magnitude of the diagonal direction
                    float sqrMagnitude = hit.distance * hit.distance;

                    // Compare squared magnitudes to find the longest diagonal direction
                    if (sqrMagnitude > longestVector.sqrMagnitude)
                    {
                        // New longest dir found, overwrite
                        longestVector = diagonalDir;
                    }
                    else if (Mathf.Approximately(sqrMagnitude, longestVector.sqrMagnitude)) // Use Mathf.Approximately for float comparison
                    {
                        // If equal, randomly pick one direction to keep (0 or 1)
                        int randomNum = UnityEngine.Random.Range(0, 2);

                        if (randomNum == 0)
                        {
                            // Overwrite the existing direction
                            longestVector = diagonalDir;
                        }
                        // If randomNum is 1, do not overwrite (keep existing direction)
                    }
            }

            if (longestVector == Vector2.zero)
            {
                Debug.LogWarning("Lost Creature couldn't determine best angle to fire!");
            }
            
            stateMachine.currentAttackTarget = longestVector;
            
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}