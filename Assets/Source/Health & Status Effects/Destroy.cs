using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys the game object
/// </summary>
public class Destroy : MonoBehaviour
{
    /// <summary>
    /// Destroys the game object
    /// </summary>
    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
