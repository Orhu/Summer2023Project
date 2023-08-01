using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Component that allows for something to channel in order to reset its cooldowns.
    /// </summary>\
    [RequireComponent(typeof(PlayerController))]
    public class DashAbility : MonoBehaviour
    {
        [Tooltip("The cooldown between dashes (seconds)")] [Min(0f)]
        [SerializeField] private float cooldown = 1f;

        [Tooltip("The speed multiplier of the dash (multiplies the player speed)")] [Min(0f)]
        [SerializeField] private float speedMultiplier = 3f;

        [Tooltip("The time a dash is active for")] [Min(0f)]
        [SerializeField] private float time = 0.2f;

        [Tooltip("The damage that a dash does")]
        [SerializeField] private int damage = 1;

        [Tooltip("The amount that card cooldowns are reduced by")]
        [SerializeField] private float cooldownReduction = 0.5f;

        [Tooltip("The collisions to disable")]
        [SerializeField] private List<CapsuleCollider2D> hitboxesToDisable;

        [Tooltip("The collisions to enable")]
        [SerializeField] private List<CapsuleCollider2D> hitboxesToEnable;

        [Tooltip("The visual indicator of whether you can dash or not")]
        [SerializeField] private GameObject dashIndicator;

        // Whether or not a dash is currently happening
        [System.NonSerialized] public bool dashing = false;

        // Whether or not a dash can be performed
        [System.NonSerialized] public bool canDash = true;

        // A reference to the player controller
        [System.NonSerialized] private PlayerController playerController;

        // A reference to the movement component of the player
        [System.NonSerialized] private Movement movement;

        /// <summary>
        /// Sets the reference to the player controller
        /// </summary>
        private void Start()
        {
            playerController = GetComponent<PlayerController>();
            movement = GetComponent<Movement>();
        }

        /// <summary>
        /// Starts a dash
        /// </summary>
        public void StartDash()
        {
            if (canDash)
            {
                StartCoroutine(nameof(Dash));
            }
        }

        /// <summary>
        /// Performs the dash
        /// </summary>
        /// <returns> Waits for the dash time </returns>
        private IEnumerator Dash()
        {
            dashing = true;
            canDash = false;
            playerController.movingEnabled = false;
            movement.movementInput = playerController.attemptedMovementInput * speedMultiplier;
            dashIndicator.SetActive(false);

            if (damage > 0 && hitboxesToDisable != null && hitboxesToEnable != null)
            {
                foreach (CapsuleCollider2D hitbox in hitboxesToDisable)
                {
                    hitbox.enabled = false;
                }
                foreach (CapsuleCollider2D hitbox in hitboxesToEnable)
                {
                    hitbox.enabled = true;
                }
            }

            yield return new WaitForSeconds(time);

            playerController.movingEnabled = true;

            if (damage > 0 && hitboxesToDisable != null && hitboxesToEnable != null)
            {
                foreach (CapsuleCollider2D hitbox in hitboxesToDisable)
                {
                    hitbox.enabled = true;
                }
                foreach (CapsuleCollider2D hitbox in hitboxesToEnable)
                {
                    hitbox.enabled = false;
                }
            }

            yield return new WaitForSeconds(cooldown);

            dashIndicator.SetActive(true);
            canDash = true;
        }

        /// <summary>
        /// Handles the dash damage
        /// </summary>
        /// <param name="collision"> The thing we've collided with </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (dashing && collision.gameObject.GetComponent<Health>() != null)
            {
                DamageData dashDamage = new DamageData(new List<StatusEffect>(), gameObject);
                dashDamage.damage = damage;
                collision.gameObject.GetComponent<Health>().ReceiveAttack(dashDamage);

                foreach (KeyValuePair<int, float> cardIndexToCooldown in new Dictionary<int, float>(Deck.playerDeck.cardIndicesToCooldowns))
                {
                    float newValue = cardIndexToCooldown.Value - cooldownReduction;
                    if (newValue <= 0)
                    {
                        Deck.playerDeck.cardIndicesToCooldowns.Remove(cardIndexToCooldown.Key);
                    }
                    else
                    {
                        Deck.playerDeck.cardIndicesToCooldowns[cardIndexToCooldown.Key] = newValue;
                    }
                }
            }
        }
    }
}