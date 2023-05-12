using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    /// <summary>
    /// A scriptable object for storing data about a projectile type.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProjectile", menuName = "Cards/Actions/Projectile")]
    public class Projectile : CardAction
    {
        // The damage this projectile will deal.
        public int damage = 1;
        // The distance this projectile will travel.
        public float range = 6;
        // The speed this projectile will travel at.
        public float speed = 12;

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