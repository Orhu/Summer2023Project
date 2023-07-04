using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
                TransitionToFirstLevel();
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
            // Transition
            TransitionToFirstLevel();
        }

        /// <summary>
        /// Uses SceneManager to transition us to the first scene / level.
        /// </summary>
        private void TransitionToFirstLevel()
        {
            SceneManager.LoadScene("Floor 1");
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
