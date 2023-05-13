using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;

/// <summary>
/// Represents a deck of cards and a hand that cards can be previewed and played from.
/// </summary>
public class DeckManager : MonoBehaviour
{
    // Global singleton for the player's deck.
    public static DeckManager playerDeck;
    // All the cards in the deck.
    public List<Card> cards;
    // The side of this deck's hand.
    public int handSize = 3;
    // The cards in the draw pile.
    [System.NonSerialized]
    public List<Card> drawableCards;
    // The cards in hand.
    [System.NonSerialized]
    public List<Card> inHandCards = new List<Card>();
    // The cards in the discard pile.
    [System.NonSerialized]
    public List<Card> discardedCards = new List<Card>();
    // The indices in the hand of the cards currently being previewed.
    [System.NonSerialized]
    public List<int> previewedCardIndices = new List<int>();
    // The indices of the cards on cooldown mapped to the time remaining on the cooldown.
    [System.NonSerialized]
    public Dictionary<int, float> cardIndicesToCooldowns = new Dictionary<int, float>();

    public delegate void deckChangedNotification();
    // Called when a card is drawn.
    public deckChangedNotification onCardDrawn;

    /// <summary>
    /// Initializes Singleton
    /// </summary>
    void Awake()
    {
        if (playerDeck == null)
        {
            playerDeck = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes the draw pile and hand.
    /// </summary>
    void Start()
    {
        drawableCards = new List<Card>(cards);

        for (int i = 0; i < handSize; i++)
        {
            DrawCard();
        }
    }

    /// <summary>
    /// Updates all cooldowns and draws new cards when needed.
    /// </summary>
    void Update()
    {
        foreach (KeyValuePair<int, float> cardIndexToCooldown in new Dictionary<int, float>(cardIndicesToCooldowns))
        {
            float newValue = cardIndexToCooldown.Value - Time.deltaTime;
            if (newValue <= 0)
            {
                cardIndicesToCooldowns.Remove(cardIndexToCooldown.Key);
                discardedCards.Add(inHandCards[cardIndexToCooldown.Key]);
                inHandCards[cardIndexToCooldown.Key] = null;
                DrawCard();
            }
            else
            {
                cardIndicesToCooldowns[cardIndexToCooldown.Key] = newValue;
            }
        }
    }

    /// <summary>
    /// Moves all cards from the discard pile to the draw pile and shuffles.
    /// </summary>
    public void ReshuffleDrawPile()
    {
        while (drawableCards.Count > 0)
        {
            discardedCards.Add(drawableCards[drawableCards.Count - 1]);
            drawableCards.RemoveAt(drawableCards.Count - 1);
        }
        while(discardedCards.Count > 0)
        {
            int index = Random.Range(0, discardedCards.Count);
            drawableCards.Add(discardedCards[index]);
            discardedCards.RemoveAt(index);
        }
    }

    /// <summary>
    /// Toggles the preview for the a card.
    /// </summary>
    /// <param name="handIndex"> The index in the hand of the card. </param>
    public void TogglePreviewCard(int handIndex)
    {
        if (inHandCards.Count <= handIndex || cardIndicesToCooldowns.ContainsKey(handIndex))
        {
            return;
        }
        Card card = inHandCards[handIndex];
        if (previewedCardIndices.Contains(handIndex))
        {
            card.CancelPreviewActions();
            previewedCardIndices.Remove(handIndex);
        }
        else
        {
            card.PreviewActions();
            previewedCardIndices.Add(handIndex);
        }
    }

    /// <summary>
    /// Play any cards currently being previewed.
    /// </summary>
    public void PlayPreveiwedCards()
    {
        foreach (int previewedCardIndex in previewedCardIndices)
        {
            Card card = inHandCards[previewedCardIndex];
            card.PlayActions();
            cardIndicesToCooldowns.Add(previewedCardIndex, card.cooldown);
        }
        previewedCardIndices.Clear();
    }

    /// <summary>
    /// Fills the first empty spot in the player's hand with a card from the draw pile.
    /// </summary>
    /// <returns> Whether or not an empty spot was found. </returns>
    bool DrawCard()
    {
        while (inHandCards.Count < handSize)
        {
            inHandCards.Add(null);
        }

        for (int i = 0; i < handSize; i++)
        {
            if (inHandCards[i] == null)
            {
                Card drawnCard = drawableCards[drawableCards.Count - 1];
                drawableCards.RemoveAt(drawableCards.Count - 1);
                inHandCards[i] = drawnCard;

                if (drawableCards.Count == 0)
                {
                    ReshuffleDrawPile();
                }

                onCardDrawn();
                return true;
            }
        }
        return false;
    }
}
