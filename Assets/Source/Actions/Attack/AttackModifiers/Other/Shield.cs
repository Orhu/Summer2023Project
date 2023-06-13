using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>
[CreateAssetMenu(fileName = "NewShield", menuName = "Cards/AttackModifers/Shield")]
public class Shield : AttackModifier
{
    // The owners of projectiles to ignore.
    private Projectile projectile;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            GameObject shieldObject = new GameObject();
            shieldObject.name = "Shield";
            shieldObject.transform.parent = value.transform;
            shieldObject.transform.localPosition = Vector2.zero;
            shieldObject.transform.localRotation = Quaternion.identity;
            shieldObject.layer = LayerMask.NameToLayer("Shield");
            value.shape.CreateCollider(shieldObject).isTrigger = true;

            value.onOverlap += destroyProjectiles;
            projectile = value;
        }
    }

    /// <summary>
    /// Destroys any projectiles that collide with the shield.
    /// </summary>
    /// <param name="collider"> The collided object. </param>
    private void destroyProjectiles(Collider2D collider)
    {
        Projectile hitProjectile = collider.GetComponent<Projectile>();
        if (hitProjectile != null && !projectile.ignoredObjects.Contains(hitProjectile.causer))
        {
            Destroy(collider.gameObject);

            if (--projectile.remainingHits <= 0)
            {
                projectile.onDestroyed?.Invoke();
                Destroy(projectile.gameObject);
            }
        }
    }
}
