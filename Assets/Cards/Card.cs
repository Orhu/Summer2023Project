using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CardSystem
{
    /// <summary>
    /// A scriptable object for containing data about a card type including:
    /// - Card visuals
    /// - The cooldown of the card
    /// - Actions - What the card does when played as a root
    /// - Action Modifiers - What the card does when played as part of a combo
    /// - Effects - How the card effects the dungeon
    /// </summary>
    [CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card", order = 1)]
    public class Card : ScriptableObject
    {
        [Header("Mechanics")]
        // The amount of time this card reserves the hand slot for after being played.
        public float cooldown = 1.0f;
        // The actions that will be taken when this card is played as the root of a combo.
        public Action[] actions;
        // The how this card will modify actions when used in a combo.
        public List<ActionModifier> actionModifiers;
        // The effects that this card will have on the dungeon while in the actor's deck.
        public DungeonEffect[] effects;

        [Header("Visuals")]
        // The name of the card as shown to the actor.
        public string displayName = "Unnamed";
        // The card specific sprite on the Actions side of the card.
        public Sprite actionImage;
        // The general background card sprite on the Actions side of the card.
        public Sprite actionBackground;
        // The card specific sprite on the Effects side of the card.
        public Sprite effectImage;
        // The general background card sprite on the Effects side of the card.
        public Sprite effectBackground;


        /// <summary>
        /// Gets the description of this card by collecting all the formated descriptions from the card's mechanics.
        /// </summary>
        /// <param name="isActionSide"> Whether or not to get the description for the action side or the effect side of the card.</param>
        /// <returns>The description.</returns>
        public string GetDescription(bool isActionSide)
        {
            string description = "";
            if (!isActionSide)
            {
                foreach (DungeonEffect cardEffect in effects)
                {
                    description += cardEffect.GetFormattedDescription() + "\n";
                }
            }
            else
            {
                foreach (Action cardAction in actions)
                {
                    description += cardAction.GetFormattedDescription() + "\n";
                }
            }
            return description;
        }

        /// <summary>
        /// Causes all of this cards Actions to start rendering their previews around the actor.
        /// </summary>
        public void PreviewActions(IActor actor)
        {
            foreach (Action cardAction in actions)
            {
                cardAction.Preview(actor);
            }
        }

        public void AddCountToPreview(IActor actor, int count)
        {
            foreach (Action cardAction in actions)
            {
                cardAction.AddCountToPreview(actor, count);
            }
        }

        public void ApplyModifiersToPreview(IActor actor, List<ActionModifier> actionModifiers)
        {
            foreach (Action cardAction in actions)
            {
                cardAction.ApplyModifiersToPreview(actor, actionModifiers);
            }
        }

        internal void RemoveModifiersFromPreview(IActor actor, List<ActionModifier> actionModifiers)
        {
            foreach (Action cardAction in actions)
            {
                cardAction.RemoveModifiersFromPreview(actor, actionModifiers);
            }
        }

        /// <summary>
        /// Causes all of this cards Actions to stop rendering their previews.
        /// </summary>
        public void CancelPreviewActions(IActor actor)
        {
            foreach (Action cardAction in actions)
            {
                cardAction.CancelPreview(actor);
            }
        }

        /// <summary>
        /// Plays all of the actions of this card from the actor.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="count"> The number of times action is to be played. </param>
        /// <param name="modifiers"> The modifier affecting this action. </param>
        public void PlayActions(IActor actor, int count, List<ActionModifier> modifiers)
        {
            foreach (Action cardAction in actions)
            {
                cardAction.Play(actor, count, modifiers);
            }
        }
    }
}