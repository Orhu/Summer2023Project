using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterPack : MonoBehaviour
{
    [Tooltip("Number of cards that this booster pack will produce. Max = 5, min = 1")]
    public int numCards;
    [Tooltip("Card Table to get probability of card drop")]
    public CardLootTable lootTable;

    /// <summary>
    /// When the player enters the trigger zone, open booster pack menu
    /// populate and display cards on the menu
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (lootTable != null)
            {
                // Set booster pack menu to active
                MenuManager.instance.boosterPackMenu.gameObject.SetActive(true);
                // Set local booster pack variable in booster pack menu to be this pack
                MenuManager.instance.boosterPackMenu.boosterPackObject = this;
                // Draw new cards and display them on the booster pack menu
                MenuManager.instance.boosterPackMenu.PopulateBoosterPackCards(numCards, lootTable);
            }
            else
            {
                throw new System.Exception("Loot table not assigned to BoosterPack prefab");
            }
        }
    }
}
