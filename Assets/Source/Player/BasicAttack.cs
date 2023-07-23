using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A script controlling a basic attack for the player.
    /// </summary>
    public class BasicAttack : MonoBehaviour
    {
        [Tooltip("The action to be played as a basic attack.")]
        public Action basicAttackAction;

        [Tooltip("The rate of fire of the attack, measured in seconds between attacks.")]
        public float fireRate = 1.0f;

        // determines if the basic attack is actively trying to be fired or not.
        private bool isFiring = false;

        // reference to the Coroutine that controls basic attack cooldown.
        private Coroutine firingCoroutine;
        private bool firingCoroutineRunning = false;

        /// <summary>
        /// Begins the firing coroutine.
        /// </summary>
        public void StartFiringBasicAttack() 
        {
            if (isFiring) { return; }
            isFiring = true;
            if (!firingCoroutineRunning) {
                firingCoroutine = StartCoroutine(FireBasicAttack());
            }
        }

        /// <summary>
        /// Ends the firing coroutine.
        /// </summary>
        public void StopFiringBasicAttack() 
        {
            if (!isFiring) { return; }
            isFiring = false;
        }

        /// <summary>
        /// Ticks down the cooldown between fires of the basic attack.
        /// </summary>
        private IEnumerator FireBasicAttack() 
        {
            firingCoroutineRunning = true;
            while (isFiring)
            {
                basicAttackAction.Play(Deck.playerDeck.actor);
                yield return new WaitForSeconds(fireRate);
            }
            firingCoroutineRunning = false;
        }
    }
}
