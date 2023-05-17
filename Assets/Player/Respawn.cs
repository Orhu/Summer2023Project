using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that resets the health and moves the game object to the spawn location
/// </summary>
public class Respawn : MonoBehaviour
{

    [Tooltip("The location to respawn at")]
    public Vector2 spawnLocation = new Vector2(0, 0);

    /// <summary>
    /// Sets the spawn location
    /// </summary>
    /// <param name="newLocation"> The location to set the spawn location to</param>
    public void SetSpawnLocation(Vector2 newLocation)
    {
        spawnLocation = newLocation;
    }

    /// <summary>
    /// Resets the health and moves the game object to the spawn location
    /// </summary>
    public void RespawnAtSpawnLocation()
    {
        transform.position = new Vector3(spawnLocation.x, spawnLocation.y, 0);
        Health healthComponent = GetComponent<Health>();
        healthComponent?.Heal(healthComponent.maxHealth);
    }
}
