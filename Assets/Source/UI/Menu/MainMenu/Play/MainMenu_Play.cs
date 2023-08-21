using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cardificer
{ 
    /// <summary>
    /// Handles the functionality of the play screen on the main menu.
    /// </summary>
    public class MainMenu_Play : Menu
    {
        [Tooltip("The button bound to the Continue() function.")]
        [SerializeField] private Button continueButton;

        [Tooltip("The button bound to the NewGame() function.")]
        [SerializeField] private Button newGameButton;

        [Tooltip("The button bound to the Tutorial() function.")]
        [SerializeField] private Button tutorialButton;

        /// <summary>
        /// Set initial selection.
        /// </summary>
        private void OnEnable()
        {
            initialSelection = (SaveManager.autosaveExists ? continueButton : (SaveManager.tutorialCompleted ? newGameButton : tutorialButton)).gameObject;
        }

        /// <summary>
        /// Reset initial selection.
        /// </summary>
        private void OnDisable()
        {
            initialSelection = null;
        }

        /// <summary>
        /// Continues the current game.
        /// </summary>
        public void Continue()
        {
            if (!SaveManager.autosaveExists)
            {
                throw new System.Exception("Tried to continue game when no autosave exists");
            }

            if (!FloorSceneManager.LoadFloor(SaveManager.savedCurrentFloor))
            {
                SaveManager.AutosaveCorrupted("Floor " + SaveManager.savedCurrentFloor + " does not exist");
            }
            MenuManager.Close<MainMenu_Play>();
        }

        /// <summary>
        /// Clears autosaves and starts a new game.
        /// </summary>
        public void NewGame()
        {
            SaveManager.ClearTransientSaves();
            FloorSceneManager.LoadFloor(0);
            MenuManager.Close<MainMenu_Play>();
        }

        /// <summary>
        /// Clears autosaves and starts a new game.
        /// </summary>
        public void Tutorial()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync("Tutorial");
            MenuManager.Open<LoadingScreen>(true, true);
            MenuManager.Close<MainMenu_Play>();
        }

        /// <summary>
        /// Clears autosaves and starts a new game.
        /// </summary>
        public void Sanctum()
        {
            MenuManager.Close<MainMenu_Play>();
            throw new System.NotImplementedException("Sanctum hasn't been made yet.");
        }
    }
}