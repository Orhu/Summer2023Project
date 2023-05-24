using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Skaillz.EditInline;
using CardSystem.Effects;

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
    [CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Cards/Attack Card", order = 1)]
    public class AttackCard : Card
    {
        [Header("Cording Modifiers")]
        [EditInline]
        [Tooltip("The how this card will modify actions when used in a combo.")]
        public List<AttackModifier> cordModifiers;
        [EditInline]
        [Tooltip("The how this card will modify actions when used in a combo with itself.")]
        public List<AttackModifier> duplicateModifiers;

        /// <summary>
        /// Causes all of this cards Actions to start rendering their previews around the actor.
        /// </summary>
        public void PreviewActions(IActor actor)
        {
            foreach (Action cardAction in actions)
            {
                //cardAction.Preview(actor);
            }
        }

        /// <summary>
        /// Applies modifiers to all the previews of this card's actions.
        /// </summary>
        /// <param name="actor"> The actor previewing </param>
        /// <param name="actionModifiers"> The modifiers to apply </param>
        public void ApplyModifiersToPreview(IActor actor, List<AttackModifier> actionModifiers)
        {
            foreach (Action cardAction in actions)
            {
                //cardAction.ApplyModifiersToPreview(actor, actionModifiers);
            }
        }

        /// <summary>
        /// Removes modifiers from all the previews of this card's actions.
        /// </summary>
        /// <param name="actor"> The actor previewing </param>
        /// <param name="actionModifiers"> The modifiers to remove </param>
        internal void RemoveModifiersFromPreview(IActor actor, List<AttackModifier> actionModifiers)
        {
            foreach (Action cardAction in actions)
            {
                //cardAction.RemoveModifiersFromPreview(actor, actionModifiers);
            }
        }

        /// <summary>
        /// Causes all of this cards Actions to stop rendering their previews.
        /// </summary>
        public void CancelPreviewActions(IActor actor)
        {
            foreach (Action cardAction in actions)
            {
                //cardAction.CancelPreview(actor);
            }
        }

        public void PlayActions(IActor actor, List<AttackCard> cordedCards)
        {
            List<AttackModifier> modifiers = new List<AttackModifier>();
            foreach (AttackCard cordedCard in cordedCards)
            {
                if (cordedCard == this)
                {
                    modifiers.AddRange(cordedCard.duplicateModifiers);
                }
                else
                {
                    modifiers.AddRange(cordedCard.cordModifiers);
                }
            }

            PlayActions(actor, modifiers);
        }

        public void PlayActions(IActor actor, List<AttackModifier> modifiers)
        {
            foreach(Action action in actions)
            {
                if (action is Attack)
                {
                    (action as Attack).Play(actor, modifiers);
                }
                else
                {
                    action.Play(actor);
                }
            }
        }
    }
}