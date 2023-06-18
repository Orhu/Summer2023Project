using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// This component allows something to summon a prefab under it every X seconds
    /// </summary>
    public class DropEveryXSeconds : MonoBehaviour
    {
        [Tooltip("How long before the first drop?")]
        [SerializeField] private float initialDropDelay;

        [Tooltip("How many seconds between every drop?")]
        [SerializeField] private float delayBetweenDrops = 1;

        [Tooltip("What prefab to drop?")]
        [SerializeField] private GameObject dropPrefab;

        /// <summary>
        /// Begins the InvokeRepeating call to repeatedly drop items 
        /// </summary>
        void Start()
        {
            InvokeRepeating(nameof(DropItem), initialDropDelay, delayBetweenDrops);
        }

        /// <summary>
        /// Instantiates an item on the ground below this unit
        /// </summary>
        void DropItem()
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }
    }
}