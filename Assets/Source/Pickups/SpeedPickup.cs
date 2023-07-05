using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Increases the player's max speed when picked up.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SpeedPickup : MonoBehaviour
    {
        [Tooltip("The number of tiles/s to increase max speed by.")] [Min(0)]
        public float maxSpeedIncrease = 0.33333333333333333333f;

        /// <summary>
        /// Pickup max speed.
        /// </summary>
        /// <param name="collision"> The thing to test if it is the player. </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponentInParent<SimpleMovement>().maxSpeed += maxSpeedIncrease;
                Destroy(gameObject);
            }
        }
    }
}