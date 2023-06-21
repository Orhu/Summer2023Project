using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Cardificer
{
    /// <summary>
    /// A slot that contains an item to be sold for coins.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ShopSlot : MonoBehaviour
    {
        [Tooltip("The loot table to determine what is sellable in this slot and in what frequencies.")]
        public ShopSlotLootTable possibleItems;

        [Tooltip("The number of times this item can be bought.")] [Min(1)]
        public int buyCount = 1;

        [Tooltip("Called when the price of this item is set. Passes the price as a parameter.")]
        public UnityEvent<string> onPriceSet;

        [Tooltip("Called when this is made buyable.")]
        public UnityEvent onBuyable;

        [Tooltip("Called when this is made not buyable.")]
        public UnityEvent onNotBuyable;

        [Tooltip("Called when has been bought.")]
        public UnityEvent onBought;


        // The locations mapped to the remaining buy count of every shop slot that has been bought from.
        public static Dictionary<Vector2Int, int> _locationsToRemainingShopBuys;
        private static Dictionary<Vector2Int, int> locationsToRemainingShopBuys
        {
            get
            {
                if (_locationsToRemainingShopBuys != null) { return _locationsToRemainingShopBuys; }

                if (SaveManager.autosaveExists)
                {
                    Vector3Int[] keyValuePairs = SaveManager.savedRemainingShopBuys;
                    _locationsToRemainingShopBuys = keyValuePairs.ToDictionary(
                        // Extract Keys
                        (Vector3Int keyValuePair) =>
                        {
                            return new Vector2Int(keyValuePair.x, keyValuePair.y);
                        },
                        // Extract Values
                        (Vector3Int keyValuePair) =>
                        {
                            return keyValuePair.z;
                        });
                }
                else
                {
                    _locationsToRemainingShopBuys = new Dictionary<Vector2Int, int>();
                }
                SceneManager.sceneUnloaded += (Scene scene) => { locationsToRemainingShopBuys = null; };

                return _locationsToRemainingShopBuys;
            }
            set => _locationsToRemainingShopBuys = value;
        }

        // The locations mapped to the remaining buy count of every shop slot that has been bought from formated as a single Vector3Int.
        public static Vector3Int[] savableRemainingShopBuys
        {
            get
            {
                return locationsToRemainingShopBuys.Select(
                    // Combine key value pairs
                    (KeyValuePair<Vector2Int, int> keyValuePair) =>
                    {
                        return new Vector3Int(keyValuePair.Key.x, keyValuePair.Key.y, keyValuePair.Value);
                    }).ToArray();
            }
        }


        // The collider responsible for detecting if the player is overlapping.
        private new Collider2D collider;

        // The integer position of this slot.
        private Vector2Int intPosition;



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
            public System.Action onDestroyed;

            // Whether or not this has been bought.
            bool bought = false;



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
                if (bought) { return; }

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
                if (bought) { return; }
                if (!collision.CompareTag("Player")) { return; }

                bought = true;
                Player.AddMoney(-price);
                shopSlot.onBought?.Invoke();
            }


            private void OnDestroy()
            {
                Player.onMoneyChanged -= UpdateBuyability;
                if (!gameObject.scene.isLoaded) { return; }
                onDestroyed?.Invoke();
            }
        }




        /// <summary>
        /// Spawns the sellable object and gives it a price.
        /// </summary>
        private void Start()
        {
            intPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

            int savedBuyCount;
            if (locationsToRemainingShopBuys.TryGetValue(intPosition, out savedBuyCount))
            {
                buyCount = savedBuyCount;

                if (buyCount <= 0)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            collider = GetComponent<Collider2D>();
            StartCoroutine(SpawnBuyableObject());

            onBought.AddListener(
                // Update buy count
                () => 
                { 
                    buyCount--;
                    locationsToRemainingShopBuys[intPosition] = buyCount;
                });
        }

        /// <summary>
        /// Spawn a new object that can be bought.
        /// </summary>
        private IEnumerator SpawnBuyableObject()
        {
            while (collider.IsTouching(Player.feet))
            {
                yield return new WaitForSeconds(1f);
            }

            PricedObject pricedObject = possibleItems.PullFromTable();

            GameObject sellableItem = Instantiate(pricedObject.gameObject);
            sellableItem.transform.parent = transform;
            sellableItem.transform.localPosition = Vector3.zero;

            Price price = sellableItem.AddComponent<Price>();
            price.price = pricedObject.price;
            onPriceSet?.Invoke(pricedObject.price.ToString());


            if (buyCount - 1 <= 0)
            {
                price.onDestroyed += () => { Destroy(gameObject); };
            }
            else
            {
                price.onDestroyed += () => { StartCoroutine(SpawnBuyableObject()); };
            }
        }
    }
}