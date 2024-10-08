using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
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
        private void Start()
        {
            for (int i = 0; i < previewRangeEnd - previewRangeStart; i++)
            {
                cardRenderers.Add(Instantiate(cardRendererTemplate.gameObject, transform).GetComponent<CardRenderer>());
            }
            Deck.playerDeck.onDrawPileChanged += OnCardDrawn;
        }

        /// <summary>
        /// Updates the displayed cards when a card is drawn.
        /// </summary>
        private void OnCardDrawn()
        {
            for (int i = 0; i < previewRangeEnd - previewRangeStart; i++)
            {
                if ((previewRangeEnd - 1 - i) < Deck.playerDeck.drawableCards.Count)
                {
                    Card card = Deck.playerDeck.drawableCards[Deck.playerDeck.drawableCards.Count - 1 - (previewRangeEnd - 1 - i)];
                    if (cardRenderers[i].card != card)
                    {
                        cardRenderers[i].card = card;
                    }
                }
                else
                {
                    cardRenderers[i].card = null;
                }
            }
        }
    }
}