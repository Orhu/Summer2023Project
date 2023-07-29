using Cardificer.FiniteStateMachine;
using TMPro;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Sets a text component to be the current state of a state machine.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class SetTextToState : MonoBehaviour
    {
        /// <summary>
        /// Set Text.
        /// </summary>
        private void Update()
        {
            GetComponent<TMP_Text>().text = GetComponentInParent<BaseStateMachine>().currentState.name;
        }
    }
}
