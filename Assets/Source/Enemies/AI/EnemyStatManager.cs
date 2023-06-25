using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Represents a singleton that enemies will query for their stats
    /// </summary>
    public class EnemyStatManager : MonoBehaviour
    {
        [Header("Enemies")]
        [Tooltip("Health multiplier (1.33 = 133% base)")] [Min(1f)]
        public static readonly float healthMultiplier = 1f;

        [Tooltip("Projectile damage multiplier (1.33 = 133% base)")] [Min(1f)]
        public static readonly float projectileDamageMultiplier = 1f;

        [Tooltip("Feet damage multiplier (1.33 = 133% base)")] [Min(1f)]
        public static readonly float onTouchDamageMultiplier = 1f;

        [Tooltip("Speed multiplier (1.33 = 133% base)")] [Min(1f)]
        public static readonly float moveSpeedMultiplier = 1f;

        [Header("Traps and Turrets")]
        [Tooltip("Turret damage multiplier (1.33 = 133% base)")] [Min(1f)]
        public static readonly float turretDamageMultiplier = 1f;
        
        [Tooltip("Spike trap damage  multiplier (1.33 = 133% base)")] [Min(1f)]
        public static readonly float spikeTrapDamageMultiplier = 1f; // TODO unimplemented
    }
}