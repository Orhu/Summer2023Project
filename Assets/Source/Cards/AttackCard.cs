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
        [Header("Chording Modifiers")]
        [EditInline]
        [Tooltip("The how this card will modify actions when used in a combo.")]
        public List<AttackModifier> chordModifiers;
        [EditInline]
        [Tooltip("The how this card will modify actions when used in a combo with itself.")]
        public List<AttackModifier> duplicateModifiers;

        #region Previewing
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

        public void PreviewActions(IActor actor, List<AttackCard> chordedCards)
        {
            PreviewActions(actor);
            foreach (AttackCard chordedCard in chordedCards)
            {
                AddToPreview(actor, chordedCard);
            }
        }

        /// <summary>
        /// Applies modifiers to all the previews of this card's actions.
        /// </summary>
        /// <param name="actor"> The actor previewing </param>
        /// <param name="actionModifiers"> The modifiers to apply </param>
        public void AddToPreview(IActor actor, List<AttackModifier> actionModifiers)
        {
            foreach (Action cardAction in actions)
            {
                //cardAction.ApplyModifiersToPreview(actor, actionModifiers);
            }
        }
        public void AddToPreview(IActor actor, AttackCard chordedCard)
        {
            AddToPreview(actor, GetAppliedModifers(chordedCard));
        }

        /// <summary>
        /// Removes modifiers from all the previews of this card's actions.
        /// </summary>
        /// <param name="actor"> The actor previewing </param>
        /// <param name="actionModifiers"> The modifiers to remove </param>
        internal void RemoveFromPreview(IActor actor, List<AttackModifier> actionModifiers)
        {
            foreach (Action cardAction in actions)
            {
                //cardAction.RemoveModifiersFromPreview(actor, actionModifiers);
            }
        }
        internal void RemoveFromPreview(IActor actor, AttackCard chordedCard)
        {
            RemoveFromPreview(actor, GetAppliedModifers(chordedCard));
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
        #endregion

        public void PlayActions(IActor actor, List<AttackCard> chordedCards)
        {
            List<AttackModifier> modifiers = new List<AttackModifier>();
            foreach (AttackCard chordedCard in chordedCards)
            {
                modifiers.AddRange(GetAppliedModifers(chordedCard));
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


        List<AttackModifier> GetAppliedModifers(AttackCard modifingCard)
        {
            if (modifingCard == this)
            {
                return modifingCard.duplicateModifiers;
            }
            else
            {
                return modifingCard.chordModifiers;
            }
        }
    }
}