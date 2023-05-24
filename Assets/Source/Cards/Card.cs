using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Skaillz.EditInline;

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
    [CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Cards/Normal Card", order = 1)]
    //[ExecuteInEditMode]
    public class Card : ScriptableObject
    {
        [Header("Mechanics")]
        [Tooltip("The amount of time this card takes to be played.")]
        public float actionTime = 1.0f;
        [Tooltip("The amount of time this card reserves the hand slot for after being played.")]
        public float cooldownTime = 1.0f;
        [EditInline]
        [Tooltip("The actions that will be taken when this card is played as the root of a combo.")]
        public Action[] actions;
        [Tooltip("The effects that this card will have on the dungeon while in the actor's deck.")]
        [EditInline]
        public DungeonEffect[] effects;

        [Header("Visuals")]
        [Tooltip("The name of the card as shown to the player.")]
        public string displayName = "Unnamed";
        [Tooltip("The description where variable names inside of [] will be replaced with the variable's value when shown to the player.")]
        public string description = "No Description";
        [Tooltip("The card specific sprite on the Actions side of the card.")]
        public Sprite actionImage;
        [Tooltip("The general background card sprite on the Actions side of the card.")]
        public Sprite actionBackground;
        [Tooltip("The card specific sprite on the Effects side of the card.")]
        public Sprite effectImage;
        [Tooltip("The general background card sprite on the Effects side of the card.")]
        public Sprite effectBackground;


        /// <summary>
        /// Gets the description of this card by collecting all the formated descriptions from the card's mechanics.
        /// </summary>
        /// <param name="isActionSide"> Whether or not to get the description for the action side or the effect side of the card.</param>
        /// <returns>The description.</returns>
        public string GetDescription(bool isActionSide)
        {
            string description = "";
            //if (!isActionSide)
            //{
            //    foreach (DungeonEffect cardEffect in effects)
            //    {
            //        description += cardEffect.GetFormattedDescription() + "\n";
            //    }
            //}
            //else
            //{
            //    foreach (Action cardAction in actions)
            //    {
            //        description += cardAction.GetFormattedDescription() + "\n";
            //    }
            //}
            return description;
        }


        /// <summary>
        /// Plays all of the actions of this card from the actor.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="modifiers"> The modifier affecting this action. </param>
        public void PlayActions(IActor actor)
        {
            foreach (Action cardAction in actions)
            {
                cardAction.Play(actor);
            }
        }
    }
}