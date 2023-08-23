using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Cardificer
{
    /// <summary>
    /// Script to go on the error screen UI in the card printer subscreens
    /// handles logic and rendering.
    /// </summary>
    public class ErrorScreen : MonoBehaviour
    {
        [Tooltip("Public reference to the card printer menu")]
        public CardPrinterMenu cardPrinterMenu;

        [Tooltip("Time it takes to return back to the main menu.")]
        [SerializeField] private float returnTime;

        /// <summary>
        /// Assign variables
        /// </summary>
        private void Awake()
        {
            cardPrinterMenu = gameObject.GetComponentInParent<CardPrinterMenu>();
        }

        /// <summary>
        /// When the error screen is enabled,
        /// start a coroutine to move back to the main screen
        /// </summary>
        private void OnEnable()
        {
            StartCoroutine(cardPrinterMenu.ReturnBackToMainScreen(returnTime));
        }
    }
}
