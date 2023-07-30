using System;
using UnityEngine;
using UnityEngine.Events;

namespace Cardificer
{
    /// <summary>
    /// Class for setting text to the value of save data.
    /// </summary>
    public class GetCurrentRunData : MonoBehaviour
    {
        [Tooltip("The current floor the player is on.")]
        [SerializeField] private RunDataum<DateTime> lastPlayed;

        [Tooltip("The current floor the player is on.")]
        [SerializeField] private RunDataum<TimeSpan> playTime;

        [Tooltip("The current floor the player is on.")]
        [SerializeField] private RunDataum<string> currentFloor;

        [Tooltip("The current size of the player deck.")]
        [SerializeField] private RunDataum<int> deckSize;

        [Tooltip("The current amount of health the player has.")]
        [SerializeField] private RunDataum<int> health;

        [Tooltip("The current amount of money the player has.")]
        [SerializeField] private RunDataum<int> money;

        /// <summary>
        /// Calls all of the unity events to assign the values.
        /// </summary>
        void OnEnable()
        {
            lastPlayed.Invoke(SaveManager.lastAutosaveTime);
            playTime.Invoke(SaveManager.savedPlaytime);
            currentFloor.Invoke(FloorSceneManager.GetFloorName(SaveManager.savedCurrentFloor));
            deckSize.Invoke((SaveManager.savedPlayerDeck?.pathToCards.Count).GetValueOrDefault());
            health.Invoke(SaveManager.savedPlayerHealth);
            money.Invoke(SaveManager.savedPlayerMoney);
        }


        /// <summary>
        /// Class for grouping info about a single piece of data from the run
        /// </summary>
        /// <typeparam name="Type"> The type of data. </typeparam>
        [System.Serializable]
        private class RunDataum<T>
        {
            [Tooltip("The text added to the beginning.")]
            public string prefixText;

            [Tooltip("The text added to the end.")]
            public string suffixText;

            [Tooltip("The text to use if there is no autosave.")]
            public string fallbackText;

            [Tooltip("Called on enable and passes in the saved value.")]
            public UnityEvent<string> getValueAsString;

            [Tooltip("The value to use if there is no autosave.")]
            public T fallbackValue;

            [Tooltip("Called on enable and passes in the saved value.")]
            public UnityEvent<T> getValue;

            /// <summary>
            /// Invokes get value.
            /// </summary>
            public void Invoke(T value)
            {
                if (SaveManager.autosaveExists)
                {
                    getValueAsString?.Invoke(prefixText + value.ToString() + suffixText);
                    getValue?.Invoke(value);
                }
                else
                {
                    getValueAsString?.Invoke(fallbackText);
                }
            }
        }
    }
}