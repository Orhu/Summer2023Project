using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that spawns enemies at the requested locations
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Hero of Black Petals/Spawn Enemies at Locations")]
    public class HOBP_SpawnEnemies : SingleAction
    {
        [Tooltip("The thing to spawn")]
        [SerializeField] private GameObject summonThing;

        [Tooltip("How many seconds to wait before starting spawning?")] [Min(0f)]
        [SerializeField] private float initialDelay;

        [Tooltip("How many seconds to wait between each spawn?")] [Min(0f)]
        [SerializeField] private float delayBetweenSpawns = 1f;
        
        [Tooltip("Spawn locations relative to room center (one enemy will be spawned for every vector in the list")]
        [SerializeField] private Vector2[] spawnLocations;

        /// <summary>
        /// Formulates a path as requested and sets it to be the state machine's path.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            Vector2 roomPos = FloorGenerator.currentRoom.transform.position;
            
            yield return new WaitForSeconds(initialDelay);

            foreach (Vector2 vector in spawnLocations)
            {
                Instantiate(summonThing).transform.position = roomPos + vector;
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
            
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}