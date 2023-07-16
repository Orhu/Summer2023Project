using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a manager singleton that allows for control of the boss health bar in the HUD
/// </summary>
public class BossHealthbarManager : MonoBehaviour
{
    [Tooltip("Duration of animation between health values, in seconds")]
    [SerializeField] private float healthFillDuration;
    
    // The instance of this boss healthbar manager
    public static BossHealthbarManager instance;

    // The reference to the slider component
    private Slider slider;

    /// <summary>
    /// Initialize components and variables, then disable the health bar 
    /// </summary>
    void Awake()
    {
        instance = this;
        slider = GetComponentInChildren<Slider>();
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// When called, initializes the health bar with the given max health value, beginning the animation
    /// </summary>
    /// <param name="maxHealth"> Amount of health for the boss health bar to represent </param>
    public void StartHealthbar(float maxHealth)
    {
        gameObject.SetActive(true);
        slider.maxValue = maxHealth;
        slider.value = 0;
        StartCoroutine(AnimateHealthbar(maxHealth));
    }

    /// <summary>
    /// When called, animates the health bar to fill to the newly provided value (ie damage taken)
    /// </summary>
    /// <param name="newValue"> New value to set the health bar to </param>
    public void UpdateHealth(float newValue)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AnimateHealthbar(newValue));
    }

    /// <summary>
    /// Resets all values to default and disables the health bar
    /// </summary>
    public void DisableHealthbar()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Animates the health bar to fill to the provided value
    /// </summary>
    /// <param name="healthVal"> New value to set the health bar to </param>
    /// <returns> Waits one frame between every fill step </returns>
    private IEnumerator AnimateHealthbar(float healthVal)
    {
        float startValue = slider.value;
        float elapsedTime = 0f;

        while (elapsedTime < healthFillDuration)
        {
            float normalizedTime = elapsedTime / healthFillDuration;
            float easedValue = Mathf.Lerp(startValue, healthVal, normalizedTime);

            slider.value = easedValue;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the health bar reaches the target value
        slider.value = healthVal;
    }
}
