using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
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
