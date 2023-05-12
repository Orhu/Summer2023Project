using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    /// <summary>
    /// A scriptable object for storing data about a damage cone type.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDamageInCone", menuName = "Cards/Actions/DamageInCone")]
    public class DamageInCone : CardAction
    {
        // The damage that will be dealt.
        public int damage = 1;
        // The radius of the cone.
        public float range = 6;
        // The arc width in degrees of the cone.
        public float arcWidth = 12;

        /// <summary>
        /// Gets the formated description of this card.
        /// </summary>
        /// <returns> The description with any Serialized Field names that appear in [] replaced with their actual value.</returns>
        public override string GetFormattedDescription()
        {
            return description.Replace("[Damage]", damage.ToString());
        }

        /// <summary>
        /// Starts rendering a preview of what this action will do.
        /// </summary>
        /// <param name="player"> The player that will be playing this action. </param>
        public override void Preview(ICardPlayer player)
        {

        }

        /// <summary>
        /// Stops rendering a preview of what this action will do.
        /// </summary>
        /// <param name="player"> The player that will no longer be playing this action. </param>
        public override void CancelPreview(ICardPlayer player)
        {

        }

        /// <summary>
        /// Plays this action and causes all its effects. Also cancels any relevant previews.
        /// </summary>
        /// <param name="player"> The player that will be playing this action. </param>
        public override void Play(ICardPlayer player)
        {

        }
    }
}