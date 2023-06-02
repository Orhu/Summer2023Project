using Skaillz.EditInline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
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

        [Tooltip("The modifiers that are always applied to this projectile")] [EditInline]
        public List<AttackModifier> modifiers;

        [Tooltip("The radius of the projectile.")] [EditInline]
        public ProjectileShape shape;



        [Header("Movement Info")]

        [Tooltip("The lifetime of projectiles spawned by this.")]
        public float lifetime = 10f;

        [Tooltip("The speed projectile will start traveling at. In tiles/second")]
        public float initialSpeed = 10f;

        [Tooltip("The acceleration this projectile will experience. In tiles/second^2")]
        public float acceleration = -1f;

        [Tooltip("The minimum speed projectile will travel at. In tiles/second")]
        public float minSpeed = 5f;

        [Tooltip("The maximum speed projectile will travel at. In tiles/second")]
        public float maxSpeed = 10f;



        [Header("Homing")]
        
        [Tooltip("The percentage change in rotation every 1/50 second towards the target.")] [Range(0, 1)]
        public float homingSpeed = 0;
        
        [Tooltip("The duration that this will home for.")] [Min(0)]
        public float homingTime = 0;

        [Tooltip("What the homing will rotate the projectile towards")]
        public AimMode homingAimMode;



        [Header("Visuals")]

        [Tooltip("The game object used to render the projectiles.")]
        public GameObject visualObject;

        [Tooltip("Whether or not the visuals should be detached before the object is destroyed so that they can handle their own lifetime.")]
        public bool detachVisualsBeforeDestroy = false;



        [Header("Spawning")]

        [Tooltip("The location to spawn the projectiles at.")]
        public SpawnLocation spawnLocation;

        [Tooltip("Whether or not the player needs to aim. If false it will be aimed at the closet enemy")]
        public AimMode aimMode;


        #region Previewing
        /// <summary>
        /// Starts rendering a preview of what this action will do.
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
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        public virtual void Play(IActor actor, List<AttackModifier> modifiers, GameObject causer, List<GameObject> ignoredObjects = null)
        {
            SpawnProjectile(actor, modifiers, causer, ignoredObjects);
        }
        public virtual void Play(IActor actor, GameObject causer, List<GameObject> ignoredObjects = null)
        {
            Play(actor, new List<AttackModifier>(), causer, ignoredObjects);
        }
        public virtual void Play(IActor actor, List<AttackModifier> modifiers, List<GameObject> ignoredObjects = null)
        {
            Play(actor, modifiers, actor.GetActionSourceTransform().gameObject, ignoredObjects);
        }
        public override void Play(IActor actor, List<GameObject> ignoredObjects = null)
        {
            Play(actor, new List<AttackModifier>(), actor.GetActionSourceTransform().gameObject, ignoredObjects);
        }

        protected Projectile SpawnProjectile(IActor actor, List<AttackModifier> modifiers, GameObject causer, List<GameObject> ignoredObjects)
        {
            Projectile projectile = Instantiate(projectilePrefab.gameObject).GetComponent<Projectile>();
            projectile.attack = this;
            projectile.actor = actor;
            projectile.modifiers = modifiers;
            projectile.causer = causer;
            projectile.IgnoredObjects = ignoredObjects;
            return projectile;
        }
        #endregion

        /// <summary>
        /// The different location where a projectile could spawn at.
        /// </summary>
        public enum SpawnLocation
        {
            Actor,
            AimPosition
        }

        /// <summary>
        /// The different things a projectile could be aimed at.
        /// </summary>
        public enum AimMode
        {
            AtMouse,
            AtClosestEnemy,
            AtRandomEnemy
        }
    }
}