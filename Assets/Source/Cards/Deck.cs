using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;
using System;

/// <summary>
/// Represents a deck of cards and a hand that cards can be previewed and played from.
/// </summary>
public class Deck : MonoBehaviour
{
    #region Variables
    [Tooltip("All the cards in the deck.")]
    public List<Card> cards;
    [Tooltip("The side of this deck's hand.")]
    public int handSize = 3;

    #region Hidden in Inspector Variables
    // Global singleton for the actor's deck.
    public static Deck playerDeck;

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
    // The indices of the cards on being acted mapped to the remaining action time.
    [HideInInspector]
    public Dictionary<int, float> cardIndicesToActionTimes = new Dictionary<int, float>();
    // The indices of the cards on cooldown mapped to the time remaining on the cooldown.
    [HideInInspector]
    public Dictionary<int, float> cardIndicesToCooldowns = new Dictionary<int, float>();
    #endregion

    #region Delegates
    // Called when the draw pile changes.
    public System.Action onDrawPileChanged;
    // Called when the hand changes.
    public System.Action onHandChanged;
    // Called when the discard pile changes.
    public System.Action onDiscardPileChanged;
    // Called when a card is added
    public Action<Card> onCardAdded;
    // Called when a card is removed
    public Action<Card> onCardRemoved;
    #endregion
    #endregion

    #region Initialization
    /// <summary>
    /// Initializes Singleton
    /// </summary>
    void Awake()
    {
        if (playerDeck == null)
        {
            playerDeck = this;
            playerDeck.actor = GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>();
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
            if (drawableCards.Count == 0)
            {
                ReshuffleDrawPile();
            }

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

    /// <summary>
    /// Discards a card from the deck's hand.
    /// </summary>
    /// <param name="handIndex"> The index in the hand of the card to discard. </param>
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
            int index = UnityEngine.Random.Range(0, discardedCards.Count);
            drawableCards.Add(discardedCards[index]);
            discardedCards.RemoveAt(index);
        }

        onDiscardPileChanged?.Invoke();
        onDrawPileChanged?.Invoke();
    }


    /// <summary>
    /// Updates all action times and cooldowns, and it draws new cards when needed.
    /// </summary>
    void Update()
    {
        foreach (KeyValuePair<int, float> cardIndexToCooldown in new Dictionary<int, float>(cardIndicesToCooldowns))
        {
            float newValue = cardIndexToCooldown.Value - Time.deltaTime;
            if (newValue <= 0)
            {
                cardIndicesToCooldowns.Remove(cardIndexToCooldown.Key);
                DiscardCard(cardIndexToCooldown.Key);
            }
            else
            {
                cardIndicesToCooldowns[cardIndexToCooldown.Key] = newValue;
            }
        }

        foreach (KeyValuePair<int, float> cardIndexToActionTime in new Dictionary<int, float>(cardIndicesToActionTimes))
        {
            float newValue = cardIndexToActionTime.Value - Time.deltaTime;
            if (newValue <= 0)
            {
                cardIndicesToActionTimes.Remove(cardIndexToActionTime.Key);
                cardIndicesToCooldowns.Add(cardIndexToActionTime.Key, inHandCards[cardIndexToActionTime.Key].cooldownTime);
            }
            else
            {
                cardIndicesToActionTimes[cardIndexToActionTime.Key] = newValue;
            }
        }
    }
    #endregion

