using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Cardificer
{
    /// <summary>
    /// Card printer manager script for handling rendering and logic
    /// for the card printer UI object
    /// </summary>
    public class CardPrinterMenu : MonoBehaviour
    {
        [Tooltip("Time taken to transition between different screens.")]
        [SerializeField] private float defaultTransitionTime;

        [Tooltip("The links to the necessary components for rendering.")]
        [SerializeField] private ComponentLinks links;

        // Screen that the card printer is currently on
        private GameObject currentScreen;

        public delegate void TransitionFunction();


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

            [Tooltip("The gameobject used to display the copier select card screen")]
            public GameObject CopierSelectScreen;

            [Tooltip("The gameobject used to display the copier confirm card screen")]
            public GameObject CopierConfirmScreen;

            [Tooltip("The gameobject used to display the shredder select card screen")]
            public GameObject ShredderSelectScreen;

            [Tooltip("The gameobject used to display the shredder confirm card screen")]
            public GameObject ShredderConfirmScreen;

            [Tooltip("The gameobject used to display an error screen.")]
            public GameObject ErrorScreen;
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
            new GameScreenComposition("CopyMachineSelect",links.CopierSelectScreen, "Initializing Copy_Machine.exe", "Exiting Copy_Machine.exe"),
            new GameScreenComposition("CopyMachineConfirm",links.CopierConfirmScreen, "Loading Selection", ""),
            new GameScreenComposition("ShredderSelect", links.ShredderSelectScreen, "Initializing Card_Shredder.exe", "Exiting Card_Shredder.exe"),
            new GameScreenComposition("ShredderConfirm", links.ShredderConfirmScreen, "Loading Selection", ""),
            new GameScreenComposition("Error", links.ErrorScreen, "Processing request", ""),
            });
        }

        /// <summary>
        /// When the card printer first comes on display the main screen loading text
        /// </summary>
        private void OnEnable()
        {
            StartCoroutine(DisplayTransitionScreen(screenCompositions.Find(screen => screen.screenName == "MainScreen"), true, null, 1));
        }

        /// <summary>
        /// Display transitioning text between different screens.
        /// </summary>
        /// <param name="screenComposition">The composition of the screen you are transitioning to</param>
        /// <param name="isLoading">Whether this is loading text or exit text</param>
        /// <returns></returns>
        private IEnumerator DisplayTransitionScreen(GameScreenComposition screenComposition, bool isLoading, TransitionFunction transitionFunction = null, float transitionTime = 0)
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
            if (transitionTime != 0)
            {
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
            }
            
            screenComposition.screen.SetActive(true);
            currentScreen = screenComposition.screen;

            transitionFunction?.Invoke();
        }

        public void MainMenuCardCopierButton()
        {
            StartCoroutine(DisplayTransitionScreen(screenCompositions.Find(screen => screen.screenName == "CopyMachineSelect"), true, null, 1));
        }

        public void MainMenuShredderButton()
        {
            StartCoroutine(DisplayTransitionScreen(screenCompositions.Find(screen => screen.screenName == "ShredderSelect"), true, null, 1));
        }

        public void SelectScreenToConfirmScreenCopier(Card card)
        {
            StartCoroutine(DisplayTransitionScreen(screenCompositions.Find(screen => screen.screenName == "CopyMachineConfirm"), true));
            links.CopierConfirmScreen.GetComponent<ConfirmScreenScript>().SetCopyCardRendererCard(card);
        }

        public void SelectScreenToConfirmScreenShredder(Card card)
        {
            StartCoroutine(DisplayTransitionScreen(screenCompositions.Find(screen => screen.screenName == "ShredderConfirm"), true));
            links.CopierConfirmScreen.GetComponent<ConfirmScreenScript>().SetShredCardRendererCard(card);
        }

        public void MoveToErrorScreen(string errorMessage)
        {
            StartCoroutine(DisplayTransitionScreen(screenCompositions.Find(screen => screen.screenName == "Error"), true, null, 1));
            links.ErrorScreen.GetComponentInChildren<TextMeshProUGUI>().text = errorMessage;
        }

        private void RestoreGridLayout()
        {
            currentScreen.GetComponentInChildren<GridLayoutGroup>().gameObject.SetActive(true);
        }

        public void CancelTransaction()
        {
            StartCoroutine(DisplayTransitionScreen(screenCompositions.Find(screen => screen.screenName == "CopyMachineSelect"), true, RestoreGridLayout));  
        }

        public IEnumerator ReturnBackToMainScreen(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            StartCoroutine(DisplayTransitionScreen(screenCompositions.Find(screen => screen.screenName == "MainScreen"), true));
        }

    }
}