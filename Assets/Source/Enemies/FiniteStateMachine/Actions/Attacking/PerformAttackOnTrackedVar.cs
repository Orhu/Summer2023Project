using System.Collections.Generic;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that fires an attack that can only hit the tracked var.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Attacking/Perform Attack (on Tracked Var)")]
    public class PerformAttackOnTrackedVar : PerformAttack
    {
        [Tooltip("The name of the variable that this will hit")]
        [SerializeField] private string variableName;

        [Tooltip("Whether this can hit the player.")]
        [SerializeField] private bool hitPlayer = false;


        /// <summary>
        /// The objects the attacks of this will ignore.
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack </param>
        protected override List<GameObject> GetIgnoredObjects(BaseStateMachine stateMachine)
        {
            if (friendlyFire) { return new List<GameObject>(); }

            if (stateMachine.trackedVariables.TryGetValue(variableName, out object trackedValue) &&
                trackedValue is GameObject trackedObject)
            {
                List<GameObject> ignoredObjects = new List<GameObject>(FloorGenerator.currentRoom.livingEnemies);
                if (!hitPlayer)
                {
                    ignoredObjects.Add(Player.Get());
                }
                ignoredObjects.Remove(trackedObject);
                return ignoredObjects;
            }
            else if (trackedValue == null)
            {
                return new List<GameObject>(FloorGenerator.currentRoom.livingEnemies);
            }
            throw new System.Exception($"Can't perform attack only on {variableName} as it does not exist or is not a game object on {stateMachine}.");
        }
    }
}