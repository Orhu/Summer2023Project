using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A status effect that increases movement speed.
    /// </summary>
    [CreateAssetMenu(fileName = "NewFlowing", menuName = "Status Effects/Flowing")]
    public class Flowing : StatusEffect
    {
        [Tooltip("The speed in tiles/s that is added to the player's speed")] [Range(0f, 1f)]
        [SerializeField] private float addedSpeed = 0.25f;

        [Tooltip("Max duration of the effect")]
        [SerializeField] private float maxDuration = 20f;

        /// <summary>
        /// Creates a new status effect that is a copy of the caller.
        /// </summary>
        /// <param name="gameObject"> The object to apply the status effect.</param>
        /// <returns> The status effect that was created. </returns>
        public override StatusEffect CreateCopy(GameObject gameObject)
        {
            Flowing instance = (Flowing)base.CreateCopy(gameObject);

            gameObject.GetComponent<Movement>().requestSpeedModifications += instance.IncreaseMovement;

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

            other.remainingDuration = Mathf.Min(duration + other.remainingDuration, maxDuration);
            return true;
        }

        /// <summary>
        /// Responds to a movement components speed modification request, and sets the speed to 0.
        /// </summary>
        /// <param name="speed"> The speed variable to be modified. </param>
        private void IncreaseMovement(ref float speed)
        {
            if (speed <= 0) { return; } 
            speed += addedSpeed * stacks;
        }

        /// <summary>
        /// Cleans up binding.
        /// </summary>
        private new void OnDestroy()
        {
            if (gameObject != null)
            {
                gameObject.GetComponent<Movement>().requestSpeedModifications -= IncreaseMovement;
            }
            base.OnDestroy();
        }
    }
}