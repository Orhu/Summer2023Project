using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// A component for rendering Player's heart count to screen
    /// </summary>
    public class HeartManager : MonoBehaviour
    {
        [Tooltip("Prefab representing player hearts")]
        public GameObject heartCounterPrefab;
        // Copy of the player health
        private Health playerHealthScript;
        // Local variable - what the UI thinks the player's health is
        private int currentPlayerHealth;

        [Tooltip("Collection of heart sprite variants, remainder of heart based on index")]
        [SerializeField] private Sprite[] heartSpriteVariations;

        /// <summary>
        /// First time initialize UI hearts
        /// </summary>
        void Start()
        {
            playerHealthScript = Player.health;
            currentPlayerHealth = playerHealthScript.maxHealth;
            UpdateHeartManager();
        }

        /// <summary>
        /// Observer pattern, check local health with player's current health
        /// if there is a change, update health in UI
        /// </summary>
        private void Update()
        {
            if (currentPlayerHealth != playerHealthScript.currentHealth)
            {
                currentPlayerHealth = playerHealthScript.currentHealth;
                ResetHeartManager();
                UpdateHeartManager();
            }
        }

        /// <summary>
        /// Remove all hearts from UI
        /// </summary>
        void ResetHeartManager()
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Instantiate all player hearts in UI
        /// </summary>
        void UpdateHeartManager()
        {
            // Number of totally full hearts, no half or quarter hearts
            int fullHearts = currentPlayerHealth / 4;
            // How much left over from a multiple of 4
            int remainder = currentPlayerHealth % 4;

            // Create all full hearts
            for (int i = 0; i < fullHearts; i++)
            {
                Instantiate(heartCounterPrefab, transform);
            }

            // If there is some remainder leftover
            if (remainder > 0)
            {
                GameObject lastHeart = Instantiate(heartCounterPrefab, transform);
                // Adjust the fill amount of the last heart based on the remainder
                int spriteIndex = Mathf.Clamp(remainder - 1, 0, heartSpriteVariations.Length - 1);
                // Set the image of the last heart based on the remainder
                lastHeart.GetComponent<Image>().sprite = heartSpriteVariations[spriteIndex];
            }

            // Play animation on the last heart
            if (currentPlayerHealth > 4) // When the health is normal
            {
                transform.GetChild(transform.childCount - 1).GetComponent<Animator>().Play("A_Heart_Enlarge");
            }
            else if (currentPlayerHealth <= 4) // When the player is close to death
            {
                transform.GetChild(transform.childCount - 1).GetComponent<Animator>().Play("A_Heart_Danger");
            }

        }
    }
}