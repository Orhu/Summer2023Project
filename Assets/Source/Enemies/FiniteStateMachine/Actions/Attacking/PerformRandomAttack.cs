using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that fires an attack
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Attacking/Perform Random Attack")]
    public class PerformRandomAttack : PerformAction
    {
        [Tooltip("The actions that will be taken when the enemy attempts to issue an action and the probability of each happening.")]
        public GenericWeightedThings<Action[]> actions;

        /// <summary>
        /// Gets the attacks that will be used by this
        /// </summary>
        /// <returns> The attacks that will be launched when the enemy attempts to attack. </returns>
        public override Action[] GetAttacks()
        {
            return actions.GetRandomThing();
        }
    }
}