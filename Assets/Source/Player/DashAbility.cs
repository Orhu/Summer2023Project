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
        [SerializeField] private float dashCooldown = 1f;

        [Tooltip("The speed multiplier of the dash (multiplies the player speed)")] [Min(0f)]
        [SerializeField] private float dashSpeedMultiplier = 3f;

        [Tooltip("The time a dash is active for")] [Min(0f)]
        [SerializeField] private float dashTime = 0.2f;

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
            movement.movementInput = playerController.attemptedMovementInput * dashSpeedMultiplier;

            yield return new WaitForSeconds(dashTime);

            playerController.movingEnabled = true;

            yield return new WaitForSeconds(dashCooldown);

            canDash = true;
        }
    }
}