using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// UI element for rendering the actor's current hand.
/// </summary>
public class HandRenderer : MonoBehaviour
{
    // The card renderers that were created to display the hand.
    // * Must be added from inspector currently, not being instantiated.
    public List<CardRenderer> cardRenderers = new List<CardRenderer>();

    /// <summary>
    /// Updates the renders to show the appropriate cards and their preview/cooldown state.
    /// </summary>
    void Update()
    {
        for (int i = 0; i < Deck.playerDeck.handSize; i++)
        {
            Card card = Deck.playerDeck.inHandCards[i];
            if (cardRenderers[i].card != card)
            {
                cardRenderers[i].card = card;
            }
            cardRenderers[i].previewing = Deck.playerDeck.previewedCardIndices.Contains(i);

            if(Deck.playerDeck.cardIndicesToActionTimes.ContainsKey(i))
            {
                cardRenderers[i].cooldownTime = 0;
                cardRenderers[i].actionTime = Deck.playerDeck.cardIndicesToActionTimes[i];
            }
            else if (Deck.playerDeck.cardIndicesToCooldowns.ContainsKey(i))
            {
                cardRenderers[i].actionTime = 0;
                cardRenderers[i].cooldownTime = Deck.playerDeck.cardIndicesToCooldowns[i];
            }
            else
            {
                cardRenderers[i].cooldownTime = 0;
                cardRenderers[i].actionTime = 0;
            }
        }
    }
}
