using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem
{
    /// <summary>
    /// A scriptable object that serves as the base of any action a card can have when played.
    /// </summary>
    [ExecuteInEditMode]
    abstract public class Action : ScriptableObject
    {
        // 
        internal 
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

        public void Awake()
        {
            UnityEditor.EditorGUIUtility.SetIconForObject(this, UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Content/Developer Utilities/Icons/ActionIcon.png"));
        }

        /// <summary>
        /// Plays this action and causes all its effects. Also cancels any relevant previews.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        public abstract void Play(IActor actor, List<GameObject> ignoredObjects = null);
    }
}