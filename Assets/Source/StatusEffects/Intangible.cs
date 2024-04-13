using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// A status effect that prevents health from receiving attacks.
    /// </summary>
    [CreateAssetMenu(fileName = "NewIntangible", menuName = "Status Effects/Intangible")]
    public class Intangible : StatusEffect
    {
        [Tooltip("Max duration of the effect")]
        [SerializeField] private float maxDuration = 16f;
        
        /// <summary>
        /// Creates a new status effect that is a copy of the caller.
        /// </summary>
        /// <param name="gameObject"> The object to apply the status effect.</param>
        /// <returns> The status effect that was created. </returns>
        public override StatusEffect CreateCopy(GameObject gameObject)
        {
            Intangible instance = (Intangible)base.CreateCopy(gameObject);

            gameObject.GetComponent<Collider2D>().enabled = false;

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
            other.remainingDuration = Mathf.Min(duration + other.remainingDuration, maxDuration);
            return true;
        }

        /// <summary>
        /// Cleans up binding.
        /// </summary>
        private new void OnDestroy()
        {
            base.OnDestroy();

            if (gameObject == null) { return; }
            gameObject.GetComponent<Collider2D>().enabled = true;
        }

        public override StatusEffectType StatusType()
        {
            return StatusEffectType.Intangible;
        }
    }
}