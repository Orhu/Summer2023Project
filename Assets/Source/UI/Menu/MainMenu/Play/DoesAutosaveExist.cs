using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cardificer
{
    /// <summary>
    /// Used to detect if an autosave exists.
    /// </summary>
    public class DoesAutosaveExist : MonoBehaviour
    {
        [Tooltip("Called on enable if an autosave exists.")]
        public UnityEvent autosaveExists;

        [Tooltip("Called on enable if an autosave DOSE NOT exist.")]
        public UnityEvent autosaveDoesNotExist;

        /// <summary>
        /// Calls the appropriate event.
        /// </summary>
        private void OnEnable()
        {
            if (SaveManager.autosaveExists)
            {
                autosaveExists?.Invoke();
            }
            else
            {
                autosaveDoesNotExist?.Invoke();
            }
        }
    }
}