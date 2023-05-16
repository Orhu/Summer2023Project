using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;

/// <summary>
/// Represents a deck of cards and a hand that cards can be previewed and played from.
/// </summary>
public class DeckManager : MonoBehaviour
{
    // Global singleton for the actor's deck.
    public static DeckManager playerDeck;

    [Tooltip("All the cards in the deck.")]
    public List<Card> cards;
    [Tooltip("The side of this deck's hand.")]
    public int handSize = 3;

    // The actor that plays cards from this deck.
    [HideInInspector]
    public IActor actor;
    // The cards in the draw pile.
    [HideInInspector]
    public List<Card> drawableCards;
    // The cards in hand.
    [HideInInspector]
    public List<Card> inHandCards = new List<Card>();
    // The cards in the discard pile.
    [HideInInspector]
    public List<Card> discardedCards = new List<Card>();
    // The indices in the hand of the cards currently being previewed.
    [HideInInspector]
    public List<int> previewedCardIndices = new List<int>();
    // The indices of the cards on cooldown mapped to the time remaining on the cooldown.
    [HideInInspector]
    public Dictionary<int, float> cardIndicesToCooldowns = new Dictionary<int, float>();

    public delegate void handChangedNotification();
    // Called when the draw pile changes.
    public handChangedNotification onDrawPileChanged;
    // Called when the hand changes.
    public handChangedNotification onHandChanged;
    // Called when the discard pile changes.
    public handChangedNotification onDiscardPileChanged;


    public delegate void deckChangedNotification(Card card);

    // Called when a card is added
    public deckChangedNotification onCardAdded;

    // Called when a card is removed
    public deckChangedNotification onCardRemoved;

    #region Initialization
    /// <summary>
    /// Initializes Singleton
    /// </summary>
    void Awake()
    {
        if (playerDeck == null)
        {
            playerDeck = this;
            playerDeck.actor = GameObject.FindGameObjectWithTag("Player").GetComponent<Agent>();
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

        ReshuffleDrawPile();
        for (int i = 0; i < handSize; i++)
        {
            DrawCard();
        }
    }
    #endregion

    #region Card Draw
    /// <summary>
    /// Fills the first empty spot in the actor's hand with a card from the draw pile.
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
            if (inHandCards[i] == null && drawableCards.Count > 0)
            {
                Card drawnCard = drawableCards[drawableCards.Count - 1];
                drawableCards.RemoveAt(drawableCards.Count - 1);
                inHandCards[i] = drawnCard;

                if (drawableCards.Count == 0)
                {
                    ReshuffleDrawPile();
                }

                onDrawPileChanged?.Invoke();
                onHandChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public void DiscardCard(int handIndex)
    {
        discardedCards.Add(inHandCards[handIndex]);
        inHandCards[handIndex] = null;
        onDiscardPileChanged?.Invoke();
        DrawCard();
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

        while (discardedCards.Count > 0)
        {
            int index = Random.Range(0, discardedCards.Count);
            drawableCards.Add(discardedCards[index]);
            discardedCards.RemoveAt(index);
        }

        onDiscardPileChanged?.Invoke();
        onDrawPileChanged?.Invoke();
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
                onDiscardPileChanged?.Invoke();
                DrawCard();
            }
            else
            {
                cardIndicesToCooldowns[cardIndexToCooldown.Key] = newValue;
            }
        }
    }
    #endregion

    #region Playing & Previewing
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
        if (previewedCardIndices.Count == 0)
        {
            card.PreviewActions(actor);
            previewedCardIndices.Add(handIndex);
            return;
        }
        Card rootCard = inHandCards[previewedCardIndices[0]];

        if (handIndex == previewedCardIndices[0])
        {
            previewedCardIndices.Remove(handIndex);
            rootCard.CancelPreviewActions(actor);
            if (previewedCardIndices.Count > 0)
            {
                rootCard = inHandCards[previewedCardIndices[0]];

                int playCount = 0;
                List<ActionModifier> modifiers = new List<ActionModifier>();
                for (int i = 1; i < previewedCardIndices.Count; i++)
                {
                    if (inHandCards[previewedCardIndices[i]] == rootCard)
                    {
                        playCount++;
                    }
                    else
                    {
                        modifiers.AddRange(inHandCards[previewedCardIndices[i]].actionModifiers);
                    }
                }

                rootCard.PreviewActions(actor);
                rootCard.AddStacksToPreview(actor, playCount);
                rootCard.ApplyModifiersToPreview(actor, modifiers);
            }
            return;
        }


