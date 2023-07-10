using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Singleton manager class for UI menus
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        // Singleton for the menu manager
        [HideInInspector] public static MenuManager instance;
        [Tooltip("Booster pack menu reference, assigned in inspector")]
        [SerializeField] private BoosterPackMenu boosterPackMenu;
        [Tooltip("Pause Menu reference, assigned in inspector")]
        [SerializeField] private PauseMenu pauseMenu;
        [Tooltip("Map Menu reference, assigned in inspector")]
        [SerializeField] private MapMenu mapMenu;
        [Tooltip("Card Menu reference, assigned in inspector")]
        [SerializeField] private CardMenu cardMenu;
        [Tooltip("Game Over Menu reference, assigned in inspector")]
        [SerializeField] private GameOverMenu gameOverMenu;
        // Reference to the player's game object
        private GameObject playerGameObject;
        // Know whether we currently have a menu open or not
        public bool menuOpen { get; private set; }

        /// <summary>
        /// Enum for each of the menu types
        /// </summary>
        private enum MenuTypes
        {
            Pause,
            Booster,
            Map,
            Card,
            GameOver
        }

        // Internally know which menu is open
        private MenuTypes currentMenu;

        /// <summary>
        /// Assign singleton variable
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                playerGameObject = Player.Get();

                // Assign menu if they aren't found
                if (boosterPackMenu == null)
                {
                    boosterPackMenu = GetComponentInChildren<BoosterPackMenu>();
                }
                if (pauseMenu == null)
                {
                    pauseMenu = GetComponentInChildren<PauseMenu>();
                }
                if (cardMenu == null)
                {
                    cardMenu = GetComponentInChildren<CardMenu>();
                }
                if (mapMenu == null)
                {
                    mapMenu = GetComponentInChildren<MapMenu>();
                }
                if (gameOverMenu == null)
                {
                    gameOverMenu = GetComponentInChildren<GameOverMenu>();
                }

                // The current menu is set to pause menu
                currentMenu = MenuTypes.Pause;
            }
        }

        /// <summary>
        /// Toggles whether the pause menu is open
        /// </summary>
        public static void TogglePause()
        {
            // Toggling pause always closes all menus
            if (instance.menuOpen)
            {
                instance.CloseMenu();
            }
            else
            {
                OpenPauseMenu();
            }
        }

        /// <summary>
        /// Toggles whether the card menu is open
        /// </summary>
        public static void ToggleCardMenu()
        {
            if (instance.menuOpen && instance.currentMenu == MenuTypes.Card)
            {
                instance.CloseMenu();
            }
            else
            {
                OpenCardMenu();
            }
        }

        /// <summary>
        /// Toggles whether the map menu is open
        /// </summary>
        public static void ToggleMap()
        {

            if (instance.menuOpen && instance.currentMenu == MenuTypes.Map)
            {
                instance.CloseMenu();
            }
            else
            {
                OpenMapMenu();
            }
        }

        /// <summary>
        /// Opens the booster pack menu and populates the cards
        /// </summary>
        /// <param name="boosterPack">Booster pack prefab used in populating cards</param>
        public static void OpenBoosterPackMenu(BoosterPack boosterPack)
        {
            if (!instance.menuOpen)
            {
                // "Pause the game", should probably be replaced with a more effective method
                // Sets timeScale to 0, so all time related functions are stopped
                Time.timeScale = 0;
                instance.boosterPackMenu.gameObject.SetActive(true);
                if (instance.boosterPackMenu.boosterPackObject != boosterPack)
                {
                    instance.boosterPackMenu.boosterPackObject = boosterPack;
                }
                // Disable player movement
                instance.playerGameObject.GetComponent<PlayerController>().enabled = false;
                instance.currentMenu = MenuTypes.Booster;
                instance.menuOpen = true;
            }
        }

        /// <summary>
        /// Opens the pause menu
        /// </summary>
        public static void OpenPauseMenu()
        {
            if (!instance.menuOpen)
            {
                // "Pause the game", should probably be replaced with a more effective method
                // Sets timeScale to 0, so all time related functions are stopped
                Time.timeScale = 0;
                instance.pauseMenu.gameObject.SetActive(true);
                // Disable player movement
                instance.playerGameObject.GetComponent<PlayerController>().enabled = false;
                instance.currentMenu = MenuTypes.Pause;
                instance.menuOpen = true;
            }
        }

        /// <summary>
        /// Opens the map menu
        /// </summary>
        public static void OpenMapMenu()
        {
            if (!instance.menuOpen)
            {
                // "Pause the game", should probably be replaced with a more effective method
                // Sets timeScale to 0, so all time related functions are stopped
                Time.timeScale = 0;
                instance.mapMenu.gameObject.SetActive(true);
                // Disable player movement
                instance.playerGameObject.GetComponent<PlayerController>().enabled = false;
                instance.currentMenu = MenuTypes.Map;
                instance.menuOpen = true;
            }
        }

        /// <summary>
        /// Opens the card menu
        /// </summary>
        public static void OpenCardMenu()
        {
            if (!instance.menuOpen)
            {
                // "Pause the game", should probably be replaced with a more effective method
                // Sets timeScale to 0, so all time related functions are stopped
                Time.timeScale = 0;
                instance.cardMenu.gameObject.SetActive(true);
                // Disable player movement
                instance.playerGameObject.GetComponent<PlayerController>().enabled = false;
                instance.currentMenu = MenuTypes.Card;
                instance.menuOpen = true;
            }
        }

        /// <summary>
        /// Opens the game over menu
        /// </summary>
        public static void OpenGameOverMenu()
        {
            if (!instance.menuOpen)
            {
                // "Pause the game", should probably be replaced with a more effective method
                // Sets timeScale to 0, so all time related functions are stopped
                Time.timeScale = 0;
                instance.gameOverMenu.gameObject.SetActive(true);
                // Disable player movement
                instance.playerGameObject.GetComponent<PlayerController>().enabled = false;
                instance.currentMenu = MenuTypes.GameOver;
                instance.menuOpen = true;
            }
        }

        /// <summary>
        /// Called when any menu needs to be closed
        /// </summary>
        public void CloseMenu()
        {
            if (menuOpen)
            {
                // "Resume the game", resumes all time related function
                Time.timeScale = 1;
                // Re-enable player movement
                playerGameObject.GetComponent<PlayerController>().enabled = true;

                // close all menus
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                menuOpen = false;
            }
        }
    }
}