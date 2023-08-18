using System;
using System.Collections;
using System.Collections.Generic;
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

        [Tooltip("Offset from player position")]
        [SerializeField] private Vector2 offsetFromPlayer;
        
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
            List<Vector2> possiblePositions = new List<Vector2>
            {
                new(offsetFromPlayer.x, offsetFromPlayer.y),
                new(-offsetFromPlayer.x, offsetFromPlayer.y),
                new(offsetFromPlayer.x, -offsetFromPlayer.y),
                new(-offsetFromPlayer.x, -offsetFromPlayer.y)
            };
            
            int randomIndex = Random.Range(0, possiblePositions.Count);
            Vector2 randomPosition = possiblePositions[randomIndex];
            possiblePositions.RemoveAt(randomIndex);

            // Create all clones and teleport state machine to position
            Vector2 playerPos = Player.Get().transform.position;
            foreach (Vector2 vector in possiblePositions)
            {
                Instantiate(cloneGameObject).transform.position = playerPos + vector;
            }
            stateMachine.transform.position = playerPos + randomPosition;

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}