using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton manager class for UI menus
/// </summary>
public class MenuManager : MonoBehaviour
{
    // Singleton for the menu manager
    [HideInInspector]public static MenuManager instance;
    // Booster pack menu reference, assigned in inspector
    [SerializeField]private BoosterPackMenu boosterPackMenu;
    // Reference to the player's game object
    private GameObject playerGameObject;

    /// <summary>
    /// Assign singleton variable
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            playerGameObject = Player.Get();
            if (boosterPackMenu == null)
            {
                boosterPackMenu = GetComponentInChildren<BoosterPackMenu>();
            }
        } 
    }

    /// <summary>
    /// Opens the booster pack menu and populates the cards
    /// </summary>
    /// <param name="boosterPack">Booster pack prefab used in populating cards</param>
    public static void OpenBoosterPackMenu(BoosterPack boosterPack)
    {
        // "Pause the game", should probably be replaced with a more effective method
        // Sets timeScale to 0, so all time related functions are stopped
        Time.timeScale = 0;
        instance.boosterPackMenu.gameObject.SetActive(true);
        instance.boosterPackMenu.boosterPackObject = boosterPack;
        // Disable player movement
        instance.playerGameObject.GetComponent<InputHandler>().enabled = false;
    }

    /// <summary>
    /// Called when any menu needs to be closed
    /// </summary>
    public void CloseMenu()
    {
        // "Resume the game", resumes all time related function
        Time.timeScale = 1;
        // Re-enable player movement
        playerGameObject.GetComponent<InputHandler>().enabled = true;

        // close all menus
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
