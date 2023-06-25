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
        [Min(1f)]
        [SerializeField] private float _healthMultiplier = 1f;
        [Tooltip("Health multiplier (1.33 = 133% base)")]
        public static float healthMultiplier => instance._healthMultiplier;
        
        [Min(1f)]
        [SerializeField] private float _projectileDamageMultiplier = 1f;
        [Tooltip("Projectile damage multiplier (1.33 = 133% base)")]
        public static float projectileDamageMultiplier => instance._projectileDamageMultiplier;

        [Min(1f)]
        [SerializeField] private float _onTouchDamageMultiplier = 1f;
        [Tooltip("Feet damage multiplier (1.33 = 133% base)")] 
        public static float onTouchDamageMultiplier => instance._onTouchDamageMultiplier;

        [Min(1f)]
        [SerializeField] private float _moveSpeedMultiplier = 1f;
        [Tooltip("Speed multiplier (1.33 = 133% base)")] 
        public static float moveSpeedMultiplier => instance._moveSpeedMultiplier;

        [Header("Traps and Turrets")]
        [Min(1f)]
        [SerializeField] private float _turretDamageMultiplier = 1f;
        [Tooltip("Turret damage multiplier (1.33 = 133% base)")] 
        public static float turretDamageMultiplier => instance._turretDamageMultiplier;

        [Min(1f)]
        [SerializeField] private float _spikeTrapDamageMultiplier = 1f;
        [Tooltip("Spike trap damage  multiplier (1.33 = 133% base)")] 
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