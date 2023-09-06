using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A status effect that increases incoming damage.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCursed", menuName = "Status Effects/Cursed")]
    public class Cursed : StatusEffect
    {
        [Tooltip("The amount incoming damage is multiplied by.")]
        public float damageMultiplier = 2f;

        [Tooltip("The amount of time to add when stacked in seconds.")]
        public float additionalStackTime = 1f;

        [Tooltip("The max duration of this effect.")]
        public float maxDuration = 5f;

        /// <summary>
        /// Creates a new status effect that is a copy of the caller.
        /// </summary>
        /// <param name="gameObject"> The object to apply the status effect.</param>
        /// <returns> The status effect that was created. </returns>
        public override StatusEffect CreateCopy(GameObject gameObject)
        {
            Cursed instance = (Cursed)base.CreateCopy(gameObject);

            gameObject.GetComponent<Health>().onRequestIncomingAttackModification += instance.MultiplyDamage;

            return instance;
        }

        /// <summary>
        /// Stacks this effect onto another status effect.
        /// </summary>
        /// <param name="other"> The other particle effect to stack this onto. </param>
        /// <returns> Whether or not this status effect was consumed by the stacking. </returns>
        public override bool Stack(StatusEffect other)
        {
            if (!base.Stack(other))
            {
                return false;
            }

            other.remainingDuration = Mathf.Max(maxDuration, other.remainingDuration + additionalStackTime);
            return true;
        }

        /// <summary>
        /// Responds to a health's incoming damage modification request, and prevents the attack from passing.
        /// </summary>
        /// <param name="attack"> The attack to prevent. </param>
        private void MultiplyDamage(ref DamageData attack)
        {
            attack.damage = (int)(attack.damage * damageMultiplier);
        }

        /// <summary>
        /// Cleans up binding.
        /// </summary>
        private new void OnDestroy()
        {
            base.OnDestroy();

            if (gameObject == null) { return; }

            gameObject.GetComponent<Health>().onRequestIncomingAttackModification -= MultiplyDamage;
        }

        public override StatusEffectType StatusType()
        {
            return StatusEffectType.Cursed;
        }

    }
}