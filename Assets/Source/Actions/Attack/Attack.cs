using Skaillz.EditInline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    /// <summary>
    /// A scriptable object for storing data about a projectile type.
    /// </summary>
    internal abstract class Attack : Action
    {
        [Header("Mechanics")]
        [Tooltip("The damage, damage type, status effects, and knockback this projectile will deal.")]
        public AttackData attack;

        [EditInline]
        [Tooltip("The modifiers that are always applied to this projectile")]
        public List<AttackModifier> modifiers;

        [EditInline]
        [Tooltip("The radius of the projectile.")]
        public ProjectileShape shape;

        [Header("Movement Info")]
        [Tooltip("The lifetime of projectiles spawned by this.")]
        public float lifetime = 10f;
        [Tooltip("The speed projectile will start traveling at. In tiles/second")]
        public float initialSpeed;
        [Tooltip("The acceleration this projectile will experience. In tiles/second^2")]
        public float acceleration = 12f;
        [Tooltip("The minimum speed projectile will travel at. In tiles/second")]
        public float minSpeed = 0;
        [Tooltip("The maximum speed projectile will travel at. In tiles/second")]
        public float maxSpeed = 0;

        [Header("Homing")]
        [Range(0, 1)]
        [Tooltip("The percent change every that this will point towards the closes enemy. 0 no homing, 1 always point at closest enemy")]
        public float homingWeight = 0;
        [Tooltip("The duration that this will home for.")]
        public float homingTime = 0;

        [Header("Visuals")]
        [Tooltip("The game object used to render the projectiles.")]
        public GameObject visualObject;

        [Header("Spawning")]
        [Tooltip("The location to spawn the projectiles at.")]
        public SpawnLocation spawnLocation;
        [Tooltip("Whether or not the player needs to aim. If false it will be aimed at the closet enemy")]
        public bool isAimed = true;

        // The projectile to spawn
        private Projectile projectilePrefab;
        // The previewer prefab to use.
        public ProjectilePreviewer previewerPrefab;


        // Maps players to their previewers.
        Dictionary<IActor, ProjectilePreviewer> playersToPreviewers = new Dictionary<IActor, ProjectilePreviewer>();

        private new void Awake()
        {
            base.Awake();
            projectilePrefab = Resources.Load<Projectile>("Projectile");
        }

        /// <summary>
        /// Gets the formated description of this card.
        /// </summary>
        /// <returns> The description with any Serialized Field names that appear in [] replaced with their actual value.</returns>
        public override string GetFormattedDescription()
        {
            return description.Replace("[Damage]", attack.damage.ToString());
        }

        /// <summary>
        /// Starts rendering a preview of what this action will do.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        public void Preview(IActor actor)
        {
            ProjectilePreviewer previewer = Instantiate<ProjectilePreviewer>(previewerPrefab, actor.GetActionSourceTransform());
            previewer.actor = actor;
            previewer.spawner = this;
            playersToPreviewers.Add(actor, previewer);
        }

        /// <summary>
        /// Applies modifiers to a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing</param>
        /// <param name="actionModifiers"> The modifiers to apply </param>
        public void ApplyModifiersToPreview(IActor actor, List<AttackModifier> actionModifiers) {}

        /// <summary>
        /// Removes modifiers from a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing</param>
        /// <param name="actionModifiers"> The modifiers to remove </param>
        public void RemoveModifiersFromPreview(IActor actor, List<AttackModifier> actionModifiers) {}

        /// <summary>
        /// Stops rendering a preview of what this action will do.
        /// </summary>
        /// <param name="actor"> The actor that will no longer be playing this action. </param>
        public void CancelPreview(IActor actor)
        {
            ProjectilePreviewer value;
            if (playersToPreviewers.TryGetValue(actor, out value))
            {
                Destroy(value.gameObject);
                playersToPreviewers.Remove(actor);
            }
        }

        /// <summary>
        /// Plays this action and causes all its effects. Also cancels any relevant previews.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="modifiers"> The modifier affecting this action. </param>
        public void Play(IActor actor, List<GameObject> ignoredObjects, List<AttackModifier> modifiers)
        {
            CancelPreview(actor);
            Projectile projectile = Instantiate<Projectile>(projectilePrefab, actor.GetActionSourceTransform().position, actor.GetActionSourceTransform().rotation);
            projectile.actor = actor;
            projectile.spawner = this;

            AttackData modifiedAttack = new AttackData(attack, actor.GetActionSourceTransform().gameObject);
            foreach (AttackModifier modifier in modifiers)
            {
                if (modifier is AttackModifier)
                {
                    (modifier as AttackModifier).ModifyAttack(ref modifiedAttack);
                }
            }
            projectile.attack = modifiedAttack;
        }

        public override void Play(IActor actor, List<GameObject> ignoredObjects)
        {
            Play(actor, ignoredObjects, new List<AttackModifier>());
        }


        public enum SpawnLocation
        {
            Actor,
            Mouse
        }
    }
}