using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;

public class DeckManager : MonoBehaviour
{
    public static DeckManager playerDeck;
    public List<Card> cards
    { get; }
    public int handSize = 3;

    [System.NonSerialized]
    public Stack<Card> drawableCards;
    [System.NonSerialized]
    public List<Card> inHandCards;
    [System.NonSerialized]
    public List<Card> previewedCards;
    [System.NonSerialized]
    public Dictionary<Card, float> cardsToCooldowns;
    [System.NonSerialized]
    public List<Card> discardedCards;

    private void Awake()
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

    private void Start()
    {
        drawableCards = new Stack<Card>(cards);
    }

    private void Update()
    {
        foreach (KeyValuePair<Card, float> cardToCooldown in cardsToCooldowns)
        {
            float newValue = cardToCooldown.Value - Time.deltaTime;
            if (newValue <= 0)
            {
                cardsToCooldowns.Remove(cardToCooldown.Key);
                discardedCards.Add(cardToCooldown.Key);
                inHandCards.Remove(cardToCooldown.Key);
            }
            else
            {
                cardsToCooldowns[cardToCooldown.Key] = newValue;
            }
        }
    }


    public void ReshuffleDrawPile()
    {
        while (drawableCards.Count > 0)
        {
            discardedCards.Add(drawableCards.Pop());
        }
        while(discardedCards.Count > 0)
        {
            drawableCards.Push(discardedCards[Random.Range(0, discardedCards.Count)]);
        }
    }

    public bool DrawCard()
    {
        if (inHandCards.Count == handSize)
        {
            return false;
        }

        if (drawableCards.Count == 0)
        {
            ReshuffleDrawPile();
        }

        Card drawnCard = drawableCards.Pop();
        inHandCards.Add(drawnCard);
        return true;
    }

    public void TogglePreviewCard(int handIndex)
    {
        if (inHandCards.Count <= handIndex || cardsToCooldowns.ContainsKey(inHandCards[handIndex]))
        {
            return;
        }
        Card card = inHandCards[handIndex];
        if (previewedCards.Contains(card))
        {
            card.CancelPreview();
            previewedCards.Remove(card);
        }
        else
        {
            card.PreviewEffect();
            previewedCards.Add(card);
        }
    }

    public void PlayPreveiwedCards()
    {
        foreach (Card previewedCard in previewedCards)
        {
            previewedCard.ConfirmPreview();
            cardsToCooldowns.Add(previewedCard, previewedCard.cooldown);
        }
        previewedCards.Clear();
    }
}
