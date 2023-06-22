using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a singleton that enemies will query for their stats
/// </summary>
public class EnemyStatManager : MonoBehaviour
{
    [Tooltip("Health multiplier (1 = base)")]
    public float healthMultiplier = 1f;
    
    [Tooltip("Damage multiplier (1 = base)")]
    public float damageMultiplier = 1f;

    [Tooltip("Speed multiplier (1 = base)")]
    public float moveSpeedMultiplier = 1f;

    // instance
    [HideInInspector] public static EnemyStatManager _instance;

    /// <summary>
    /// Initialize instance to this instance
    /// </summary>
    void Awake()
    {
        _instance = this;
    }
}
