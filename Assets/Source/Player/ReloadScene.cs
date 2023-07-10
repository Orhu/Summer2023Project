using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cardificer
{
    /// <summary>
    /// Script that reloads the current scene.
    /// </summary>
    public class ReloadScene : MonoBehaviour
    {
        /// <summary>
        /// Reloads the current scene.
        /// </summary>
        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Unity event for opening the game over menu.
        /// </summary>
        public void OpenGameOverMenuEvent()
        {
            MenuManager.OpenGameOverMenu();
        }
    }
}