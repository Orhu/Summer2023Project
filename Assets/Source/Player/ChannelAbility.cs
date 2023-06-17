using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    public class ChannelAbility : MonoBehaviour
    {
        [Tooltip("The time it takes to channel in seconds")]
        [SerializeField] private float channelDuration = 2f;

        [Tooltip("The status effect to apply while channeling")]
        [SerializeField] private StatusEffect statusEffect;

        [Tooltip("The interval at which the status effect is applied")]
        [SerializeField] private float statusEffectInertval = 0.1f;

        public void StartChanneling()
        {
            Invoke("Channel", channelDuration);
            InvokeRepeating("ApplyStatusEffect", 0f, statusEffectInertval);
        }

        public void StopChanneling()
        {
            CancelInvoke();
        }

        private void ApplyStatusEffect()
        {
            Player.health.ReceiveAttack(new DamageData(new List<StatusEffect>() { statusEffect }, null));
        }

        private void Channel()
        {
            Deck.playerDeck.ClearCooldowns();
            CancelInvoke();
        }
    }
}