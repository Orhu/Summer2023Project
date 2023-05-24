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
    float speed;
    float remainingLifetime;

    /// <summary>
    /// Initializes components based on spawner stats.
    /// </summary>
    protected void Start()
    {
        // Setup collision
        rigidBody = GetComponent<Rigidbody2D>();
        if (actor.GetCollider() != null)
        {
            Collider2D collider = attack.shape.CreateCollider(gameObject);
            Physics2D.IgnoreCollision(collider, actor.GetCollider());

            // Ignore collision on ignored objects
            if (ignoredObjects != null)
            {
                foreach (GameObject ignoredObject in ignoredObjects)
                {
                    List<Collider2D> ignoredColliders = new List<Collider2D>();
                    ignoredObject.GetComponentsInChildren(ignoredColliders);
                    ignoredObject.GetComponents(ignoredColliders);
                    if (ignoredColliders.Count > 0)
                    {
                        foreach (Collider2D ignoredCollider in ignoredColliders)
                        {
                            Physics2D.IgnoreCollision(ignoredCollider, actor.GetCollider());
                        }
                    }
                }
            }
        }

        // Set up visuals
        GameObject visuals = Instantiate(attack.visualObject);
        visuals.transform.parent = transform;
        visuals.transform.localPosition = Vector2.zero;
        visuals.transform.localRotation = Quaternion.identity;

        // Set of vars
        speed = attack.initialSpeed;
        remainingLifetime = attack.lifetime;
    }

    /// <summary>
    /// Updates position.
    /// </summary>
    void FixedUpdate()
    {
        speed = Mathf.Clamp(speed + attack.acceleration * Time.fixedDeltaTime, attack.minSpeed, attack.maxSpeed);
        rigidBody.velocity = transform.right * speed;
    }

    /// <summary>
    /// Handles lifetime
    /// </summary>
    void Update()
    {
        remainingLifetime -= Time.deltaTime;
        if(remainingLifetime <= 0)
        {
            Destroy(gameObject);
        }
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