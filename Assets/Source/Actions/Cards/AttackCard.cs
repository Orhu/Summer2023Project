using System.Collections.Generic;
using UnityEngine;
using Skaillz.EditInline;
using System.Linq;
using UnityEditor.Presets;

namespace Cardificer
{
    /// <summary>
    /// A scriptable object for containing data about a card type including:
    /// - Card visuals
    /// - The cooldown of the card
    /// - Actions - What the card does when played as a root
    /// - Duplicate Modifiers - What the card does when played in addition to itself
    /// - Cord Modifiers - What the card does when played with other cards.
    /// - Effects - How the card effects the dungeon
    /// </summary>
    [CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Cards/Attack Card", order = 1)]
    public class AttackCard : Card
    {
        [Header("Chording Modifiers")]
        
        [Tooltip("The how this card will modify actions when used in a combo.")] [EditInline]
        public List<AttackModifier> chordModifiers;

        [Tooltip("The prefabs that will be attached to the projectile when chorded.")]
        public List<GameObject> chordVFXPrefabs;

        [Tooltip("The how this card will modify actions when used in a combo with itself.")] [EditInline]
        public List<AttackModifier> duplicateModifiers;
        
        [Tooltip("Which modifiers cannot be applied to each of the actions on this card. The index of an entry maps to the index of the attack action that will have its modifiers filtered.")] [EditInline]
        public List<ModifierFilter> modifierFilters;

        #region Previewing
        /// <summary>
        /// Renders a preview of this action.
        /// </summary>
        /// <param name="actor"> The actor that would play the action. </param>
        public void PreviewActions(IActor actor)
        {
            foreach (Action cardAction in actions)
            {
                //cardAction.Preview(actor);
            }
        }

        /// <summary>
        /// Renders a preview of this action.
        /// </summary>
        /// <param name="actor"> The actor that would play the action. </param>
        /// <param name="chordedCards"> The cards being corded with this. </param>
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

        /// <summary>
        /// Applies modifiers to all the previews of this card's actions.
        /// </summary>
        /// <param name="actor"> The actor previewing </param>
        /// <param name="chordedCard"> The card's who's modifiers should be added </param>
        public void AddToPreview(IActor actor, AttackCard chordedCard)
        {
            //AddToPreview(actor, GetAppliedModifers(chordedCard));
        }

        /// <summary>
        /// Removes modifiers from all the previews of this card's actions.
        /// </summary>
        /// <param name="actor"> The actor previewing </param>
        /// <param name="actionModifiers"> The modifiers to remove </param>
        public void RemoveFromPreview(IActor actor, List<AttackModifier> actionModifiers)
        {
            foreach (Action cardAction in actions)
            {
                //cardAction.RemoveModifiersFromPreview(actor, actionModifiers);
            }
        }

        /// <summary>
        /// Removes modifiers from all the previews of this card's actions.
        /// </summary>
        /// <param name="actor"> The actor previewing </param>
        /// <param name="chordedCard"> The card's who's modifiers should be removed </param>
        public void RemoveFromPreview(IActor actor, AttackCard chordedCard)
        {
            //RemoveFromPreview(actor, GetAppliedModifers(chordedCard));
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

        /// <summary>
        /// Plays the actions of this card.
        /// </summary>
        /// <param name="actor"> The actor playing the card. </param>
        /// <param name="chordedCards"> The cards being corded with this. </param>
        /// <param name="attackFinished"> A callback for when the action is finished. </param>
        public void PlayActions(IActor actor, List<AttackCard> chordedCards, System.Action attackFinished = null)
        {
            int numUnfinishedAttacks = 0;

            for (int i = 0; i < actions.Length; i++)
            {
                Action action = actions[i];
                if (action is Attack)
                {
                    numUnfinishedAttacks++;

                    List<AttackModifier> modifiers = new List<AttackModifier>();
                    foreach (AttackCard chordedCard in chordedCards)
                    {
                        modifiers.AddRange(GetAppliedModifers(chordedCard, i));

                        // Chord VFX
                        if (chordedCard.chordVFXPrefabs != null && chordedCard.chordVFXPrefabs.Count > 0)
                        {
                            ChordVFXModifier vfxModifer = CreateInstance<ChordVFXModifier>();
                            vfxModifer.cordVFXPrefabs = chordedCard.chordVFXPrefabs;
                            modifiers.Add(vfxModifer);
                        }
                    }

                    (action as Attack).Play(actor, modifiers,
                        // Calls attack finished when all attacks have been finished
                        attackFinished: () =>
                        {
                            if (--numUnfinishedAttacks == 0)
                            {
                                attackFinished?.Invoke();
                            }
                        });
                }
                else
                {
                    action.Play(actor);
                }
            }

            if (numUnfinishedAttacks == 0)
            {
                attackFinished?.Invoke();
            }
        }

        /// <summary>
        /// Plays the actions of this card.
        /// </summary>
        /// <param name="actor"> The actor playing the card. </param>
        /// <param name="modifiers"> The modifiers being applied to this. </param>
        /// <param name="attackFinished"> A callback for when the action is finished. </param>
        public void PlayActions(IActor actor, List<AttackModifier> modifiers, System.Action attackFinished = null)
        {
            int numUnfinishedAttacks = 0;

            for (int i = 0; i < actions.Length; i++)
            {
                Action action = actions[i];
                if (action is Attack)
                {
                    numUnfinishedAttacks++;

                    List<AttackModifier> actionModifiers = modifiers;
                    if (modifierFilters.Count < i && modifierFilters[i] != null)
                    {
                        actionModifiers = modifierFilters[i].FilterModifierList(modifiers);
                    }

                    (action as Attack).Play(actor, modifiers,
                        // Calls attack finished when all attacks have been finished
                        attackFinished: () =>
                        {
                            if (--numUnfinishedAttacks == 0)
                            {
                                attackFinished?.Invoke();
                            }
                        });
                }
                else
                {
                    action.Play(actor);
                }
            }

            if (numUnfinishedAttacks == 0)
            {
                attackFinished?.Invoke();
            }
        }

        /// <summary>
        /// Gets the modifiers that another attack card would apply to this.
        /// </summary>
        /// <param name="modifingCard"> The card doing the modification. </param>
        /// <param name="index"> The index of the attack being modified. </param>
        /// <returns></returns>
        List<AttackModifier> GetAppliedModifers(AttackCard modifingCard, int index)
        {
            if (modifingCard == this)
            {
                return modifingCard.duplicateModifiers.Cast<AttackModifier>().ToList();
            }
            else
            {
                List<AttackModifier> modifiers = modifingCard.chordModifiers;
                if (modifierFilters.Count > index && modifierFilters[index] != null)
                {
                    modifiers = modifierFilters[index].FilterModifierList(modifiers);
                }
                return modifiers;
            }
        }

    }
}