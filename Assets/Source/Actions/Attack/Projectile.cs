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
    int remainingHits;
    GameObject target;
    float remainingHomingTime;

    protected GameObject Target
    {
        get
        {
            if (target != null)
            {
                return target;
            }

            Collider2D[] roomObjects = Physics2D.OverlapBoxAll(transform.position, ProceduralGeneration.proceduralGenerationInstance.roomSize, 0f);
            foreach (Collider2D roomObject in roomObjects)
            {
                // If has health, is not ignored, and is the closest object.
                if (roomObject.GetComponent<Health>() != null && actor.GetActionSourceTransform().gameObject != roomObject.gameObject && (ignoredObjects == null || !ignoredObjects.Contains(roomObject.gameObject)) && 
                    (target == null || (roomObject.transform.position - transform.position).sqrMagnitude < (target.transform.position - transform.position).sqrMagnitude))
                {
                    target = roomObject.gameObject;
                }
            }

            return target;
        }
    }

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
        remainingHits = attack.hitCount;
        remainingHomingTime = attack.homingTime;
    }

    /// <summary>
    /// Updates position.
    /// </summary>
    void FixedUpdate()
    {
        if (remainingHomingTime > 0 && attack.homingSpeed > 0 && Target != null)
        {
            Vector3 targetDirection = (Target.transform.position - transform.position).normalized;
            float targetRotation = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            float currentRotation = transform.rotation.eulerAngles.z;
            transform.rotation = Quaternion.AngleAxis(Mathf.LerpAngle(currentRotation, targetRotation, (attack.homingSpeed * Time.fixedDeltaTime) / Mathf.Abs(targetRotation - currentRotation)), Vector3.forward);
        }

        speed = Mathf.Clamp(speed + attack.acceleration * Time.fixedDeltaTime, attack.minSpeed, attack.maxSpeed);
        rigidBody.velocity = transform.right * speed;
    }

    /// <summary>
    /// Handles lifetime
    /// </summary>
    void Update()
    {
        remainingLifetime -= Time.deltaTime;
        remainingHomingTime -= Time.deltaTime;
        if (remainingLifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Applies an attack to the hit object
    /// </summary>
    /// <param name="collision"> The collision data </param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            return;
        }

        Health hitHealth = collision.GetComponent<Health>();
        if (hitHealth != null)
        {
            hitHealth.ReceiveAttack(attack.attack, transform.right);
            if (--remainingHits <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }
}