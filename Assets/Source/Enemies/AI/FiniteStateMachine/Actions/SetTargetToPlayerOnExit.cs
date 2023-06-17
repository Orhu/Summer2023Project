using System.Collections;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Represents an action to update our target to be the player when the state is exited
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Set Target To Player On Exit")]
    public class SetTargetToPlayerOnExit : FSMAction
    {
        [Tooltip("Target feet? Should be set to true if this target is used for pathfinding. Should be set to false if this target is used for shooting an attack.")]
        [SerializeField] private bool targetFeet;

        /// <summary>
        /// Nothing to do here, required for FSMAction implementation
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        public override void OnStateUpdate(BaseStateMachine stateMachine)
        {
        }

        /// <summary>
        /// Updates the currentTarget to be the player
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        public override void OnStateEnter(BaseStateMachine stateMachine)
        {
        }

        /// <summary>
        /// Nothing to do here,required for FSMAction implementation
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        public override void OnStateExit(BaseStateMachine stateMachine)
        {
            SetPlayerTarget(stateMachine);
        }

        /// <summary>
        /// Set the current target to be the player
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        void SetPlayerTarget(BaseStateMachine stateMachine)
        {
            if (targetFeet)
            {
                stateMachine.currentTarget = Player.feet.transform.position;
            }
            else
            {
                stateMachine.currentTarget = Player.Get().transform.position;
            }

            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}