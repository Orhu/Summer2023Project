using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>
[CreateAssetMenu(fileName = "NewTickingDamage", menuName = "Cards/AttackModifers/TickingDamage")]
public class TickingDamage : AttackModifier
{
    [Tooltip("The time in seconds to wait to apply damage.")] [Min(0f)]
    public float damageInterval = 0.5f;

    // The projectile to apply ticking damage under.
    private Rigidbody2D tickingDamageRigidbody;

    // The projectile to apply ticking damage under.
    private Projectile tickingDamageProjectile;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            value.onOverlap += StartTicking;
            tickingDamageProjectile = value;
            tickingDamageRigidbody = value.GetComponent<Rigidbody2D>();
        }
    }


    private void StartTicking(Collider2D collider)
    {
        tickingDamageProjectile.StartCoroutine(DealTickingDamage(collider.GetComponent<Health>(), collider));
    }

    private IEnumerator DealTickingDamage(Health healthToDamage, Collider2D collider)
    {
        yield return new WaitForSeconds(damageInterval);
        while (tickingDamageRigidbody != null && collider != null && tickingDamageRigidbody.IsTouching(collider))
        {
            healthToDamage.ReceiveAttack(tickingDamageProjectile.attackData);

            if (--tickingDamageProjectile.remainingHits <= 0)
            {
                tickingDamageProjectile.onDestroyed?.Invoke();
                Destroy(tickingDamageProjectile.gameObject);
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }
}
