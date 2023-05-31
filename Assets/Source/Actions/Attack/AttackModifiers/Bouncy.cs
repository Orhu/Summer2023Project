using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>
[CreateAssetMenu(fileName = "NewBouncy", menuName = "Cards/AttackModifers/Bouncy")]
public class Bouncy : AttackModifier
{
    [Tooltip("Whether or not bullets will pass through or bounce off damageable targets.")]
    public bool bounceOffDamageable = false;

    // The rigid body to bounce.
    private Projectile bouncyProjectile;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            if (!value.onHit.GetInvocationList().Contains((Action<Collider2D>)Bounce))
            {
                value.onHit += Bounce;
                bouncyProjectile = value;
            }
        }
    }


    void Bounce(Collider2D collision)
    {
        if (!bounceOffDamageable && collision.GetComponent<Health>() != null) { return; }

        float distance = (collision.transform.position - bouncyProjectile.transform.position).sqrMagnitude;
        RaycastHit2D[] hits = Physics2D.RaycastAll(bouncyProjectile.transform.position, bouncyProjectile.transform.right, distance);

        RaycastHit2D collidedHit = hits[0];
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == collision)
            {
                collidedHit = hit;
                break;
            }
        }

        Debug.Log(bouncyProjectile.transform.right + " off " + collidedHit.normal + " = " + Vector2.Reflect(bouncyProjectile.transform.right, collidedHit.normal));
        bouncyProjectile.transform.right = Vector2.Reflect(bouncyProjectile.transform.right, collidedHit.normal);

        bouncyProjectile.CancelInvoke("DestroyOnWallHit");
    }
}
