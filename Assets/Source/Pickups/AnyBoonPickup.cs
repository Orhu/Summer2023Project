using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A pickup that opens the pick a boon menu.
    /// </summary>
    public class AnyBoonPickup : MonoBehaviour
    {
        /// <summary>
        /// Open pick a boon menu
        /// </summary>
        /// <param name="collision"> Thing that collided with boon ground pickup </param>
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