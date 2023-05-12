using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem
{
    /// <summary>
    /// A scriptable object that serves as the base of any action a card can have when played.
    /// </summary>
    abstract public class CardAction : ScriptableObject
    {
        // The description of this action. Any Serialized Field names that appear in [] will be replaced with their actual value.
        protected string description = "";

        /// <summary>
        /// Gets the formated description of this card.
        /// </summary>
        /// <returns> The description with any Serialized Field names that appear in [] replaced with their actual value.</returns>
        public virtual string GetFormattedDescription()
        {
            return description;
        }

        /// <summary>
        /// Starts rendering a preview of what this action will do.
        /// </summary>
        /// <param name="player"> The player that will be playing this action. </param>
        public abstract void Preview(ICardPlayer player);

        /// <summary>
        /// Stops rendering a preview of what this action will do.
        /// </summary>
        /// <param name="player"> The player that will no longer be playing this action. </param>
        public abstract void CancelPreview(ICardPlayer player);

        /// <summary>
        /// Plays this action and causes all its effects. Also cancels any relevant previews.
        /// </summary>
        /// <param name="player"> The player that will be playing this action. </param>
        public abstract void Play(ICardPlayer player);
    }
}