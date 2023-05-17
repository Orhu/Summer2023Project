using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStealer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && DeckManager.playerDeck.cards.Count > 0)
        {
            DeckManager.playerDeck.RemoveCard(DeckManager.playerDeck.cards[Random.Range(0, DeckManager.playerDeck.cards.Count - 1)]);
        }
    }
}
