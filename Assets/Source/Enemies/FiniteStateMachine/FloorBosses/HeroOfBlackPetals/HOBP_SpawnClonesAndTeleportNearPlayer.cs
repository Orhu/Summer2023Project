using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that spawns clones around the player and places the hero at one of the spots
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Hero of Black Petals/Spawn Clones")]
    public class HOBP_SpawnClonesAndTeleportNearPlayer : SingleAction
    {
        [Tooltip("Clone GameObject")]
        [SerializeField] private GameObject cloneGameObject;

        [Tooltip("Distance from player position")]
        [SerializeField] private float distanceFromPlayer;

        /// <summary>
        /// Enum to represent the pattern of clones via serialized field
        /// </summary>
        private enum ClonePattern
        {
            Cardinal,
            Diagonal
        }
        [Tooltip("Pattern to spawn clones")]
        [SerializeField] private ClonePattern pattern;
        
        [Tooltip("Delay before teleporting the hero and spawning his clones")] [Min(0f)]
        [SerializeField] private float delay;

        /// <summary>
        /// Picks a random pos for the stateMachine, and places clone gameobjects in all other positions.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Waits delay seconds before performing the action </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            yield return new WaitForSeconds(delay);
            List<Vector2> possiblePositions;

            switch (pattern)
            {
                case ClonePattern.Cardinal:
                possiblePositions = new List<Vector2>
                {
                    new(distanceFromPlayer, 0),
                    new(-distanceFromPlayer, 0),
                    new(0, distanceFromPlayer),
                    new(0, -distanceFromPlayer)
                };
                break;
                case ClonePattern.Diagonal:
                    possiblePositions = new List<Vector2>
                    {
                        new(distanceFromPlayer, distanceFromPlayer),
                        new(distanceFromPlayer, -distanceFromPlayer),
                        new(-distanceFromPlayer, distanceFromPlayer),
                        new(-distanceFromPlayer, -distanceFromPlayer)
                    };
                    break;
                default:
                    Debug.LogError("Unknown pattern: " + pattern);
                    possiblePositions = new List<Vector2>();
                    break;
            }
            
            Vector2 playerPos = Player.Get().transform.position;
            
            // Need to purge possiblePositions of any pos that is not able to be moved to
            for (int i = 0; i < possiblePositions.Count; i++)
            {
                var tileLookupResult = RoomInterface.instance.WorldPosToTile(playerPos + possiblePositions[i]);
                if (!(tileLookupResult.Item2 && tileLookupResult.Item1.allowedMovementTypes.HasFlag(stateMachine.currentMovementType)))
                {
                    possiblePositions.RemoveAt(i);
                    i--;
                }
            }
            
            if (possiblePositions.Count is 0 or 1)
            {
                // 0 means there is not even one valid positions
                // 1 means there is a valid teleport position, but no valid clone positions
                Debug.LogWarning("No valid position to summon clones. Illusory Clones will not be performed.");
                yield break;
            }

            int randomIndex = Random.Range(0, possiblePositions.Count);
            Vector2 randomPosition = possiblePositions[randomIndex];
            possiblePositions.RemoveAt(randomIndex);

            // Create all clones and teleport state machine to position
            
            foreach (Vector2 vector in possiblePositions)
            {
                Instantiate(cloneGameObject).transform.position = playerPos + vector;
                yield return null;
            }
            stateMachine.transform.position = playerPos + randomPosition;

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}