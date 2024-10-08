using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cardificer
{
    /// <summary>
    /// Game Over manager script for handling rendering and logic
    /// for the Game Over menu UI object
    /// </summary>
    public class GameOverMenu : Menu
    {
        [Tooltip("Animator for the player death")]
        [SerializeField] private Animator playerDeathAnim;

        /// <summary>
        /// Assign reference variables
        /// </summary>
        void Awake()
        {
            if (playerDeathAnim == null)
            {
                playerDeathAnim = GetComponentInChildren<Animator>();
            }
        }

        /// <summary>
        /// Ability to reset the game.
        /// Clears saves and reloads the current scene
        /// </summary>
        public void Restart()
        {
            // Reset our current menu
            MenuManager.CloseAllMenus(true);
            SaveManager.ClearTransientSaves();
            FloorSceneManager.LoadFloor(0);
        }

        /// <summary>
        /// Asks for the user to confirm they want to quit,
        /// saves, then returns the player to the main menu
        /// </summary>
        public void SaveAndQuit()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}