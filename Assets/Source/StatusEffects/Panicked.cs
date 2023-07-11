using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A status effect that causes things to move in the opposite direction as intended.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPanicked", menuName = "Status Effects/Panicked")]
    public class Panicked : StatusEffect
    {
        /// <summary>
        /// Creates a new status effect that is a copy of the caller.
        /// </summary>
        /// <param name="gameObject"> The object to apply the status effect.</param>
        /// <returns> The status effect that was created. </returns>
        public override StatusEffect CreateCopy(GameObject gameObject)
        {
            Panicked instance = (Panicked)base.CreateCopy(gameObject);

            if (!gameObject.CompareTag("Boss"))
            {
                gameObject.GetComponent<Movement>().requestSpeedModifications += instance.ReverseMovement;
            }

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

            other.remainingDuration += duration / 2f;
            return true;
        }

        /// <summary>
        /// Responds to a movement components speed modification request, and reverses its speed.
        /// </summary>
        /// <param name="speed"> The speed variable to be modified. </param>
        private void ReverseMovement(ref float speed)
        {
            speed *= -1;
        }

        /// <summary>
        /// Cleans up binding.
        /// </summary>
        private new void OnDestroy()
        {
            if (gameObject != null && !gameObject.CompareTag("Boss"))
            {
                gameObject.GetComponent<Movement>().requestSpeedModifications -= ReverseMovement;
            }
            base.OnDestroy();
        }
    }
}