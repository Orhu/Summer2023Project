using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// UI element for rendering the actor's current hand.
/// </summary>
public class HandRenderer : MonoBehaviour
{
    // The card renderer prefab to instantiate.
    public CardRenderer cardRendererTemplate;

    // The card renderers that were created to display the hand.
    List<CardRenderer> cardRenderers = new List<CardRenderer>();

    /// <summary>
    /// Creates the card needed renderers.
    /// </summary>
    void Start()
    {
        for (int i = 0; i < Deck.playerDeck.handSize; i++)
        {
            cardRenderers.Add(Instantiate(cardRendererTemplate.gameObject, transform).GetComponent<CardRenderer>());
        }
    }

    /// <summary>
    /// Updates the renders to show the appropriate cards and their preview/cooldown state.
    /// </summary>
    void Update()
    {
        for (int i = 0; i < Deck.playerDeck.handSize; i++)
        {
            Card card = Deck.playerDeck.inHandCards[i];
            if (cardRenderers[i].Card != card)
            {
                cardRenderers[i].Card = card;
            }
            cardRenderers[i].Previewing = Deck.playerDeck.previewedCardIndices.Contains(i);

            if(Deck.playerDeck.cardIndicesToActionTimes.ContainsKey(i))
            {
                cardRenderers[i].CooldownTime = 0;
                cardRenderers[i].ActionTime = Deck.playerDeck.cardIndicesToActionTimes[i];
            }
            else if (Deck.playerDeck.cardIndicesToCooldowns.ContainsKey(i))
            {
                cardRenderers[i].ActionTime = 0;
                cardRenderers[i].CooldownTime = Deck.playerDeck.cardIndicesToCooldowns[i];
            }
            else
            {
                cardRenderers[i].CooldownTime = 0;
                cardRenderers[i].ActionTime = 0;
            }
        }
    }
}
