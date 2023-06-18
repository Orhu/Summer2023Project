using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Sets text to the number of cards in the draw pile
    /// </summary>
    public class RemainingDrawCards : MonoBehaviour
    {
        // The number subtracted from the text to account for other UI elements.
        public int offset = 0;

        // The text box to set the text on.
        TMP_Text textBox;

        /// <summary>
        /// Initializes bindings and references.
        /// </summary>
        void Start()
        {
            textBox = GetComponent<TMP_Text>();
            Deck.playerDeck.onDrawPileChanged += OnCardDrawn;
            GetComponentInParent<UnityEngine.UI.VerticalLayoutGroup>().enabled = false;
            Invoke("RefreshParent", 0.1f);
            OnCardDrawn();
        }

        /// <summary>
        /// Updates the text after card is drawn.
        /// </summary>
        void OnCardDrawn()
        {
            if (Deck.playerDeck.drawableCards != null)
            {
                textBox.text = "+" + Mathf.Max(0, Deck.playerDeck.drawableCards.Count - offset);
            }
        }

        /// <summary>
        /// Forces the parent to update the layout to ensure correct formating.
        /// </summary>
        void RefreshParent()
        {
            GetComponentInParent<UnityEngine.UI.LayoutGroup>().enabled = true;
        }
    }
}