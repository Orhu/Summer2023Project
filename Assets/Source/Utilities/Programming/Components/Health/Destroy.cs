using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Destroys the game object
    /// </summary>
    public class Destroy : MonoBehaviour
    {
        [Tooltip("The delay before destruction in seconds. If set to 0, has no effect")]
        [SerializeField]
        private float destructionDelay;

        /// <summary>
        /// Allows for auto-destruction of prefabs with this component attached via inspector variables
        /// </summary>
        private void Start()
        {
            // if it is set to 0, don't destroy. that object is probably using the DelayedDestroyMe method instead.
            if (destructionDelay != 0)
                Invoke(nameof(DestroyMe), destructionDelay);
        }

        /// <summary>
        /// Allows for destruction of prefabs with this component attached via a public method
        /// </summary>
        /// <param name="delay"> The delay before destruction in seconds. </param>
        public void DelayedDestroyMe(float delay)
        {
            Invoke(nameof(DestroyMe), delay);
        }

        /// <summary>
        /// Destroys the game object
        /// </summary>
        public void DestroyMe()
        {
            Destroy(gameObject);
        }
    }
}