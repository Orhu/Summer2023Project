using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cardificer
{
    /// <summary>
    /// Pause manager script for handling rendering and logic
    /// for the pause menu UI object
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Tooltip("Main container that opens when you pause the game (not options or anything)")]
        [SerializeField] private GameObject mainPauseMenuContainer;
        [Tooltip("Use this to close all menu containers without closing things like game title.")]
        [SerializeField] private GameObject pauseMenuController;
        /// <summary>
        /// Resumes the game
        /// </summary>
        public void Resume()
        {
            MenuManager.instance.CloseMenu();
        }

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
        /// saves, then returns the player to the main menu
        /// </summary>
        public void SaveAndQuit()
        {
            SceneManager.LoadScene("MainMenu");
        }
        /// <summary>
        /// When the pause menu is reenabled, 
        /// set the main pause menu to be active again
        /// </summary>
        private void OnEnable()
        {
            mainPauseMenuContainer.SetActive(true);
        }

        /// <summary>
        /// When the pause menu is disabled, set the mainPauseMenu to be the active menu
        /// (instead of like, options menu)
        /// </summary>
        private void OnDisable()
        {
            // close all menus
            for (int i = 0; i < pauseMenuController.transform.childCount; i++)
            {
                pauseMenuController.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}