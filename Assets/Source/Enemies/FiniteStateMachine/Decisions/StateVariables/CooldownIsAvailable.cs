using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision that returns true if the given action's cooldownData.cooldownReady is true
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Decisions/Cooldown Ready")]
    public class CooldownIsAvailable : Decision
    {
        [Tooltip("Action to check the cooldown of (if the action doesn't exist when this check happens, null error might happen)")]
        [SerializeField] private BaseAction actionToCheck;

        /// <summary>
        /// Returns true if the given action's cooldownData.cooldownReady is true
        /// </summary>
        /// <param name="state"> The state machine to use </param>
        /// <returns> True if the given action's cooldownData.cooldownReady is true, false otherwise </returns>
        public override bool Decide(BaseStateMachine state)
        {
            return state.cooldownData.cooldownReady[actionToCheck];
        }
    }
}