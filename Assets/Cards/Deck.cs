using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;

public class Deck : MonoBehaviour
{
    public static Deck playerDeck;
    public List<Card> cards;

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
}
