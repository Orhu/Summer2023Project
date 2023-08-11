using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Cardificer
{
    public class ErrorScreen : MonoBehaviour
    {

        public CardPrinterMenu cardPrinterMenu;

        [Tooltip("Time it takes to return back to the main menu.")]
        [SerializeField] private float returnTime;

        private void Awake()
        {
            cardPrinterMenu = gameObject.GetComponentInParent<CardPrinterMenu>();
        }

        private void OnEnable()
        {
            StartCoroutine(cardPrinterMenu.ReturnBackToMainScreen(returnTime));
        }
    }
}
