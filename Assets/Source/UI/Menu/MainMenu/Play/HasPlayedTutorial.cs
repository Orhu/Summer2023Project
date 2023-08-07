using UnityEngine;
using UnityEngine.Events;

namespace Cardificer
{
    /// <summary>
    /// Used to detect if an autosave exists.
    /// </summary>
    public class HasPlayedTutorial : MonoBehaviour
    {
        [Tooltip("Called on enable if an autosave exists.")]
        public UnityEvent tutorialPlayed;

        [Tooltip("Called on enable if an autosave DOSE NOT exist.")]
        public UnityEvent tutorialNotPlayed;

        /// <summary>
        /// Calls the appropriate event.
        /// </summary>
        private void OnEnable()
        {
            if (SaveManager.tutorialCompleted)
            {
                tutorialPlayed?.Invoke();
            }
            else
            {
                tutorialNotPlayed?.Invoke();
            }
        }
    }
}