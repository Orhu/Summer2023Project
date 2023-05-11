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
    public List<int> previewedCardIndices = new List<int>();
    [System.NonSerialized]
    public Dictionary<int, float> cardIndicesToCooldowns = new Dictionary<int, float>();
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
        foreach (KeyValuePair<int, float> cardIndexToCooldown in cardIndicesToCooldowns)
        {
            float newValue = cardIndexToCooldown.Value - Time.deltaTime;
            if (newValue <= 0)
            {
                cardIndicesToCooldowns.Remove(cardIndexToCooldown.Key);

                Card card = inHandCards[cardIndexToCooldown.Key];
                discardedCards.Add(card);
                inHandCards.Remove(card);
                DrawCard();
            }
            else
            {
                cardIndicesToCooldowns[cardIndexToCooldown.Key] = newValue;
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
        if (inHandCards.Count <= handIndex || cardIndicesToCooldowns.ContainsKey(handIndex))
        {
            return;
        }
        Card card = inHandCards[handIndex];
        if (previewedCardIndices.Contains(handIndex))
        {
            card.CancelPreview();
            previewedCardIndices.Remove(handIndex);
        }
        else
        {
            card.PreviewEffect();
            previewedCardIndices.Add(handIndex);
        }
    }

    public void PlayPreveiwedCards()
    {
        for (int i = 0; i < previewedCardIndices.Count; i++)
        {
            Card card = inHandCards[i];
            card.ConfirmPreview();
            cardIndicesToCooldowns.Add(i, card.cooldown);
        }
        previewedCardIndices.Clear();
    }
}
