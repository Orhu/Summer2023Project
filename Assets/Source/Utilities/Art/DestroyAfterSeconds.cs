using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Utility class to destroy visuals after a set amount of time (i.e. explosion sprites).
    /// </summary>
    public class DestroyAfterSeconds : MonoBehaviour
    {
        [Tooltip("Time in seconds before object is destroyed.")]
        [SerializeField] private float timeToDestruction = 1f;

        /// <summary>
        /// Starts destruction countdown.
        /// </summary>
        private void Awake()
        {
            Destroy(this.gameObject, timeToDestruction);
        }
    }
}
