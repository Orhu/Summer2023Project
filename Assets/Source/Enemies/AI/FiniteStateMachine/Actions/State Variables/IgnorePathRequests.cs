
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Sets the ignore path requests variable to serialized bool
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/State Variables/Ignore Path Requests")]
    public class IgnorePathRequests : FSMAction
    {
        [SerializeField] private bool newIgnorePathRequests;
        public override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.pathData.ignorePathRequests = newIgnorePathRequests;
            yield break;
        }
    }