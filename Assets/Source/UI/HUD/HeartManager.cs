using UnityEngine;
using UnityEngine.UI;

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
        playerHealthScript = Player.Get().GetComponent<Health>();
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
        int fullHearts = currentPlayerHealth / 4;
        int remainder = currentPlayerHealth % 4;

        for (int i = 0; i < fullHearts; i++)
        {
            Instantiate(heartCounterPrefab, transform);
        }

        if (remainder > 0)
        {
            GameObject lastHeart = Instantiate(heartCounterPrefab, transform);
            // Adjust the fill amount of the last heart based on the remainder
            int spriteIndex = Mathf.Clamp(remainder-1, 0, heartSpriteVariations.Length - 1);
            lastHeart.GetComponent<Image>().sprite = heartSpriteVariations[spriteIndex];
        }

        if (currentPlayerHealth > playerHealthScript.maxHealth / 2)
        {
            transform.GetChild(transform.childCount - 1).GetComponent<Animator>().Play("A_Heart_Enlarge");
        }
        else if (currentPlayerHealth <= playerHealthScript.maxHealth / 2)
        {
            transform.GetChild(transform.childCount - 1).GetComponent<Animator>().Play("A_Heart_Danger");
        }
        
    }
}
