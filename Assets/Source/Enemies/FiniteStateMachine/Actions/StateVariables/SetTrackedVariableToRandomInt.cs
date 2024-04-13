using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets a given variable name to a given integer value
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Tracked Variables/Integer/Actions/Set Tracked Variable to Random Int")]
    public class SetTrackedVariableToRandomInt : SingleAction
    {
        [Tooltip("Tracked variable to set (creates it at set number if it doesn't exist)")]
        [SerializeField] private string trackedVariableName;

        [Tooltip("Min number (inclusive)")]
        [SerializeField] private int minNumber;

        [Tooltip("Max number (exclusive)")]
        [SerializeField] private int maxNumber;

        [Tooltip("Allow the same number to be picked again? If false, will only pick a unique number (not the one already assigned to the var).")]
        [SerializeField] private bool allowSameNumber = true;

        /// <summary>
        /// Sets the requested name variable to a randomly picked int.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            bool valAlreadyExists = stateMachine.trackedVariables.TryGetValue(trackedVariableName, out var currentNum);
            int randomNum = Random.Range(minNumber, maxNumber);

            if (valAlreadyExists && !allowSameNumber)
            {
                while (randomNum == (int)currentNum)
                {
                    // Reroll until we get a unique number
                    randomNum = Random.Range(minNumber, maxNumber);
                }
                stateMachine.trackedVariables[trackedVariableName] = randomNum;
                stateMachine.cooldownData.cooldownReady[this] = true;
            }
            else
            {
                stateMachine.trackedVariables.TryAdd(trackedVariableName, randomNum);
                stateMachine.trackedVariables[trackedVariableName] = randomNum;
                stateMachine.cooldownData.cooldownReady[this] = true;
            }

            yield break;
        }
    }
}