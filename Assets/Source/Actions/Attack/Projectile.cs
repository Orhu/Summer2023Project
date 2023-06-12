using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that travels and deals an attack when it collides with an object.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    #region Variables
    // The attack this is a part of.
    [NonSerialized] public Attack attack;

    // The actor of the projectile.
    [NonSerialized] public IActor actor;

    // The actor of the projectile.
    [NonSerialized] public GameObject causer;

    // The modifiers applied to this.
    [NonSerialized] public List<AttackModifier> modifiers;

    // The rigidbody responsible for the collision of this projectile.
    protected Rigidbody2D rigidBody;

    // The modified attack data of this projectile.
    [NonSerialized] public DamageData attackData;

    // The index of this in the spawn sequence.
    [NonSerialized] public int index;


    #region Modifiable Properties
    // The speed of the projectile in tiles/s.
    [NonSerialized] public float speed;

    // The max speed of the projectile in tiles/s. 
    [NonSerialized] public float maxSpeed;

    // The min speed of the projectile in tiles/s. 
    [NonSerialized] public float minSpeed;

    // The acceleration of the projectile in tiles/s�. 
    [NonSerialized] public float acceleration;

    // The time until this is destroyed in seconds.
    [NonSerialized] public float remainingLifetime;

    // The number of times this can hit objects it can pass though before being destroyed.
    [NonSerialized] public int remainingHits;

    // The remaining time this will home for in seconds.
    [NonSerialized] public float remainingHomingTime;

    // The speed that this projectile will turn at.
    [NonSerialized] public float homingSpeed;

    // The sequence that spawned this.
    [NonSerialized] public List<ProjectileSpawnInfo> spawnSequence;

    // The object for this to ignore.
    List<GameObject> _ignoredObjects;
    public List<GameObject> ignoredObjects
    {
        get
        {
            if (_ignoredObjects != null)
            {
                return _ignoredObjects;
            }

            _ignoredObjects = new List<GameObject>();
            _ignoredObjects.Add(actor.GetActionSourceTransform().gameObject);
            _ignoredObjects.Add(causer);
            return _ignoredObjects;
        }
        set { _ignoredObjects = value; }
    }
    #endregion

    #region Delegates
    // Invoked when this projectile hits a wall, passes the hit collision as a parameter.
    public System.Action<Collision2D> onHit;

    // Invoked when this projectile hits something damageable, passes the hit collider as a parameter.
    public System.Action<Collider2D> onOverlap;

    // Invoked when this is destroyed.
    public System.Action onDestroyed;
    #endregion



    // The current closest target.
    private GameObject closestTarget;

    // The current randomly picked target.
    private GameObject randomTarget;
    #endregion




    #region Initialization
    /// <summary>
    /// Initializes components based on spawner stats.
    /// </summary>
    protected void Start()
    {
        // Setup collision
        rigidBody = GetComponent<Rigidbody2D>();
        Collider2D collider = attack.shape.CreateCollider(gameObject);
        if (actor.GetCollider() != null)
        {
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
    #endregion


    #region Aiming and Projectile Movement
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
            transform.rotation = Quaternion.AngleAxis(Mathf.LerpAngle(currentRotation, targetRotation, homingSpeed), Vector3.forward);
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

            case SpawnLocation.RoomCenter:
                return FloorGenerator.floorGeneratorInstance.currentRoom.transform.position;
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

                Collider2D[] roomObjects = Physics2D.OverlapBoxAll(transform.position, FloorGenerator.floorGeneratorInstance.roomSize * 2, 0f);
                foreach (Collider2D roomObject in roomObjects)
                {
                    // If has health, is not ignored, and is the closest object.
                    if (roomObject.GetComponent<Health>() != null && !ignoredObjects.Contains(roomObject.gameObject) &&
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

                Collider2D[] roomColliders = Physics2D.OverlapBoxAll(transform.position, FloorGenerator.floorGeneratorInstance.roomSize * 2, 0f);
                List<GameObject> possibleTargets = new List<GameObject>(roomColliders.Length);
                foreach (Collider2D roomCollider in roomColliders)
                {
                    // If has health, is not ignored, and is the closest object.
                    if (roomCollider.GetComponent<Health>() != null && !ignoredObjects.Contains(roomCollider.gameObject))
                    {
                        possibleTargets.Add(roomCollider.gameObject);
                    }
                }

                randomTarget = possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)].gameObject;
                if (randomTarget == null)
                {
                    return transform.position + transform.right;
                }
                return randomTarget.transform.position;

            case AimMode.Right:
                return transform.position + transform.right;
        }
        return transform.position + transform.right;
    }
    #endregion


    #region Collision
    /// <summary>
    /// Called when the projectile hits something it can pass though.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ignoredObjects.Contains(collision.gameObject))
        {
            return;
        }

        onOverlap?.Invoke(collision);
        Health hitHealth = collision.gameObject.GetComponent<Health>();
        if (hitHealth != null && attack.applyDamageOnHit)
        {
            hitHealth.ReceiveAttack(attackData);

            if (--remainingHits <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Called when the projectile hits something it cannot pass through.
    /// </summary>
    /// <param name="collision"> The collision data </param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Invoke(nameof(DestroyOnWallHit), Time.fixedDeltaTime);
        onHit?.Invoke(collision);
    }

    /// <summary>
    /// Cancelable function for destroying self upon hitting a wall.
    /// </summary>
    private void DestroyOnWallHit()
    {
        Destroy(gameObject);
    }
    #endregion


    /// <summary>
    /// Allows for visuals to be detached and calls delegate.
    /// </summary>
    protected void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) { return; }

        onDestroyed?.Invoke();
        if (attack.detachVisualsBeforeDestroy)
        {
            transform.GetChild(0).transform.parent = null;
        }
    }

}