using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;

public class DeckManager : MonoBehaviour
{
    public static DeckManager playerDeck;
    public List<Card> cards;
    public int handSize = 3;

    [System.NonSerialized]
    public Stack<Card> drawableCards;
    [System.NonSerialized]
    public List<Card> inHandCards = new List<Card>();
    [System.NonSerialized]
    public List<Card> previewedCards = new List<Card>();
    [System.NonSerialized]
    public Dictionary<Card, float> cardsToCooldowns = new Dictionary<Card, float>();
    [System.NonSerialized]
    public List<Card> discardedCards = new List<Card>();

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

        for (int i = 0; i < handSize; i++)
        {
            DrawCard();
        }
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
                DrawCard();
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

    private bool DrawCard()
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
