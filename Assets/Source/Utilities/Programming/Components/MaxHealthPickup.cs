using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    [RequireComponent(typeof(Collider2D))]
    public class MaxHealthPickup : MonoBehaviour
    {
        [Tooltip("The number of quarter hearts to increase max health by")] [Min(1)]
        public int increaseAmount = 4;

        /// <summary>
        /// Pickup health.
        /// </summary>
        /// <param name="collision"> The thing to test if it is the player. </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponentInParent<Health>().maxHealth += increaseAmount;
                collision.GetComponentInParent<Health>().Heal(increaseAmount);
                Destroy(gameObject);
            }
        }
    }
}