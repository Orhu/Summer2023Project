using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Card printer manager script for handling rendering and logic
/// for the card printer UI object
/// </summary>
public class CardPrinterMenu : MonoBehaviour
{
    [Tooltip("Time taken to transition between different screens.")]
    [SerializeField] private float transitionTime;

    [Tooltip("The links to the necessary components for rendering.")]
    [SerializeField] private ComponentLinks links;

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

    struct GameScreenComposition
    {
        public GameObject screen;

        public string loadingText;
    }

    private void OnEnable()
    {
        StartCoroutine(DisplayTransitionScreen("Initializing service interface...", links.MainScreen));
    }

    private IEnumerator DisplayTransitionScreen(string text, GameObject screen)
    {
        links.TransitionScreen.SetActive(true);
        links.TransitionScreen.GetComponentInChildren<TextMeshProUGUI>().text = text;

        print("WAITING");
        yield return new WaitForSecondsRealtime(transitionTime);
        print("NO LONGER WAITING");

        links.TransitionScreen.SetActive(false);
        screen.SetActive(true);
    }
}
