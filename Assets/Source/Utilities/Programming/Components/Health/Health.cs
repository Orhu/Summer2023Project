using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


namespace Cardificer
{
    /// <summary>
    /// Add this component to make an object damageable.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [Tooltip("The Max health of this object")]
        [Min(1)]
        [SerializeField] private int _maxHealth = 5;
        public int maxHealth
        {
            set
            {
                if (_maxHealth == value) { return; }

                _maxHealth = value;
            }
            get => _maxHealth;
        }
        
        // The current health of this object
        private int _currentHealth = 0;
        public int currentHealth
        {
            get => _currentHealth;
            set
            {
                if (_currentHealth == value) { return; }

                _currentHealth = value;
            }
        }

        [Tooltip("How long of a duration does this unit get invincibility when hit?")]
        public float invincibilityDuration = 0.25f;

        [Tooltip("All status effects this is immune to.")]
        public List<StatusEffect> immuneStatusEffects = new List<StatusEffect>();

        [Tooltip("Called when this dies")]
        public UnityEvent onDeath;

        // Called when invincibility changes and passes the new invincibility
        public Action<bool> onInvincibilityChanged;

        // Called before this processes an attack and passes the incoming attack so can be modified.   
        public RequestIncomingAttackModification onRequestIncomingAttackModification;
        public delegate void RequestIncomingAttackModification(ref DamageData attack);
        
        // Called when damage is taken.
        public System.Action onDamageTaken;

        // is this unit currently invincible?
        private bool _invincible = false;
        private bool invincible
        {
            set
            {
                CancelInvoke(nameof(TurnOffInvincibility));
                if (value)
                {
                    Invoke(nameof(TurnOffInvincibility), invincibilityDuration);
                }

                _invincible = value;
                onInvincibilityChanged?.Invoke(value);
            }
            get { return _invincible; }
        }

        // All status effects currently affecting this
        private List<StatusEffect> statusEffects = new List<StatusEffect>();

        /// <summary>
        /// Initializes current health.
        /// </summary>
        void Start()
        {            
            if (currentHealth != 0) { return; }
            currentHealth = maxHealth;
        }

        /// <summary>
        /// Update status effects and prunes null values
        /// </summary>
        void Update()
        {
            for (int i = 0; i < statusEffects.Count; i++)
            {
                statusEffects[i]?.Update();
                if (statusEffects[i] == null)
                {
                    statusEffects.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }

        /// <summary>
        /// Receive an attack and kill the owner if out of health.
        /// </summary>
        /// <param name="attack"> The attack being received. </param>
        /// <param name="ignoreInvincibility"> Whether or not invincibility frames should effect this attack. </param>
        public void ReceiveAttack(DamageData attack, bool ignoreInvincibility = false)
        {
            if (currentHealth <= 0 || (invincible && !ignoreInvincibility)) { return; }

            // Damage
            onRequestIncomingAttackModification?.Invoke(ref attack);
            var prevHealth = currentHealth;
            currentHealth = Mathf.Clamp(currentHealth - attack.damage, 0, maxHealth);

            if (attack.damage > 0)
            {
                onDamageTaken?.Invoke();
            }

            if (currentHealth <= 0 && prevHealth > 0)
            {
                onDeath?.Invoke();
                return;
            }
            else if (invincibilityDuration != 0 && (attack.damage > 0 != attack.invertInvincibility))
            {
                invincible = true;
            }

            // Status effects
            if (CompareTag("Inanimate")) { return; }
            foreach (StatusEffect statusEffect in attack.statusEffects)
            {
                if (!immuneStatusEffects.Contains(statusEffect))
                {
                    StatusEffect matchingEffect = statusEffects.Find(statusEffect.Stack);
                    if (matchingEffect == null)
                    {
                        statusEffects.Add(statusEffect.CreateCopy(gameObject));
                    }
                }
            }
        }

        /// <summary>
        /// Increases the current health by the given amount, maxed out at the max health
        /// </summary>
        /// <param name="healAmount"> The amount to heal by. </param>
        public void Heal(int healAmount)
        {
            currentHealth = Mathf.Min(Math.Max(healAmount, 0) + currentHealth, maxHealth);
        }

        /// <summary>
        /// Whether or not this has a given status effect.
        /// </summary>
        /// <param name="statusEffect"> The effect to check for. </param>
        /// <returns> True if this has the given effect. </returns>
        public bool HasStatusEffect(StatusEffect statusEffect)
        {
            return statusEffects.FirstOrDefault((StatusEffect eachEffect) => { return eachEffect.GetType() == statusEffect.GetType(); }) != null;
        }


        /// <summary>
        /// Removes all status effects
        /// </summary>
        public void Cleanse()
        {
            while(statusEffects.Count > 0)
            {
                Destroy(statusEffects[statusEffects.Count - 1]);
                statusEffects.RemoveAt(statusEffects.Count - 1);
            }
        }

        /// <summary>
        /// Disable invincibility
        /// </summary>
        private void TurnOffInvincibility()
        {
            invincible = false;
        }
    }
}
