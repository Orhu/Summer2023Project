using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        InvokeRepeating(nameof(DropItem), initialDropDelay, delayBetweenDrops);
    }

    void DropItem()
    {
        Instantiate(dropPrefab, transform.position, Quaternion.identity);
    }
}
