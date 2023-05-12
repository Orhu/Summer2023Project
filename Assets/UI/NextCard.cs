using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;

/// <summary>
/// Makes a card renderer show the card at the top of the draw pile.
/// </summary>
public class NextCard : MonoBehaviour
{
    // The card renderer to update.
    private CardRenderer cardRenderer;

    /// <summary>
    /// Initializes reference and binding.
    /// </summary>
    private void Start()
    {
        cardRenderer = gameObject.GetComponent<CardRenderer>();
        DeckManager.playerDeck.onCardDrawn += OnCardDrawn;
    }

    /// <summary>
    /// Updates the renderer when a card is drawn.
    /// </summary>
    void OnCardDrawn()
    {
        if (DeckManager.playerDeck.drawableCards.Count == 0)
        {
            cardRenderer.Card = null;
            return;
        }

        Card card = DeckManager.playerDeck.drawableCards[DeckManager.playerDeck.drawableCards.Count - 1];
        if (cardRenderer.Card != card)
        {
            cardRenderer.Card = card;
        }
    }
}
