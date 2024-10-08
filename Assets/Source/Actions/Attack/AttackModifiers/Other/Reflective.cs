using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action modifier that changes the attack of an action modifier.
    /// </summary>
    [CreateAssetMenu(fileName = "NewReflective", menuName = "Cards/AttackModifers/Reflective")]
    public class Reflective : AttackModifier
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

            value.onOverlap += ReflectProjectiles;
            projectile = value;
        }

        /// <summary>
        /// Destroys any projectiles that collide with the shield.
        /// </summary>
        /// <param name="collider"> The collided object. </param>
        private void ReflectProjectiles(Collider2D collider)
        {
            if (collider.GetComponent<Projectile>() is Projectile hitProjectile
                && !hitProjectile.immuneToReflect
                && !projectile.ignoredObjects.Contains(hitProjectile.causer))
            {
                hitProjectile.transform.right *= -1;
                hitProjectile.ignoredObjects = projectile.ignoredObjects;

                if (--projectile.remainingHits <= 0)
                {
                    projectile.Destroy();
                }
            }
        }
    }
}
