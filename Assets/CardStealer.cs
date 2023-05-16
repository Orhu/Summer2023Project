using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStealer : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player && DeckManager.playerDeck.cards.Count > 0)
        {
            DeckManager.playerDeck.RemoveCard(DeckManager.playerDeck.cards[Random.Range(0, DeckManager.playerDeck.cards.Count - 1)]);
        }
    }
}
