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
        [Tooltip("Deactivate if no save file exists.")] // TODO: Delete when converted to a menu
        [SerializeField] private GameObject initialSelection;

        /// <summary>
        /// TODO: Also, set verion number here
        /// </summary>
        private void Start()
        {
            Time.timeScale = 1;
            EventSystem.current.SetSelectedGameObject(initialSelection);
        }

        private void Update()
        {
            if (MenuManager.usingNavigation && (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy))
            {
                EventSystem.current.SetSelectedGameObject(initialSelection);
            }
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
