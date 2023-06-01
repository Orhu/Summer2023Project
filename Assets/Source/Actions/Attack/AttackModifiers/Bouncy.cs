using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>
[CreateAssetMenu(fileName = "NewBouncy", menuName = "Cards/AttackModifers/Bouncy")]
public class Bouncy : AttackModifier
{
    //The cosine of the minimum angle between vectors to consider the same.
    private const float DOUBLE_BOUNCE_PROTECTION_FACTOR = 0.9f;

    // The rigid body to bounce.
    private Rigidbody2D bouncyRigidbody;

    // The projectile to bounce.
    private Projectile bouncyProjectile;

    // The normal of the last wall bounced off of this frame
    Vector2 lastBounceNormal = Vector2.zero;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            if (value.onHitWall == null || !value.onHitWall.GetInvocationList().Contains((Action<Collision2D>)Bounce))
            {
                value.onHitWall += Bounce;
                bouncyProjectile = value;
                bouncyRigidbody = value.GetComponent<Rigidbody2D>();
            }
        }
    }


    private void Bounce(Collision2D collision)
    {
        Vector2 bounceNormal = collision.GetContact(0).normal;

        bouncyProjectile.CancelInvoke("DestroyOnWallHit");
        if (lastBounceNormal == Vector2.zero)
        {
            bouncyProjectile.StartCoroutine(ClearLastNormal());

            bouncyProjectile.transform.right = Vector2.Reflect(bouncyProjectile.transform.right, bounceNormal);
            lastBounceNormal = bounceNormal;
        }
        else if (Vector2.Dot(bounceNormal, lastBounceNormal) < DOUBLE_BOUNCE_PROTECTION_FACTOR)
        {
            bouncyProjectile.transform.right = Vector2.Reflect(bouncyProjectile.transform.right, bounceNormal);
            lastBounceNormal = bounceNormal;
        }
    }

    private IEnumerator ClearLastNormal()
    {
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        lastBounceNormal = Vector2.zero;
    }
}
