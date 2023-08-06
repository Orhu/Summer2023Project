using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Allows dashing
    /// </summary>
    [RequireComponent(typeof(Movement))]
    public class DashAbility : MonoBehaviour
    {
        [Tooltip("Delegate called when a dash is begun")]
        [SerializeField] public System.Action onDashBegin;

        [Tooltip("Delegate called when a dash ends")]
        [SerializeField] public System.Action onDashEnd;

        [Tooltip("Delegate called when the dash cooldown is over and canDash is true again")]
        [SerializeField] public System.Action onCooldownEnd;

        [Tooltip("The speed multiplier of the dash (multiplies the movement component's speed")]
        [SerializeField] private float speedMultiplier;

        [Tooltip("The amount of time the dash is active for")]
        [SerializeField] private float time;

        [Tooltip("The amount of time after a dash where you can't dash again (seconds)")]
        [SerializeField] private float cooldown;

        [Tooltip("The amount of damage the dash does")]
        [SerializeField] private int damage;

        [Tooltip("The amount that dashing through projectiles decreases card's cooldowns by (seconds)")]
        [SerializeField] private float cardCooldownSubtraction;

        [Tooltip("The dash indicator")]
        [SerializeField] private GameObject indicator;

        [Tooltip("The components to enable when dashing")]
        [SerializeField] private List<MonoBehaviour> componentsToEnable;

        [Tooltip("The components to disable when dashing")]
        [SerializeField] private List<MonoBehaviour> componentsToDisable;

        // Tracks whether it's currently possible to dash
        [System.NonSerialized] public bool canDash;

        // Tracks whether a dash is currently happening
        [System.NonSerialized] public bool dashing;

        // A reference to the movement component
        [System.NonSerialized] private Movement movement;

        // Tracks the current deck to decrease cooldowns of
        private Deck deck = null;

        /// <summary>
        /// Sets the reference to the movement component
        /// </summary>
        private void Start()
        {
            movement = GetComponent<Movement>();
        }

        /// <summary>
        /// Starts a dash
        /// </summary>
        /// <param name="dashDirection"> The direction to dash in </param>
        /// <param name="deck"> The deck to reduce the cooldowns of </param>
        public void StartDash(Vector2 dashDirection, Deck deck = null)
        {
            if (!canDash || dashing || dashDirection == Vector2.zero) { return; }

            onDashBegin?.Invoke();

            dashing = true;
            canDash = false;
            this.deck = deck;
            movement.movementInput = dashDirection * speedMultiplier;

            if (indicator)
            {
                indicator.SetActive(true);
            }

            if (componentsToEnable != null)
            {
                foreach (MonoBehaviour component in componentsToEnable)
                {
                    component.enabled = true;
                }
            }

            if (componentsToDisable != null)
            {
                foreach (MonoBehaviour component in componentsToDisable)
                {
                    component.enabled = false;
                }
            }

            StartCoroutine(Dash());
        }

        /// <summary>
        /// Handles the timing of the dash
        /// </summary>
        /// <returns> Waits for the dash time to end the dash, then the cooldown time to reset canDash </returns>
        private IEnumerator Dash()
        {
            yield return new WaitForSeconds(time);
            EndDash();
            yield return new WaitForSeconds(cooldown);

            onCooldownEnd?.Invoke();

            if (indicator)
            {
                indicator.SetActive(true);
            }

            canDash = true;
        }

        /// <summary>
        /// Handles ending the dash
        /// </summary>
        private void EndDash()
        {
            onDashEnd?.Invoke();
            dashing = false;
            deck = null;

            if (indicator)
            {
                indicator.SetActive(false);
            }

            if (componentsToEnable != null)
            {
                foreach (MonoBehaviour component in componentsToEnable)
                {
                    component.enabled = false;
                }
            }

            if (componentsToDisable != null)
            {
                foreach (MonoBehaviour component in componentsToDisable)
                {
                    component.enabled = true;
                }
            }
        }

        /// <summary>
        /// Handles the dash overlap ineractions
        /// </summary>
        /// <param name="collision"> The thing we've collided with </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!dashing) { return; }

            if (damage > 0 && collision.gameObject.GetComponent<Health>() != null)
            {
                DamageData dashDamage = new DamageData(new List<StatusEffect>(), gameObject);
                dashDamage.damage = damage;
                collision.gameObject.GetComponent<Health>().ReceiveAttack(dashDamage);
            }

            if (deck != null) // and collision is projectile or enemy
            {
                foreach (KeyValuePair<int, float> cardIndexToCooldown in new Dictionary<int, float>(deck.cardIndicesToCooldowns))
                {
                    float newValue = cardIndexToCooldown.Value - cardCooldownSubtraction;
                    if (newValue <= 0)
                    {
                        deck.cardIndicesToCooldowns.Remove(cardIndexToCooldown.Key);
                    }
                    else
                    {
                        deck.cardIndicesToCooldowns[cardIndexToCooldown.Key] = newValue;
                    }
                }
            }
        }
    }
}