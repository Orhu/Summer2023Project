using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Component that allows for something to channel in order to reset its cooldowns.
    /// </summary>
    public class ChannelAbility : MonoBehaviour
    {
        [Tooltip("The cooldown multiplier when channeling is first started.")] [Min(1f)]
        [SerializeField] private float initialCooldownReduction = 1f;

        [Tooltip("The max cooldown reduction from channeling.")] [Min(1f)]
        [SerializeField] private float maxCooldownReduction = 3f;

        [Tooltip("The amount of additional cooldown reduction gained every second")]
        [SerializeField] private float cooldownReductionIncreaseRate = 1f;

        [Tooltip("The status effect to apply while channeling")]
        [SerializeField] private StatusEffect statusEffect;

        [Tooltip("The interval at which the status effect is applied")]
        [SerializeField] private float statusEffectInertval = 0.1f;


        // Whether or not this is currently channeling
        public bool isChanneling { get; private set; } = false;

        private float currentCooldownReduction;

        /// <summary>
        /// Starts channeling and applying the status effect.
        /// </summary>
        public void StartChanneling()
        {
            isChanneling = true;
            currentCooldownReduction = initialCooldownReduction;
            Deck.playerDeck.cooldownReduction *= initialCooldownReduction;
            InvokeRepeating("ApplyStatusEffect", 0f, statusEffectInertval);
        }

        /// <summary>
        /// Cancels the channeling without causing its final effect.
        /// </summary>
        public void StopChanneling()
        {
            isChanneling = false;
            CancelInvoke();
        }

        /// <summary>
        /// Applies the status effect of this channeling ability.
        /// </summary>
        private void ApplyStatusEffect()
        {
            Player.health.ReceiveAttack(new DamageData(new List<StatusEffect>() { statusEffect }, null));
        }

        /// <summary>
        /// Resets the player's cooldowns, and stops applying status effects.
        /// </summary>
        private void Channel()
        {
            Deck.playerDeck.ClearCooldowns();
            CancelInvoke();
        }

        /// <summary>
        /// Updates the player's cooldowns
        /// </summary>
        private void Update()
        {
            if (!isChanneling) { return; }
            Deck.playerDeck.cooldownReduction /= currentCooldownReduction;
            currentCooldownReduction += cooldownReductionIncreaseRate * Time.deltaTime;
            Deck.playerDeck.cooldownReduction *= currentCooldownReduction;
        }
    }
}