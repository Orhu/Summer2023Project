using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartManager : MonoBehaviour
{
    // First heart shown
    public GameObject forwardHeartPrefab;
    // All subsequent hearts
    public GameObject sideHeartCounterPrefab;
    // Copy of the player health
    private Health playerHealthScript;
    // Local variable - what the UI thinks the player's health is
    private int currentPlayerHealth;

    /// <summary>
    /// First time initialize UI hearts
    /// </summary>
    void Start()
    {
        playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
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
        
        for (int i = 0; i < currentPlayerHealth; i++)
        {
            if (i == 0)
            {
                Instantiate(forwardHeartPrefab, transform);
            }
            else
            {
                Instantiate(sideHeartCounterPrefab, transform);
            }
        }
    }
}
