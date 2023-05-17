using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;

/// <summary>
/// A UI element for rendering the actor's draw pile.
/// </summary>
public class DrawPileRenderer : MonoBehaviour
{
    // The card renderer prefab to instantiate.
    public CardRenderer cardRendererTemplate;
    // The start (inclusive) of the range of indices from the top of the draw pile to preview.
    public int previewRangeStart = 0;
    // The end (inclusive) of the range of indices from the top of the draw pile to preview.
    public int previewRangeEnd = 4;
    List<CardRenderer> cardRenderers = new List<CardRenderer>();

    /// <summary>
    /// Creates the necessary card renderers.
    /// </summary>
    void Start()
    {
        for (int i = 0; i < previewRangeEnd - previewRangeStart; i++)
        {
            cardRenderers.Add(Instantiate(cardRendererTemplate.gameObject, transform).GetComponent<CardRenderer>());
        }
        DeckManager.playerDeck.onDrawPileChanged += OnCardDrawn;
    }

    /// <summary>
    /// Updates the displayed cards when a card is drawn.
    /// </summary>
    void OnCardDrawn()
    {
        for (int i = 0; i < previewRangeEnd - previewRangeStart; i++)
        {
            if ((previewRangeEnd - 1 - i) < DeckManager.playerDeck.drawableCards.Count)
            {
                Card card = DeckManager.playerDeck.drawableCards[DeckManager.playerDeck.drawableCards.Count - 1 - (previewRangeEnd - 1 - i)];
                if (cardRenderers[i].Card != card)
                {
                    cardRenderers[i].Card = card;
                }
            }
            else
            {
                cardRenderers[i].Card = null;
            }
        }
    }
}
