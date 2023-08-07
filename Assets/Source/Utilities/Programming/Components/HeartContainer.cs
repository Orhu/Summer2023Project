using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for rendering a certain amount of hearts.
/// </summary>
public class HeartContainer : MonoBehaviour
{
    [Tooltip("Prefab representing player hearts")]
    public GameObject heartCounterPrefab;

    [Tooltip("Collection of heart sprite variants, remainder of heart based on index")]
    [SerializeField] private Sprite[] heartSpriteVariations;

    [Tooltip("Value that health must be at or below to initiate 'low health' (make the hearts flash red)")]
    [SerializeField] private int lowHealthThreshhold = 4;


    // The last number of hearts this rendered.
    private int lastHearts = -1; 

    /// <summary>
    /// Sets the amount of health this is rendering.
    /// </summary>
    /// <param name="newHearts"> The new health to render. </param>
    public void SetHearts(int newHearts)
    {
        if (lastHearts == newHearts) { return; }
        lastHearts = newHearts;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Number of totally full hearts, no half or quarter hearts
        int fullHearts = newHearts / heartSpriteVariations.Length;
        // How much left over from a multiple of 4
        int remainder = newHearts % heartSpriteVariations.Length;

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
        if (newHearts > lowHealthThreshhold) // When the health is normal
        {
            transform.GetChild(transform.childCount - 1).GetComponent<Animator>().Play("A_Heart_Enlarge");
        }
        else if (newHearts <= lowHealthThreshhold) // When the player is close to death
        {
            transform.GetChild(transform.childCount - 1).GetComponent<Animator>().Play("A_Heart_Danger");
        }
    }
}