    #region Playing & Previewing
    /// <summary>
    /// Selects a card. Will immediately play any non-chordable cards. Will toggle the preview for chordable cards.
    /// </summary>
    /// <param name="handIndex"> The index in the hand of the card. </param>
    public void SelectCard(int handIndex)
    {
        if (inHandCards.Count <= handIndex || inHandCards[handIndex] == null || cardIndicesToCooldowns.ContainsKey(handIndex) || cardIndicesToActionTimes.ContainsKey(handIndex))
        {
            return;
        }

        // Play normal cards.
        if (inHandCards[handIndex] is not AttackCard)
        {
            PlayCard(handIndex);
            return;
        }


        AttackCard card = inHandCards[handIndex] as AttackCard;
        // If nothing is being previewed, start previewing.
        if (previewedCardIndices.Count == 0)
        {
            card.PreviewActions(actor);
            previewedCardIndices.Add(handIndex);
            return;
        }

        AttackCard rootCard = inHandCards[previewedCardIndices[0]] as AttackCard;
        // If root of chord is changed
        if (handIndex == previewedCardIndices[0])
        {
            previewedCardIndices.RemoveAt(0);
            rootCard.CancelPreviewActions(actor);

            // If there are still previewed cards
            if (previewedCardIndices.Count > 0)
            {
                rootCard = inHandCards[previewedCardIndices[0]] as AttackCard;

                List<AttackCard> chordedCards = new List<AttackCard>();
                for (int i = 1; i < previewedCardIndices.Count; i++)
                {
                    chordedCards.Add(inHandCards[previewedCardIndices[i]] as AttackCard);
                }

                rootCard.PreviewActions(actor, chordedCards);
            }
            return;
        }

        // Add or remove cards from preview
        if (previewedCardIndices.Contains(handIndex))
        {
            rootCard.RemoveFromPreview(actor, card);
            previewedCardIndices.Remove(handIndex);
        }
        else
        {
            rootCard.AddToPreview(actor, card);
            previewedCardIndices.Add(handIndex);
        }
    }

    /// <summary>
    /// Plays a card in the hand.
    /// </summary>
    /// <param name="handIndex"> The index in the hand of the card to play. </param>
    public void PlayCard(int handIndex)
    {
        if (handIndex >= inHandCards.Count)
        {
            return;
        }

        Card card = inHandCards[handIndex];
        if (card == null)
        {
            return;
        }

        card.PlayActions(actor);
        cardIndicesToActionTimes.Add(handIndex, card.actionTime);
    }

    /// <summary>
    /// Plays a cord consisting of cards from the hand.
    /// </summary>
    /// <param name="handIndices"> The indices of the cards to play. Index 0 will be the root of the chord. </param>
    public void PlayChord(List<int> handIndices)
    {
        AttackCard cardToPlay = null;
        List<AttackCard> chordedCards = new List<AttackCard>();

        foreach (int handIndex in handIndices)
        {
            Card card = inHandCards[handIndex];
            if (card == null)
            {
                continue;
            }

            cardIndicesToActionTimes.Add(handIndex, card.actionTime);
            if (cardToPlay == null && card is AttackCard)
            {
                cardToPlay = card as AttackCard;
                continue;
            }

            if (card is AttackCard)
            {
                chordedCards.Add(card as AttackCard);
            }
            else
            {
                PlayCard(handIndex);
            }
        }

        cardToPlay?.PlayActions(actor, chordedCards);
    }

    /// <summary>
    /// Plays a cord consisting of the cards being previewed.
    /// </summary>
    public void PlayChord()
    {
        if (previewedCardIndices.Count == 0)
        {
            return;
        }

        PlayChord(previewedCardIndices);

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

        onCardAdded?.Invoke(card);
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
        switch (location)
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
        onCardRemoved?.Invoke(card);
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
        Card removedCard = null;
        switch (location)
        {
            case CardLocation.DrawPile:
                if (index >= drawableCards.Count)
                {
                    return false;
                }
                removedCard = drawableCards[index];
                cards.Remove(removedCard);
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
                removedCard = inHandCards[index];
                cards.Remove(removedCard);
                inHandCards[index] = null;
                DrawCard();
                break;

            case CardLocation.DiscardPile:
                if (index >= discardedCards.Count)
                {
                    return false;
                }
                removedCard = inHandCards[index];
                cards.Remove(removedCard);
                discardedCards.RemoveAt(index);
                onDiscardPileChanged?.Invoke();
                break;
        }

        onCardRemoved?.Invoke(removedCard);
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
