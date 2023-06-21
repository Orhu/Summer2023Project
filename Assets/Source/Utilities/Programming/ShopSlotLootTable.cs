using UnityEngine;

namespace Cardificer
{
    [CreateAssetMenu(fileName = "NewShopSlotLootTable", menuName = "Loot/ShopSlotLootTable")]
    public class ShopSlotLootTable : LootTable<ShopSlot.PricedObject> { }
}