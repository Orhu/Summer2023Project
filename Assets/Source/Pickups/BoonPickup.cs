using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// If the player triggers this object, then they will receive the boon.
    /// </summary>
    public class BoonPickup : MonoBehaviour
    {
        [Tooltip("The boon to give")]
        public Boon boon;

        /// <summary>
        /// Pickup boon.
        /// </summary>
        /// <param name="collision"> Thing that collided with boon ground pickup </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                boon.Apply();
                Destroy(gameObject);
            }
        }
    }
}