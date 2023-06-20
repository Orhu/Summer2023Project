using System.Collections;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Represents an action to update our target to be the player
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Set Target To Player With Condition")]
    public class SetTargetToPlayerWithCondition : FSMAction
    {
        [Tooltip("Target feet? Should be set to true if this target is used for pathfinding. Should be set to false if this target is used for shooting an attack.")]
        [SerializeField] private bool targetFeet;

        [Tooltip("The condition when to pick a new target")]
        [SerializeField] private FSMDecision whenToPickNewTile;

        /// <summary>
        /// When condition is met and cooldown is ready, set player target
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        public override void OnStateUpdate(BaseStateMachine stateMachine)
        {
            if (stateMachine.cooldownData.cooldownReady[this] && whenToPickNewTile.Decide(stateMachine))
            {
                stateMachine.cooldownData.cooldownReady[this] = false;
                stateMachine.StartCoroutine(SetPlayerTarget(stateMachine));
            }
        }

        /// <summary>
        /// Updates the currentTarget to be the player
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        public override void OnStateEnter(BaseStateMachine stateMachine)
        {
            stateMachine.cooldownData.cooldownReady.Add(this, true);
            stateMachine.StartCoroutine(SetPlayerTarget(stateMachine));
        }

        /// <summary>
        /// Nothing to do here,required for FSMAction implementation
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        public override void OnStateExit(BaseStateMachine stateMachine)
        {
            stateMachine.cooldownData.cooldownReady.Remove(this);
        }

        /// <summary>
        /// Sets the current target to player
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns></returns>
        IEnumerator SetPlayerTarget(BaseStateMachine stateMachine)
        {
            if (targetFeet)
            {
                stateMachine.currentTarget = Player.GetFeet().transform.position;
            }
            else
            {
                stateMachine.currentTarget = Player.Get().transform.position;
            }

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}