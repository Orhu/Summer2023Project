using System.Collections.Generic;
using UnityEngine;
using Skaillz.EditInline;

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
    [EditInline]
    [Tooltip("The how this card will modify actions when used in a combo.")]
    public List<AttackModifier> chordModifiers;
    [EditInline]
    [Tooltip("The how this card will modify actions when used in a combo with itself.")]
    public List<DuplicateAttackModifier> duplicateModifiers;

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
        AddToPreview(actor, GetAppliedModifers(chordedCard));
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

    /// <summary>
    /// Plays the actions of this card.
    /// </summary>
    /// <param name="actor"> The actor playing the card. </param>
    /// <param name="chordedCards"> The cards being corded with this. </param>
    public void PlayActions(IActor actor, List<AttackCard> chordedCards)
    {
        List<AttackModifier> modifiers = new List<AttackModifier>();
        foreach (AttackCard chordedCard in chordedCards)
        {
            modifiers.AddRange(GetAppliedModifers(chordedCard));
        }

        PlayActions(actor, modifiers);
    }

    /// <summary>
    /// Plays the actions of this card.
    /// </summary>
    /// <param name="actor"> The actor playing the card. </param>
    /// <param name="modifiers"> The modifiers being applied to this. </param>
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

    /// <summary>
    /// Gets the modifiers that another attack card would apply to this.
    /// </summary>
    /// <param name="modifingCard"></param>
    /// <returns></returns>
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