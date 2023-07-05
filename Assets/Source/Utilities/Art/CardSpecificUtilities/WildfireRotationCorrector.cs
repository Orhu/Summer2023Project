using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Utility Script to make Wildfire projectiles point up once they stop moving.
    /// </summary>
    public class WildfireRotationCorrector: MonoBehaviour
    {
        // reference to projectile's rigidbody
        private Rigidbody2D body;

        /// <summary>
        /// Gets reference to Parent's rigidbody (the actual movey thing)
        /// </summary>
        private void Awake()
        {
            body = GetComponentInParent<Rigidbody2D>();
        }

        /// <summary>
        /// If the rigidbody has stopped moving, reset the parent's rotation so it is zeroed.
        /// </summary>
        private void LateUpdate()
        {
            if (body == null)
            {
                body = GetComponentInParent<Rigidbody2D>();
                return;
            }
            if (body.velocity.magnitude == 0f)
            {
                this.transform.parent.transform.rotation = Quaternion.identity;
                Destroy(this); // get rid of the script so it doesn't keep trying to reset the rotation after the first time the projectile stops
            }
        }
    }
}

