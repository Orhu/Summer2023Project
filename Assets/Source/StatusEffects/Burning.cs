using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// A status effect the does damage over time proportional to stacks, and who's duration is reset on each stack.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBurning", menuName = "Status Effects/Burning")]
    public class Burning : StatusEffect
    {
        [Tooltip("Damage per second per stack.")]
        [SerializeField] private float dps = 2f;

        [Tooltip("The max duration of this effect.")]
        [SerializeField] private float maxDuration = 4f;

        // The time until the next damage tick is applied
        private float timeToDamage;

        /// <summary>
        /// Initialize timeToDamage
        /// </summary>
        private void Awake()
        {
            timeToDamage = 1f / dps;
        }

        /// <summary>
        /// Causes damage over time.
        /// </summary>
        public override void Update()
        {
            base.Update();
            timeToDamage -= Time.deltaTime;
            if (timeToDamage <= 0)
            {
                gameObject.GetComponent<Health>().ReceiveAttack(new DamageData(1, DamageData.DamageType.Special, null, true), true);
                timeToDamage += 1f / dps;
            }
        }

        /// <summary>
        /// Stacks this effect onto another status effect.
        /// </summary>
        /// <param name="other"> The other particle effect to stack this onto. </param>
        /// <returns> Whether or not this status effect was consumed by the stacking. </returns>
        public override bool Stack(StatusEffect other)
        {
            if (base.Stack(other))
            {
                other.remainingDuration = Mathf.Min(other.remainingDuration + duration, maxDuration);
                return true;
            }
            return false;
        }
    }
}