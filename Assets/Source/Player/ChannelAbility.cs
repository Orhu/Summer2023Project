using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Component that allows for something to channel in order to reset its cooldowns.
    /// </summary>
    public class ChannelAbility : MonoBehaviour
    {
        [Tooltip("The time it takes to channel in seconds")]
        [SerializeField] private float channelDuration = 2f;

        [Tooltip("The status effect to apply while channeling")]
        [SerializeField] private StatusEffect statusEffect;

        [Tooltip("The interval at which the status effect is applied")]
        [SerializeField] private float statusEffectInertval = 0.1f;

        /// <summary>
        /// Starts channeling and applying the status effect.
        /// </summary>
        public void StartChanneling()
        {
            Invoke("Channel", channelDuration);
            InvokeRepeating("ApplyStatusEffect", 0f, statusEffectInertval);
        }

        /// <summary>
        /// Cancels the channeling without causing its final effect.
        /// </summary>
        public void StopChanneling()
        {
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
    }
}