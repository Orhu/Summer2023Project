using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Decreases the players cooldowns in an exponential decay fashion.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CooldownPickup : MonoBehaviour
    {
        [Tooltip("The amount to multiply the current cooldown reduction by.")] [Range(0, 1)]
        [SerializeField] private float cooldownMultiplier = 0.9f;

        /// <summary>
        /// Pickup reduced cooldowns.
        /// </summary>
        /// <param name="collision"> The thing to test if it is the player. </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Deck.playerDeck.cooldownReduction *= cooldownMultiplier;
                Destroy(gameObject);
            }
        }
    }
}