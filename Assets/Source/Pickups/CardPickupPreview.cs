using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Enables the card renderer when the trigger is entered.
    /// </summary>
    public class CardPickupPreview : MonoBehaviour
    {
        /// <summary>
        /// Activates the card's previewer.
        /// </summary>
        /// <param name="other"> If is the player then activate. </param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) { return; }

            transform.GetChild(0).gameObject.SetActive(true);
        }

        /// <summary>
        /// Deactivates the card's previewer.
        /// </summary>
        /// <param name="other"> If is the player then deactivate. </param>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) { return; }

            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}