using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Represents an action that sets an adjacent direction as the target when a condition is met
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Set Random Adjacent Direction As Target With Condition")]
    public class SetAdjacentDirectionAsTargetWithCondition : FSMAction
    {
        [Tooltip("How long of a delay between each target reset?")]
        [SerializeField] private float delayBetweenTargetSelects = 0.25f;

        [Tooltip("Condition to evaluate before executing the action")]
        [SerializeField] private FSMDecision decision;

        /// <summary>
        /// Every frame, if cooldown is ready and condition is true, selects a random adjacent target
        /// </summary>
        /// <param name="stateMachine"></param>
        public override void OnStateUpdate(BaseStateMachine stateMachine)
        {
            if (stateMachine.cooldownData.cooldownReady[this] && decision.Decide(stateMachine))
            {
                stateMachine.cooldownData.cooldownReady[this] = false;
                var coroutine = SetAdjacentDirectionAsTarget(stateMachine);
                stateMachine.StartCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Adds to cooldown list in state machine if necessary
        /// </summary>
        /// <param name="stateMachine"> The state machine to use </param>
        public override void OnStateEnter(BaseStateMachine stateMachine)
        {
            // sometimes, because transitions can occur every frame, rapid transitions cause the key not to be deleted properly and error. this check prevents that error
            if (!stateMachine.cooldownData.cooldownReady.ContainsKey(this))
            {
                // if the key has not yet been added, add it with 0 cooldown
                stateMachine.cooldownData.cooldownReady.Add(this, true);
                RoomInterface.instance.GrabCurrentRoom();
            }
            else
            {
                // in this case, there may be a cooldown still running from a previous state exit/re-entry so don't set the value at all
            }
        }

        /// <summary>
        /// Not needed for this but required for FSMAction implementation
        /// </summary>
        /// <param name="stateMachine"></param>
        public override void OnStateExit(BaseStateMachine stateMachine)
        {
        }

        /// <summary>
        /// Retrieves a random neighbor tile that is walkable, and sets it as the current target
        /// </summary>
        /// <param name="stateMachine"></param>
        /// <returns></returns>
        private IEnumerator SetAdjacentDirectionAsTarget(BaseStateMachine stateMachine)
        {
            var tileResult = RoomInterface.instance.WorldPosToTile(stateMachine.feetCollider.transform.position);
            if (tileResult.Item2)
            {
                List<RoomInterface.PathfindingTile> viableMovementNeighbors = new List<RoomInterface.PathfindingTile>();
                var neighbors = RoomInterface.instance.GetNeighbors(tileResult.Item1);
                foreach (var tile in neighbors)
                {
                    if (tile.walkable)
                    {
                        viableMovementNeighbors.Add(tile);
                    }
                }

                if (viableMovementNeighbors.Count == 0)
                {
                    stateMachine.cooldownData.cooldownReady[this] = true;
                    yield break;
                }
                else
                {
                    var tileNewTarget = viableMovementNeighbors[Random.Range(0, viableMovementNeighbors.Count)];
                    stateMachine.currentTarget = RoomInterface.instance.TileToWorldPos(tileNewTarget);
                    yield return new WaitForSeconds(delayBetweenTargetSelects);
                    stateMachine.cooldownData.cooldownReady[this] = true;
                    yield break;
                }
            }
            else
            {
                // if we are here, it means the stateMachine was not on the current room grid. do nothing
                stateMachine.cooldownData.cooldownReady[this] = true;
                yield break;
            }
        }
    }
}