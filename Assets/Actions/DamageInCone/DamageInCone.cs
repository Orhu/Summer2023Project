using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    /// <summary>
    /// A scriptable object for storing data about a damage cone type.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDamageInCone", menuName = "Cards/Actions/DamageInCone")]
    public class DamageInCone : Action
    {
        [Header("Mechanics")]
        [Tooltip("The damage that will be dealt to anything this hits.")]
        public Attack attack;
        [Tooltip("Whether or not the range is multiplied by the number stacks when played.")]
        public bool stackAttack = true;
        [Tooltip("The radius of the cone.")]
        public float range = 1;
        [Tooltip("Whether or not the range is multiplied by the number stacks when played.")]
        public bool stackRange = true;
        [Tooltip("The arc width in degrees of the cone.")]
        public float arcWidth = 12;
        [Tooltip("Whether or not the arc width is multiplied by the number stacks when played.")]
        public bool stackArcWidth = true;

        [Header("Visuals")]
        [Tooltip("The previewer prefab to use.")]
        public DamageInConePreviewer previewerPrefab;
        [Tooltip("The tint of the previewer spawned.")]
        public Color previewColor;

        // Maps players to their previewers.
        Dictionary<IActor, DamageInConePreviewer> playersToPreviewers = new Dictionary<IActor, DamageInConePreviewer>();

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
            DamageInConePreviewer previewer = Instantiate<DamageInConePreviewer>(previewerPrefab, actor.GetActionSourceTransform());
            previewer.actor = actor;
            previewer.spawner = this;
            previewer.NumStacks = 1;
            playersToPreviewers.Add(actor, previewer);
        }


        /// <summary>
        /// Adds a stacks to a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing </param>
        /// <param name="numStacks"> The number of stacks to add </param>
        public override void AddStacksToPreview(IActor actor, int numStacks)
        {
            playersToPreviewers[actor].NumStacks += numStacks;
        }

        /// <summary>
        /// Applies modifiers to a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing</param>
        /// <param name="actionModifiers"> The modifiers to apply </param>
        public override void ApplyModifiersToPreview(IActor actor, List<ActionModifier> actionModifiers) { }

        /// <summary>
        /// Removes modifiers from a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing</param>
        /// <param name="actionModifiers"> The modifiers to remove </param>
        public override void RemoveModifiersFromPreview(IActor actor, List<ActionModifier> actionModifiers) { }

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
        /// <param name="numStacks"> The number of times action is to be played. </param>
        /// <param name="modifiers"> The modifier affecting this action. </param>
        public override void Play(IActor actor, int numStacks, List<ActionModifier> modifiers)
        {
            CancelPreview(actor);

            Attack modifiedAttack = new Attack(attack, actor.GetActionSourceTransform().gameObject);
            foreach (ActionModifier modifier in modifiers)
            {
                if (modifier is AttackModifier)
                {
                    (modifier as AttackModifier).ModifyAttack(modifiedAttack);
                }
            }
            attack = modifiedAttack;

            Collider2D[] OverlapingColliders = Physics2D.OverlapCircleAll(actor.GetActionSourceTransform().position, range * (stackRange ? numStacks : 1));
            foreach (Collider2D OverlapingCollider in OverlapingColliders)
            {
                Health hitHealth = OverlapingCollider.GetComponent<Health>();
                if (hitHealth != null && OverlapingCollider.gameObject != actor.GetActionSourceTransform().gameObject)
                {
                    Vector2 aimDirection = (actor.GetActionAimPosition() - actor.GetActionSourceTransform().position).normalized;
                    Vector2 overlapDirection = (OverlapingCollider.transform.position - actor.GetActionSourceTransform().position).normalized;
                    Debug.Log(aimDirection + " dot " + overlapDirection + " = " + Vector2.Dot(aimDirection, overlapDirection));


                    if (Vector2.Dot(aimDirection, overlapDirection) > Mathf.Cos(arcWidth * (stackArcWidth ? numStacks : 1) / 2f))
                    {
                        hitHealth.ReceiveAttack(attack * (stackAttack ? numStacks : 1));
                    }
                }
            }
        }
    }
}