using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    /// <summary>
    /// A scriptable object for storing data about a projectile type.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProjectile", menuName = "Cards/Actions/Projectile")]
    public class SpawnProjectile : Action
    {
        [Header("Mechanics")]
        [Tooltip("The damage this projectile will deal.")]
        public Attack attack;
        [Tooltip("The radius of the projectile.")]
        public float size = 12;
        [Tooltip("The distance this projectile will travel.")]
        public float range = 6;
        [Tooltip("The speed this projectile will travel at.")]
        public float speed = 12;
        [Tooltip("The projectile to spawn")]
        public Projectile projectilePrefab;

        [Header("Visuals")]
        [Tooltip("The previewer prefab to use.")]
        public ProjectilePreviewer previewerPrefab;
        [Tooltip("The tint of the previewer spawned.")]
        public Color previewColor;
        [Tooltip("The projectile's sprite.")]
        public Sprite sprite;
        [Tooltip("The particle system to add to the projectile.")]
        public ParticleSystem particleEffect;

        // Maps players to their previewers.
        private Dictionary<IActor, ProjectilePreviewer> playersToPreviewers = new Dictionary<IActor, ProjectilePreviewer>();

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
        public override void Preview(IActor actor)
        {
            ProjectilePreviewer previewer = Instantiate<ProjectilePreviewer>(previewerPrefab, actor.GetActionSourceTransform());
            previewer.actor = actor;
            previewer.spawner = this;
            playersToPreviewers.Add(actor, previewer);
        }
        public override void AddCountToPreview(IActor actor, int count)
        {
            playersToPreviewers[actor].Count += count;
        }

        public override void ApplyModifiersToPreview(IActor actor, List<ActionModifier> actionModifiers) {}

        public override void RemoveModifiersFromPreview(IActor actor, List<ActionModifier> actionModifiers) {}

        /// <summary>
        /// Stops rendering a preview of what this action will do.
        /// </summary>
        /// <param name="actor"> The actor that will no longer be playing this action. </param>
        public override void CancelPreview(IActor actor)
        {
            Destroy(playersToPreviewers[actor].gameObject);
            playersToPreviewers.Remove(actor);
        }

        /// <summary>
        /// Plays this action and causes all its effects. Also cancels any relevant previews.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="count"> The number of times action is to be played. </param>
        /// <param name="modifiers"> The modifier affecting this action. </param>
        public override void Play(IActor actor, int count, List<ActionModifier> modifiers)
        {
            CancelPreview(actor);
            Projectile projectile = Instantiate<Projectile>(projectilePrefab, actor.GetActionSourceTransform().position, actor.GetActionSourceTransform().rotation);
            projectile.actor = actor;
            projectile.spawner = this;
            projectile.count = count;

            Attack modifedAttack = new Attack(attack, actor.GetActionSourceTransform().gameObject);
            foreach (ActionModifier modifier in modifiers)
            {
                if (modifier is AttackModifier)
                {
                    (modifier as AttackModifier).ModifyAttack(modifedAttack);
                }
            }
            projectile.attack = modifedAttack;
        }
    }
}