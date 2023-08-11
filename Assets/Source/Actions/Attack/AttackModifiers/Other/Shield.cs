using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action modifier that changes the attack of an action modifier.
    /// </summary>
    [CreateAssetMenu(fileName = "NewShield", menuName = "Cards/AttackModifers/Shield")]
    public class Shield : AttackModifier
    {
        // The owners of projectiles to ignore.
        private Projectile projectile;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            GameObject shieldObject = new GameObject();
            shieldObject.name = "Shield";
            shieldObject.transform.parent = value.transform;
            shieldObject.transform.localPosition = Vector2.zero;
            shieldObject.transform.localRotation = Quaternion.identity;
            shieldObject.layer = LayerMask.NameToLayer("Shield");
            value.shape.CreateCollider(shieldObject).isTrigger = true;

            value.onOverlap += DestroyProjectiles;
            projectile = value;
        }

        /// <summary>
        /// Destroys any projectiles that collide with the shield.
        /// </summary>
        /// <param name="collider"> The collided object. </param>
        private void DestroyProjectiles(Collider2D collider)
        {
            if (collider.GetComponent<Projectile>() is Projectile hitProjectile
                && !hitProjectile.immuneToShield
                && !projectile.ignoredObjects.Contains(hitProjectile.causer))
            {
                Destroy(collider.gameObject);

                if (--projectile.remainingHits <= 0)
                {
                    Destroy(projectile.gameObject);
                }
            }
        }
    }
}
