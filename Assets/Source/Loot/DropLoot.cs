using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLoot : MonoBehaviour
{
    [Tooltip("The loot table pull the loot from.")]
    [SerializeField] private GameObjectLootTable lootTable;

    public void Drop()
    {
        GameObject loot = lootTable.PullFromTable();

        if (loot == null) { return; }

        Instantiate(loot).transform.position = transform.position;
    }
}
