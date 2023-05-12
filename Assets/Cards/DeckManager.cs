using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;

public class DeckManager : MonoBehaviour
{
    public static DeckManager playerDeck;
    public List<Card> cards;
    public int handSize = 3;

    public delegate void deckChangedNotification();
    public deckChangedNotification onCardDrawn;

    [System.NonSerialized]
    public List<Card> drawableCards;
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
        drawableCards = new List<Card>(cards);

        for (int i = 0; i < handSize; i++)
        {
            DrawCard();
        }
    }

    private void Update()
    {
        List<int> offCooldownCardIndices = new List<int>();
        foreach (KeyValuePair<int, float> cardIndexToCooldown in new Dictionary<int, float>(cardIndicesToCooldowns))
        {
            float newValue = cardIndexToCooldown.Value - Time.deltaTime;
            if (newValue <= 0)
            {
                cardIndicesToCooldowns.Remove(cardIndexToCooldown.Key);

                offCooldownCardIndices.Add(cardIndexToCooldown.Key);
            }
            else
            {
                cardIndicesToCooldowns[cardIndexToCooldown.Key] = newValue;
            }
        }
        offCooldownCardIndices.Sort();
        offCooldownCardIndices.Reverse();
        foreach (int offCooldownCardIndex in offCooldownCardIndices)
        {
            Card card = inHandCards[offCooldownCardIndex];
            discardedCards.Add(card);
            inHandCards.Remove(card);
            DrawCard();
        }
    }


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

        Card drawnCard = drawableCards[drawableCards.Count - 1];
        drawableCards.RemoveAt(drawableCards.Count - 1);
        inHandCards.Add(drawnCard);
        onCardDrawn();
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
        foreach (int previewedCardIndex in previewedCardIndices)
        {
            Card card = inHandCards[previewedCardIndex];
            card.ConfirmPreview();
            cardIndicesToCooldowns.Add(previewedCardIndex, card.cooldown);
        }
        previewedCardIndices.Clear();
    }
}
