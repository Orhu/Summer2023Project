using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cardificer
{ 
    public class MainMenu_Play : MonoBehaviour
    {
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
        }

        /// <summary>
        /// Clears autosaves and starts a new game.
        /// </summary>
        public void NewGame()
        {
            SaveManager.ClearTransientSaves();
            FloorSceneManager.LoadFloor(0);
        }

        /// <summary>
        /// Clears autosaves and starts a new game.
        /// </summary>
        public void Tutorail()
        {
            // TODO: Implement loading screen.
            SceneManager.LoadSceneAsync("Tutorial");
        }

        /// <summary>
        /// Clears autosaves and starts a new game.
        /// </summary>
        public void Sanctum()
        {
            // TODO: Implement loading screen.
            throw new System.NotImplementedException("Sanctum hasn't been made yet.");
        }
    }
}