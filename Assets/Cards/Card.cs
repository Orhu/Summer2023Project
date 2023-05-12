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
    /// - Actions - What the card does when played
    /// - Effects - How the card effects the dungeon
    /// </summary>
    [CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card", order = 1)]
    public class Card : ScriptableObject
    {
        [Header("Mechanics")]
        // The amount of time this card reserves the hand slot for after being played.
        public float cooldown = 1.0f;
        // The actions that will be taken when this card is played.
        public CardAction[] cardActions;
        // The effects that this card will have on the dungeon while in the player's deck.
        public CardDungeonEffect[] cardEffects;

        [Header("Visuals")]
        // The name of the card as shown to the player.
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
                foreach (CardDungeonEffect cardEffect in cardEffects)
                {
                    description += cardEffect.GetFormattedDescription() + "\n";
                }
            }
            else
            {
                foreach (CardAction cardAction in cardActions)
                {
                    description += cardAction.GetFormattedDescription() + "\n";
                }
            }
            return description;
        }

        /// <summary>
        /// Causes all of this cards Actions to start rendering their previews around the player.
        /// </summary>
        public void PreviewActions()
        {
            foreach (CardAction cardAction in cardActions)
            {
                cardAction.Preview(Player._instance);
            }
        }

        /// <summary>
        /// Causes all of this cards Actions to stop rendering their previews.
        /// </summary>
        public void CancelPreviewActions()
        {
            foreach (CardAction cardAction in cardActions)
            {
                cardAction.CancelPreview(Player._instance);
            }
        }

        /// <summary>
        /// Plays all of the actions of this card from the player.
        /// </summary>
        public void PlayActions()
        {
            foreach (CardAction cardAction in cardActions)
            {
                cardAction.Play(Player._instance);
            }
        }
    }
}