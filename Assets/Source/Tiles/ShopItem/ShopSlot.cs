using UnityEngine;
using UnityEngine.Events;

namespace Cardificer
{
    /// <summary>
    /// A slot that contains an item to be sold for coins.
    /// </summary>
    public class ShopSlot : MonoBehaviour
    {
        [Tooltip("The loot table to determine what is sellable in this slot and in what frequencies.")]
        public ShopSlotLootTable possibleItems;

        [Tooltip("The number of times this item can be bought.")] [Min(1)]
        public int buyCount = 1;

        [Tooltip("Called when the price of this item is set. Passes the price as a parameter.")]
        public UnityEvent<int> onPriceSet;

        [Tooltip("Called when this is made buyable.")]
        public UnityEvent onBuyable;

        [Tooltip("Called when this is made not buyable.")]
        public UnityEvent onNotBuyable;

        [Tooltip("Called when has been bought.")]
        public UnityEvent onBought;


        /// <summary>
        /// An object to sell and its price 
        /// </summary>
        [System.Serializable]
        public class PricedObject
        {
            [Tooltip("The price this sells for.")] [Min(1)]
            public int price = 1;

            [Tooltip("The object to sell. Must have a trigger collider to detect when the object is picked up.")]
            public GameObject gameObject;
        }

        /// <summary>
        /// Class that gives an object a price, prevents it form overlapping until the player has enough money, subtracts that money when it does overlap.
        /// </summary>
        [RequireComponent(typeof(Collider2D))]
        private class Price : MonoBehaviour
        {
            // The shop slot this is a part of.
            private ShopSlot shopSlot;

            // The collider responsible for picking up this object.
            private new Collider2D collider;

            // The cost to pick this up.
            public int price;

            // Called when this is destroyed.
            System.Action onDestroyed;



            /// <summary>
            /// Initializes reference.
            /// </summary>
            private void Awake()
            {
                shopSlot = GetComponentInParent<ShopSlot>();
                collider = GetComponent<Collider2D>();
            }

            /// <summary>
            /// Binds updating buyability.
            /// </summary>
            private void Start()
            {
                Player.onMoneyChanged += UpdateBuyability;
                UpdateBuyability();
            }

            /// <summary>
            /// Updates collision based on player money.
            /// </summary>
            private void UpdateBuyability()
            {
                bool buyable = Player.GetMoney() >= price;
                collider.isTrigger = buyable;

                if (buyable)
                {
                    shopSlot.onBuyable?.Invoke();
                }
                else
                {
                    shopSlot.onNotBuyable?.Invoke();
                }
            }

            /// <summary>
            /// Subtracts this objects price when the player overlaps.
            /// </summary>
            /// <param name="collision"> The thing that was overlapped. </param>
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (!collision.CompareTag("Player")) { return; }

                Player.AddMoney(-price);
                price = 0;
            }


            private void OnDestroy()
            {
                if (!gameObject.scene.isLoaded) { return; }
                onDestroyed?.Invoke();
            }
        }

        /// <summary>
        /// Spawns the sellable object and gives it a price.
        /// </summary>
        private void Start()
        {
            PricedObject pricedObject = possibleItems.PullFromTable();

            GameObject sellableItem = Instantiate(pricedObject.gameObject);
            sellableItem.transform.parent = transform;

            Price price = sellableItem.AddComponent<Price>();
            price.price = pricedObject.price;
            onPriceSet?.Invoke(pricedObject.price);
        }

        
    }
}