using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that resets the health and moves the game object to the spawn location
/// </summary>
public class ReloadScene : MonoBehaviour
{
    /// <summary>
    /// Resets the health and moves the game object to the spawn location
    /// </summary>
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
