using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    /// <summary>
    /// A scriptable object for storing data about a projectile type.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProjectile", menuName = "Cards/Actions/Projectile")]
    public class SpawnProjectile : CardAction
    {
        [Header("Mechanics")]
        // The damage this projectile will deal.
        public int damage = 1;
        // The radius of the projectile.
        public float size = 12;
        // The distance this projectile will travel.
        public float range = 6;
        // The speed this projectile will travel at.
        public float speed = 12;
        // The projectile to spawn
        public Projectile projectilePrefab;

        [Header("Visuals")]
        // The previewer prefab to use.
        public ProjectilePreviewer previewerPrefab;
        // The tint of the previewer spawned.
        public Color previewColor;
        // The projectile's sprite.
        public Sprite sprite;
        // The particle system to add to the projectile.
        public ParticleSystem particleEffect;

        // Maps players to the previewer
        Dictionary<ICardPlayer, ProjectilePreviewer> playersToPreviewers = new Dictionary<ICardPlayer, ProjectilePreviewer>();

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
            if (playersToPreviewers.ContainsKey(player))
            {
                playersToPreviewers[player].Count++;
            }
            else
            {
                ProjectilePreviewer previewer = Instantiate<ProjectilePreviewer>(previewerPrefab, player.GetActionSourceTransform());
                previewer.player = player;
                previewer.spawner = this;
                playersToPreviewers.Add(player, previewer);
            }
        }

        /// <summary>
        /// Stops rendering a preview of what this action will do.
        /// </summary>
        /// <param name="player"> The player that will no longer be playing this action. </param>
        public override void CancelPreview(ICardPlayer player)
        {
            if (playersToPreviewers.ContainsKey(player))
            {
                playersToPreviewers[player].Count--;
                if (playersToPreviewers[player].Count <= 0)
                {
                    Destroy(playersToPreviewers[player]);
                    playersToPreviewers.Remove(player);
                }
            }
        }

        /// <summary>
        /// Plays this action and causes all its effects. Also cancels any relevant previews.
        /// </summary>
        /// <param name="player"> The player that will be playing this action. </param>
        public override void Play(ICardPlayer player)
        {
            CancelPreview(player);
            Projectile projectile = Instantiate<Projectile>(projectilePrefab, player.GetActionSourceTransform().position, player.GetActionSourceTransform().rotation);
            projectile.player = player;
            projectile.spawner = this;
        }
    }
}