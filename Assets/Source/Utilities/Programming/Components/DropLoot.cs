using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Allows an object to spawn items based off of a loot table.
    /// </summary>
    public class DropLoot : MonoBehaviour
    {
        [Tooltip("The loot table pull the loot from.")]
        [SerializeField] private GameObjectLootTable lootTable;

        /// <summary>
        /// Drops a single item pulled from the loot table.
        /// </summary>
        public void Drop()
        {
            GameObject loot = lootTable.PullFromTable();

            if (loot == null) { return; }

            Instantiate(loot).transform.position = transform.position;
        }
    }
}