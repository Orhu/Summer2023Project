using UnityEngine;

namespace Cardificer
{
    [RequireComponent(typeof(Collider2D))]
    public class HealthPickup : MonoBehaviour
    {
        [Tooltip("The number of quarter hearts to heal")]
        [Min(1)]
        public int healAmount = 1;

        /// <summary>
        /// Pickup health.
        /// </summary>
        /// <param name="collision"> The thing to test if it is the player. </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Health playerHealth = collision.GetComponentInParent<Health>();

                // Make sure that the player will actually heal from picking this heart up.
                if (playerHealth.currentHealth < playerHealth.maxHealth) 
                {
                    playerHealth.Heal(healAmount);
                    AudioManager.instance.PlaySoundBaseAtPos(SoundGetter.Instance.healthPickupSound, transform.position, gameObject.name);
                    Destroy(gameObject);
                }
            }
        }
    }
}