        if (previewedCardIndices.Contains(handIndex))
        {
            if (card == rootCard)
            {
                rootCard.AddStacksToPreview(actor, - 1);
            }
            else
            {
                rootCard.RemoveModifiersFromPreview(actor, card.actionModifiers);
            }
            previewedCardIndices.Remove(handIndex);
        }
        else
        {
            if (card == rootCard)
            {
                rootCard.AddStacksToPreview(actor, 1);
            }
            else
            {
                rootCard.ApplyModifiersToPreview(actor, card.actionModifiers);
            }
            previewedCardIndices.Add(handIndex);
        }
    }

    /// <summary>
    /// Play any cards currently being previewed.
    /// </summary>
    public void PlayPreviewedCard()
    {
        if (previewedCardIndices.Count == 0)
        {
            return;
        }

        Card cardToPlay = inHandCards[previewedCardIndices[0]];
        int playCount = 1;
        List<ActionModifier> modifiers = new List<ActionModifier>();
        for (int i = 1; i < previewedCardIndices.Count; i++)
        {
            cardIndicesToCooldowns.Add(previewedCardIndices[i], cardToPlay.cooldown);
            if (inHandCards[previewedCardIndices[i]] == cardToPlay)
            {
                playCount++;
            }
            else
            {
                modifiers.AddRange(inHandCards[previewedCardIndices[i]].actionModifiers);
            }
        }

        cardIndicesToCooldowns.Add(previewedCardIndices[0], cardToPlay.cooldown);
        cardToPlay.PlayActions(actor, playCount, modifiers);

        previewedCardIndices.Clear();
    }
    #endregion

    #region Deck Modifications
    /// <summary>
    /// Adds a card to the deck
    /// </summary>
    /// <param name="card"> The card to add. </param>
    /// <param name="addLocation"> The place to add the card. </param>
    public void AddCard(Card card, AddCardLocation addLocation)
    {
        cards.Add(card);

        switch (addLocation)
        {
            case AddCardLocation.BottomOfDrawPile:
                drawableCards.Insert(0, card);
                onDrawPileChanged?.Invoke();
                break;

            case AddCardLocation.TopOfDrawPile:
                drawableCards.Add(card);
                onDrawPileChanged?.Invoke();
                break;

            case AddCardLocation.DiscardPile:
                discardedCards.Add(card);
                onDrawPileChanged?.Invoke();
                break;
        }


        onCardAdded(card);
    }

    /// <summary>
    /// Removes a card from the deck.
    /// </summary>
    /// <param name="card"> The card to remove </param>
    /// <returns> Whether or not it was successfully removed. </returns>
    public bool RemoveCard(Card card)
    {
        if (RemoveCard(card, CardLocation.DiscardPile))
        {
            return true;
        }

        if (RemoveCard(card, CardLocation.DrawPile))
        {
            return true;
        }

        if (RemoveCard(card, CardLocation.Hand))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes a card from the deck.
    /// </summary>
    /// <param name="card"> The card to remove. </param>
    /// <param name="location"> The place in the deck to remove it from. </param>
    /// <returns> Whether or not it was successfully removed. </returns>
    public bool RemoveCard(Card card, CardLocation location)
    {
        switch(location)
        {
            case CardLocation.DrawPile:
                if (!drawableCards.Remove(card))
                {
                    return false;
                }

                if (drawableCards.Count == 0)
                {
                    ReshuffleDrawPile();
                }
                onDrawPileChanged?.Invoke();
                break;

            case CardLocation.Hand:
                if (!inHandCards.Contains(card))
                {
                    return false;
                }
                inHandCards[inHandCards.IndexOf(card)] = null;
                cardIndicesToCooldowns.Remove(inHandCards.IndexOf(card));
                DrawCard();
                break;

            case CardLocation.DiscardPile:
                if (!discardedCards.Remove(card))
                {
                    return false;
                }
                onDiscardPileChanged?.Invoke();
                break;
        }

        cards.Remove(card);
        return true;
    }

    /// <summary>
    /// Removes a card from the deck.
    /// </summary>
    /// <param name="location"> The place in the deck to remove the card from. </param>
    /// <param name="index"> The index of the card to remove. </param>
    /// <returns> Whether or not it was successfully removed. </returns>
    public bool RemoveCard(CardLocation location, int index)
    {
        switch (location)
        {
            case CardLocation.DrawPile:
                if (index >= drawableCards.Count)
                {
                    return false;
                }
                cards.Remove(drawableCards[index]);
                drawableCards.RemoveAt(index);

                if (drawableCards.Count == 0)
                {
                    ReshuffleDrawPile();
                }
                onDrawPileChanged?.Invoke();
                break;

            case CardLocation.Hand:
                if (index >= inHandCards.Count)
                {
                    return false;
                }
                cards.Remove(inHandCards[index]);
                inHandCards[index] = null;
                DrawCard();
                break;

            case CardLocation.DiscardPile:
                if (index >= discardedCards.Count)
                {
                    return false;
                }
                cards.Remove(discardedCards[index]);
                discardedCards.RemoveAt(index);
                onDiscardPileChanged?.Invoke();
                break;
        }

        return true;
    }

    /// <summary>
    /// Used to determine where to add a card to the deck.
    /// </summary>
    public enum AddCardLocation
    {
        BottomOfDrawPile,
        TopOfDrawPile,
        DiscardPile
    }

    /// <summary>
    /// Which pile a card is in.
    /// </summary>
    public enum CardLocation
    {
        DrawPile,
        Hand,
        DiscardPile
    }
    #endregion
}
