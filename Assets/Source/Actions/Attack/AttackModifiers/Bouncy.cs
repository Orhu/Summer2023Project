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
    // The rigid body to bounce.
    private Rigidbody2D bouncyRigidbody;

    // The projectile to bounce.
    private Projectile bouncyProjectile;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            if (!value.onHitWall.GetInvocationList().Contains((Action<Collision2D>)Bounce))
            {
                value.onHitWall += Bounce;
                bouncyProjectile = value;
                bouncyRigidbody = value.GetComponent<Rigidbody2D>();
            }
        }
    }


    private void Bounce(Collision2D collision)
    {
        Vector2 normalSum = Vector2.zero;
        for (int i = 0; i < collision.contactCount; i++)
        {
            bouncyProjectile.transform.right = Vector2.Reflect(bouncyProjectile.transform.right, collision.GetContact(i).normal);
        }
        bouncyProjectile.CancelInvoke("DestroyOnWallHit");
        bouncyProjectile.StartCoroutine(CheckForDoulbeBounce());
    }

    private IEnumerator CheckForDoulbeBounce()
    {
        List<ContactPoint2D> oldContacts = new List<ContactPoint2D>();
        bouncyRigidbody.GetContacts(oldContacts);

        yield return new WaitForSeconds(Time.fixedDeltaTime * 2f);

        List<ContactPoint2D> newContacts = new List<ContactPoint2D>();
        bouncyRigidbody.GetContacts(newContacts);

        IEnumerable<ContactPoint2D> unquieContacts = newContacts
            .Where(newContact =>
            {
                foreach (ContactPoint2D oldContact in oldContacts)
                {
                    if(oldContact.normal == newContact.normal) { return false; }
                }
                return true;
            });

        foreach(ContactPoint2D unquieContact in unquieContacts)
        {
            bouncyProjectile.transform.right = Vector2.Reflect(bouncyProjectile.transform.right, unquieContact.normal);
        }
    }
}
