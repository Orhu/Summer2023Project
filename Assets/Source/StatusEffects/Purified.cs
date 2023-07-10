using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A status effect that prevents health from receiving status effects.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPurified", menuName = "Status Effects/Purified")]
    public class Purified : StatusEffect
    {
        /// <summary>
        /// Creates a new status effect that is a copy of the caller.
        /// </summary>
        /// <param name="gameObject"> The object to apply the status effect.</param>
        /// <returns> The status effect that was created. </returns>
        public override StatusEffect CreateCopy(GameObject gameObject)
        {
            Purified instance = (Purified)base.CreateCopy(gameObject);

            gameObject.GetComponent<Health>().onRequestIncomingAttackModification += instance.PreventStatusEffects;

            return instance;
        }

        /// <summary>
        /// Stacks this effect onto another status effect.
        /// </summary>
        /// <param name="other"> The other particle effect to stack this onto. </param>
        /// <returns> Whether or not this status effect was consumed by the stacking. </returns>
        public override bool Stack(StatusEffect other)
        {
            if (other.GetType() != GetType())
            {
                return false;
            }

            other.remainingDuration += duration / 2f;
            return true;
        }

        /// <summary>
        /// Responds to a health's incoming damage modification request, and prevents the status effects from passing.
        /// </summary>
        /// <param name="attack"> The attack to prevent. </param>
        private void PreventStatusEffects(ref DamageData attack)
        {
            attack = new DamageData(attack.damage, attack.damageType, attack.causer);
        }

        /// <summary>
        /// Cleans up binding.
        /// </summary>
        private new void OnDestroy()
        {
            base.OnDestroy();

            if (gameObject == null) { return; }

            gameObject.GetComponent<Health>().onRequestIncomingAttackModification -= PreventStatusEffects;
        }
    }
}
