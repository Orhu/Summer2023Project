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
        [SerializeField] [Min(0)] private float speedMultiplier;

        [Tooltip("The amount of time the dash is active for")]
        [SerializeField] [Min(0)] private float time;

        [Tooltip("The amount of time after a dash where you can't dash again (seconds)")]
        [SerializeField] [Min(0)] private float cooldown;

        [Tooltip("The damage that the dash does")]
        [SerializeField] private DamageData damage;

        [Tooltip("The amount that dashing through projectiles decreases card's cooldowns by (seconds)")]
        [SerializeField] [Min(0)] private float cardCooldownSubtraction;

        [Tooltip("The dash indicator; indicates whether or not dashing is possible")]
        [SerializeField] private GameObject indicator;

        [Tooltip("The colliders to enable when dashing")]
        [SerializeField] private List<Collider2D> collidersToEnable;

        [Tooltip("The colliders to disable when dashing")]
        [SerializeField] private List<Collider2D> collidersToDisable;

        [Tooltip("The sprites to enable when dashing")]
        [SerializeField] private List<SpriteRenderer> spritesToEnable;

        [Tooltip("The sprites to disable when dashing")]
        [SerializeField] private List<SpriteRenderer> spritesToDisable;

        [Tooltip("The layers that the dash should interact with")]
        [SerializeField] private LayerMask layers;

        // Tracks whether it's currently possible to dash
        [System.NonSerialized] public bool canDash = true;

        // Tracks whether a dash is currently happening
        [System.NonSerialized] public bool dashing = false;

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
            if (!canDash || dashing || dashDirection == Vector2.zero) 
            {
                return; 
            }

            onDashBegin?.Invoke();

            dashing = true;
            canDash = false;
            this.deck = deck;
            movement.movementInput = dashDirection * speedMultiplier;

            if (indicator)
            {
                indicator.SetActive(false);
            }

            SetComponentsEnabled(true);

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
            SetComponentsEnabled(false);
        }

        /// <summary>
        /// Sets the dash components to enabled, and the non-dash components to disabled. Swaps if "enabled" is false.
        /// </summary>
        /// <param name="enabled"> Swaps the behavior of what components will be enabled </param>
        private void SetComponentsEnabled(bool enabled)
        {
            if (collidersToEnable != null)
            {
                foreach (Collider2D collider in collidersToEnable)
                {
                    collider.enabled = enabled;
                }
            }

            if (collidersToDisable != null)
            {
                foreach (Collider2D collider in collidersToDisable)
                {
                    collider.enabled = !enabled;
                }
            }

            if (spritesToEnable != null)
            {
                foreach (SpriteRenderer sprites in spritesToEnable)
                {
                    sprites.enabled = enabled;
                }
            }

            if (spritesToDisable != null)
            {
                foreach (SpriteRenderer sprites in spritesToDisable)
                {
                    sprites.enabled = !enabled;
                }
            }
        }

        /// <summary>
        /// Handles the dash overlap ineractions
        /// </summary>
        /// <param name="collision"> The thing we've collided with </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!dashing || (layers & (1 << collision.gameObject.layer)) == 0) { return; }

            if (damage.damage > 0 && collision.gameObject.GetComponent<Health>() != null)
            {
                collision.gameObject.GetComponent<Health>().ReceiveAttack(damage);
            }

            deck?.SubtractFromCooldowns(cardCooldownSubtraction);
        }
    }
}