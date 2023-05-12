using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;

public class DrawStack : MonoBehaviour
{
    public CardRenderer cardRendererTemplate;
    public int numCardsToPreview = 5;
    private List<CardRenderer> cardRenderers = new List<CardRenderer>();

    private void Start()
    {
        for (int i = 0; i < numCardsToPreview; i++)
        {
            cardRenderers.Add(Instantiate(cardRendererTemplate.gameObject, transform).GetComponent<CardRenderer>());
        }
        DeckManager.playerDeck.onCardDrawn += OnCardDrawn;
    }

    // Update is called once per frame
    void OnCardDrawn()
    {
        for (int i = 0; i < numCardsToPreview; i++)
        {
            if ((numCardsToPreview - i) < DeckManager.playerDeck.drawableCards.Count)
            {
                Card card = DeckManager.playerDeck.drawableCards[DeckManager.playerDeck.drawableCards.Count - 1 - (numCardsToPreview - i)];
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
