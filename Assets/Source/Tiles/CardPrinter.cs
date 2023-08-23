using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    public class CardPrinter : MonoBehaviour
    {
        /// <summary>
        /// When the player enters the trigger zone, open Card Printer menu,
        /// </summary>
        /// <param name="collision">Whatever is colliding with the Card Printer prefab</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                MenuManager.Open<CardPrinterMenu>();
            }
        }
    }
}

