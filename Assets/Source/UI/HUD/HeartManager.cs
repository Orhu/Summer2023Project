using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        for (int i = 0; i < currentPlayerHealth / 4; i++)
        {
            Instantiate(heartCounterPrefab, transform);
        }
    }
}
