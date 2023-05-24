using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that travels and deals an attack when it collides with an object.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    // The attack this is a part of.
    internal Attack attack;
    // The actor of the projectile.
    internal IActor actor;
    // The modifiers applied to this.
    internal List<AttackModifier> modifiers;
    // The object for this to ignore.
    internal List<GameObject> ignoredObjects;

    Rigidbody2D rigidBody;

    /// <summary>
    /// Initializes components based on spawner stats.
    /// </summary>
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        if (actor.GetCollider() != null)
        {
            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            //collider.radius = spawner.size * (spawner.stackSize ? numStacks : 1);
            Physics2D.IgnoreCollision(collider, actor.GetCollider());
        }

        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();

        Vector3 diff = actor.GetActionAimPosition() - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
        //transform.position += transform.right * spawner.size * (spawner.stackSize ? numStacks : 1);
    }

    /// <summary>
    /// Updates position and kills self when at max range.
    /// </summary>
    void FixedUpdate()
    {
        //distanceTraveled += Time.fixedDeltaTime * spawner.speed * (spawner.stackSpeed ? numStacks : 1);
        //rigidBody.MovePosition(transform.position +
        //                       transform.right * (Time.fixedDeltaTime * spawner.speed *
        //                                          (spawner.stackSpeed ? numStacks : 1)));
        //if (distanceTraveled > spawner.range * (spawner.stackRange ? numStacks : 1))
        //{
        //    Destroy(gameObject);
        //}
    }

    /// <summary>
    /// Applies an attack to the hit object
    /// </summary>
    /// <param name="collision"> The collision data </param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        Health hitHealth = collision.gameObject.GetComponent<Health>();
        if (hitHealth != null)
        {
            hitHealth.ReceiveAttack(attack.attack);
        }

        Destroy(gameObject);
    }
}