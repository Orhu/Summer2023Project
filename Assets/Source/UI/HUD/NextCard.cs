using UnityEngine;

/// <summary>
/// Makes a card renderer show the card at the top of the draw pile.
/// </summary>
public class NextCard : MonoBehaviour
{
    // The card renderer to update.
    CardRenderer cardRenderer;

    /// <summary>
    /// Initializes reference and binding.
    /// </summary>
    void Start()
    {
        cardRenderer = GetComponent<CardRenderer>();
        Deck.playerDeck.onDrawPileChanged += OnCardDrawn;
    }

    /// <summary>
    /// Updates the renderer when a card is drawn.
    /// </summary>
    void OnCardDrawn()
    {
        if (Deck.playerDeck.drawableCards.Count == 0)
        {
            cardRenderer.Card = null;
            return;
        }

        Card card = Deck.playerDeck.drawableCards[Deck.playerDeck.drawableCards.Count - 1];
        if (cardRenderer.Card != card)
        {
            cardRenderer.Card = card;
        }
    }
}
