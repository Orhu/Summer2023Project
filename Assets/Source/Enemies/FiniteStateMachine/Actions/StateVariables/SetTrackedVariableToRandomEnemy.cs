using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets a given variable name to a random (non-self) enemy.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Tracked Variables/GameObjects/Actions/Set Tracked Variable to Random Enemy")]
    public class SetTrackedVariableToRandomEnemy : SingleAction
    {
        [Tooltip("Tracked variable to set (creates it at set number if it doesn't exist)")]
        [SerializeField] private string trackedVariableName;

        /// <summary>
        /// Sets the requested name variable to a randomly picked int.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            List<GameObject> validEnemies = new List<GameObject>(FloorGenerator.currentRoom.livingEnemies);
            validEnemies.Remove(stateMachine.gameObject);
            GameObject randomEnemy = validEnemies[Random.Range(0, validEnemies.Count)];
            stateMachine.trackedVariables.TryAdd(trackedVariableName, randomEnemy);
            stateMachine.trackedVariables[trackedVariableName] = randomEnemy;
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}