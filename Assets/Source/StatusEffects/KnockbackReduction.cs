using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A status effect that reduces incoming knockback.
    /// </summary>
    [CreateAssetMenu(menuName = "Status Effects/Damage Reduction")]
    public class KnockbackReduction : StatusEffect
    {
        [Tooltip("The amount incoming knockback is multiplied by.")]
        public float multiplier = 0.25f;

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
            KnockbackReduction instance = (KnockbackReduction)base.CreateCopy(gameObject);

            gameObject.GetComponent<Movement>().requestKnockbackModifications += instance.MultiplyKnockback;

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
        /// <param name="knockbackMultiplier"> The attack to prevent. </param>
        private void MultiplyKnockback(ref float knockbackMultiplier)
        {
            knockbackMultiplier *= multiplier;
        }

        /// <summary>
        /// Cleans up binding.
        /// </summary>
        private new void OnDestroy()
        {
            base.OnDestroy();

            if (gameObject == null) { return; }

            gameObject.GetComponent<Movement>().requestKnockbackModifications -= MultiplyKnockback;
        }
    }
}