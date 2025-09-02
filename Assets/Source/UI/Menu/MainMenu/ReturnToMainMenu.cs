using UnityEngine; 
using UnityEngine.SceneManagement;

namespace Cardificer
{
    /// <summary>
    /// Component to allow a button to easily return to main menu
    /// </summary>
    public class ReturnToMainMenu : MonoBehaviour
    {
        public void ReturnToMenu()
        {
            SaveManager.ClearTransientSaves();
            SceneManager.LoadScene("MainMenu");
        }
    }

}
