using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// UI element for rendering the actor's current hand.
/// </summary>
public class HandRenderer : MonoBehaviour
{
    // The card renderers that were created to display the hand.
    // * Must be added from inspector currently, not being instantiated.
    public List<RuneRenderer> runeRenderers = new List<RuneRenderer>();

    /// <summary>
    /// Updates the renders to show the appropriate cards and their preview/cooldown state.
    /// </summary>
    void Update()
    {
        for (int i = 0; i < Deck.playerDeck.handSize; i++)
        {
            Card card = Deck.playerDeck.inHandCards[i];
            if (runeRenderers[i].card != card)
            {
                runeRenderers[i].card = card;
            }
            runeRenderers[i].previewing = Deck.playerDeck.previewedCardIndices.Contains(i);

            if(Deck.playerDeck.cardIndicesToActionTimes.ContainsKey(i))
            {
                runeRenderers[i].cooldownTime = 0;
                runeRenderers[i].actionTime = Deck.playerDeck.cardIndicesToActionTimes[i];
            }
            else if (Deck.playerDeck.cardIndicesToCooldowns.ContainsKey(i))
            {
                runeRenderers[i].actionTime = 0;
                runeRenderers[i].cooldownTime = Deck.playerDeck.cardIndicesToCooldowns[i];
            }
            else
            {
                runeRenderers[i].cooldownTime = 0;
                runeRenderers[i].actionTime = 0;
            }
        }
    }
}
