using System.Collections;
using System.Collections.Generic;
using Cardificer;
using UnityEngine;

public class Counterspell : MonoBehaviour
{
    [SerializeField] private string counterspellCardName = "Counterspell";
    
    // Placeholder bool, this should be replaced with an actual check for whether the spell played is chorded or not
    private bool cardIsChorded = false;

    // Tracks whether Counterspell is currently in the Cardificer's hand
    private bool currentlyInHand = false;
    
    // If the card is currently in hand, track the index it is at. If the card is not in the hand, set this to -1
    private int counterspellHandIndex = -1;
    
    /// <summary>
    /// Updates the currentlyInHand and counterspellHandIndex variables depending on whether counterspell is currently in hand or not
    /// </summary>
    void FixedUpdate()
    {
        // Determine if Counterspell is currently in the Cardificer's hand
        bool foundCounterspell = false;
        
        for (int i = 0; i < CardificerDeck.cardsInHand - 1; i++)
        {
            CardificerCard currentCard = CardificerDeck.GetCardFromHand(i);
            if (currentCard != null && currentCard.name == counterspellCardName)
            {
                foundCounterspell = true;
                currentlyInHand = true;
                counterspellHandIndex = i;
                break;
            }
        }

        // Counterspell is not in hand, set the correct vars
        if (!foundCounterspell)
        {
            currentlyInHand = false;
            counterspellHandIndex = -1;
        }
    }

    /// <summary>
    /// Determines if the player is currently playing a card, and if it is a chorded card, casts Counterspell.
    /// </summary>
    void Update()
    {
        // TODO logic for grabbing the currently played card. 

        if (currentlyInHand && cardIsChorded) // cardIsChorded might be like "playerCardBeingPlayed.isChorded"
        {
            currentlyInHand = false;
            CardificerDeck.DiscardFromHand(counterspellHandIndex);
            // This is where the logic for Counterspell should be. I would expect to see things such as:
            // * Setting animation trigger if there is an animation for counterspell
            // * Intercepting and deleting the chorded card, ideally preventing the player from playing it to begin with but wiping its projectile would also work.
            // * Any other logic that should occur the moment Counterspell is triggered
        }
    }
}
