using CardSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    [CreateAssetMenu(fileName = "NewInvincibility", menuName = "Cards/Actions/Invincibility")]
    public class CauseInvincibility : Action
    {
        [Header("Mechanics")]
        [Tooltip("The time that the owner will be invincible for")]
        public float invincibilityTime = 1f;
        [Tooltip("Whether or not to multiply the invincibility time by the number of stacks")]
        public bool stackInvincibilityTime = true;

        [Header("Visuals")]
        [Tooltip("The sprite to render the preview as")]
        public Sprite previewSprite;
        [Tooltip("The sprite to render the preview as")]
        public Color previewColor;
        [Tooltip("The sprite to render the invincibility as")]
        public Sprite sprite;
        [Tooltip("Whether or not to spawn sprites for each stack")]
        public bool stackSprites = true;
        [Tooltip("The scale applied to each additional sprite spawned when stacking")]
        public float stackSpriteScale = 1.1f;


        // The actors to the last time they acted this action.
        Dictionary<IActor, List<SpriteRenderer>> actorsToPreviewers = new Dictionary<IActor, List<SpriteRenderer>>();

        /// <summary>
        /// Starts rendering a preview of what this action will do.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        public override void Preview(IActor actor)
        {
            SpriteRenderer spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = previewSprite;
            spriteRenderer.color = previewColor;
            spriteRenderer.sortingOrder = 1;
            spriteRenderer.transform.parent = actor.GetActionSourceTransform();
            spriteRenderer.transform.localPosition = Vector3.zero;
            spriteRenderer.transform.localRotation = Quaternion.identity;

            List<SpriteRenderer> previewers = new List<SpriteRenderer>();
            previewers.Add(spriteRenderer);
            actorsToPreviewers.Add(actor, previewers);
        }

        /// <summary>
        /// Adds a stacks to a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing </param>
        /// <param name="numStacks"> The number of stacks to add </param>
        public override void AddStacksToPreview(IActor actor, int numStacks)
        {
            if (numStacks > 0)
            {
                int totalStacks = (stackSprites ? numStacks : 1) + actorsToPreviewers[actor].Count;
                for (int i = actorsToPreviewers[actor].Count; i < totalStacks; i++)
                {
                    SpriteRenderer spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = previewSprite;
                    spriteRenderer.color = previewColor;
                    spriteRenderer.sortingOrder = 1;
                    spriteRenderer.transform.localScale = new Vector3(1 + i * stackSpriteScale, 1 + i * stackSpriteScale, 1 + i * stackSpriteScale);
                    spriteRenderer.transform.parent = actor.GetActionSourceTransform();
                    spriteRenderer.transform.localPosition = Vector3.zero;
                    spriteRenderer.transform.localRotation = Quaternion.identity;

                    actorsToPreviewers[actor].Add(spriteRenderer);
                }
            }
            else
            {
                while (numStacks < 0)
                {
                    Destroy(actorsToPreviewers[actor][actorsToPreviewers[actor].Count - 1].gameObject);
                    actorsToPreviewers[actor].RemoveAt(actorsToPreviewers[actor].Count - 1);
                    numStacks++;
                }
            }
        }

        /// <summary>
        /// Applies modifiers to a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing</param>
        /// <param name="actionModifiers"> The modifiers to apply </param>
        public override void ApplyModifiersToPreview(IActor actor, List<ActionModifier> actionModifiers) { /*TODO: Implement*/ }

        /// <summary>
        /// Removes modifiers from a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing</param>
        /// <param name="actionModifiers"> The modifiers to remove </param>
        public override void RemoveModifiersFromPreview(IActor actor, List<ActionModifier> actionModifiers) { /*TODO: Implement*/ }

        /// <summary>
        /// Stops rendering a preview of what this action will do.
        /// </summary>
        /// <param name="actor"> The actor that will no longer be playing this action. </param>
        public override void CancelPreview(IActor actor)
        {
            while (actorsToPreviewers[actor].Count > 0)
            {
                Destroy(actorsToPreviewers[actor][actorsToPreviewers[actor].Count - 1].gameObject);
                actorsToPreviewers[actor].RemoveAt(actorsToPreviewers[actor].Count - 1);
            }

            actorsToPreviewers.Remove(actor);
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
            actor.GetActionSourceTransform().GetComponent<Health>().InvincibilityTime += invincibilityTime * (stackInvincibilityTime ? numStacks : 1);
            for (int i = 0; i < (stackSprites ? numStacks : 1); i++)
            {
                SpriteRenderer spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                spriteRenderer.sortingOrder = 1;
                spriteRenderer.transform.localScale = new Vector3((i + 1) * stackSpriteScale, (i + 1) * stackSpriteScale, (i + 1) * stackSpriteScale);
                spriteRenderer.transform.parent = actor.GetActionSourceTransform();
                spriteRenderer.transform.localPosition = Vector3.zero;
                spriteRenderer.transform.localRotation = Quaternion.identity;
            }
        }
    }
}
