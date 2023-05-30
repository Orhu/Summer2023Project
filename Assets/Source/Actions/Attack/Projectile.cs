using System.Collections.Generic;
using UnityEngine;
using static Attack;

/// <summary>
/// An object that travels and deals an attack when it collides with an object.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    // The attack this is a part of.
    public Attack attack;
    // The actor of the projectile.
    public IActor actor;
    // The actor of the projectile.
    public GameObject causer;
    // The modifiers applied to this.
    public List<AttackModifier> modifiers;
    // The object for this to ignore.
    public List<GameObject> IgnoredObjects
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

    // Invoked when this projectile hits something, passes the hit collider as a parameter.
    public System.Action<Collider2D> onHit;
    // Invoked when this is destroyed.
    public System.Action onDestroyed;


    // The rigidbody responsible for the collision of this projectile.
    protected Rigidbody2D rigidBody;
    // The modified attack data of this projectile.
    public DamageData attackData;

    public float speed;
    public float maxSpeed;
    public float minSpeed;
    public float acceleration;
    public float remainingLifetime;
    public int remainingHits;
    GameObject closestTarget;
    GameObject randomTarget;
    public float remainingHomingTime;
    public float homingSpeed;


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
        homingSpeed = attack.homingSpeed;
        maxSpeed = attack.maxSpeed;
        minSpeed = attack.minSpeed;
        acceleration = attack.acceleration;

        attackData = new DamageData(attack.attack, causer);

        InitializeModifiers();
    }

    /// <summary>
    /// Creates instances of all the modifiers and applies their initial effects.
    /// </summary>
    void InitializeModifiers()
    {
        List<AttackModifier> newModifiers = new List<AttackModifier>(modifiers.Count);
        foreach (AttackModifier modifier in modifiers)
        {
            AttackModifier instance = Instantiate(modifier);
            instance.modifiedProjectile = this;
            newModifiers.Add(Instantiate(modifier));
        }

        modifiers = newModifiers;
    }


    /// <summary>
    /// Updates position.
    /// </summary>
    protected void FixedUpdate()
    {
        if (remainingHomingTime > 0 && homingSpeed > 0)
        {
            Vector3 targetDirection = (GetAimTarget(attack.homingAimMode) - transform.position).normalized;
            float targetRotation = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            float currentRotation = transform.rotation.eulerAngles.z;
            transform.rotation = Quaternion.AngleAxis(Mathf.LerpAngle(currentRotation, targetRotation, (homingSpeed * Time.fixedDeltaTime) / Mathf.Abs(targetRotation - currentRotation)), Vector3.forward);
        }

        speed = Mathf.Clamp(speed + acceleration * Time.fixedDeltaTime, minSpeed, maxSpeed);
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
            if (attack.applyDamageOnHit)
            {
                hitHealth.ReceiveAttack(attackData, transform.right);
            }
                
            onHit?.Invoke(collision);
            if (--remainingHits <= 0)
            {
                onDestroyed?.Invoke();
                Destroy(gameObject);
            }
        }
        else
        {
            onHit?.Invoke(collision);
            onDestroyed?.Invoke();
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

                Collider2D[] roomObjects = Physics2D.OverlapBoxAll(transform.position, ProceduralGeneration.proceduralGenerationInstance.roomSize * 2, 0f);
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

                Collider2D[] roomColliders = Physics2D.OverlapBoxAll(transform.position, ProceduralGeneration.proceduralGenerationInstance.roomSize * 2, 0f);
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