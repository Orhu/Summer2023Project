using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pause manager script for handling rendering and logic
/// for the pause menu UI object
/// </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// Debug ability to reset the game.
    /// Clears saves and reloads the current scene
    /// </summary>
    public void Reset()
    {
        MenuManager.instance.CloseMenu();
        SaveManager.ClearTransientSaves();
        Player.Get().GetComponent<ReloadScene>().ReloadCurrentScene();
    }

    /// <summary>
    /// Render the Options portion of the Pause Menu
    /// </summary>
    public void Options()
    {

    }

    /// <summary>
    /// Asks for the user to confirm they want to quit,
    /// saves, then exits the game.
    /// </summary>
    public void SaveAndQuit()
    {

    }
}
