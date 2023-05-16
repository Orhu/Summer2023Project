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
    // Called when a card is drawn.
    public handChangedNotification onCardDrawn;


    public delegate void deckChangedNotification(Card card);

    // Called when a card is added
    public deckChangedNotification onCardAdded;

    // Called when a card is removed
    public deckChangedNotification onCardRemoved;

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
