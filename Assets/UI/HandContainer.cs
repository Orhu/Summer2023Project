using CardSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandContainer : MonoBehaviour
{
    public CardRenderer cardRendererTemplate;
    private List<CardRenderer> cardRenderers = new List<CardRenderer>();

    private void Start()
    {
        for (int i = 0; i < DeckManager.playerDeck.handSize; i++)
        {
            cardRenderers.Add(Instantiate(cardRendererTemplate.gameObject, transform).GetComponent<CardRenderer>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < DeckManager.playerDeck.handSize; i++)
        {
            cardRenderers[i].Card = DeckManager.playerDeck.inHandCards[i];
        }
    }
}
