using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Cardificer
{
    /// <summary>
    /// MainMenu manager script for handling rendering and logic
    /// for the main menu UI object
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Tooltip("Deactivate if no save file exists.")]
        [SerializeField] private Button continueButton;

        [Tooltip("Display this if the user wishes to override their save")]
        [SerializeField] private GameObject overrideSavePopup;

        // Local bool to know if there is already a save.
        private bool saveExists;

        /// <summary>
        /// Check to see if autosave exists already
        /// if it does, show continue button,
        /// if not, only show new game
        /// 
        /// Also, set verion number here
        /// </summary>
        private void Start()
        {
            Time.timeScale = 1;
            if (SaveManager.autosaveExists)
            {
                saveExists = true;
                continueButton.gameObject.SetActive(true);
            }
            else
            {
                saveExists = false;
                continueButton.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// If a save is found. continue the game
        /// from that save point
        /// </summary>
        public void Continue()
        {
            if (saveExists)
            {
                if (FloorSceneManager.LoadFloor(SaveManager.savedCurrentFloor)) { return; }

                // If load failed
                TransitionToFirstLevel();
                SaveManager.AutosaveCorrupted("Floor " + SaveManager.savedCurrentFloor + " does not exist");
            }
            else
            {
                throw new System.Exception("SHOULD NOT BE ABLE TO CONTINUE WITHOUT SAVE");
            }
        }

        /// <summary>
        /// If a save is found, delete the save and start
        /// a new game
        /// If no save is found, start a new game
        /// </summary>
        public void NewGame()
        {
            if (saveExists)
            {
                // Prompt the user if they're sure they want
                // to override the save
                overrideSavePopup.SetActive(true);
            }
            else
            {
                // Ensure player sees the manual again on new game
                PlayerPrefs.SetFloat("seenManual", 0);
                // initiate new game
                TransitionToFirstLevel();
            }
        }

        /// <summary>
        /// When the user presses "Yes" to overriding their save
        /// </summary>
        public void ConfirmNewGame()
        {
            // Clear all current saves
            SaveManager.ClearTransientSaves();
            // Ensure player sees the manual again on new game
            PlayerPrefs.SetFloat("seenManual", 0);
            // Transition
            TransitionToFirstLevel();
        }

        /// <summary>
        /// Uses FloorSceneManager to transition us to the first scene / level.
        /// </summary>
        private void TransitionToFirstLevel()
        {
            FloorSceneManager.LoadFloor(0);
        }

        /// <summary>
        /// Exit the game from the main menu
        /// </summary>
        public void Quit()
        {
            Application.Quit();
        }
    }

}
