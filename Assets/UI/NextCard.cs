using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;

public class NextCard : MonoBehaviour
{
    private CardRenderer cardRenderer;

    private void Start()
    {
        cardRenderer = gameObject.GetComponent<CardRenderer>();
        DeckManager.playerDeck.onCardDrawn += OnCardDrawn;
    }

    // Update is called once per frame
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
