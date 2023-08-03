using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// Card printer manager script for handling rendering and logic
    /// for the card printer UI object
    /// </summary>
    public class CardPrinterMenu : MonoBehaviour
    {
        [Tooltip("Time taken to transition between different screens.")]
        [SerializeField] private float transitionTime;

        [Tooltip("The prefab used to draw cards")]
        [SerializeField] private GameObject cardRendererPrefab;

        [Tooltip("The links to the necessary components for rendering.")]
        [SerializeField] private ComponentLinks links;

        // Screen that the card printer is currently on
        private GameObject currentScreen;

        /// <summary>
        /// Struct for storing the needed component references.
        /// </summary>
        [System.Serializable]
        struct ComponentLinks
        {
            [Tooltip("The gameobject used to display the transition screen")]
            public GameObject TransitionScreen;

            [Tooltip("The gameobject used to display the Main screen")]
            public GameObject MainScreen;

            [Tooltip("The gameobject used to display the copier screen")]
            public GameObject CopierScreen;

            [Tooltip("The gameobject used to display the shredder screen")]
            public GameObject ShredderScreen;
        }

        /// <summary>
        /// What composes a game screen in code.
        /// 
        /// screenName - A way to find the screen in a collection
        /// screen - gameObject attached to the screen
        /// loadingText - text used to show you're loading the screen
        /// exitText - text used to show you're exiting the screen
        /// </summary>
        struct GameScreenComposition
        {
            public readonly string screenName;

            public readonly GameObject screen;

            public readonly string loadingText;

            public readonly string exitText;

            public GameScreenComposition(string screenName, GameObject screen, string loadingText, string exitText)
            {
                this.screenName = screenName;
                this.screen = screen;
                this.loadingText = loadingText;
                this.exitText = exitText;
            }
        }

        // Internally built list of screen "compositions" to be used in code.
        private List<GameScreenComposition> screenCompositions;

        /// <summary>
        /// Initialize the list of screen compositions
        /// </summary>
        private void Awake()
        {
            screenCompositions = new List<GameScreenComposition>(
            new[]
            {
            new GameScreenComposition("MainScreen", links.MainScreen, "Initializing service interface", "Exiting service interface. Have a nice day!"),
            new GameScreenComposition("CopyMachine",links.CopierScreen, "Initializing Copy_Machine.exe", "Exiting Copy_Machine.exe"),
            new GameScreenComposition("Shredder", links.ShredderScreen, "Initializing Card_Shredder.exe", "Exiting Card_Shredder.exe")
            });
        }

        /// <summary>
        /// When the card printer first comes on display the main screen loading text
        /// </summary>
        private void OnEnable()
        {
            StartCoroutine(DisplayTransitionScreen(screenCompositions.Find(screen => screen.screenName == "MainScreen"), true));
        }

        /// <summary>
        /// Display transitioning text between different screens.
        /// </summary>
        /// <param name="screenComposition">The composition of the screen you are transitioning to</param>
        /// <param name="isLoading">Whether this is loading text or exit text</param>
        /// <returns></returns>
        private IEnumerator DisplayTransitionScreen(GameScreenComposition screenComposition, bool isLoading, EndOfTransitionFunction endOfTransitionFunction = null)
        {
            if (currentScreen != null)
            {
                currentScreen.SetActive(false);
            }

            string textToDisplay;
            if (isLoading)
            {
                textToDisplay = screenComposition.loadingText;
            }
            else
            {
                textToDisplay = screenComposition.exitText;
            }
            links.TransitionScreen.SetActive(true);
            links.TransitionScreen.GetComponentInChildren<TextMeshProUGUI>().text = textToDisplay;
            yield return new WaitForSecondsRealtime(transitionTime / 4);
            links.TransitionScreen.GetComponentInChildren<TextMeshProUGUI>().text = textToDisplay + ".";
            yield return new WaitForSecondsRealtime(transitionTime / 4);
            links.TransitionScreen.GetComponentInChildren<TextMeshProUGUI>().text = textToDisplay + "..";
            yield return new WaitForSecondsRealtime(transitionTime / 4);
            links.TransitionScreen.GetComponentInChildren<TextMeshProUGUI>().text = textToDisplay + "...";
            yield return new WaitForSecondsRealtime(transitionTime / 4);

            links.TransitionScreen.SetActive(false);
            screenComposition.screen.SetActive(true);
            print("SCREEN CHANGE" + screenComposition.screen.name);
            currentScreen = screenComposition.screen;
            if (endOfTransitionFunction != null)
            {
                endOfTransitionFunction();
            }
        }

        public void MainMenuCardCopierButton()
        {
            StartCoroutine(DisplayTransitionScreen(screenCompositions.Find(screen => screen.screenName == "CopyMachine"), true, PopulateCardList()));
            PopulateCardList();
        }

        public void MainMenuShredderButton()
        {
            StartCoroutine(DisplayTransitionScreen(screenCompositions.Find(screen => screen.screenName == "Shredder"), true));
            PopulateCardList();
        }

        private delegate void EndOfTransitionFunction();

        private void PopulateCardList()
        {
            foreach(Transform child in currentScreen.transform)
            {
                print(child.name);
            }
            // Get the grid layout of the cards from the screen we're on.
            GridLayoutGroup cardListLayout = currentScreen.transform.Find("SelectCardScreen").GetComponentInChildren<GridLayoutGroup>();
            print(cardListLayout);

            // We need 6 card slots
            for (int i = 0; i < 5; i++)
            {
                Instantiate(cardRendererPrefab, cardListLayout.transform);
            }
        }
    }
}