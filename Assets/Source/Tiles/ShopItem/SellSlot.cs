using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cardificer;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Handles logic related to a sell slot and buying cards from the player
/// </summary>
public class SellSlot : MonoBehaviour
{
    [Tooltip("What is the value of a regular card at this slot?")]
    [SerializeField] private int regularValue = 5;
    
    [Tooltip("What is the value of a rare card at this slot?")]
    [SerializeField] private int rareValue = 10;
    
    [Tooltip("How many times can a card be sold at this slot?")]
    [SerializeField] private int sellCount = 1;
    
    [Tooltip("What is the sell pool of this slot?")]
    [SerializeField] private SellPool sellPool = SellPool.AllCards;

    /// <summary>
    /// Enum for sell pool types
    /// </summary>
    private enum SellPool
    {
        AllCards,
        PlayerDeck,
    }

    [Tooltip("Stores a reference to the text object that displays the card's value")]
    [SerializeField] private TextMeshProUGUI cardValueText;
    
    [Tooltip("Sell particle effect")]
    [SerializeField] private ParticleSystem sellParticles;

    // Tracks the card contained in this slot
    private Card selectedCard;
    
    // Tracks the value of the card in this slot
    private int cardValue;

    // Tracks whether we can sell another card
    private bool canSellAnotherCard => sellCount > 0;

    /// <summary>
    /// Picks a random card to sell and displays it and its value
    /// </summary>
    void Start()
    {
        List<Card> cardPool = new List<Card>();
        GenericWeightedThings<Card> possibleCardPool = new GenericWeightedThings<Card>();
        
        switch (sellPool)
        {
            case SellPool.AllCards:
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "cards"));
                
                cardPool = assetBundle.LoadAllAssets<Card>().ToList();
                break;
            }
            case SellPool.PlayerDeck:
            {
                cardPool = Deck.playerDeck.cards;
                break;
            }
        }
        
        // add all cards to the generic weighted thing then pick one
        foreach (Card card in cardPool)
        {
            possibleCardPool.Add(card, 1);
        }
        selectedCard = possibleCardPool.GetRandomThing(transform.position);

        // set sprite, value, and text
        cardValue = selectedCard.isRare ? rareValue : regularValue;
        GetComponent<SpriteRenderer>().sprite = selectedCard.actionImage;
        cardValueText.text = cardValue.ToString();
    }

    /// <summary>
    /// If the player can sell a card, sell it.
    /// </summary>
    /// <param name="other"> The collider of the thing that collided with the tile. </param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canSellAnotherCard && other.CompareTag("Player") && Deck.playerDeck.RemoveCard(selectedCard))
        {
            BuyCardFromPlayer();
        }
    }

    /// <summary>
    /// Decreases sell count, gives the value of the card to the player, and removes the card from the player's deck.
    /// </summary>
    private void BuyCardFromPlayer()
    {
        sellCount--;
        Player.AddMoney(cardValue);
        Instantiate(sellParticles);

        if (!canSellAnotherCard)
        {
            gameObject.SetActive(false);
        }
    }
}
