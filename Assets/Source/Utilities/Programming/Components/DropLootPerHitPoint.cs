using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Allows an object to spawn items based off of a loot table.
    /// </summary>
    public class DropLootPerHitPoint : MonoBehaviour
    {
        [Tooltip("The loot table pull the loot from.")]
        [SerializeField] private GameObjectLootTable lootTable;

        /// <summary>
        /// Drops a single item pulled from the loot table.
        /// </summary>
        public void Drop()
        {
            for (int i = 0; i < GetComponent<Health>().maxHealth; i++)
            {
                GameObject loot = lootTable.weightedLoot.GetRandomThing(transform.position);

                if (loot == null) { continue; }

                Instantiate(loot).transform.position = transform.position + (Vector3)Random.insideUnitCircle;
            }
        }
    }
}