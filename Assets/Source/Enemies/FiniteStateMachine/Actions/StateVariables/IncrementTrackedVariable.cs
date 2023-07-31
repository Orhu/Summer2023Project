
    using System.Collections;
    using UnityEngine;

    namespace Cardificer.FiniteStateMachine
    {
        /// <summary>
        /// Represents an action that increments a given state variable by 1 every time it is called
        /// </summary>
        [CreateAssetMenu(menuName = "FSM/Tracked Variables/Integer/Actions/Increment Tracked Variable")]
        public class IncrementTrackedVariable : SingleAction
        {
            [Tooltip("Name of the state variable to increment (will create if it doesn't exist)")]
            [SerializeField] private string varToIncrement;
            
            [Tooltip("How much to increment the variable by")]
            [SerializeField] private int incrementBy = 1;

            [Tooltip("How many seconds before another increment is allowed?")]
            [SerializeField] private float rateLimit = 0f;
            
            /// <summary>
            /// Increments the requested var by the requested amount
            /// </summary>
            /// <param name="stateMachine"> The state machine to be used. </param>
            /// <returns> Ends when the action is complete. </returns>
            protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
            {
                stateMachine.trackedVariables.TryAdd(varToIncrement, 0);

                stateMachine.trackedVariables[varToIncrement] = (int)stateMachine.trackedVariables[varToIncrement] + incrementBy;
                BaseStateMachine.print(stateMachine.trackedVariables[varToIncrement]);

                yield return new WaitForSeconds(rateLimit);
                
                stateMachine.cooldownData.cooldownReady[this] = true;
                
                yield break;
            }
        }
    }
