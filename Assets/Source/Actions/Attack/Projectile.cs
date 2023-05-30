using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardSystem.Effects.Attack;

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
    // The actor of the projectile.
    internal GameObject causer;
    // The modifiers applied to this.
    internal List<AttackModifier> modifiers;
    // The object for this to ignore.
    internal List<GameObject> IgnoredObjects
    {
        get
        {
            if (ignoredObjects != null)
            {
                return ignoredObjects;
            }

            ignoredObjects = new List<GameObject>();
            ignoredObjects.Add(actor.GetActionSourceTransform().gameObject);
            ignoredObjects.Add(causer);
            return ignoredObjects;
        }
        set { ignoredObjects = value; }
    }
    List<GameObject> ignoredObjects;


    // The rigidbody responsible for the collision of this projectile.
    protected Rigidbody2D rigidBody;
    // The modified attack data of this projectile.
    DamageData attackData;

    protected float speed;
    protected float remainingLifetime;
    int remainingHits;
    GameObject closestTarget;
    GameObject randomTarget;
    protected float remainingHomingTime;

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
            if (IgnoredObjects != null)
            {
                foreach (GameObject ignoredObject in IgnoredObjects)
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

        // Set up vars
        speed = attack.initialSpeed;
        remainingLifetime = attack.lifetime;
        remainingHits = attack.hitCount;
        remainingHomingTime = attack.homingTime;
        attackData = new DamageData(attack.attack, causer);
    }

    /// <summary>
    /// Updates position.
    /// </summary>
    protected void FixedUpdate()
    {
        if (remainingHomingTime > 0 && attack.homingSpeed > 0)
        {
            Vector3 targetDirection = (GetAimTarget(attack.homingAimMode) - transform.position).normalized;
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
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger || IgnoredObjects.Contains(collision.gameObject))
        {
            return;
        }

        Health hitHealth = collision.GetComponent<Health>();
        if (hitHealth != null)
        {
            hitHealth.ReceiveAttack(attackData, transform.right);
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

    /// <summary>
    /// Gets the desired spawn location
    /// </summary>
    /// <returns> The position in world space of the spawn location. </returns>
    protected Vector3 GetSpawnLocation()
    {
        switch (attack.spawnLocation)
        {
            case SpawnLocation.Actor:
                return actor.GetActionSourceTransform().position;
            case SpawnLocation.AimPosition:
                return actor.GetActionAimPosition();
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Gets the desired aim target
    /// </summary>
    /// <param name="aimMode"> The aim mode to get the target for. </param>
    /// <returns> The position in world space of the target. </returns>
    protected Vector3 GetAimTarget(AimMode aimMode)
    {
        switch (aimMode)
        {
            case AimMode.AtMouse:
                if (actor == null)
                {
                    return transform.position + transform.right;
                }
                return actor.GetActionAimPosition();

            case AimMode.AtClosestEnemy:
                if (closestTarget != null)
                {
                    return closestTarget.transform.position;
                }

                Collider2D[] roomObjects = Physics2D.OverlapBoxAll(transform.position, DeprecatedProceduralGeneration.ProceduralGeneration.proceduralGenerationInstance.roomSize * 2, 0f);
                foreach (Collider2D roomObject in roomObjects)
                {
                    // If has health, is not ignored, and is the closest object.
                    if (roomObject.GetComponent<Health>() != null && !IgnoredObjects.Contains(roomObject.gameObject) &&
                        (closestTarget == null || (roomObject.transform.position - transform.position).sqrMagnitude < (closestTarget.transform.position - transform.position).sqrMagnitude))
                    {
                        closestTarget = roomObject.gameObject;
                    }
                }

                if (closestTarget == null)
                {
                    return transform.position + transform.right;
                }
                return closestTarget.transform.position;

            case AimMode.AtRandomEnemy:
                if (randomTarget != null)
                {
                    return randomTarget.transform.position;
                }

                Collider2D[] roomColliders = Physics2D.OverlapBoxAll(transform.position, DeprecatedProceduralGeneration.ProceduralGeneration.proceduralGenerationInstance.roomSize * 2, 0f);
                List<GameObject> possibleTargets = new List<GameObject>(roomColliders.Length);
                foreach (Collider2D roomCollider in roomColliders)
                {
                    // If has health, is not ignored, and is the closest object.
                    if (roomCollider.GetComponent<Health>() != null && !IgnoredObjects.Contains(roomCollider.gameObject))
                    {
                        possibleTargets.Add(roomCollider.gameObject);
                    }
                }
                
                randomTarget = possibleTargets[Random.Range(0, possibleTargets.Count)].gameObject;
                if (randomTarget == null)
                {
                    return transform.position + transform.right;
                }
                return randomTarget.transform.position;
        }
        return transform.position + transform.right;
    }

    protected void OnDestroy()
    {
        if (attack.detachVisualsBeforeDestroy)
        {
            transform.GetChild(0).transform.parent = null;
        }
    }
}