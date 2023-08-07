using UnityEngine;

namespace Cardificer
{
    public class AnyBoonPickup : MonoBehaviour
    {
        /// <summary>
        /// Pickup coins.
        /// </summary>
        /// <param name="collision"> If player give coins. </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                MenuManager.Open<PickABoonMenu>(lockOpen: true);
                Destroy(gameObject);
            }
        }
    }
}