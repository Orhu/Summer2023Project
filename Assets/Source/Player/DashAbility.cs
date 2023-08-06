using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Allows dashing
    /// </summary>
    public class DashAbility : MonoBehaviour
    {
        [Tooltip("The speed multiplier of the dash")]
        [SerializeField] private float speedMultiplier;

        [Tooltip("The amount of time the dash is active for")]
        [SerializeField] private float time;

        [Tooltip("The amount of time after a dash where you can't dash again")]
        [SerializeField] private float cooldown;

        [Tooltip("The amount of damage the dash does")]
        [SerializeField] private int damage;

        [Tooltip("The amount that dashing through projectiles decreases card's cooldowns by in seconds")]
        [SerializeField] private float cardCooldownSubtraction;

        [Tooltip("The aim indicator sprite")]
        [SerializeField] private GameObject aimIndicator;

        [Tooltip("The components to enable when dashing")]
        [SerializeField] private List<MonoBehaviour> componentsToEnable;

        [Tooltip("The components to disable when dashing")]
        [SerializeField] private List<MonoBehaviour> componentsToDisable;

        // Tracks whether it's currently possible to dash
        [System.NonSerialized] public bool canDash;

        // Tracks whether a dash is currently happening
        [System.NonSerialized] public bool currentlyDashing;

        // Tracks the current deck to decrease cooldowns of
        private Deck deck = null;

        public void StartDash(Movement movement, Vector2 dashDirection, Deck deck = null)
        {
            if (!canDash || currentlyDashing) { return; }

            currentlyDashing = true;
            canDash = false;
            this.deck = deck;

            foreach (MonoBehaviour component in componentsToEnable)
            {
                component.enabled = true;
            }
            foreach (MonoBehaviour component in componentsToDisable)
            {
                component.enabled = false;
            }

            StartCoroutine(Dash(movement, dashDirection));
        }

        private IEnumerator Dash(Movement movement, Vector2 dashDirection)
        {

        }

        private void EndDash()
        {
            currentlyDashing = false;
            deck = null;

            foreach (MonoBehaviour component in componentsToEnable)
            {
                component.enabled = false;
            }
            foreach (MonoBehaviour component in componentsToDisable)
            {
                component.enabled = true;
            }
        }
    }
}