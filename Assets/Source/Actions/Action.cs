using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem
{
    /// <summary>
    /// A scriptable object that serves as the base of any action a card can have when played.
    /// </summary>
    abstract public class Action : ScriptableObject
    {
        // The description of this action. Any Serialized Field names that appear in [] will be replaced with their actual value.
        protected string description = "";

        /// <summary>
        /// Gets the formated description of this card.
        /// </summary>
        /// <returns> The description with any Serialized Field names that appear in [] replaced with their actual value.</returns>
        public virtual string GetFormattedDescription()
        {
            // TODO: GetType().GetProperties()[0].GetValue(this);
            return description;
        }

        /// <summary>
        /// Starts rendering a preview of what this action will do.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        public virtual void Preview(IActor actor) { }

        /// <summary>
        /// Adds a stacks to a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing </param>
        /// <param name="numStacks"> The number of stacks to add </param>
        public virtual void AddStacksToPreview(IActor actor, int numStacks) { }

        /// <summary>
        /// Applies modifiers to a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing</param>
        /// <param name="actionModifiers"> The modifiers to apply </param>
        public virtual void ApplyModifiersToPreview(IActor actor, List<ActionModifier> actionModifiers) { }

        /// <summary>
        /// Removes modifiers from a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing</param>
        /// <param name="actionModifiers"> The modifiers to remove </param>
        public virtual void RemoveModifiersFromPreview(IActor actor, List<ActionModifier> actionModifiers) { }

        /// <summary>
        /// Stops rendering a preview of what this action will do.
        /// </summary>
        /// <param name="actor"> The actor that will no longer be playing this action. </param>
        public virtual void CancelPreview(IActor actor) { }

        /// <summary>
        /// Plays this action and causes all its effects. Also cancels any relevant previews.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="numStacks"> The number of times action is to be played. </param>
        /// <param name="modifiers"> The modifier affecting this action. </param>
        public abstract void Play(IActor actor, int numStacks, List<ActionModifier> modifiers);
    }
}