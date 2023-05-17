using CardSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Tooltip("The cards that can be found in this chest")]
    public List<Card> loots;
    [Tooltip("The weight of each card when picking which card spawned in this chest. Must me the same length as Loots")]
    public List<float> weights;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float totalWeight = 0f;
            foreach (float value in weights)
            {
                totalWeight += value;
            }

            float target = Random.Range(0f, totalWeight);
            float weightProgress = 0f;
            for (int i = 0; i < loots.Count; i++)
            {
                weightProgress += weights[i];
                if (weightProgress >= target)
                {
                    DeckManager.playerDeck.AddCard(loots[i], DeckManager.AddCardLocation.TopOfDrawPile);
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }
}
