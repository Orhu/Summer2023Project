using System.Collections.Generic;
using UnityEngine;

namespace Cardificer {

    
    /// <summary>
    /// A scriptable object that serves as the base of any action a card can have when played.
    /// </summary>
    abstract public class Action : ScriptableObject
    {
        // The description of this action. Any Serialized Field names that appear in [] will be replaced with their actual value.
        protected string description = "";

        [Header("Action Audio")]
        [Tooltip("Taking an Action Sound variable")]
        [SerializeField] protected BasicSound actionSound; 
        [SerializeField] protected bool stopActionSoundOnActionComplete;

        /// <summary>
        /// Gets the formatted description of this card.
        /// </summary>
        /// <returns> The description with any Serialized Field names that appear in [] replaced with their actual value. </returns>
        public virtual string GetFormattedDescription()
        {
            // TODO: GetType().GetProperties()[0].GetValue(this);
            return description;
        }


        /// <summary>
        /// Plays this action and causes all its effects. Also cancels any relevant previews.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        public abstract void Play(IActor actor, List<GameObject> ignoredObjects = null);
    }
}