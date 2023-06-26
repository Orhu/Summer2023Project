using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Represents a singleton that enemies will query for their stats
    /// </summary>
    public class DifficultyProgressionManager : MonoBehaviour
    {
        // The instance representing the DifficultyProgressionManager singleton
        private static DifficultyProgressionManager instance;
        
        [Header("Enemies")]
        [Tooltip("Health multiplier (1.33 = 133% base)")] [Min(1f)]
        [SerializeField] private float _healthMultiplier = 1f;
        public static float healthMultiplier => instance._healthMultiplier;
        
        [Tooltip("Projectile damage multiplier (1.33 = 133% base)")] [Min(1f)]
        [SerializeField] private float _projectileDamageMultiplier = 1f;
        public static float projectileDamageMultiplier => instance._projectileDamageMultiplier;

        [Tooltip("Feet damage multiplier (1.33 = 133% base)")] [Min(1f)]
        [SerializeField] private float _onTouchDamageMultiplier = 1f;
        public static float onTouchDamageMultiplier => instance._onTouchDamageMultiplier;

        [Tooltip("Speed multiplier (1.33 = 133% base)")] [Min(1f)]
        [SerializeField] private float _moveSpeedMultiplier = 1f;
        public static float moveSpeedMultiplier => instance._moveSpeedMultiplier;

        [Header("Traps and Turrets")]
        [Tooltip("Turret damage multiplier (1.33 = 133% base)")] [Min(1f)]
        [SerializeField] private float _turretDamageMultiplier = 1f;
        public static float turretDamageMultiplier => instance._turretDamageMultiplier;

        [Tooltip("Spike trap damage  multiplier (1.33 = 133% base)")] [Min(1f)]
        [SerializeField] private float _spikeTrapDamageMultiplier = 1f;
        public static float spikeTrapDamageMultiplier => instance._spikeTrapDamageMultiplier; // TODO unimplemented

        /// <summary>
        /// Assigns the instance to this instance
        /// </summary>
        private void Awake()
        {
            instance = this;
        }
    }
}