using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys the game object
/// </summary>
public class Destroy : MonoBehaviour
{
    /// <summary>
    /// Destroys the game object after a delay.
    /// </summary>
    /// <param name="delay"> The delay before destruction in seconds. </param>
    public void DelayedDestroyMe(float delay)
    {
        Invoke("DestroyMe", delay);
    }

    /// <summary>
    /// Destroys the game object
    /// </summary>
    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
