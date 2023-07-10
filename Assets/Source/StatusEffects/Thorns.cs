using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A status effect that causes this to deal damage back to the attacker.
    /// </summary>
    [CreateAssetMenu(fileName = "NewThorns", menuName = "Status Effects/Thorns")]
    public class Thorns : StatusEffect
    {
        [Tooltip("The damage that will be dealt back to attackers")]
        public DamageData damage;

        /// <summary>
        /// Creates a new status effect that is a copy of the caller.
        /// </summary>
        /// <param name="gameObject"> The object to apply the status effect.</param>
        /// <returns> The status effect that was created. </returns>
        public override StatusEffect CreateCopy(GameObject gameObject)
        {
            Thorns instance = (Thorns)base.CreateCopy(gameObject);

            gameObject.GetComponent<Health>().onRequestIncomingAttackModification += instance.AttackBack;

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
        /// Responds to a health's incoming damage modification request, and deals damage back to the causer.
        /// </summary>
        /// <param name="attack"> The attack to prevent. </param>
        private void AttackBack(ref DamageData attack)
        {
            attack.causer?.GetComponent<Health>()?.ReceiveAttack(damage);
        }

        /// <summary>
        /// Cleans up binding.
        /// </summary>
        private new void OnDestroy()
        {
            base.OnDestroy();

            if (gameObject == null) { return; }

            gameObject.GetComponent<Health>().onRequestIncomingAttackModification -= AttackBack;
        }
    }
}
