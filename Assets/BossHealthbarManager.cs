using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void Awake()
    {
        instance = this;
        slider = GetComponentInChildren<Slider>();
        gameObject.SetActive(false);
    }
    
    public void StartHealthbar(float maxHealth)
    {
        gameObject.SetActive(true);
        startTime = Time.time;
        slider.maxValue = maxHealth;
        slider.value = 0;
        StartCoroutine(FillHealthbar(maxHealth));
    }

    public void UpdateHealth(float newValue)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FillHealthbar(newValue));
    }

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
