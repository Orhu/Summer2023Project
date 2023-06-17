
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Represents an action that disables the follow path coroutine
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/State Variables/Stop Following Path")]
    public class StopFollowingPath : FSMAction
    {
        public override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.StopCoroutine(stateMachine.pathData.prevFollowCoroutine);
            yield break;
        }
    }
