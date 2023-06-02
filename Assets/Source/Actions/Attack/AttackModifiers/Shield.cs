using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>
[CreateAssetMenu(fileName = "NewShield", menuName = "Cards/AttackModifers/Shield")]
public class Shield : AttackModifier
{
    // The owners of projectiles to ignore.
    private List<GameObject> ignoredObjects;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            GameObject shieldObject = new GameObject();
            shieldObject.transform.parent = value.transform;
            shieldObject.transform.localPosition = Vector2.zero;
            shieldObject.transform.localRotation = Quaternion.identity;
            shieldObject.layer = LayerMask.NameToLayer("Shield");
            value.attack.shape.CreateCollider(shieldObject).isTrigger = true;

            value.onOverlap += destroyProjectiles;
            ignoredObjects = value.IgnoredObjects;
        }
    }

    private void destroyProjectiles(Collider2D collider)
    {
        Projectile hitProjectile = collider.GetComponent<Projectile>();
        if (hitProjectile != null && !ignoredObjects.Contains(hitProjectile.causer))
        {
            Destroy(collider.gameObject);
        }
    }
}
