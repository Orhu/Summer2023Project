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
    public class GameOverMenu : MonoBehaviour
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
        /// When we enable the menu, play the player death animation
        /// </summary>
        private void OnEnable()
        {
            playerDeathAnim.Play("A_DeathScreen_PlayerSpin");
        }

        /// <summary>
        /// Ability to reset the game.
        /// Clears saves and reloads the current scene
        /// </summary>
        public void Restart()
        {
            // Reset our current menu
            MenuManager.instance.SetCurrentMenu(MenuManager.MenuTypes.Default);
            MenuManager.instance.CloseMenu();
            SaveManager.ClearTransientSaves();

            if (FloorSceneManager.IsValid())
            {
                // Load the first level
                FloorSceneManager.LoadFloor(0);
            }
            else
            {
                // Reload the current scene. This should only be allowed when not starting at the main menu, which the player always should.
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
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

