using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageFromCertainProjectile : MonoBehaviour
{
    [SerializeField] private string projectileTag;
    [SerializeField] private int damageOfProjectile; 
    // TODO gonna want to pull this from the projectile prefab instead of serializing

    private Health healthComponent;
    
    void Start()
    {
        try
        {
            healthComponent = gameObject.GetComponent<Health>();
        }
        catch (Exception e)
        {
            Debug.LogError("Attempting to TakeDamageFromCertainProjectile but no Health component found");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(projectileTag))
        {
            // if this is the correct projectile type, take damage
            healthComponent.CurrentHealth -= damageOfProjectile;
        }
    }
}
