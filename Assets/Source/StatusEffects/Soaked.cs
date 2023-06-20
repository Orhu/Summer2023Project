using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A status effect that increases incoming damage.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSoaked", menuName = "Status Effects/Soaked")]
    public class Soaked : StatusEffect
    {
        [Tooltip("The number of extra stacks that are applied every time a status effect is applied.")]
        public int additionalStacks = 1;

        [Tooltip("The amount of time to add when stacked in seconds.")]
        public float additionalStackTime = 2f;

        [Tooltip("The max duration of this effect.")]
        public float maxDuration = 10f;

        /// <summary>
        /// Creates a new status effect that is a copy of the caller.
        /// </summary>
        /// <param name="gameObject"> The object to apply the status effect.</param>
        /// <returns> The status effect that was created. </returns>
        public override StatusEffect CreateCopy(GameObject gameObject)
        {
            Soaked instance = (Soaked)base.CreateCopy(gameObject);

            gameObject.GetComponent<Health>().onRequestIncomingAttackModification += instance.MutiplyStatusEffects;

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

            other.remainingDuration = Mathf.Min(maxDuration, other.remainingDuration + additionalStackTime);
            return true;
        }

        /// <summary>
        /// Responds to a health's incoming damage modification request, and prevents the attack from passing.
        /// </summary>
        /// <param name="attack"> The attack to prevent. </param>
        private void MutiplyStatusEffects(ref DamageData attack)
        {
            DamageData orignalAttack = new DamageData(attack, attack.causer);
            for (int i = 0; i < additionalStacks; i++)
            {
                attack += orignalAttack.statusEffects;
            }
        }

        /// <summary>
        /// Cleans up binding.
        /// </summary>
        private new void OnDestroy()
        {
            base.OnDestroy();

            if (gameObject == null) { return; }

            gameObject.GetComponent<Health>().onRequestIncomingAttackModification -= MutiplyStatusEffects;
        }
    }
}