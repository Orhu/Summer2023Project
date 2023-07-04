using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a manager singleton that allows for control of the boss health bar in the HUD
/// </summary>
public class BossHealthbarManager : MonoBehaviour
{
    [Tooltip("Duration to fill the health bar during the health bar starting animation, in seconds")]
    [SerializeField] private float healthFillDuration;
    
    // The instance of this boss healthbar manager
    [HideInInspector] public static BossHealthbarManager instance;

    // The reference to the slider component
    private Slider slider;

    // Tracks time the health bar was initiated
    private float startTime;

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
        startTime = Time.time;
        slider.maxValue = maxHealth;
        slider.value = 0;
        StartCoroutine(FillHealthbar(maxHealth));
    }

    /// <summary>
    /// When called, animates the health bar to fill to the newly provided value (ie damage taken)
    /// </summary>
    /// <param name="newValue"> New value to set the health bar to </param>
    public void UpdateHealth(float newValue)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FillHealthbar(newValue));
    }

    /// <summary>
    /// Animates the health bar to fill to the provided value
    /// </summary>
    /// <param name="healthVal"> New value to set the health bar to </param>
    /// <returns> Waits one frame between every fill step </returns>
    IEnumerator FillHealthbar(float healthVal)
    {
        while (Mathf.Abs(slider.value - healthVal) > 0.01)
        {
            float step = (Time.time - startTime) / healthFillDuration;
            slider.value = Mathf.SmoothStep(slider.value, healthVal, step);
            yield return null; // wait one frame between each loop
        }
    }
}
