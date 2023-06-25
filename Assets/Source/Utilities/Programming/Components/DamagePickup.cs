using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Decreases the players cooldowns in an exponential decay fashion.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DamagePickup : MonoBehaviour
    {
        [Tooltip("The amount to add to the player's current damage multiplier.")] [Range(0, 1)]
        [SerializeField] private float damageIncrease = 0.9f;

        /// <summary>
        /// Pickup reduced cooldowns.
        /// </summary>
        /// <param name="collision"> The thing to test if it is the player. </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Player.Get().GetComponent<PlayerController>().damageMultiplier += damageIncrease;
                Destroy(gameObject);
            }
        }
    }
}