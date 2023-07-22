using Skaillz.EditInline;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A scriptable object data about an attack that can be used by cards and enemies.
    /// </summary>
    public abstract class Attack : Action
    {
        // The projectile to spawn
        public Projectile projectilePrefab;
        // The previewer prefab to use.
        public AttackPreviewer previewerPrefab;

        [Header("Hits")]

        [Tooltip("The damage, damage type, status effects, and knockback this projectile will deal.")]
        public DamageData attack;

        [Tooltip("The number of objects this can hit before being destroyed.")] [Min(1)]
        public int hitCount = 1;

        [Tooltip("Whether or not this projectile should apply damage on hit.")]
        public bool applyDamageOnHit = true;

        [Tooltip("The modifiers that are always applied to this projectile")] [EditInline]
        public List<AttackModifier> modifiers;

        [Tooltip("Whether or not this projectile passes through shields.")]
        public bool immuneToShield = false;

        [Tooltip("Whether or not this projectile can be reflected.")]
        public bool immuneToReflect = false;

        [Tooltip("The radius of the projectile.")] [EditInline]
        public ProjectileShape shape;



        [Header("Movement Info")]

        [Tooltip("The lifetime of projectiles spawned by this.")]
        public float lifetime = 10f;

        [Tooltip("The speed projectile will start traveling at. In tiles/second")] [Min(0)]
        public float initialSpeed = 10f;

        [Tooltip("The acceleration this projectile will experience. In tiles/second^2")]
        public float acceleration = -1f;

        [Tooltip("The minimum speed projectile will travel at. In tiles/second")] [Min(0)]
        public float minSpeed = 5f;

        [Tooltip("The maximum speed projectile will travel at. In tiles/second")] [Min(0)]
        public float maxSpeed = 10f;



        [Header("Homing")]

        [Tooltip("The speed in tile/s^2 that projectiles will accelerate towards the closest enemy")] [Min(0)]
        public float homingSpeed = 0;

        [Tooltip("The duration that this will home for.")] [Min(0)]
        public float homingTime = 0;

        [Tooltip("The time to wait before homing begins, in seconds")] [Min(0)]
        public float homingDelay = 0;

        [Tooltip("What the homing will rotate the projectile towards")]
        public AimMode homingAimMode;

        [Tooltip("Whether or not homing target should be reset after each hit.")]
        public bool switchAfterHit = false;



        [Header("Visuals")]

        [Tooltip("The game object used to render the projectiles.")]
        public GameObject visualObject;

        [Tooltip("Whether or not the visuals should be detached before the object is destroyed so that they can handle their own lifetime.")]
        public bool detachVisualsBeforeDestroy = false;

        [Tooltip("Time to wait before destroying detatched visuals.")]
        public float detatchedVisualsTimeBeforeDestroy = 0f;



        [Header("Projectile Audio")]

        [Tooltip("AudioClip for projectile travel")]
        [SerializeField] protected AudioClip travelAudioClip;

        [Tooltip("AudioClip for projectile impact.")]
        [SerializeField] protected AudioClip impactAudioClip;

        // The root of all projectiles
        private static GameObject projectileRoot;



        [Header("Spawning")]

        [Tooltip("The location to spawn the projectiles at.")]
        public SpawnLocation spawnLocation;

        [Tooltip("Whether or not the player needs to aim. If false it will be aimed at the closet enemy")]
        public AimMode aimMode;

        [Tooltip("The sequence of when and where to spawn projectiles")]
        public abstract List<ProjectileSpawnInfo> spawnSequence { set; get; }

        [Tooltip("Whether or not to wait for all spawned projectiles to die before the action is complete.")]
        public bool waitForProjectileDeath = false;

        [Tooltip("The time to wait before starting the attack sequence.")] [Min(0f)]
        public float chargeTime = 0f;

        [Tooltip("The time to wait after the spawn sequence is finished and (optionally) all projectile have died until the action is officially complete.")] [Min(0f)]
        public float additionalActionTime = 0f;



        #region Previewing
        /// <summary>
        /// A scriptable object data about an attack that can be used by cards and enemies.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        public abstract void Preview(IActor actor);

        /// <summary>
        /// Applies modifiers to a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing. </param>
        /// <param name="actionModifiers"> The modifiers to apply </param>
        public abstract void ApplyModifiersToPreview(IActor actor, List<AttackModifier> actionModifiers);

        /// <summary>
        /// Removes modifiers from a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing. </param>
        /// <param name="actionModifiers"> The modifiers to remove </param>
        public abstract void RemoveModifiersFromPreview(IActor actor, List<AttackModifier> actionModifiers);

        /// <summary>
        /// Stops rendering a preview of what this action will do.
        /// </summary>
        /// <param name="actor"> The actor that will no longer be playing this action. </param>
        public abstract void CancelPreview(IActor actor);
        #endregion

        #region Playing

        /// <summary>
        /// Plays this action and causes all its effects. Also cancels any relevant previews.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="modifiers"> The modifiers to be applied to this attack. </param>
        /// <param name="causer"> The causer of damage dealt by this attack. </param>
        /// <param name="attackFinished"> A callback for when the action is finished playing. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        public virtual void Play(IActor actor, List<AttackModifier> modifiers, GameObject causer, System.Action attackFinished = null, List<GameObject> ignoredObjects = null)
        {
            AudioManager.instance.PlayAudioAtActor(actionAudioClip, actor);
            actor.GetActionSourceTransform().GetComponent<MonoBehaviour>().StartCoroutine(PlaySpawnSequence(actor, modifiers, causer, attackFinished, ignoredObjects));

        }
        public void Play(IActor actor, GameObject causer, List<GameObject> ignoredObjects = null)
        {
            Play(actor, new List<AttackModifier>(), causer, ignoredObjects: ignoredObjects);
        }
        public void Play(IActor actor, List<AttackModifier> modifiers, System.Action attackFinished = null, List < GameObject> ignoredObjects = null)
        {
            Play(actor, modifiers, actor.GetActionSourceTransform().gameObject, attackFinished, ignoredObjects);
        }
        
        public void Play(IActor actor, List<GameObject> ignoredObjects = null, System.Action attackFinished = null)
        {
            Play(actor, new List<AttackModifier>(), actor.GetActionSourceTransform().gameObject, attackFinished, ignoredObjects);
        }
        public sealed override void Play(IActor actor, List<GameObject> ignoredObjects = null)
        {
            Play(actor, new List<AttackModifier>(), actor.GetActionSourceTransform().gameObject, ignoredObjects: ignoredObjects);
        }


        /// <summary>
        /// Spawns all of the projectiles in spawnSequence and creates delays appropriately.
        /// </summary>
        /// <param name="actor"> The actor that is playing this action. </param>
        /// <param name="modifiers"> The modifiers that are applied to this attack. </param>
        /// <param name="causer"> The causer of damage dealt by this attack. </param>
        /// <param name="attackFinished"> A callback for when the action is finished playing. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        protected IEnumerator PlaySpawnSequence(IActor actor, List<AttackModifier> modifiers, GameObject causer, System.Action attackFinished, List<GameObject> ignoredObjects)
        {
            List<ProjectileSpawnInfo> spawnSequence = new List<ProjectileSpawnInfo>(this.spawnSequence);
            var projectileList = new List<Projectile>();
            int destroyedProejectiles = 0;

            yield return new WaitForSeconds(chargeTime);

            for (int i = 0; i < spawnSequence.Count; i++)
            {
                if (spawnSequence[i].delay > 0)
                {
                    yield return new WaitForSeconds(spawnSequence[i].delay);
                }
                var spawnedProjectile = SpawnProjectile(actor, modifiers, causer, ignoredObjects, i, spawnSequence);
                projectileList.Add(spawnedProjectile);
                spawnedProjectile.playImpactAudio += (Vector2 pos) => PlayImpactAtPos(pos);

                if (waitForProjectileDeath)
                {
                    spawnedProjectile.onDestroyed += () => { destroyedProejectiles++; };
                }


                // Wait to ensure the sequence doesn't miss new additions
                if (i + 1 >= spawnSequence.Count)
                {
                    yield return null;
                    yield return null;
                }
            }
            
            // Wait for projectile death
            while (waitForProjectileDeath && destroyedProejectiles != spawnSequence.Count)
            {
                yield return null;
            }

            yield return new WaitForSeconds(additionalActionTime);
            attackFinished?.Invoke();
            
            AudioManager.instance.GetAverageAudioSource(projectileList, travelAudioClip, projectileList.Count > 1);
        }


        /// <summary>
        /// Spawns a single projectile from the spawn sequence.
        /// </summary>
        /// <param name="actor"> The actor spawning the projectile. </param>
        /// <param name="modifiers"> The modifiers that are applied to this attack. </param>
        /// <param name="causer"> The causer of damage dealt by this attack. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        /// <param name="index"> The index in the spawn sequence that the projectile was spawned from. </param>
        /// <param name="spawnSequence"> The spawn sequence instance that is being used. </param>
        /// <returns> The projectile that was spawned. </returns>
        protected virtual Projectile SpawnProjectile(IActor actor, List<AttackModifier> modifiers, GameObject causer, List<GameObject> ignoredObjects, int index, List<ProjectileSpawnInfo> spawnSequence)
        {
            if (projectileRoot == null)
            {
                projectileRoot = new GameObject("Projectiles");
            }

            Projectile projectile = Instantiate(projectilePrefab.gameObject).GetComponent<Projectile>();
            projectile.attack = this;
            projectile.actor = actor;
            projectile.modifiers = this.modifiers.Concat(modifiers).SkipWhile(
                    // Get only applicable modifiers
                    (AttackModifier attackModifier) =>
                    {
                        int adjIndex = index % attackModifier.attackSequenceLoopInterval;
                        return adjIndex < attackModifier.minAttackSequenceIndex || adjIndex > attackModifier.maxAttackSequenceIndex;
                    }).ToList();
            projectile.causer = causer;
            projectile.ignoredObjects = ignoredObjects == null ? null : new List<GameObject>(ignoredObjects);
            projectile.index = index;
            projectile.spawnSequence = spawnSequence;
            projectile.transform.parent = projectileRoot.transform;
            return projectile;
        }

        /// <summary>
        /// Play impact audio at specific position in scene.
        /// </summary>
        /// <param name="pos">Position of the impact audio sound</param>
        protected void PlayImpactAtPos(Vector2 pos)
        {
            AudioManager.instance.PlayAudioAtPos(impactAudioClip, pos);
        } 
        #endregion
    }

    /// <summary>
    /// The different location where a projectile could spawn at.
    /// </summary>
    public enum SpawnLocation
    {
        Actor,
        AimPosition,
        RoomCenter,
        Causer,
        Player,
        RandomEnemy,
    }

    /// <summary>
    /// The different things a projectile could be aimed at.
    /// </summary>
    public enum AimMode
    {
        AtMouse,
        AtClosestEnemyToProjectile,
        AtClosestEnemyToAimLocation,
        AtClosestEnemyToActor,
        AtRandomEnemy,
        Right,
        AtPlayer,
    }

    /// <summary>
    /// The information about a single bullet spawning event.
    /// </summary>
    [System.Serializable]
    public abstract class ProjectileSpawnInfo
    {
        [Tooltip("The time to wait after the previous bullet to spawn this one")]
        public float delay = 0;

        /// <summary>
        /// Creates a duplicate of this.
        /// </summary>
        /// <returns> The created duplicate. </returns>
        public abstract ProjectileSpawnInfo Instantiate();
    }
}