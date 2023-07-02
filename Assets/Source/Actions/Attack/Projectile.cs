using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
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

        // The current velocity of this projectile.
        [NonSerialized] public Vector2 velocity;

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

        // The remaining time until this will home in seconds.
        [NonSerialized] public float remainingHomingDelay;

        // The speed that this projectile will turn at.
        [NonSerialized] public float homingSpeed;

        // The shape of the projectile.
        [NonSerialized] public ProjectileShape shape;

        // The visual object of this.
        [NonSerialized] public GameObject visualObject;

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

        // Invoked when projectile hits something and plays the relevant impact AudioClip. 
        public System.Action<Vector2> playImpactAudio;

        // Invoked when this is destroyed.
        public System.Action onDestroyed;
        #endregion


        // The current closest target to the actor.
        private GameObject closestTargetToActor;

        // The current closest target to the projectile.
        private GameObject closestTargetToProjectile;

        // The current closest target to the aim location.
        private GameObject closestTargetToAimLocation;

        // The current randomly picked target.
        private GameObject randomTarget;
        #endregion




        #region Initialization
        /// <summary>
        /// Initializes components based on spawner stats.
        /// </summary>
        protected void Start()
        {

            // Set up visuals
            visualObject = Instantiate(attack.visualObject);
            visualObject.transform.parent = transform;
            visualObject.transform.localPosition = Vector2.zero;
            visualObject.transform.localRotation = Quaternion.identity;

            // Set up vars
            speed = attack.initialSpeed;
            remainingLifetime = attack.lifetime;
            remainingHits = attack.hitCount;
            remainingHomingTime = attack.homingTime;
            remainingHomingDelay = attack.homingDelay;
            homingSpeed = attack.homingSpeed;
            maxSpeed = attack.maxSpeed;
            minSpeed = attack.minSpeed;
            acceleration = attack.acceleration;
            shape = Instantiate(attack.shape);
            
            // Set up attack
            attackData = new DamageData(attack.attack, causer);
            IActor causedBy = causer.GetComponent<IActor>();
            if (causedBy != null)
            {
                attackData.damage = Mathf.RoundToInt(attackData.damage * causedBy.GetDamageMultiplier());
            }
            
            if (attack.switchAfterHit)
            {
                onOverlap +=
                    (Collider2D colldier) =>
                    {
                        closestTargetToActor = null;
                        closestTargetToProjectile = null;
                        closestTargetToAimLocation = null;
                        randomTarget = null;
                    };
            }


            InitializeModifiers();


            // Setup collision
            rigidBody = GetComponent<Rigidbody2D>();
            Collider2D collider = shape.CreateCollider(gameObject);
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
        }

        /// <summary>
        /// Creates instances of all the modifiers and applies their initial effects.
        /// </summary>
        void InitializeModifiers()
        {
            List<AttackModifier> newModifiers = new List<AttackModifier>(modifiers.Count);
            foreach (AttackModifier modifier in modifiers)
            {
                if (modifier == null) { continue; }

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
            velocity += (Vector2)transform.right * acceleration * Time.fixedDeltaTime;

            if (remainingHomingDelay <= 0 && remainingHomingTime > 0 && homingSpeed > 0)
            {
                Vector2 targetVelocity = (GetAimTarget(attack.homingAimMode) - transform.position).normalized * maxSpeed;
                if (targetVelocity.sqrMagnitude > Vector2.kEpsilon)
                {
                    velocity += (targetVelocity - velocity).normalized * homingSpeed * Time.fixedDeltaTime;
                }
            }

            speed = Mathf.Clamp(velocity.magnitude, minSpeed, maxSpeed);
            rigidBody.velocity = velocity.normalized * speed;
        }

        /// <summary>
        /// Handles lifetime
        /// </summary>
        protected void Update()
        {
            remainingLifetime -= Time.deltaTime;
            if (remainingHomingDelay <= 0)
            {
                remainingHomingTime -= Time.deltaTime;
            }
            remainingHomingDelay -= Time.deltaTime;
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
            if (actor == null) { return transform.position; }

            switch (attack.spawnLocation)
            {
                case SpawnLocation.Actor:
                    return actor.GetActionSourceTransform().position;

                case SpawnLocation.AimPosition:
                    return actor.GetActionAimPosition();

                case SpawnLocation.RoomCenter:
                    return FloorGenerator.currentRoom.transform.position;

                case SpawnLocation.Causer:
                    return causer.transform.position;
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

                case AimMode.AtClosestEnemyToProjectile:
                    return FindClosestTarget(transform.position, ref closestTargetToProjectile);

                case AimMode.AtClosestEnemyToActor:
                    return FindClosestTarget(actor.GetActionSourceTransform().position, ref closestTargetToActor);

                case AimMode.AtClosestEnemyToAimLocation:
                    return FindClosestTarget(GetAimTarget(AimMode.AtMouse), ref closestTargetToAimLocation);

                case AimMode.AtRandomEnemy:
                    if (randomTarget != null)
                    {
                        return randomTarget.transform.position;
                    }

                    List<GameObject> possibleTargets = new List<GameObject>(FloorGenerator.currentRoom.livingEnemies);
                    possibleTargets.Add(Player.Get());
                    possibleTargets.RemoveAll(
                        // Removes ignored objects
                        (GameObject possibleTarget) =>
                        {
                            return ignoredObjects.Contains(possibleTarget);
                        });

                    if (possibleTargets.Count <= 0)
                    {
                        return transform.position + transform.right;
                    }
                    randomTarget = possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)].gameObject;
                    return randomTarget.transform.position;

                case AimMode.Right:
                    return transform.position + actor.GetActionSourceTransform().right;
            }
            return transform.position + transform.right;
        }

        /// <summary>
        /// Gets the current closest target to a location.
        /// </summary>
        /// <param name="location"> The location to find the closest target to. </param>
        /// <param name="currentTarget"> The variable storing the last found target. </param>
        /// <returns> The position of the closest target in world space. </returns>
        private Vector2 FindClosestTarget(Vector2 location, ref GameObject currentTarget)
        {
            if (currentTarget != null)
            {
                return currentTarget.transform.position;
            }

            List<GameObject> possibleTargets = new List<GameObject>(FloorGenerator.currentRoom.livingEnemies);
            possibleTargets.Add(Player.Get());

            foreach (GameObject possibleTarget in possibleTargets)
            {
                // If has health, is not ignored, and is the closest object.
                if (!ignoredObjects.Contains(possibleTarget)
                    && (currentTarget == null || ((Vector2)possibleTarget.transform.position - location).sqrMagnitude < ((Vector2)currentTarget.transform.position - location).sqrMagnitude)
                    )
                {
                    currentTarget = possibleTarget;
                }
            }


            if (currentTarget == null)
            {
                return transform.position;
            }
            return currentTarget.transform.position;
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
                    playImpactAudio?.Invoke(transform.position);
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
            playImpactAudio?.Invoke(transform.position);
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
                GameObject childObj = transform.GetChild(0).gameObject;
                childObj.transform.parent = null;
                if (attack.detatchedVisualsTimeBeforeDestroy != 0f)
                {
                    Destroy(childObj, attack.detatchedVisualsTimeBeforeDestroy);
                }
            }
        }

    }
}